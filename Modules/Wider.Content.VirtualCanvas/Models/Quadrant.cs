//-----------------------------------------------------------------------
// <copyright file="QuadTree.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Wider.Content.VirtualCanvas.Models
{

    /// <summary>
    /// The canvas is split up into four Quadrants and objects are stored in the quadrant that contains them
    /// and each quadrant is split up into four child Quadrants recurrsively.  Objects that overlap more than
    /// one quadrant are stored in the _nodes list for this Quadrant.
    /// </summary>
    public class Quadrant<T>
    {
        Rect _bounds; // quadrant bounds.

        QuadNode<T> _nodes; // nodes that overlap the sub quadrant boundaries.

        // The quadrant is subdivided when nodes are inserted that are 
        // completely contained within those subdivisions.
        Quadrant<T> _topLeft;
        Quadrant<T> _topRight;
        Quadrant<T> _bottomLeft;
        Quadrant<T> _bottomRight;

#warning update show quad tree
        public void ShowQuadTree(Canvas c)
        {
            Rectangle r = new Rectangle
            {
                Width = _bounds.Width,
                Height = _bounds.Height
            };

            Canvas.SetLeft(r, _bounds.Left);
            Canvas.SetTop(r, _bounds.Top);
            r.Stroke = Brushes.DarkRed;
            r.StrokeThickness = 1;
            r.StrokeDashArray = new DoubleCollection(new Double[] { 2.0, 3.0 });
            c.Children.Add(r);

            _topLeft?.ShowQuadTree(c);
            _topRight?.ShowQuadTree(c);
            _bottomLeft?.ShowQuadTree(c);
            _bottomRight?.ShowQuadTree(c);
        }

#warning remove export
        //public void Dump(LogWriter w)
        //{
        //    w.WriteAttribute("Bounds", _bounds.ToString());
        //    if (_nodes != null)
        //    {
        //        QuadNode n = _nodes;
        //        do
        //        {
        //            n = n.Next; // first node.
        //            w.Open("node");
        //            w.WriteAttribute("Bounds", n.Bounds.ToString());
        //            w.Close();
        //        } while (n != _nodes);
        //    }
        //    DumpQuadrant("TopLeft", _topLeft, w);
        //    DumpQuadrant("TopRight", _topRight, w);
        //    DumpQuadrant("BottomLeft", _bottomLeft, w);
        //    DumpQuadrant("BottomRight", _bottomRight, w);
        //}

#warning remove export 2
        //public void DumpQuadrant(String label, Quadrant q, LogWriter w)
        //{
        //    if (q != null)
        //    {
        //        w.Open("Quadrant");
        //        w.WriteAttribute("Name", label);
        //        q.Dump(w);
        //        w.Close();
        //    }
        //}

        /// <summary>
        /// Construct new Quadrant with a given bounds all nodes stored inside this quadrant
        /// will fit inside this bounds.  
        /// </summary>
        /// <param name="parent">The parent quadrant (if any)</param>
        /// <param name="bounds">The bounds of this quadrant</param>
        public Quadrant(Quadrant<T> parent, Rect bounds)
        {
            Parent = parent;
            Debug.Assert(bounds.Width != 0 && bounds.Height != 0);
            if (bounds.Width == 0 || bounds.Height == 0)
            {
                // todo: localize
                throw new ArgumentException("Bounds of quadrant cannot be zero width or height");
            }
            _bounds = bounds;
        }

        /// <summary>
        /// The parent Quadrant or null if this is the root
        /// </summary>
        internal Quadrant<T> Parent { get; }

        /// <summary>
        /// The bounds of this quadrant
        /// </summary>
        internal Rect Bounds => _bounds;

        /// <summary>
        /// Insert the given node
        /// </summary>
        /// <param name="node">The node </param>
        /// <param name="bounds">The bounds of that node</param>
        /// <returns></returns>
        internal Quadrant<T> Insert(T node, Rect bounds)
        {
            Debug.Assert(bounds.Width != 0 && bounds.Height != 0);
            if (bounds.Width == 0 || bounds.Height == 0)
            {
                // todo: localize
                throw new ArgumentException("Bounds of quadrant cannot be zero width or height");
            }

            Double w = _bounds.Width / 2;
            if (w == 0)
            {
                w = 1;
            }
            Double h = _bounds.Height / 2;
            if (h == 0)
            {
                h = 1;
            }

            // assumption that the Rect struct is almost as fast as doing the operations
            // manually since Rect is a value type.

            Rect topLeft = new Rect(_bounds.Left, _bounds.Top, w, h);
            Rect topRight = new Rect(_bounds.Left + w, _bounds.Top, w, h);
            Rect bottomLeft = new Rect(_bounds.Left, _bounds.Top + h, w, h);
            Rect bottomRight = new Rect(_bounds.Left + w, _bounds.Top + h, w, h);

            Quadrant<T> child = null;

            // See if any child quadrants completely contain this node.
            if (topLeft.Contains(bounds))
            {
                if (_topLeft == null)
                {
                    _topLeft = new Quadrant<T>(this, topLeft);
                }
                child = _topLeft;
            }
            else if (topRight.Contains(bounds))
            {
                if (_topRight == null)
                {
                    _topRight = new Quadrant<T>(this, topRight);
                }
                child = _topRight;
            }
            else if (bottomLeft.Contains(bounds))
            {
                if (_bottomLeft == null)
                {
                    _bottomLeft = new Quadrant<T>(this, bottomLeft);
                }
                child = _bottomLeft;
            }
            else if (bottomRight.Contains(bounds))
            {
                if (_bottomRight == null)
                {
                    _bottomRight = new Quadrant<T>(this, bottomRight);
                }
                child = _bottomRight;
            }

            if (child != null)
            {
                return child.Insert(node, bounds);
            }
            else
            {
                QuadNode<T> n = new QuadNode<T>(node, bounds);
                if (_nodes == null)
                {
                    n.Next = n;
                }
                else
                {
                    // link up in circular link list.
                    QuadNode<T> x = _nodes;
                    n.Next = x.Next;
                    x.Next = n;
                }
                _nodes = n;
                return this;
            }
        }

        /// <summary>
        /// Returns all nodes in this quadrant that intersect the given bounds.
        /// The nodes are returned in pretty much random order as far as the caller is concerned.
        /// </summary>
        /// <param name="nodes">List of nodes found in the given bounds</param>
        /// <param name="bounds">The bounds that contains the nodes you want returned</param>
        internal void GetIntersectingNodes(List<QuadNode<T>> nodes, Rect bounds)
        {
            if (bounds.IsEmpty) { return; }
            Double w = _bounds.Width / 2;
            Double h = _bounds.Height / 2;

            // assumption that the Rect struct is almost as fast as doing the operations
            // manually since Rect is a value type.

            Rect topLeft = new Rect(_bounds.Left, _bounds.Top, w, h);
            Rect topRight = new Rect(_bounds.Left + w, _bounds.Top, w, h);
            Rect bottomLeft = new Rect(_bounds.Left, _bounds.Top + h, w, h);
            Rect bottomRight = new Rect(_bounds.Left + w, _bounds.Top + h, w, h);

            // See if any child quadrants completely contain this node.
            if (topLeft.IntersectsWith(bounds) && _topLeft != null)
            {
                _topLeft.GetIntersectingNodes(nodes, bounds);
            }

            if (topRight.IntersectsWith(bounds) && _topRight != null)
            {
                _topRight.GetIntersectingNodes(nodes, bounds);
            }

            if (bottomLeft.IntersectsWith(bounds) && _bottomLeft != null)
            {
                _bottomLeft.GetIntersectingNodes(nodes, bounds);
            }

            if (bottomRight.IntersectsWith(bounds) && _bottomRight != null)
            {
                _bottomRight.GetIntersectingNodes(nodes, bounds);
            }

            GetIntersectingNodes(_nodes, nodes, bounds);
        }

        /// <summary>
        /// Walk the given linked list of QuadNodes and check them against the given bounds.
        /// Add all nodes that intersect the bounds in to the list.
        /// </summary>
        /// <param name="last">The last QuadNode in a circularly linked list</param>
        /// <param name="nodes">The resulting nodes are added to this list</param>
        /// <param name="bounds">The bounds to test against each node</param>
        static void GetIntersectingNodes(QuadNode<T> last, List<QuadNode<T>> nodes, Rect bounds)
        {
            if (last != null)
            {
                QuadNode<T> n = last;
                do
                {
                    n = n.Next; // first node.
                    if (n.Bounds.IntersectsWith(bounds))
                    {
                        nodes.Add(n);
                    }
                } while (n != last);
            }
        }

        /// <summary>
        /// Return true if there are any nodes in this Quadrant that intersect the given bounds.
        /// </summary>
        /// <param name="bounds">The bounds to test</param>
        /// <returns>boolean</returns>
        internal Boolean HasIntersectingNodes(Rect bounds)
        {
            if (bounds.IsEmpty) { return false; }
            Double w = _bounds.Width / 2;
            Double h = _bounds.Height / 2;

            // assumption that the Rect struct is almost as fast as doing the operations
            // manually since Rect is a value type.

            Rect topLeft = new Rect(_bounds.Left, _bounds.Top, w, h);
            Rect topRight = new Rect(_bounds.Left + w, _bounds.Top, w, h);
            Rect bottomLeft = new Rect(_bounds.Left, _bounds.Top + h, w, h);
            Rect bottomRight = new Rect(_bounds.Left + w, _bounds.Top + h, w, h);

            Boolean found = false;

            // See if any child quadrants completely contain this node.
            if (topLeft.IntersectsWith(bounds) && _topLeft != null)
            {
                found = _topLeft.HasIntersectingNodes(bounds);
            }

            if (!found && topRight.IntersectsWith(bounds) && _topRight != null)
            {
                found = _topRight.HasIntersectingNodes(bounds);
            }

            if (!found && bottomLeft.IntersectsWith(bounds) && _bottomLeft != null)
            {
                found = _bottomLeft.HasIntersectingNodes(bounds);
            }

            if (!found && bottomRight.IntersectsWith(bounds) && _bottomRight != null)
            {
                found = _bottomRight.HasIntersectingNodes(bounds);
            }
            if (!found)
            {
                found = HasIntersectingNodes(_nodes, bounds);
            }
            return found;
        }

        /// <summary>
        /// Walk the given linked list and test each node against the given bounds/
        /// </summary>
        /// <param name="last">The last node in the circularly linked list.</param>
        /// <param name="bounds">Bounds to test</param>
        /// <returns>Return true if a node in the list intersects the bounds</returns>
        static Boolean HasIntersectingNodes(QuadNode<T> last, Rect bounds)
        {
            if (last != null)
            {
                QuadNode<T> n = last;
                do
                {
                    n = n.Next; // first node.
                    if (n.Bounds.IntersectsWith(bounds))
                    {
                        return true;
                    }
                } while (n != last);
            }
            return false;
        }

        /// <summary>
        /// Remove the given node from this Quadrant.
        /// </summary>
        /// <param name="node">The node to remove</param>
        /// <returns>Returns true if the node was found and removed.</returns>
        internal Boolean RemoveNode(T node)
        {
            Boolean rc = false;
            if (_nodes != null)
            {
                QuadNode<T> p = _nodes;
                while (!p.Next.Node.Equals(node) && p.Next != _nodes)
                {
                    p = p.Next;
                }
                if (p.Next.Node.Equals(node))
                {
                    rc = true;
                    QuadNode<T> n = p.Next;
                    if (p == n)
                    {
                        // list goes to empty
                        _nodes = null;
                    }
                    else
                    {
                        if (_nodes == n) { _nodes = p; }
                        p.Next = n.Next;
                    }
                }
            }
            return rc;
        }

    }
}

