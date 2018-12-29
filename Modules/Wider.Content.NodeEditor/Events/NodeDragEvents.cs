using System;
using System.Collections;
using System.Windows;

namespace Wider.Content.NodeEditor.Events
{
    /// <summary>
    /// Base class for node dragging event args.
    /// </summary>
    public class NodeDragEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// The NodeItem's or their DataContext (when non-NULL).
        /// </summary>
        public ICollection nodes = null;

        protected NodeDragEventArgs(RoutedEvent routedEvent, Object source, ICollection nodes) :
            base(routedEvent, source) => this.nodes = nodes;

        /// <summary>
        /// The NodeItem's or their DataContext (when non-NULL).
        /// </summary>
        public ICollection Nodes => nodes;
    }

    /// <summary>
    /// Defines the event handler for NodeDragStarted events.
    /// </summary>
    public delegate void NodeDragEventHandler(Object sender, NodeDragEventArgs e);

    /// <summary>
    /// Arguments for event raised when the user starts to drag a node in the network.
    /// </summary>
    public class NodeDragStartedEventArgs : NodeDragEventArgs
    {
        internal NodeDragStartedEventArgs(RoutedEvent routedEvent, Object source, ICollection nodes) :
            base(routedEvent, source, nodes)
        {
        }

        /// <summary>
        /// Set to 'false' to disallow dragging.
        /// </summary>
        public Boolean Cancel { get; set; } = false;
    }

    /// <summary>
    /// Defines the event handler for NodeDragStarted events.
    /// </summary>
    public delegate void NodeDragStartedEventHandler(Object sender, NodeDragStartedEventArgs e);

    /// <summary>
    /// Arguments for event raised while user is dragging a node in the network.
    /// </summary>
    public class NodeDraggingEventArgs : NodeDragEventArgs
    {
        /// <summary>
        /// The amount the node has been dragged horizontally.
        /// </summary>
        public Double horizontalChange = 0;

        /// <summary>
        /// The amount the node has been dragged vertically.
        /// </summary>
        public Double verticalChange = 0;

        internal NodeDraggingEventArgs(RoutedEvent routedEvent, Object source, ICollection nodes, Double horizontalChange, Double verticalChange) :
            base(routedEvent, source, nodes)
        {
            this.horizontalChange = horizontalChange;
            this.verticalChange = verticalChange;
        }

        /// <summary>
        /// The amount the node has been dragged horizontally.
        /// </summary>
        public Double HorizontalChange => horizontalChange;

        /// <summary>
        /// The amount the node has been dragged vertically.
        /// </summary>
        public Double VerticalChange => verticalChange;
    }

    /// <summary>
    /// Defines the event handler for NodeDragStarted events.
    /// </summary>
    public delegate void NodeDraggingEventHandler(Object sender, NodeDraggingEventArgs e);

    /// <summary>
    /// Arguments for event raised when the user has completed dragging a node in the network.
    /// </summary>
    public class NodeDragCompletedEventArgs : NodeDragEventArgs
    {
        public NodeDragCompletedEventArgs(RoutedEvent routedEvent, Object source, ICollection nodes) :
            base(routedEvent, source, nodes)
        {
        }
    }

    /// <summary>
    /// Defines the event handler for NodeDragCompleted events.
    /// </summary>
    public delegate void NodeDragCompletedEventHandler(Object sender, NodeDragCompletedEventArgs e);
}
