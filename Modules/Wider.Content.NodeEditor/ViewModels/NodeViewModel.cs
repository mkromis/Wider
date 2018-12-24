using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Wider.Content.VirtualCanvas.Controls;

namespace Wider.Content.NodeEditor.ViewModels
{
    public class NodeViewModel : BindableBase, INode, IVirtualChild
    {
        public String Name { get; }
        public Double X { get; }
        public Double Y { get; }
        public Boolean IsSelected { get; }
        public IEnumerable<IConnection> Connections { get; }
        public IEnumerable<IConnector> Connectors { get; }
        public Rect Bounds => new Rect(X, Y, UI?.Width ?? 100, UI?.Height ?? 100);
        public UIElement Visual { get; }

        Views.Node UI = null;

        public NodeViewModel()
        {
            X = 100;
            Y = 100;
            Name = "Test";
        }

        public event EventHandler BoundsChanged;

        public UIElement CreateVisual(VirtualCanvas.Controls.VirtualCanvas parent)
        {
            if (UI == null)
            {
                UI = new Views.Node
                {
                    DataContext = this
                };

                BoundsChanged?.Invoke(this, null);
            }
            return UI;
        }

        public void DisposeVisual()
        {
            UI = null;
        }
    }
}
