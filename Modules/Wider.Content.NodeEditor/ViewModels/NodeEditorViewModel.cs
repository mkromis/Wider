using NetworkModel;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Windows;
using Wider.Content.NodeEditor.Controls;
using Wider.Content.NodeEditor.ViewModels;
using Wider.Content.NodeEditor.Views;
using Wider.Core.Services;

namespace Wider.Content.NodeEditor.ViewModels
{
    /// <summary>
    /// The view-model for the main window.
    /// </summary>
    public class NodeEditorViewModel : ContentViewModel
    {
        #region Internal Data Members

        /// <summary>
        /// This is the network that is displayed in the window.
        /// It is the main part of the view-model.
        /// </summary>
        public NetworkViewModel network = null;


        ///
        /// The X coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        private Double contentOffsetX = 0;

        ///
        /// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        private Double contentOffsetY = 0;

        ///
        /// The width of the content (in content coordinates).
        /// 
        private Double contentWidth = 1000;

        ///
        /// The heigth of the content (in content coordinates).
        /// 
        private Double contentHeight = 1000;

        ///
        /// The width of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// view-model so that the value can be shared with the overview window.
        /// 
        private Double contentViewportWidth = 0;

        ///
        /// The height of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// view-model so that the value can be shared with the overview window.
        /// 
        private Double contentViewportHeight = 0;
        private Double contentScale;

        #endregion Internal Data Members

        public NodeEditorViewModel(IContainerExtension container) : base(container)
        {
            Views.NodeEditor editor = new Views.NodeEditor();
            NetworkControl = editor.networkControl;
            View = editor;
            Title = "Node Editor";

            // Add some test data to the view-model.
            PopulateWithTestData();
        }

        /// <summary>
        /// This is the network that is displayed in the window.
        /// It is the main part of the view-model.
        /// </summary>
        public NetworkViewModel Network
        {
            get => network;
            set => SetProperty(ref network, value);
        }

        ///
        /// The current scale at which the content is being viewed.
        /// 
        public Double ContentScale
        {
            get => contentScale;
            set => SetProperty(ref contentScale, value);
        }

        ///
        /// The X coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        public Double ContentOffsetX
        {
            get => contentOffsetX;
            set => SetProperty(ref contentOffsetX, value);
        }

        ///
        /// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        public Double ContentOffsetY
        {
            get => contentOffsetY;
            set => SetProperty(ref contentOffsetY, value);
        }

        ///
        /// The width of the content (in content coordinates).
        /// 
        public Double ContentWidth
        {
            get => contentWidth;
            set => SetProperty(ref contentWidth, value);
        }

        ///
        /// The heigth of the content (in content coordinates).
        /// 
        public Double ContentHeight
        {
            get => contentHeight;
            set => SetProperty(ref contentHeight, value);
        }

        ///
        /// The width of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// view-model so that the value can be shared with the overview window.
        /// 
        public Double ContentViewportWidth
        {
            get => contentViewportWidth;
            set => SetProperty(ref contentViewportWidth, value);
        }

        ///
        /// The heigth of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// view-model so that the value can be shared with the overview window.
        /// 
        public Double ContentViewportHeight
        {
            get => contentViewportHeight;
            set => SetProperty(ref contentViewportHeight, value);
        }

        public NetworkView NetworkControl { get; }

        /// <summary>
        /// Called when the user has started to drag out a connector, thus creating a new connection.
        /// </summary>
        public ConnectionViewModel ConnectionDragStarted(ConnectorViewModel draggedOutConnector, Point curDragPoint)
        {
            //
            // Create a new connection to add to the view-model.
            //
            ConnectionViewModel connection = new ConnectionViewModel();

            if (draggedOutConnector.Type == ConnectorType.Output)
            {
                //
                // The user is dragging out a source connector (an output) and will connect it to a destination connector (an input).
                //
                connection.SourceConnector = draggedOutConnector;
                connection.DestConnectorHotspot = curDragPoint;
            }
            else
            {
                //
                // The user is dragging out a destination connector (an input) and will connect it to a source connector (an output).
                //
                connection.DestConnector = draggedOutConnector;
                connection.SourceConnectorHotspot = curDragPoint;
            }

            //
            // Add the new connection to the view-model.
            //
            Network.Connections.Add(connection);

            return connection;
        }

