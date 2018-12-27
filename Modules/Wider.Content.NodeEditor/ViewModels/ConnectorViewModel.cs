using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Wider.Content.NodeEditor.ViewModels
{
    public class ConnectorViewModel : BindableBase, IConnector
    {
        public INode Parent { get; }
        public IConnection Connection { get; }
        public Point Hotspot { get; }

        public event EventHandler HotspotUpdated;

        public ConnectorViewModel()
        {

        }
    }
}
