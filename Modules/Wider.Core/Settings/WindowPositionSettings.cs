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
using System;
using System.Configuration;
using System.Windows;
using Wider.Core.Events;
using Wider.Core.Settings;

namespace Wider.Core.Settings
{
    internal class WindowPositionSettings : AbstractSettings, IWindowPositionSettings
    {
        public WindowPositionSettings(IEventAggregator eventAggregator) => 
            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(SaveWindowPositions);

        private void SaveWindowPositions(Window window)
        {
            if (window.WindowState == WindowState.Normal)
            {
                Left = window.Left;
                Top = window.Top;
                Height = window.Height;
                Width = window.Width;
            }
            State = window.WindowState;
            Save();
        }

        [UserScopedSetting()]
        [DefaultSettingValue("0")]
        public Double Left
        {
            get => (Double)this["Left"];
            set => this["Left"] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue("0")]
        public Double Top
        {
            get => (Double)this["Top"];
            set => this["Top"] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue("600")]
        public Double Height
        {
            get => (Double)this["Height"];
            set => this["Height"] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue("800")]
        public Double Width
        {
            get => (Double)this["Width"];
            set => this["Width"] = value;
        }

        [UserScopedSetting()]
        [DefaultSettingValue("Maximized")]
        public WindowState State
        {
            get => (WindowState)this["State"];
            set => this["State"] = value;
        }
    }
}