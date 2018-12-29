using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Wider.Content.NodeEditor.ViewModels
{
    public class ConnectionViewModel : BindableBase, IConnection
    {

        public IConnector SourceConnector { get; }
        public IConnector DestinationConnector { get; }
        public Point SourceConnectorHotspot { get; }
        public Point DestinationConnectorHotspot { get; }

        public ConnectionViewModel()
        {

        }
    }
}
