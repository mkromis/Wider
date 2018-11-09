#region License
// Copyright (c) 2018 Mark Kromis
// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to 
// deal in the Software without restriction, including without limitation the 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
// sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.

#endregion

using Fluent;
using Prism.Events;
using Prism.Ioc;
using Prism.Logging;
using System;
using System.Linq;
using System.Windows;
using Wider.Core.Events;
using Wider.Core.Services;
using Wider.Core.Views;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Wider.Shell.Ribbon.Views
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    internal partial class ShellView : RibbonWindow, IShell
    {
        private readonly IContainerExtension _container;
        private IEventAggregator _eventAggregator;
        private ILoggerService _logger;

        public ShellView(IContainerExtension container, IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _container = container;
            _eventAggregator = eventAggregator;
        }

        #region IShell Members
        public void LoadLayout()
        {
            ContentManager content = contentManager.Content as ContentManager;
            content.LoadLayout();
        }

        public void SaveLayout()
        {
            ContentManager content = contentManager.Content as ContentManager;
            content.LoadLayout();
        }
        #endregion

        private void Window_Closing_1(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext != null)
            {
                IWorkspace workspace = DataContext as IWorkspace;
                if (!workspace.Closing(e))
                {
                    e.Cancel = true;
                    return;
                }
                _eventAggregator.GetEvent<WindowClosingEvent>().Publish(this);
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