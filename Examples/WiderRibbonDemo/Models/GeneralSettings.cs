using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Wider.Core.Services;
using Wider.Core.Settings;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WiderRibbonDemo.Models
{
    class GeneralSettings : AbstractSettings
    {
        private readonly IThemeManager _themeManager;

        public GeneralSettings(IThemeManager themeManager)
        {
            _themeManager = themeManager;
        }

        [UserScopedSetting()]
        [DefaultSettingValue("Default")]
        [Category("Colors")]
        [DisplayName("Theme Style")]
        [Description("The background color of the text editor")]
        [ItemsSource(typeof(ThemeItemSource))]
        public String SelectedTheme
        {
            get => _themeManager.Current.Name;
            set => _themeManager.SetCurrent(value);
        }
    }
}
