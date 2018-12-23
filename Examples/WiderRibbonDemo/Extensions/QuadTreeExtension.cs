using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wider.Content.VirtualCanvas.Models;
using WiderRibbonDemo.Models;
using WiderRibbonDemo.Models.VirtualCanvasModels;

namespace WiderRibbonDemo.Extensions
{
    public static class QuadTreeExtension
    {
        public static void Dump<T>(this QuadTree<T> source, LogWriter w) where T : class
        {
            if (source.Root != null)
            {
                source.Root.Dump(w);
            }
        }
    }
}
