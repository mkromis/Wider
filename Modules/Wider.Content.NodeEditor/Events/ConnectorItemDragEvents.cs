using System;
using System.Windows;

namespace Wider.Content.NodeEditor.Events
{
    /// <summary>
    /// Arguments for event raised when the user starts to drag a connector out from a node.
    /// </summary>
    internal class ConnectorItemDragStartedEventArgs : RoutedEventArgs
    {
        internal ConnectorItemDragStartedEventArgs(RoutedEvent routedEvent, Object source) :
            base(routedEvent, source)
        {
        }

        /// <summary>
        /// Cancel dragging out of the connector.
        /// </summary>
        public Boolean Cancel
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Defines the event handler for ConnectorDragStarted events.
    /// </summary>
    internal delegate void ConnectorItemDragStartedEventHandler(Object sender, ConnectorItemDragStartedEventArgs e);

    /// <summary>
    /// Arguments for event raised while user is dragging a node in the network.
    /// </summary>
    internal class ConnectorItemDraggingEventArgs : RoutedEventArgs
    {
        public ConnectorItemDraggingEventArgs(RoutedEvent routedEvent, Object source, Double horizontalChange, Double verticalChange) :
            base(routedEvent, source)
        {
            HorizontalChange = horizontalChange;
            VerticalChange = verticalChange;
        }

        /// <summary>
        /// The amount the node has been dragged horizontally.
        /// </summary>
        public Double HorizontalChange { get; } = 0;

        /// <summary>
        /// The amount the node has been dragged vertically.
        /// </summary>
        public Double VerticalChange { get; } = 0;
    }

    /// <summary>
    /// Defines the event handler for ConnectorDragStarted events.
    /// </summary>
    internal delegate void ConnectorItemDraggingEventHandler(Object sender, ConnectorItemDraggingEventArgs e);

    /// <summary>
    /// Arguments for event raised when the user has completed dragging a connector.
    /// </summary>
    internal class ConnectorItemDragCompletedEventArgs : RoutedEventArgs
    {
        public ConnectorItemDragCompletedEventArgs(RoutedEvent routedEvent, Object source) :
            base(routedEvent, source)
        {
        }
    }

    /// <summary>
    /// Defines the event handler for ConnectorDragCompleted events.
    /// </summary>
    internal delegate void ConnectorItemDragCompletedEventHandler(Object sender, ConnectorItemDragCompletedEventArgs e);

}
