#region License

// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Wider.Core.Controls;
using Xceed.Wpf.AvalonDock.Controls;

namespace Wider.Core.Converters
{
    public class DocumentContextMenuMixingConverter : IMultiValueConverter
    {
        public Object Convert(Object[] values, Type targetType, Object parameter, CultureInfo culture)
        {
            AbstractMenuItem root = new MenuItemViewModel("$CROOT$", 1);
            Int32 i = 1;
            IReadOnlyCollection<AbstractMenuItem> menus = values[1] as IReadOnlyCollection<AbstractMenuItem>;
            ContextMenu cm;
            if (values[0] is LayoutDocumentItem doc)
            {
                try
                {
                    cm = Application.Current.FindResource("AvalonDock_ThemeVS2012_DocumentContextMenu") as ContextMenu;
                    if (cm != null)
                    {
                        foreach (MenuItem mi in cm.Items)
                        {
                            root.Add(FromMenuItem(mi, doc, i++));
                        }
                    }
                }
                catch
                {
                }

                if (menus != null)
                {
                    foreach (AbstractMenuItem abstractMenuItem in menus)
                    {
                        root.Add(abstractMenuItem);
                    }
                }
            }
            return root.Children;
        }

        public Object[] ConvertBack(Object value, Type[] targetTypes, Object parameter, CultureInfo culture) => 
            throw new NotImplementedException();

        private AbstractMenuItem FromMenuItem(MenuItem item, LayoutDocumentItem doc, Int32 priority)
        {
            Boolean hideDisabled = false;
            if (item != null)
            {
                ICommand cmd = null;
                if (doc != null)
                {
                    if (item.Header.ToString() == Xceed.Wpf.AvalonDock.Properties.Resources.Document_Close)
                    {
                        cmd = doc.CloseCommand;
                    }
                    if (item.Header.ToString() == Xceed.Wpf.AvalonDock.Properties.Resources.Document_CloseAllButThis)
                    {
                        cmd = doc.CloseAllButThisCommand;
                    }
                    if (item.Header.ToString() == Xceed.Wpf.AvalonDock.Properties.Resources.Document_Float)
                    {
                        cmd = doc.FloatCommand;
                        hideDisabled = true;
                    }
                    if (item.Header.ToString() == Xceed.Wpf.AvalonDock.Properties.Resources.Document_DockAsDocument)
                    {
                        cmd = doc.DockAsDocumentCommand;
                        hideDisabled = true;
                    }
                    if (item.Header.ToString() ==
                        Xceed.Wpf.AvalonDock.Properties.Resources.Document_NewHorizontalTabGroup)
                    {
                        cmd = doc.NewHorizontalTabGroupCommand;
                        hideDisabled = true;
                    }
                    if (item.Header.ToString() == Xceed.Wpf.AvalonDock.Properties.Resources.Document_NewVerticalTabGroup)
                    {
                        cmd = doc.NewVerticalTabGroupCommand;
                        hideDisabled = true;
                    }
                    if (item.Header.ToString() == Xceed.Wpf.AvalonDock.Properties.Resources.Document_MoveToNextTabGroup)
                    {
                        cmd = doc.MoveToNextTabGroupCommand;
                        hideDisabled = true;
                    }
                    if (item.Header.ToString() ==
                        Xceed.Wpf.AvalonDock.Properties.Resources.Document_MoveToPreviousTabGroup)
                    {
                        cmd = doc.MoveToPreviousTabGroupCommand;
                        hideDisabled = true;
                    }
                }

                MenuItemViewModel model = new MenuItemViewModel(
                    item.Header.ToString(), priority,
                    item.Icon != null ? (item.Icon as Image).Source : null,
                    cmd, null, false, hideDisabled);
                return model;
            }
            return null;
        }
    }
}