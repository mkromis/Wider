using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wider.Core.Attributes;

namespace Wider.Core.Services
{
    public interface INewFileWindow
    {
        Object DataContext { get; set; }
        NewContentAttribute NewContent { get; set; }

        Boolean? ShowDialog();
    }
}
