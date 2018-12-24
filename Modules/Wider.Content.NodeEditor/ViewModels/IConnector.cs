using System.Windows;

namespace Wider.Content.NodeEditor.ViewModels
{
    public interface IConnector
    {
        INode Parent { get; }
        IConnection Connection { get;  }
        Point Hotspot { get; }
    }
}