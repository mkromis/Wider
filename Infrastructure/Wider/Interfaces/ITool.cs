using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wider.Interfaces.Services;

namespace Wider.Interfaces
{
    public interface ITool
    {
        PaneLocation PreferredLocation { get; }
        Double PreferredWidth { get; }
        Double PreferredHeight { get; }

        Boolean IsVisible { get; set; }
    }
}
