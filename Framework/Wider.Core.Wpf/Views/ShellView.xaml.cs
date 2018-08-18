#region License

// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using Autofac;
using Prism.Events;
using Prism.Logging;
using System;
using System.Linq;
using System.Windows;
using Wider.Core.Events;
using Wider.Core.Services;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Wider.Core.Views
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    internal partial class ShellView : Window, IShell
    {
        private readonly IContainer _container;
        private IEventAggregator _eventAggregator;
        private ILoggerService _logger;
        private IWorkspace _workspace;

        public ShellView(IContainer container, IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _container = container;
            _eventAggregator = eventAggregator;
        }

        #region IShell Members

        public void LoadLayout()
        {
            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(dockManager);
            layoutSerializer.LayoutSerializationCallback += (s, e) =>
            {
                _workspace =
                    _container.Resolve<AbstractWorkspace>();

                if (e.Model is LayoutAnchorable anchorable)
                {
                    ToolViewModel model =
                        _workspace.Tools.FirstOrDefault(
                            f => f.ContentId == e.Model.ContentId);
                    if (model != null)
                    {
                        e.Content = model;
                        model.IsVisible = anchorable.IsVisible;
                        model.IsActive = anchorable.IsActive;
                        model.IsSelected = anchorable.IsSelected;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
                if (e.Model is LayoutDocument document)
                {
                    IOpenDocumentService fileService =
                        _container.Resolve<IOpenDocumentService>();
                    ContentViewModel model =
                        fileService.OpenFromID(e.Model.ContentId);
                    if (model != null)
                    {
                        e.Content = model;
                        model.IsActive = document.IsActive;
                        model.IsSelected = document.IsSelected;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            };
            try
            {
                layoutSerializer.Deserialize(@".\AvalonDock.Layout.config");
            }
            catch (Exception)
            {
            }
        }

        public void SaveLayout()
        {
            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(dockManager);
            layoutSerializer.Serialize(@".\AvalonDock.Layout.config");
        }

        #endregion

        private void Window_Closing_1(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            IWorkspace workspace = DataContext as IWorkspace;
            if (!workspace.Closing(e))
            {
                e.Cancel = true;
                return;
            }
            _eventAggregator.GetEvent<WindowClosingEvent>().Publish(this);
        }

        private void DockManager_ActiveContentChanged(Object sender, EventArgs e)
        {
            DockingManager manager = sender as DockingManager;
            ContentViewModel cvm = manager.ActiveContent as ContentViewModel;
            _eventAggregator.GetEvent<ActiveContentChangedEvent>().Publish(cvm);
            if (cvm != null)
            {
                Logger.Log("Active document changed to " + cvm.Title, Category.Info, Priority.None);
            }
        }

        private ILoggerService Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = _container.Resolve<ILoggerService>();
                }

                return _logger;
            }
        }
    }
}