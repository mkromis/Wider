using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wider.Core.Services;

namespace Wider.Core.Themes
{
    /// <summary>
    /// Class LightTheme
    /// </summary>
    public sealed class BlueTheme : ITheme
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlueTheme"/> class.
        /// </summary>
        public BlueTheme()
        {
            UriList = new List<Uri>
            {
                new Uri("pack://application:,,,/Xceed.Wpf.AvalonDock.Themes.VS2013;component/BlueTheme.xaml"),
            };
        }

        #region ITheme Members

        /// <summary>
        /// Lists of valid URIs which will be loaded in the theme dictionary
        /// </summary>
        /// <value>The URI list.</value>
        public IList<Uri> UriList { get; internal set; }

        /// <summary>
        /// The name of the theme - "Light"
        /// </summary>
        /// <value>The name.</value>
        public String Name => "Blue";

        #endregion
    }

}