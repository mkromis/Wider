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
        private Double _width = 255;
        private Double _height = 128;

        public String Name { get; }

        public Rect Bounds => new Rect(X, Y, Width, Height);
        public Double X { get; private set; }
        public Double Y { get; private set; }
        public Double Width
        {
            get => _width;
            private set
            {
                if(SetProperty(ref _width, value))
                {
                    BoundsChanged?.Invoke(this, null);
                }
            }
        }
        public Double Height
        {
            get => _height;
            private set
            {
                SetProperty(ref _height, value);
                BoundsChanged?.Invoke(this, null);
            }
        }


        public Boolean IsSelected { get; }
        public IEnumerable<IConnection> Connections { get; }
        public IEnumerable<IConnector> Connectors { get; }

        public UIElement Visual { get; set; }

        public NodeViewModel()
        {
            X = new Random().NextDouble() * 1000;
            Y = new Random().NextDouble() * 1000;
            Name = "Test";
        }

        public event EventHandler BoundsChanged;

        public UIElement CreateVisual(VirtualCanvas.Controls.VirtualCanvas parent)
        {
            if (Visual == null)
            {
                Views.Node ui = new Views.Node
                {
                    DataContext = this
                };
                Visual = ui;
            }
            return Visual;
        }

        public void DisposeVisual() => Visual = null;
    }
}
