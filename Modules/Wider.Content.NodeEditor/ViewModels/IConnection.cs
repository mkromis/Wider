using System.Windows;

namespace Wider.Content.NodeEditor.ViewModels
{
    public interface IConnection
    {
        IConnector SourceConnector { get; }
        IConnector DestinationConnector { get; }
        Point SourceConnectorHotspot { get; }
        Point DestinationConnectorHotspot { get; }
    }
}