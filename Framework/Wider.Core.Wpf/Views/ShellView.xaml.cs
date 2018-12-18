#region License

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
using Prism.Logging;
using Prism.Regions;
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
        private readonly IEventAggregator _eventAggregator;
 
        public ShellView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
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
            // This will be null if not proper initialized.
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
    }
}