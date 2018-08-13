﻿#region License

// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using Autofac;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Wider.Core.Attributes;
using Microsoft.Win32;
using Prism.Logging;
using Wider.Core.Services;

namespace WiderMD.Core
{
    [FileContent("Markdown files", "*.md", 1)]
    [NewContent("Markdown files", 1, "Creates a new Markdown file", "pack://application:,,,/WiderMD.Core;component/Icons/MDType.png")]
    internal class MDHandler : IContentHandler
    {
        /// <summary>
        /// The injected container
        /// </summary>
        private readonly IContainer _container;
        private readonly ContainerBuilder _builder;

        /// <summary>
        /// The injected logger service
        /// </summary>
        private readonly ILoggerService _loggerService;
        
        /// <summary>
        /// The save file dialog
        /// </summary>
        private SaveFileDialog _dialog;

        /// <summary>
        /// Constructor of MDHandler - all parameters are injected
        /// </summary>
        /// <param name="container">The injected container of the application</param>
        /// <param name="loggerService">The injected logger service of the application</param>
        public MDHandler(ContainerBuilder builder, IContainer container, ILoggerService loggerService)
        {
            _builder = builder;
            _container = container;
            _loggerService = loggerService;
            _dialog = new SaveFileDialog();
        }

        #region IContentHandler Members

        public ContentViewModel NewContent(Object parameter)
        {
            MDViewModel vm = _container.Resolve<MDViewModel>();
            MDModel model = _container.Resolve<MDModel>();
            MDView view = _container.Resolve<MDView>();

            //Model details
            _loggerService.Log("Creating a new simple file using MDHandler", Category.Info, Priority.Low);

            view.DataContext = model;

            //Clear the undo stack
            model.Document.UndoStack.ClearAll();

            //Set the model and view
            vm.SetModel(model);
            vm.SetView(view);
            vm.Title = "untitled-MD";
            vm.SetHandler(this);
            model.SetDirty(true);

            return vm;
        }

        /// <summary>
        /// Validates the content by checking if a file exists for the specified location
        /// </summary>
        /// <param name="info">The string containing the file location</param>
        /// <returns>True, if the file exists and has a .md extension - false otherwise</returns>
        public Boolean ValidateContentType(Object info)
        {
            String extension = "";

            if (!(info is String location))
            {
                return false;
            }

            extension = Path.GetExtension(location);
            return File.Exists(location) && extension == ".md";
        }

        /// <summary>
        /// Opens a file and returns the corresponding MDViewModel
        /// </summary>
        /// <param name="info">The string location of the file</param>
        /// <returns>The <see cref="MDViewModel"/> for the file.</returns>
        public ContentViewModel OpenContent(Object info)
        {
            if (info is String location)
            {
                MDViewModel vm = _container.Resolve<MDViewModel>();
                MDModel model = _container.Resolve<MDModel>();
                MDView view = _container.Resolve<MDView>();

                //Model details
                model.SetLocation(info);
                try
                {
                    model.Document.Text = File.ReadAllText(location);
                    model.SetDirty(false);
                }
                catch (Exception exception)
                {
                    _loggerService.Log(exception.Message, Category.Exception, Priority.High);
                    _loggerService.Log(exception.StackTrace, Category.Exception, Priority.High);
                    return null;
                }

                view.DataContext = model;

                //Clear the undo stack
                model.Document.UndoStack.ClearAll();

                //Set the model and view
                vm.SetModel(model);
                vm.SetView(view);
                vm.Title = Path.GetFileName(location);

                return vm;
            }
            return null;
        }

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
            if (!(contentViewModel is MDViewModel mdViewModel))
            {
                _loggerService.Log(
                    "ContentViewModel needs to be a MDViewModel to save details", 
                    Category.Exception, Priority.High);
                throw new ArgumentException("ContentViewModel needs to be a MDViewModel to save details");
            }

            if (!(mdViewModel.Model is MDModel mdModel))
            {
                _loggerService.Log(
                    "MDViewModel does not have a MDModel which should have the text",
                    Category.Exception, Priority.High);
                throw new ArgumentException("MDViewModel does not have a MDModel which should have the text");
            }

            String location = mdModel.Location as String;

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
                _dialog.DefaultExt = "md";
                _dialog.Filter = "Markdown files (*.md)|*.md";
                
                if (_dialog.ShowDialog() == true)
                {
                    location = _dialog.FileName;
                    mdModel.SetLocation(location);
                    mdViewModel.Title = Path.GetFileName(location);
                    try
                    {
                        File.WriteAllText(location, mdModel.Document.Text);
                        mdModel.SetDirty(false);
                        return true;
                    }
                    catch (Exception exception)
                    {
                        _loggerService.Log(exception.Message, Category.Exception, Priority.High);
                        _loggerService.Log(exception.StackTrace, Category.Exception, Priority.High);
                        return false;
                    }
                }
            }
            else
            {
                try
                {
                    File.WriteAllText(location, mdModel.Document.Text);
                    mdModel.SetDirty(false);
                    return true;
                }
                catch (Exception exception)
                {
                    _loggerService.Log(exception.Message, Category.Exception, Priority.High);
                    _loggerService.Log(exception.StackTrace, Category.Exception, Priority.High);
                    return false;
                }
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
                if (identifier == "FILE" && ValidateContentType(path))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}