        /// <summary>
        /// Called to query the application for feedback while the user is dragging the connection.
        /// </summary>
        public void QueryConnnectionFeedback(ConnectorViewModel draggedOutConnector, ConnectorViewModel draggedOverConnector, out Object feedbackIndicator, out Boolean connectionOk)
        {
            if (draggedOutConnector == draggedOverConnector)
            {
                //
                // Can't connect to self!
                // Provide feedback to indicate that this connection is not valid!
                //
                feedbackIndicator = new ConnectionBadIndicator();
                connectionOk = false;
            }
            else
            {
                ConnectorViewModel sourceConnector = draggedOutConnector;
                ConnectorViewModel destConnector = draggedOverConnector;

                //
                // Only allow connections from output connector to input connector (ie each
                // connector must have a different type).
                // Also only allocation from one node to another, never one node back to the same node.
                //
                connectionOk = sourceConnector.ParentNode != destConnector.ParentNode &&
                                 sourceConnector.Type != destConnector.Type;

                if (connectionOk)
                {
                    // 
                    // Yay, this is a valid connection!
                    // Provide feedback to indicate that this connection is ok!
                    //
                    feedbackIndicator = new ConnectionOkIndicator();
                }
                else
                {
                    //
                    // Connectors with the same connector type (eg input & input, or output & output)
                    // can't be connected.
                    // Only connectors with separate connector type (eg input & output).
                    // Provide feedback to indicate that this connection is not valid!
                    //
                    feedbackIndicator = new ConnectionBadIndicator();
                }
            }
        }

        /// <summary>
        /// Called as the user continues to drag the connection.
        /// </summary>
        public void ConnectionDragging(Point curDragPoint, ConnectionViewModel connection)
        {
            if (connection.DestConnector == null)
            {
                connection.DestConnectorHotspot = curDragPoint;
            }
            else
            {
                connection.SourceConnectorHotspot = curDragPoint;
            }
        }

        /// <summary>
        /// Called when the user has finished dragging out the new connection.
        /// </summary>
        public void ConnectionDragCompleted(ConnectionViewModel newConnection, ConnectorViewModel connectorDraggedOut, ConnectorViewModel connectorDraggedOver)
        {
            if (connectorDraggedOver == null)
            {
                //
                // The connection was unsuccessful.
                // Maybe the user dragged it out and dropped it in empty space.
                //
                Network.Connections.Remove(newConnection);
                return;
            }

            //
            // Only allow connections from output connector to input connector (ie each
            // connector must have a different type).
            // Also only allocation from one node to another, never one node back to the same node.
            //
            Boolean connectionOk = connectorDraggedOut.ParentNode != connectorDraggedOver.ParentNode &&
                                connectorDraggedOut.Type != connectorDraggedOver.Type;

            if (!connectionOk)
            {
                //
                // Connections between connectors that have the same type,
                // eg input -> input or output -> output, are not allowed,
                // Remove the connection.
                //
                Network.Connections.Remove(newConnection);
                return;
            }

            //
            // The user has dragged the connection on top of another valid connector.
            //

            //
            // Remove any existing connection between the same two connectors.
            //
            ConnectionViewModel existingConnection = FindConnection(connectorDraggedOut, connectorDraggedOver);
            if (existingConnection != null)
            {
                Network.Connections.Remove(existingConnection);
            }

            //
            // Finalize the connection by attaching it to the connector
            // that the user dragged the mouse over.
            //
            if (newConnection.DestConnector == null)
            {
                newConnection.DestConnector = connectorDraggedOver;
            }
            else
            {
                newConnection.SourceConnector = connectorDraggedOver;
            }
        }

        /// <summary>
        /// Retrieve a connection between the two connectors.
        /// Returns null if there is no connection between the connectors.
        /// </summary>
        public ConnectionViewModel FindConnection(ConnectorViewModel connector1, ConnectorViewModel connector2)
        {
            Trace.Assert(connector1.Type != connector2.Type);

            //
            // Figure out which one is the source connector and which one is the
            // destination connector based on their connector types.
            //
            ConnectorViewModel sourceConnector = connector1.Type == ConnectorType.Output ? connector1 : connector2;
            ConnectorViewModel destConnector = connector1.Type == ConnectorType.Output ? connector2 : connector1;

            //
            // Now we can just iterate attached connections of the source
            // and see if it each one is attached to the destination connector.
            //

            foreach (ConnectionViewModel connection in sourceConnector.AttachedConnections)
            {
                if (connection.DestConnector == destConnector)
                {
                    //
                    // Found a connection that is outgoing from the source connector
                    // and incoming to the destination connector.
                    //
                    return connection;
                }
            }

            return null;
        }

