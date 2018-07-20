﻿#region License

// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Wider.Interfaces;
using Wider.Interfaces.Controls;
using Wider.Interfaces.Services;
using Xceed.Wpf.AvalonDock.Converters;

namespace Wider.Core.Services
{
    /// <summary>
    /// The Wider tool bar service
    /// </summary>
    internal sealed class ToolbarService : AbstractToolbar, IToolbarService
    {
        private static BoolToVisibilityConverter btv = new BoolToVisibilityConverter();
        private AbstractMenuItem menuItem;
        private ToolBarTray tray;

        public ToolbarService() : base("$MAIN$", 0)
        {
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if successfully added, <c>false</c> otherwise</returns>
        public override String Add(AbstractCommandable item)
        {
            if (item is AbstractToolbar tb)
            {
                tb.IsCheckable = true;
                tb.IsChecked = true;
            }
            return base.Add(item);
        }

        /// <summary>
        /// The toolbar tray which will be used in the application
        /// </summary>
        public ToolBarTray ToolBarTray
        {
            get
            {
                if (tray == null)
                {
                    tray = new ToolBarTray
                    {
                        ContextMenu = new ContextMenu()
                    };
                    tray.ContextMenu.ItemsSource = _children;
                    IAddChild child = tray;
                    foreach (AbstractCommandable node in Children)
                    {
                        if (node is AbstractToolbar value)
                        {
                            ToolBar tb = new ToolBar();
                            DataTemplateSelector t =
                                Application.Current.MainWindow.FindResource("toolBarItemTemplateSelector") as
                                DataTemplateSelector;
                            tb.SetValue(ItemsControl.ItemTemplateSelectorProperty, t);

                            //Set the necessary bindings
                            Binding bandBinding = new Binding("Band");
                            Binding bandIndexBinding = new Binding("BandIndex");
                            Binding visibilityBinding = new Binding("IsChecked")
                            {
                                Converter = btv
                            };

                            bandBinding.Source = value;
                            bandIndexBinding.Source = value;
                            visibilityBinding.Source = value;

                            bandBinding.Mode = BindingMode.TwoWay;
                            bandIndexBinding.Mode = BindingMode.TwoWay;

                            tb.SetBinding(ToolBar.BandProperty, bandBinding);
                            tb.SetBinding(ToolBar.BandIndexProperty, bandIndexBinding);
                            tb.SetBinding(ToolBar.VisibilityProperty, visibilityBinding);

                            tb.ItemsSource = value.Children;
                            child.AddChild(tb);
                        }
                    }
                    tray.ContextMenu.ItemContainerStyle =
                        Application.Current.MainWindow.FindResource("ToolbarContextMenu") as Style;
                }
                return tray;
            }
        }

        public AbstractMenuItem RightClickMenu
        {
            get
            {
                if (tray == null)
                {
                    tray = ToolBarTray;
                }
                if (menuItem == null)
                {
                    menuItem = new MenuItemViewModel("_Toolbars", 100);
                    foreach (Object value in tray.ContextMenu.ItemsSource)
                    {
                        AbstractMenuItem menu = value as AbstractMenuItem;
                        menuItem.Add(menu);
                    }
                }
                return menuItem;
            }
        }
    }
}