#region License

// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Practices.Unity;
using Microsoft.Win32;
using Wide.Core.Attributes;
using Wide.Core.TextDocument;
using Wide.Interfaces;
using Wide.Interfaces.Services;

namespace Wide.Core.Services
{
    /// <summary>
    /// AllFileHandler class that supports opening of any file on the computer
    /// </summary>
    [FileContent("All files", "*.*", 10000)]
    [NewContent("Text file", 10000, "Creates a basic text file", "pack://application:,,,/Wide;component/Core/Icons/Textfile.png")]
    internal class AllFileHandler : IContentHandler
    {
        /// <summary>
        /// The injected container
        /// </summary>
        private readonly IUnityContainer _container;

        /// <summary>
        /// The injected logger service
        /// </summary>
        private readonly ILoggerService _loggerService;
        
        /// <summary>
        /// The save file dialog
        /// </summary>
        private SaveFileDialog _dialog;

        /// <summary>
        /// Constructor of AllFileHandler - all parameters are injected
        /// </summary>
        /// <param name="container">The injected container of the application</param>
        /// <param name="loggerService">The injected logger service of the application</param>
        public AllFileHandler(IUnityContainer container, ILoggerService loggerService)
        {
            _container = container;
            _loggerService = loggerService;
            _dialog = new SaveFileDialog();
        }

        #region IContentHandler Members

        public ContentViewModel NewContent(Object parameter)
        {
            TextViewModel vm = _container.Resolve<TextViewModel>();
            TextModel model = _container.Resolve<TextModel>();
            TextView view = _container.Resolve<TextView>();

            _loggerService.Log("Creating a new simple file using AllFileHandler", LogCategory.Info, LogPriority.Low);

            //Clear the undo stack
            model.Document.UndoStack.ClearAll();

            //Set the model and view
            vm.Model = model;
            vm.View = view;
            vm.Title = "untitled";
            vm.View.DataContext = model;
            vm.Handler = this;
            vm.Model.IsDirty = true;

            return vm;
        }

        /// <summary>
        /// Validates the content by checking if a file exists for the specified location
        /// </summary>
        /// <param name="info">The string containing the file location</param>
        /// <returns>True, if the file exists - false otherwise</returns>
        public Boolean ValidateContentType(Object info)
        {
            if (info is String location)
            {
                return File.Exists(location);
            }
            return false;
        }

        /// <summary>
        /// Validates the content from an ID - the ContentID from the ContentViewModel
        /// </summary>
        /// <param name="contentId">The content ID which needs to be validated</param>
        /// <returns>True, if valid from content ID - false, otherwise</returns>
        public Boolean ValidateContentFromId(String contentId)
        {
            String[] split = Regex.Split(contentId, ":##:");
            if (split.Count() == 2)
            {
                String identifier = split[0];
                String path = split[1];
                if (identifier == "FILE" && File.Exists(path))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Opens a file and returns the corresponding ContentViewModel
        /// </summary>
        /// <param name="info">The string location of the file</param>
        /// <returns>The <see cref="TextViewModel"/> for the file.</returns>
        public ContentViewModel OpenContent(Object info)
        {
            if (info is String location)
            {
                TextViewModel vm = _container.Resolve<TextViewModel>();
                TextModel model = _container.Resolve<TextModel>();
                TextView view = _container.Resolve<TextView>();

                //Model details
                model.SetLocation(info);
                try
                {
                    model.Document.Text = File.ReadAllText(location);
                    model.IsDirty = false;
                }
                catch (Exception exception)
                {
                    _loggerService.Log(exception.Message, LogCategory.Exception, LogPriority.High);
                    _loggerService.Log(exception.StackTrace, LogCategory.Exception, LogPriority.High);
                    return null;
                }

                //Clear the undo stack
                model.Document.UndoStack.ClearAll();

                //Set the model and view
                vm.Model = model;
                vm.View = view;
                vm.Title = Path.GetFileName(location);
                vm.View.DataContext = model;

                return vm;
            }
            return null;
        }

        /// <summary>
        /// Opens the content from the content ID
        /// </summary>
        /// <param name="contentId">The content ID</param>
        /// <returns></returns>
        public ContentViewModel OpenContentFromId(String contentId)
        {
            String[] split = Regex.Split(contentId, ":##:");
            if (split.Count() == 2)
            {
                String identifier = split[0];
                String path = split[1];
                if (identifier == "FILE" && File.Exists(path))
                {
                    return OpenContent(path);
                }
            }
            return null;
        }

        /// <summary>
        /// Saves the content of the TextViewModel
        /// </summary>
        /// <param name="contentViewModel">This needs to be a TextViewModel that needs to be saved</param>
        /// <param name="saveAs">Pass in true if you need to Save As?</param>
        /// <returns>true, if successful - false, otherwise</returns>
        public virtual Boolean SaveContent(ContentViewModel contentViewModel, Boolean saveAs = false)
        {

            if (!(contentViewModel is TextViewModel textViewModel))
            {
                _loggerService.Log("ContentViewModel needs to be a TextViewModel to save details", LogCategory.Exception,
                                   LogPriority.High);
                throw new ArgumentException("ContentViewModel needs to be a TextViewModel to save details");
            }

            if (!(textViewModel.Model is TextModel textModel))
            {
                _loggerService.Log("TextViewModel does not have a TextModel which should have the text",
                                   LogCategory.Exception, LogPriority.High);
                throw new ArgumentException("TextViewModel does not have a TextModel which should have the text");
            }

            String location = textModel.Location as String;

            if (location == null)
            {
                //If there is no location, just prompt for Save As..
                saveAs = true;
            }

            if (saveAs)
            {
                if (location != null)
                {
                    _dialog.InitialDirectory = Path.GetDirectoryName(location);
                }

                _dialog.CheckPathExists = true;
                _dialog.DefaultExt = "txt";
                _dialog.Filter = "All files (*.*)|*.*";

                if (_dialog.ShowDialog() == true)
                {
                    location = _dialog.FileName;
                    textModel.SetLocation(location);
                    textViewModel.Title = Path.GetFileName(location);
                    try
                    {
                        File.WriteAllText(location, textModel.Document.Text);
                        textModel.IsDirty = false;
                        return true;
                    }
                    catch (Exception exception)
                    {
                        _loggerService.Log(exception.Message, LogCategory.Exception, LogPriority.High);
                        _loggerService.Log(exception.StackTrace, LogCategory.Exception, LogPriority.High);
                        return false;
                    }
                }
            }
            else
            {
                try
                {
                    File.WriteAllText(location, textModel.Document.Text);
                    textModel.IsDirty = false;
                    return true;
                }
                catch (Exception exception)
                {
                    _loggerService.Log(exception.Message, LogCategory.Exception, LogPriority.High);
                    _loggerService.Log(exception.StackTrace, LogCategory.Exception, LogPriority.High);
                    return false;
                }
            }

            return false;
        }

        #endregion
    }
}