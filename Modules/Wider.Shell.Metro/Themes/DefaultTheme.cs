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
using System.Collections.Generic;
using Wider.Core.Services;

namespace Wider.Shell.Themes
{
    /// <summary>
    /// Class DefaultTheme
    /// </summary>
    public sealed class DefaultTheme : ITheme
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTheme"/> class.
        /// </summary>
        public DefaultTheme()
        {
            UriList = new List<Uri>
            {
                new Uri("pack://application:,,,/Xceed.Wpf.AvalonDock.Themes.Aero;component/Theme.xaml"),
                new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml"),
                new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml"),
                new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml"),
                new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml"),
                new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml"),
            };
        }

        #region ITheme Members

        /// <summary>
        /// Lists of valid URIs which will be loaded in the theme dictionary
        /// </summary>
        /// <value>The URI list.</value>
        public IList<Uri> UriList { get; internal set; }

        /// <summary>
        /// The name of the theme - "Default"
        /// </summary>
        /// <value>The name.</value>
        public String Name => "Default";

        #endregion
    }
}