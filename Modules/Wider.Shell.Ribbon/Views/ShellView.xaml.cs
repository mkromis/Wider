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
using MahApps.Metro.Controls;
using Prism.Events;
using Prism.Ioc;
using System;
using System.ComponentModel;
using System.Windows;
using Wider.Core.Events;
using Wider.Core.Services;
using Wider.Core.Views;
using Wider.Shell.Ribbon.Themes;

namespace Wider.Shell.Ribbon.Views
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    internal partial class ShellView : MetroWindow, IShell, IRibbonWindow
    {
        private readonly IEventAggregator _eventAggregator;

        public ShellView(IContainerExtension container)
        {
            InitializeComponent();

            // save initial resolved items
            _eventAggregator = container.Resolve<IEventAggregator>();
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
        /// <summary>
        /// Initial Metro setup see https://fluentribbon.github.io/documentation/interop_with_MahApps.Metro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            TitleBar = this.FindChild<RibbonTitleBar>("ribbonTitleBar");
            TitleBar.InvalidateArrange();
            TitleBar.UpdateLayout();
        }

        private void Window_Closing(Object sender, CancelEventArgs e)
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

        #region TitelBar
        /// <summary>
        /// Gets ribbon titlebar
        /// </summary>
        public RibbonTitleBar TitleBar
        {
            get => (RibbonTitleBar)GetValue(TitleBarProperty);
            private set => SetValue(TitleBarPropertyKey, value);
        }

        // ReSharper disable once InconsistentNaming
        private static readonly DependencyPropertyKey TitleBarPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(TitleBar), typeof(RibbonTitleBar), typeof(ShellView), new PropertyMetadata());

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="TitleBar"/>.
        /// </summary>
        public static readonly DependencyProperty TitleBarProperty = TitleBarPropertyKey.DependencyProperty;

        #endregion
    }
}