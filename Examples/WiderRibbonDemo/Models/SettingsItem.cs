using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wider.Core.Settings;

namespace WiderRibbonDemo.Models
{
    class SettingsItem : AbstractSettingsItem
    {
        public SettingsItem(String title, AbstractSettings settings) : base(title, settings) { }
    }
}
