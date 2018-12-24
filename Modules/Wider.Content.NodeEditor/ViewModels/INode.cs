using System;
using System.Collections.Generic;

namespace Wider.Content.NodeEditor.ViewModels
{
    public interface INode
    {
        String Name { get; }
        Double X { get; }
        Double Y { get; }
        Boolean IsSelected { get; }
        IEnumerable<IConnection> Connections { get; }
        IEnumerable<IConnector> Connectors { get; }
    }
}