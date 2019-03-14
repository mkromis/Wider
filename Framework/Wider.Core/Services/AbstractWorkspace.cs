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

using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using Wider.Core.Controls;
using Wider.Core.Events;
using Wider.Core.Services;

namespace Wider.Core.Services
{
    /// <summary>
    /// Class AbstractWorkspace
    /// </summary>
    public abstract class AbstractWorkspace : BindableBase, IWorkspace
    {
        #region Fields

        /// <summary>
        /// The injected container
        /// </summary>

        /// <summary>
        /// The injected event aggregator
        /// </summary>
        protected readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// The active document
        /// </summary>
        private ContentViewModel<ContentModel> _activeDocument;

        /// <summary>
        /// The injected command manager
        /// </summary>
        protected ICommandManager _commandManager;

        /// <summary>
        /// The list of documents
        /// </summary>
        protected ObservableCollection<ContentViewModel<ContentModel>> _docs = new ObservableCollection<ContentViewModel<ContentModel>>();

        /// <summary>
        /// The toolbar service
        /// </summary>
        protected IToolbarService _toolbarService;

        /// <summary>
        /// The list of tools
        /// </summary>
        protected ObservableCollection<ToolViewModel> _tools = new ObservableCollection<ToolViewModel>();
        private readonly IMenuService _menus;

        #endregion

        #region CTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractWorkspace" /> class.
        /// </summary>
        /// <param name="container">The injected container.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        protected AbstractWorkspace(IContainerExtension container)
        {
            Container = container;
            _eventAggregator = Container.Resolve<IEventAggregator>();
            _docs = new ObservableCollection<ContentViewModel<ContentModel>>();
            _docs.CollectionChanged += Docs_CollectionChanged;
            _tools = new ObservableCollection<ToolViewModel>();
            _menus = Container.Resolve<IMenuService>();
            _toolbarService = Container.Resolve<IToolbarService>();
            _commandManager = Container.Resolve<ICommandManager>();
        }

        #endregion

        #region IWorkspace Members
        protected IContainerExtension Container { get; }

        /// <summary>
        /// The list of documents which are open in the workspace
        /// </summary>
        /// <value>The documents.</value>
        public virtual ObservableCollection<ContentViewModel<ContentModel>> Documents
        {
            get => _docs;
            set => _docs = value;
        }

        /// <summary>
        /// The list of tools that are available in the workspace
        /// </summary>
        /// <value>The tools.</value>
        public virtual ObservableCollection<ToolViewModel> Tools
        {
            get => _tools;
            set => _tools = value;
        }

        /// <summary>
        /// The current document which is active in the workspace
        /// </summary>
        /// <value>The active document.</value>
        public virtual ContentViewModel<ContentModel> ActiveDocument
        {
            get => _activeDocument;
            set
            {
                if (_activeDocument != value)
                {
                    _activeDocument = value;
                    RaisePropertyChanged("ActiveDocument");
                    _commandManager.Refresh();
                    _menus.Refresh();
                    _toolbarService.Refresh();
                    _eventAggregator.GetEvent<ActiveContentChangedEvent>().Publish(_activeDocument);
                }
            }
        }

        /// <summary>
        /// Gets the title of the application.
        /// </summary>
        /// <value>The title.</value>
        public virtual String Title { get; protected set; } = "Wider";

        /// <summary>
        /// Gets the icon of the application.
        /// </summary>
        /// <value>The icon.</value>
        public virtual ImageSource Icon { get; protected set; }

        /// <summary>
        /// Closing this instance.
        /// </summary>
        /// <param name="e">The <see cref="CancelEventArgs" /> instance containing the event data.</param>
        /// <returns><c>true</c> if the application is closing, <c>false</c> otherwise</returns>
        public virtual Boolean Closing(CancelEventArgs e)
        {
            for (Int32 i = 0; i < Documents.Count; i++)
            {
                ContentViewModel<ContentModel> vm = Documents[i];
                if (vm.Model.IsDirty)
                {
                    ActiveDocument = vm;

                    //Execute the close command
                    vm.CloseCommand.Execute(e);

                    //If canceled
                    if (e.Cancel == true)
                    {
                        return false;
                    }

                    //If it was a new view model with no location to save, we have removed the view model from documents - so reduce the count
                    if(vm.Model.Location == null)
                    {
                        i--;
                    }
                }
            }
            return true;
        }

        #endregion

        protected void Docs_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    item.PropertyChanged -= ModelChangedEventHandler;
                }
            }

            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += ModelChangedEventHandler;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (_docs.Count == 0)
                {
                    ActiveDocument = null;
                }
            }
        }

        /// <summary>
        /// The changed event handler when a property on the model changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void ModelChangedEventHandler(Object sender, PropertyChangedEventArgs e)
        {
            _commandManager.Refresh();
            _menus.Refresh();
            _toolbarService.Refresh();
        }
    }
}