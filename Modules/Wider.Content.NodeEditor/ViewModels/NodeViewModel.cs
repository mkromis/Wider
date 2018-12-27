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
        private Double _width = 120;
        private Double _height = 60;
        private Double _x;
        private Double _y;

        public String Name { get; }

        #region Bounds Support
        // Needed for virtual canvas support
        public Rect Bounds => new Rect(X, Y, Width, Height);
        public event EventHandler BoundsChanged;

        // following needed for easy bounds adjustments
        public Double X
        {
            get => _x;
            set
            {
                if (SetProperty(ref _x, value))
                {
                    BoundsChanged?.Invoke(this, null);
                }
            }
        }
        public Double Y
        {
            get => _y;
            set
            {
                if (SetProperty(ref _y, value))
                {
                    BoundsChanged?.Invoke(this, null);
                }
            }
        }
        public Double Width
        {
            get => _width;
            private set
            {
                if (SetProperty(ref _width, value))
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
        #endregion

        public Boolean IsSelected { get; }
        public List<IConnection> Connections { get; }
        public List<IConnector> Connectors { get; }

        public UIElement Visual { get; set; }

        public NodeViewModel()
        {
            Name = "Test";
            Connectors = new List<IConnector>
            {
                new ConnectorViewModel(),
                new ConnectorViewModel(),
                new ConnectorViewModel(),
                new ConnectorViewModel(),
            };
        }


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
