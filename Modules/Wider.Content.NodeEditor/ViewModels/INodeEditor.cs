using System.Collections.Generic;

namespace Wider.Content.NodeEditor.ViewModels
{
    public interface INodeEditor 
    {
        List<INode> Nodes { get; }
        List<IConnection> Connections { get; }
    }
}