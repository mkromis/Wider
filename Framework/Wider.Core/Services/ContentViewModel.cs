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
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Wider.Core.Controls;
using Wider.Core.Services;

namespace Wider.Core.Services
{
    public abstract class ContentViewModel : ContentViewModel<ContentModel>
    {
        protected ContentViewModel(IContainerExtension container) : base(container) { }
    }

    /// <summary>
    /// The abstract class which has to be inherited if you want to create a document
    /// </summary>
    public abstract class ContentViewModel<T> : BindableBase where T : ContentModel
    {
        #region Members

        /// <summary>
        /// The static count value for "Untitled" number.
        /// </summary>
        protected static Int32 Count = 1;

        /// <summary>
        /// The model
        /// </summary>
        protected T _model;

        /// <summary>
        /// The command manager
        /// </summary>
        protected ICommandManager _commandManager;

        /// <summary>
        /// The content id of the document
        /// </summary>
        protected String _contentId = null;

        /// <summary>
        /// Is the document active
        /// </summary>
        protected Boolean _isActive = false;

        /// <summary>
        /// Is the document selected
        /// </summary>
        protected Boolean _isSelected = false;

        /// <summary>
        /// The title of the document
        /// </summary>
        protected String _title = null;

        /// <summary>
        /// The tool tip to display on the document
        /// </summary>
        protected String _tooltip = null;

        /// <summary>
        /// The menu service
        /// </summary>
        protected IMenuService _menuService;

        #endregion

        #region CTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentViewModel"/> class.
        /// </summary>
        /// <param name="workspace">The injected workspace.</param>
        /// <param name="commandManager">The injected command manager.</param>
        /// <param name="logger">The injected logger.</param>
        protected ContentViewModel(IContainerExtension continer)
        {
            _commandManager = continer.Resolve<ICommandManager>();
            _menuService = continer.Resolve<IMenuService>();
            CloseCommand = new DelegateCommand<Object>(Close, CanClose);
        }

        #endregion

        #region Property

        /// <summary>
        /// Gets or sets the close command.
        /// </summary>
        /// <value>The close command.</value>
        public virtual ICommand CloseCommand { get; protected internal set; }

        /// <summary>
        /// The content model
        /// </summary>
        /// <value>The model.</value>
        public virtual T Model
        {
            get => _model;
            set
            {
                if (_model != null)
                {
                    _model.PropertyChanged -= Model_PropertyChanged;
                }
                if (value != null)
                {
                    _model = value;
                    _model.PropertyChanged += Model_PropertyChanged;
                }
            }
        }

        /// <summary>
        /// The content view
        /// </summary>
        /// <value>The view.</value>
        public virtual IContentView View { get; set; }

        /// <summary>
        /// The content menu that should be available for the document pane
        /// </summary>
        /// <value>The view.</value>
        public IReadOnlyCollection<AbstractMenuItem> Menus
        {
            get
            {
                AbstractMenuItem item = _menuService.Get("_File").Get("_Save") as AbstractMenuItem;
                List<AbstractMenuItem> items = new List<AbstractMenuItem>
                {
                    item
                };
                return items.AsReadOnly();
            }
        }

        /// <summary>
        /// The title of the document
        /// </summary>
        /// <value>The title.</value>
        public virtual String Title
        {
            get
            {
                if (Model.IsDirty)
                {
                    return _title + "*";
                }
                return _title;
            }
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// The tool tip of the document
        /// </summary>
        /// <value>The tool tip.</value>
        public virtual String Tooltip
        {
            get => _tooltip;
            protected set => SetProperty(ref _tooltip, value);
        }

        /// <summary>
        /// The image source that can be used as an icon in the tab
        /// </summary>
        /// <value>The icon source.</value>
        public virtual ImageSource IconSource { get; protected internal set; }

        /// <summary>
        /// The content ID - unique value for each document
        /// </summary>
        /// <value>The content id.</value>
        public virtual String ContentId
        {
            get => _contentId;
            protected set => SetProperty(ref _contentId, value);
        }

        /// <summary>
        /// Is the document selected
        /// </summary>
        /// <value><c>true</c> if this document is selected; otherwise, <c>false</c>.</value>
        public virtual Boolean IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        /// <summary>
        /// Is the document active
        /// </summary>
        /// <value><c>true</c> if this document is active; otherwise, <c>false</c>.</value>
        public virtual Boolean IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        /// <summary>
        /// The content handler which does save and load of the file
        /// </summary>
        /// <value>The handler.</value>
        public IContentHandler Handler { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether this instance can close.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if this instance can close; otherwise, <c>false</c>.</returns>
        protected virtual Boolean CanClose(Object obj)
        {
            return (obj != null)
                       ? _commandManager.GetCommand("CLOSE").CanExecute(obj)
                       : _commandManager.GetCommand("CLOSE").CanExecute(this);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        protected virtual void Close(Object obj)
        {
            if (obj != null)
            {
                _commandManager.GetCommand("CLOSE").Execute(obj);
            }
            else
            {
                _commandManager.GetCommand("CLOSE").Execute(this);
            }
        }


        protected virtual void Model_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged("Model");
            RaisePropertyChanged("Title");
            RaisePropertyChanged("ContentId");
            RaisePropertyChanged("Tooltip");
            RaisePropertyChanged("IsSelected");
            RaisePropertyChanged("IsActive");
        }

        #endregion
    }
}