using System;
using System.Windows;

namespace Wider.Content.NodeEditor.Events
{
    /// <summary>
    /// Base class for connection dragging event args.
    /// </summary>
    public class ConnectionDragEventArgs : RoutedEventArgs
    {

        #region Private Data Members

        /// <summary>
        /// The connector that will be dragged out.
        /// </summary>
        protected Object connection = null;

        #endregion Private Data Members

        /// <summary>
        /// The NodeItem or it's DataContext (when non-NULL).
        /// </summary>
        public Object Node { get; } = null;

        /// <summary>
        /// The ConnectorItem or it's DataContext (when non-NULL).
        /// </summary>
        public Object ConnectorDraggedOut { get; } = null;

        #region Private Methods

        protected ConnectionDragEventArgs(RoutedEvent routedEvent, Object source, Object node, Object connection, Object connector) :
            base(routedEvent, source)
        {
            Node = node;
            ConnectorDraggedOut = connector;
            this.connection = connection;
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Arguments for event raised when the user starts to drag a connection out from a node.
    /// </summary>
    public class ConnectionDragStartedEventArgs : ConnectionDragEventArgs
    {
        /// <summary>
        /// The connection that will be dragged out.
        /// </summary>
        public Object Connection
        {
            get => connection;
            set => connection = value;
        }

        #region Private Methods

        internal ConnectionDragStartedEventArgs(RoutedEvent routedEvent, Object source, Object node, Object connector) :
            base(routedEvent, source, node, null, connector)
        {
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Defines the event handler for the ConnectionDragStarted event.
    /// </summary>
    public delegate void ConnectionDragStartedEventHandler(Object sender, ConnectionDragStartedEventArgs e);

    /// <summary>
    /// Arguments for event raised while user is dragging a node in the network.
    /// </summary>
    public class QueryConnectionFeedbackEventArgs : ConnectionDragEventArgs
    {

        /// <summary>
        /// The ConnectorItem or it's DataContext (when non-NULL).
        /// </summary>
        public Object DraggedOverConnector { get; } = null;

        /// <summary>
        /// The connection that will be dragged out.
        /// </summary>
        public Object Connection => connection;

        /// <summary>
        /// Set to 'true' / 'false' to indicate that the connection from the dragged out connection to the dragged over connector is valid.
        /// </summary>
        public Boolean ConnectionOk { get; set; } = true;

        /// <summary>
        /// The indicator to display.
        /// </summary>
        public Object FeedbackIndicator { get; set; } = null;

        #region Private Methods

        internal QueryConnectionFeedbackEventArgs(RoutedEvent routedEvent, Object source,
            Object node, Object connection, Object connector, Object draggedOverConnector) :
            base(routedEvent, source, node, connection, connector) => DraggedOverConnector = draggedOverConnector;

        #endregion Private Methods
    }

    /// <summary>
    /// Defines the event handler for the QueryConnectionFeedback event.
    /// </summary>
    public delegate void QueryConnectionFeedbackEventHandler(Object sender, QueryConnectionFeedbackEventArgs e);

    /// <summary>
    /// Arguments for event raised while user is dragging a node in the network.
    /// </summary>
    public class ConnectionDraggingEventArgs : ConnectionDragEventArgs
    {
        /// <summary>
        /// The connection being dragged out.
        /// </summary>
        public Object Connection => connection;

        #region Private Methods

        internal ConnectionDraggingEventArgs(RoutedEvent routedEvent, Object source,
                Object node, Object connection, Object connector) :
            base(routedEvent, source, node, connection, connector)
        {
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Defines the event handler for the ConnectionDragging event.
    /// </summary>
    public delegate void ConnectionDraggingEventHandler(Object sender, ConnectionDraggingEventArgs e);

    /// <summary>
    /// Arguments for event raised when the user has completed dragging a connector.
    /// </summary>
    public class ConnectionDragCompletedEventArgs : ConnectionDragEventArgs
    {
        /// <summary>
        /// The ConnectorItem or it's DataContext (when non-NULL).
        /// </summary>
        public Object ConnectorDraggedOver { get; } = null;

        /// <summary>
        /// The connection that will be dragged out.
        /// </summary>
        public Object Connection => connection;

        #region Private Methods

        internal ConnectionDragCompletedEventArgs(RoutedEvent routedEvent, Object source, Object node, Object connection, Object connector, Object connectorDraggedOver) :
            base(routedEvent, source, node, connection, connector) => ConnectorDraggedOver = connectorDraggedOver;

        #endregion Private Methods
    }

    /// <summary>
    /// Defines the event handler for the ConnectionDragCompleted event.
    /// </summary>
    public delegate void ConnectionDragCompletedEventHandler(Object sender, ConnectionDragCompletedEventArgs e);
}
