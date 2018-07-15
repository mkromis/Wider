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
using System.ComponentModel;
using Wide.Interfaces.Controls;

namespace Wide.Core.Settings
{
    [Serializable]
    [Browsable(false)]
    public sealed class ToolbarSettingItem : IToolbar
    {
        public ToolbarSettingItem()
        {
        }

        public ToolbarSettingItem(IToolbar toolbar)
        {
            BandIndex = toolbar.BandIndex;
            Band = toolbar.Band;
            Header = toolbar.Header;
            IsChecked = toolbar.IsChecked;
        }

        public Int32 Band { get; set; }

        public Int32 BandIndex { get; set; }

        public String Header { get; set; }

        public Boolean IsChecked { get; set; }

        public override Boolean Equals(Object obj) => 
            (obj is ToolbarSettingItem item) && Header.Equals(item.Header);

        public override Int32 GetHashCode() => Header.GetHashCode();

        public override String ToString() => Header.ToString();
    }
}