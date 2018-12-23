﻿#region License
// Copyright (c) 2018 Mark Kromis
// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using Microsoft.Win32;
using Prism.Events;
using Prism.Ioc;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Wider.Core.Attributes;
using Wider.Core.Events;
using Wider.Core.Settings;

namespace Wider.Core.Services
{
    /// <summary>
    /// The open file service
    /// </summary>
    internal sealed class OpenDocumentService : IOpenDocumentService
    {
        /// <summary>
        /// The injected container
        /// </summary>
        // private IContainerExtension Container { get; private set; }

        /// <summary>
        /// The injected event aggregator
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// The injected logger
        /// </summary>
        private readonly ILoggerService _logger;

        /// <summary>
        /// The Open file dialog
        /// </summary>
        private readonly OpenFileDialog _dialog;

        /// <summary>
        /// The workspace
        /// </summary>
        private readonly IWorkspace _workspace;

        /// <summary>
        /// The content handler registry
        /// </summary>
        private readonly ContentHandlerRegistry _handler;

        /// <summary>
        /// The recent settings
        /// </summary>
        private readonly RecentViewSettings _recentSettings;

        /// <summary>
        /// Constructor for Open file service
        /// </summary>
        /// <param name="container">The injected container</param>
        /// <param name="eventAggregator">The injected event aggregator</param>
        /// <param name="logger">The injected logger</param>
        public OpenDocumentService(IContainerExtension container)
        {
            _eventAggregator = container.Resolve<IEventAggregator>();
            _logger = container.Resolve<ILoggerService>();
            _workspace = container.Resolve<IWorkspace>();
            _handler = container.Resolve<IContentHandlerRegistry>() as ContentHandlerRegistry;
            _recentSettings = container.Resolve<IRecentViewSettings>() as RecentViewSettings;
            //_dialog = new OpenFileDialog();
        }

        #region IOpenDocumentService Members

        /// <summary>
        /// Opens the object - if object is null, show a open file dialog to select a file to open
        /// </summary>
        /// <param name="location">The optional object to open</param>
        /// <returns>A document which was added to the workspace as a content view model</returns>
        public ContentViewModel Open(Object location = null)
        {
            Boolean? result;
            ContentViewModel returnValue = null;

            if (location == null)
            {
                _dialog.Filter = "";
                String sep = "";
                List<FileContentAttribute> attributes =
                    _handler.ContentHandlers.SelectMany(
                        handler =>
                        (FileContentAttribute[])
                        (handler.GetType()).GetCustomAttributes(typeof (FileContentAttribute), true)).ToList();
                attributes.Sort((attribute, contentAttribute) => attribute.Priority - contentAttribute.Priority);
                foreach (FileContentAttribute contentAttribute in attributes)
                {
                    _dialog.Filter = String.Format("{0}{1}{2} ({3})|{3}", _dialog.Filter, sep, contentAttribute.Display,
                                                   contentAttribute.Extension);
                    sep = "|";
                }

                result = _dialog.ShowDialog();
                location = _dialog.FileName;
            }
            else
            {
                result = true;
            }

            if (result == true && !String.IsNullOrWhiteSpace(location.ToString()))
            {
                //Let the handler figure out which view model to return
                if (_handler != null)
                {
                    ContentViewModel openValue = _handler.GetViewModel(location);

                    if (openValue != null)
                    {
                        //Check if the document is already open
                        foreach (ContentViewModel contentViewModel in _workspace.Documents)
                        {
                            if (contentViewModel.Model.Location != null)
                            {
                                if (contentViewModel.Model.Location.Equals(openValue.Model.Location))
                                {
                                    _logger.Log(
                                        $"Document {contentViewModel.Model.Location} already open - making it active",
                                        Category.Info, Priority.Low);
                                    _workspace.ActiveDocument = contentViewModel;
                                    return contentViewModel;
                                }
                            }
                        }

                        _logger.Log("Opening file" + location + " !!", Category.Info, Priority.Low);

                        // Publish the event to the Application - subscribers can use this object
                        _eventAggregator.GetEvent<OpenContentEvent>().Publish(openValue);

                        //Add it to the actual workspace
                        _workspace.Documents.Add(openValue);

                        //Make it the active document
                        _workspace.ActiveDocument = openValue;

                        //Add it to the recent documents opened
                        _recentSettings.Update(openValue);

                        returnValue = openValue;
                    }
                    else
                    {
                        _logger.Log("Unable to find a IContentHandler to open " + location, Category.Warn, Priority.High);
                    }
                }
            }
            else
            {
                _logger.Log("Canceled out of open file dialog", Category.Info, Priority.Low);
            }
            return returnValue;
        }

        /// <summary>
        /// Opens the contentID
        /// </summary>
        /// <param name="contentID">The contentID to open</param>
        /// <param name="makeActive">if set to <c>true</c> makes the new document as the active document.</param>
        /// <returns>A document which was added to the workspace as a content view model</returns>
        public ContentViewModel OpenFromID(String contentID, Boolean makeActive = false)
        {
            //Let the handler figure out which view model to return
            ContentViewModel openValue = _handler.GetViewModelFromContentId(contentID);

            if (openValue != null)
            {
                //Check if the document is already open
                foreach (ContentViewModel contentViewModel in _workspace.Documents)
                {
                    if (contentViewModel.Model.Location != null)
                    {
                        if (contentViewModel.Model.Location.Equals(openValue.Model.Location))
                        {
                            _logger.Log($"Document {contentViewModel.Model.Location} already open.",
                                        Category.Info, Priority.Low);

                            if (makeActive)
                            {
                                _workspace.ActiveDocument = contentViewModel;
                            }

                            return contentViewModel;
                        }
                    }
                }

                _logger.Log($"Opening content with {contentID} !!", Category.Info, Priority.Low);

                // Publish the event to the Application - subscribers can use this object
                _eventAggregator.GetEvent<OpenContentEvent>().Publish(openValue);

                _workspace.Documents.Add(openValue);

                if (makeActive)
                {
                    _workspace.ActiveDocument = openValue;
                }

                //Add it to the recent documents opened
                _recentSettings.Update(openValue);

                return openValue;
            }

            _logger.Log($"Unable to find a IContentHandler to open content with ID = {contentID}",
                Category.Warn, Priority.High);
            return null;
        }

        #endregion
    }
}