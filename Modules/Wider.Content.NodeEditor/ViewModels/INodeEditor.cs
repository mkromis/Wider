using System.Collections.Generic;

namespace Wider.Content.NodeEditor.ViewModels
{
    public interface INodeEditor 
    {
        IEnumerable<INode> Nodes { get; }
    }
}