        /// <summary>
        /// Delete the currently selected nodes from the view-model.
        /// </summary>
        public void DeleteSelectedNodes()
        {
            // Take a copy of the selected nodes list so we can delete nodes while iterating.
            NodeViewModel[] nodesCopy = Network.Nodes.ToArray();
            foreach (NodeViewModel node in nodesCopy)
            {
                if (node.IsSelected)
                {
                    DeleteNode(node);
                }
            }
        }

        /// <summary>
        /// Delete the node from the view-model.
        /// Also deletes any connections to or from the node.
        /// </summary>
        public void DeleteNode(NodeViewModel node)
        {
            //
            // Remove all connections attached to the node.
            //
            Network.Connections.RemoveRange(node.AttachedConnections);

            //
            // Remove the node from the network.
            //
            Network.Nodes.Remove(node);
        }

        /// <summary>
        /// Create a node and add it to the view-model.
        /// </summary>
        public NodeViewModel CreateNode(String name, Point nodeLocation, Boolean centerNode)
        {
            NodeViewModel node = new NodeViewModel
            {
                Name = name,
                X = nodeLocation.X,
                Y = nodeLocation.Y
            };

            node.InputConnectors.Add(new ConnectorViewModel("In1"));
            node.InputConnectors.Add(new ConnectorViewModel("In2"));
            node.OutputConnectors.Add(new ConnectorViewModel("Out1"));
            node.OutputConnectors.Add(new ConnectorViewModel("Out2"));

            if (centerNode)
            {
                // 
                // We want to center the node.
                //
                // For this to happen we need to wait until the UI has determined the 
                // size based on the node's data-template.
                //
                // So we define an anonymous method to handle the SizeChanged event for a node.
                //
                // Note: If you don't declare sizeChangedEventHandler before initializing it you will get
                //       an error when you try and unsubscribe the event from within the event handler.
                //
                void sizeChangedEventHandler(Object sender, EventArgs e)
                {
                    //
                    // This event handler will be called after the size of the node has been determined.
                    // So we can now use the size of the node to modify its position.
                    //
                    node.X -= node.Size.Width / 2;
                    node.Y -= node.Size.Height / 2;

                    //
                    // Don't forget to unhook the event, after the initial centering of the node
                    // we don't need to be notified again of any size changes.
                    //
                    node.BoundsChanged -= sizeChangedEventHandler;
                }

                //
                // Now we hook the SizeChanged event so the anonymous method is called later
                // when the size of the node has actually been determined.
                //
                node.BoundsChanged += sizeChangedEventHandler;
            }

            //
            // Add the node to the view-model.
            //
            Network.Nodes.Add(node);
            NetworkControl.AddVirtualChild(node);

            return node;
        }

        /// <summary>
        /// Utility method to delete a connection from the view-model.
        /// </summary>
        public void DeleteConnection(ConnectionViewModel connection) => Network.Connections.Remove(connection);


        #region Private Methods

        /// <summary>
        /// A function to conveniently populate the view-model with test data.
        /// </summary>
        private void PopulateWithTestData()
        {
            //
            // Create a network, the root of the view-model.
            //
            Network = new NetworkViewModel();

            //
            // Create some nodes and add them to the view-model.
            //
            NodeViewModel node1 = CreateNode("Node1", new Point(100, 60), false);
            NodeViewModel node2 = CreateNode("Node2", new Point(350, 80), false);

            //
            // Create a connection between the nodes.
            //
            ConnectionViewModel connection = new ConnectionViewModel
            {
                SourceConnector = node1.OutputConnectors[0],
                DestConnector = node2.InputConnectors[0]
            };

            //
            // Add the connection to the view-model.
            //
            Network.Connections.Add(connection);
        }

        #endregion Private Methods
    }
}
