#region License
// Copyright (c) 2018 Mark Kromis
// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Wider.Core.Attributes;
using Wider.Core.Services;

namespace Wider.Core.Services
{
    /// <summary>
    /// The content handler registry which manages different content handlers
    /// </summary>
    internal sealed class ContentHandlerRegistry : IContentHandlerRegistry
    {

        #region Fields

        /// <summary>
        /// The _available new content
        /// </summary>
        private readonly List<NewContentAttribute> _availableNewContent;

        /// <summary>
        /// The dictionary
        /// </summary>
        private readonly Dictionary<NewContentAttribute, IContentHandler> _dictionary;

        /// <summary>
        /// The workspace - NEEDS to be resolved in function call. Else this leads to recursive resolution in constructor.
        /// </summary>
        private IWorkspace _workspace;

        /// <summary>
        /// The container
        /// </summary>
        private readonly IContainerExtension _container;

        /// <summary>
        /// The status bar service
        /// </summary>
        private readonly IStatusbarService _statusBar;

        /// <summary>
        /// The new glow background brush
        /// </summary>
        private readonly SolidColorBrush _newBackground = new SolidColorBrush(Color.FromRgb(104,33,122));
        #endregion

        #region Properties
        /// <summary>
        /// The new document command
        /// </summary>
        public ICommand NewCommand { get; }

        /// <summary>
        /// Gets the available contents which can be created.
        /// </summary>
        /// <value>The new content of the available.</value>
        public IReadOnlyCollection<NewContentAttribute> AvailableNewContent => null;

        /// <summary>
        /// List of content handlers
        /// </summary>
        public List<IContentHandler> ContentHandlers { get; }
        #endregion

        #region CTOR
        /// <summary>
        /// Constructor of content handler registry
        /// </summary>
        /// <param name="container">The injected container of the application</param>
        public ContentHandlerRegistry(IContainerExtension container, IStatusbarService statusBar)
        {
            ContentHandlers = new List<IContentHandler>();
            _container = container;
            _statusBar = statusBar;
            _dictionary = new Dictionary<NewContentAttribute, IContentHandler>();
            _availableNewContent = new List<NewContentAttribute>();
            NewCommand = new DelegateCommand(NewDocument, CanExecuteNewCommand);
        }
        #endregion

        #region IContentHandlerRegistry Members

        /// <summary>
        /// Register a content handler with the registry
        /// </summary>
        /// <param name="handler">The content handler</param>
        /// <returns>true, if successful - false, otherwise</returns>
        public Boolean Register(IContentHandler handler)
        {
            ContentHandlers.Add(handler);
            NewContentAttribute[] handlerAttributes = (NewContentAttribute[])(handler.GetType()).GetCustomAttributes(typeof(NewContentAttribute), true);
            _availableNewContent.AddRange(handlerAttributes);
            foreach (NewContentAttribute newContentAttribute in handlerAttributes)
            {
                _dictionary.Add(newContentAttribute, handler);
            }

            _availableNewContent.Sort((attribute, contentAttribute) => attribute.Priority - contentAttribute.Priority);
            return true;
        }

        /// <summary>
        /// Unregisters a content handler
        /// </summary>
        /// <param name="handler">The handler to remove</param>
        /// <returns></returns>
        public Boolean Unregister(IContentHandler handler)
        {
            NewContentAttribute[] handlerAttributes = (NewContentAttribute[])(handler.GetType()).GetCustomAttributes(typeof(NewContentAttribute), true);
            _availableNewContent.RemoveAll(handlerAttributes.Contains);
            foreach (NewContentAttribute newContentAttribute in handlerAttributes)
            {
                _dictionary.Remove(newContentAttribute);
            }

            _availableNewContent.Sort((attribute, contentAttribute) => attribute.Priority - contentAttribute.Priority);
            return ContentHandlers.Remove(handler);
        }

        #endregion

        #region Get the view model

        /// <summary>
        /// Returns a content view model for the specified object which needs to be displayed as a document
        /// The object could be anything - based on the handlers, a content view model is returned
        /// </summary>
        /// <param name="info">The object which needs to be displayed as a document in Wider</param>
        /// <returns>The content view model for the given info</returns>
        public ContentViewModel<ContentModel> GetViewModel(Object info)
        {
            for (Int32 i = ContentHandlers.Count - 1; i >= 0; i--)
            {
                IContentHandler opener = ContentHandlers[i];
                if (opener.ValidateContentType(info))
                {
                    ContentViewModel<ContentModel> vm = opener.OpenContent(info);
                    vm.Handler = opener;
                    return vm;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a content view model for the specified contentID which needs to be displayed as a document
        /// The contentID is the ID used in AvalonDock
        /// </summary>
        /// <param name="contentId">The contentID which needs to be displayed as a document in Wider</param>
        /// <returns>The content view model for the given info</returns>
        public ContentViewModel<ContentModel> GetViewModelFromContentId(String contentId)
        {
            for (Int32 i = ContentHandlers.Count - 1; i >= 0; i--)
            {
                IContentHandler opener = ContentHandlers[i];
                if (opener.ValidateContentFromId(contentId))
                {
                    ContentViewModel<ContentModel> vm = opener.OpenContentFromId(contentId);
                    vm.Handler = opener;
                    return vm;
                }
            }
            return null;
        }

        #endregion

        #region New Command
        private Boolean CanExecuteNewCommand() => true;

        private void NewDocument()
        {
            if (_workspace == null)
            {
                _workspace = _container.Resolve<IWorkspace>();
            }

            if (_availableNewContent.Count == 1)
            {
                IContentHandler handler = _dictionary[_availableNewContent[0]];
                ContentViewModel<ContentModel> openValue = handler.NewContent(_availableNewContent[0]);
                _workspace.Documents.Add(openValue);
                _workspace.ActiveDocument = openValue;
            }
            else
            {
                INewFileWindow window = _container.Resolve<INewFileWindow>();
                if (window == null) { return; }
                Brush backup = _statusBar.Background;
                _statusBar.Background = _newBackground;
                _statusBar.Text = "Select a new file";
                //if (Application.Current.MainWindow is MetroWindow)
                //{
                //    window.Resources = Application.Current.MainWindow.Resources;
                //    Window win = Application.Current.MainWindow as MetroWindow;
                //    window.Resources = win.Resources;
                //    //window.GlowBrush = win.GlowBrush;
                //    //window.TitleForeground = win.TitleForeground;
                //}
                window.DataContext = _availableNewContent;
                if (window.ShowDialog() == true)
                {
                    NewContentAttribute newContent = window.NewContent;
                    if (newContent != null)
                    {
                        IContentHandler handler = _dictionary[newContent];
                        ContentViewModel<ContentModel> openValue = handler.NewContent(newContent);
                        _workspace.Documents.Add(openValue);
                        _workspace.ActiveDocument = openValue;
                    }
                }
                _statusBar.Background = backup;
                _statusBar.Clear();
            }
        }
        #endregion
    }
}