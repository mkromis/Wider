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
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Wider.Core.Events;
using Wider.Core.Services;

namespace Wider.Core.Services
{
    /// <summary>
    /// The main theme manager used in Wider
    /// </summary>
    internal sealed class ThemeManager : IThemeManager
    {
        /// <summary>
        /// The injected event aggregator
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// The injected logger
        /// </summary>
        private readonly ILoggerService _logger;

        /// <summary>
        /// The shell for async method
        /// </summary>
        private readonly IShell _shell;

        /// <summary>
        /// The theme manager constructor
        /// </summary>
        /// <param name="eventAggregator">The injected event aggregator</param>
        /// <param name="logger">The injected logger</param>
        public ThemeManager(IShell shell, IEventAggregator eventAggregator, ILoggerService logger)
        {
            Themes = new ObservableCollection<ITheme>();
            _shell = shell;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        /// <summary>
        /// The current theme set in the theme manager
        /// </summary>
        public ITheme Current { get; private set; }

        #region IThemeManager Members

        /// <summary>
        /// The collection of themes
        /// </summary>
        public ObservableCollection<ITheme> Themes { get; private set; }

        /// <summary>
        /// Set the current theme
        /// </summary>
        /// <param name="name">The name of the theme</param>
        /// <returns>true if the new theme is set, false otherwise</returns>
        public Boolean SetCurrent(String name)
        {
            ITheme newTheme = Themes.Where(x => x.Name == name).FirstOrDefault();
            if (newTheme != null)
            {
                Current = newTheme;

                if (_shell is Window win)
                {
                    win.Dispatcher.InvokeAsync(() =>
                    {
                        // Setup app style
                        ResourceDictionary appTheme =
                            Application.Current.Resources.MergedDictionaries.Count > 0
                            ? Application.Current.Resources.MergedDictionaries[0] : null;

                        if (appTheme == null)
                        {
                            appTheme = new ResourceDictionary();
                            Application.Current.Resources.MergedDictionaries.Add(appTheme);
                        }

                        appTheme.MergedDictionaries.Clear();
                        appTheme.BeginInit();

                        foreach (Uri uri in newTheme.UriList)
                        {
                            ResourceDictionary newDict = new ResourceDictionary { Source = uri };
                            appTheme.MergedDictionaries.Add(newDict);
                        }
                        appTheme.EndInit();

                        _logger.Log($"Theme set to {name}", Category.Info, Priority.None);
                        _eventAggregator.GetEvent<ThemeChangeEvent>().Publish(newTheme);
                    });
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a theme to the theme manager
        /// </summary>
        /// <param name="theme">The theme to add</param>
        /// <returns>true, if successful - false, otherwise</returns>
        public Boolean Add(ITheme theme)
        {
            if (!Themes.Any(x => x.Name == theme.Name))
            {
                Themes.Add(theme);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove theme based on name of theme
        /// </summary>
        /// <param name="theme">name of theme to remove</param>
        /// <returns></returns>
        public Boolean Remove (String theme)
        {
            if (Themes.Any(x => x.Name == theme))
            {
                Themes.Clear();
                Themes.AddRange(Themes.Where(x => x.Name != theme));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the theme manager to a known empty state
        /// </summary>
        public void Clear() => Themes.Clear();
        #endregion
    }
}