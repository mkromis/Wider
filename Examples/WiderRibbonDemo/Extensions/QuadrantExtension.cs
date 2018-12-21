using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wider.Content.VirtualCanvas.Models;
using WiderRibbonDemo.Models;

namespace WiderRibbonDemo.Extensions
{
    public static class QuadrantExtension
    {
        public static void Dump<T>(this Quadrant<T> source, LogWriter w) where T : class
        {
            w.WriteAttribute("Bounds", source.Bounds.ToString());
            if (source.Nodes != null)
                {
                    QuadNode<T> n = source.Nodes;
                    do
                    {
                        n = n.Next; // first node.
                        w.Open("node");
                        w.WriteAttribute("Bounds", n.Bounds.ToString());
                        w.Close();
                    } while (n != source.Nodes);
                }
            //DumpQuadrant("TopLeft", _topLeft, w);
            //DumpQuadrant("TopRight", _topRight, w);
            //DumpQuadrant("BottomLeft", _bottomLeft, w);
            //DumpQuadrant("BottomRight", _bottomRight, w);

        }

        private static void DumpQuadrant<T>(String label, Quadrant<T> q, LogWriter w)
        {
        //    if (q != null)
        //    {
        //        w.Open("Quadrant");
        //        w.WriteAttribute("Name", label);
        //        q.Dump(w);
        //        w.Close();
        //    }
        }
    }
}
