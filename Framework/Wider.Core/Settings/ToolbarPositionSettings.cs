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
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using Wider.Core.Controls;
using Wider.Core.Events;
using Wider.Core.Services;

namespace Wider.Core.Settings
{
    internal class ToolbarPositionSettings : AbstractSettings, IToolbarPositionSettings
    {
        private readonly AbstractToolbar _toolTray;
        private readonly Dictionary<String, ToolbarSettingItem> _loadDict;

        public ToolbarPositionSettings(IContainerExtension container)
        {
            container.Resolve<IEventAggregator>().GetEvent<WindowClosingEvent>().Subscribe(SaveToolbarPositions);
            _toolTray = container.Resolve<IToolbarService>() as AbstractToolbar;
            _loadDict = new Dictionary<String, ToolbarSettingItem>();

            if (Toolbars != null && Toolbars.Count > 0)
            {
                foreach (ToolbarSettingItem setting in Toolbars)
                {
                    _loadDict[setting.Header] = setting;
                }

                for (Int32 i = 0; i < _toolTray.Children.Count; i++)
                {
                    AbstractToolbar tb = _toolTray.Children[i] as AbstractToolbar;
                    if (_loadDict.ContainsKey(tb.Header))
                    {
                        ToolbarSettingItem item = _loadDict[tb.Header];
                        tb.Band = item.Band;
                        tb.BandIndex = item.BandIndex;
                        tb.IsChecked = item.IsChecked;
                        tb.Refresh();
                    }
                }
            }
        }

        private void SaveToolbarPositions(Window window)
        {
            Toolbars = new List<ToolbarSettingItem>();
            for (Int32 i = 0; i < _toolTray.Children.Count; i++)
            {
                AbstractToolbar tb = _toolTray.Children[i] as AbstractToolbar;
                Toolbars.Add(new ToolbarSettingItem(tb));
            }
            Save();
        }

        [UserScopedSetting()]
        public List<ToolbarSettingItem> Toolbars
        {
            get
            {
                if ((List<ToolbarSettingItem>)this["Toolbars"] == null)
                {
                    this["Toolbars"] = new List<ToolbarSettingItem>();
                }

                return (List<ToolbarSettingItem>)this["Toolbars"];
            }
            set => this["Toolbars"] = value;
        }
    }
}