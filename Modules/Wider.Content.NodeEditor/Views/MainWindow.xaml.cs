﻿using NetworkModel;
using NetworkUI;
using System;
using System.Windows;
using System.Windows.Input;

namespace SampleCode
{
    /// <summary>
    /// This is a Window that uses NetworkView to display a flow-chart.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

        /// <summary>
        /// Event raised when the Window has loaded.
        /// </summary>
        private void MainWindow_Loaded(Object sender, RoutedEventArgs e)
        {
            //
            // Display help text for the sample app.
            //
            HelpTextWindow helpTextWindow = new HelpTextWindow
            {
                Left = Left + Width + 5,
                Top = Top,
                Owner = this
            };
            helpTextWindow.Show();

            OverviewWindow overviewWindow = new OverviewWindow
            {
                Left = Left,
                Top = Top + Height + 5,
                Owner = this,
                DataContext = ViewModel // Pass the view model onto the overview window.
            };
            overviewWindow.Show();
        }

        /// <summary>
        /// Event raised when the user has started to drag out a connection.
        /// </summary>
        private void NetworkControl_ConnectionDragStarted(Object sender, ConnectionDragStartedEventArgs e)
        {
            ConnectorViewModel draggedOutConnector = (ConnectorViewModel)e.ConnectorDraggedOut;
            Point curDragPoint = Mouse.GetPosition(networkControl);

            //
            // Delegate the real work to the view model.
            //
            ConnectionViewModel connection = ViewModel.ConnectionDragStarted(draggedOutConnector, curDragPoint);

            //
            // Must return the view-model object that represents the connection via the event args.
            // This is so that NetworkView can keep track of the object while it is being dragged.
            //
            e.Connection = connection;
        }

        /// <summary>
        /// Event raised, to query for feedback, while the user is dragging a connection.
        /// </summary>
        private void NetworkControl_QueryConnectionFeedback(Object sender, QueryConnectionFeedbackEventArgs e)
        {
            ConnectorViewModel draggedOutConnector = (ConnectorViewModel)e.ConnectorDraggedOut;
            ConnectorViewModel draggedOverConnector = (ConnectorViewModel)e.DraggedOverConnector;

            ViewModel.QueryConnnectionFeedback(draggedOutConnector, draggedOverConnector, out Object feedbackIndicator, out Boolean connectionOk);

            //
            // Return the feedback object to NetworkView.
            // The object combined with the data-template for it will be used to create a 'feedback icon' to
            // display (in an adorner) to the user.
            //
            e.FeedbackIndicator = feedbackIndicator;

            //
            // Let NetworkView know if the connection is ok or not ok.
            //
            e.ConnectionOk = connectionOk;
        }

        /// <summary>
        /// Event raised while the user is dragging a connection.
        /// </summary>
        private void NetworkControl_ConnectionDragging(Object sender, ConnectionDraggingEventArgs e)
        {
            Point curDragPoint = Mouse.GetPosition(networkControl);
            ConnectionViewModel connection = (ConnectionViewModel)e.Connection;
            ViewModel.ConnectionDragging(curDragPoint, connection);
        }

        /// <summary>
        /// Event raised when the user has finished dragging out a connection.
        /// </summary>
        private void NetworkControl_ConnectionDragCompleted(Object sender, ConnectionDragCompletedEventArgs e)
        {
            ConnectorViewModel connectorDraggedOut = (ConnectorViewModel)e.ConnectorDraggedOut;
            ConnectorViewModel connectorDraggedOver = (ConnectorViewModel)e.ConnectorDraggedOver;
            ConnectionViewModel newConnection = (ConnectionViewModel)e.Connection;
            ViewModel.ConnectionDragCompleted(newConnection, connectorDraggedOut, connectorDraggedOver);
        }

        /// <summary>
        /// Event raised to delete the selected node.
        /// </summary>
        private void DeleteSelectedNodes_Executed(Object sender, ExecutedRoutedEventArgs e) => ViewModel.DeleteSelectedNodes();

        /// <summary>
        /// Event raised to create a new node.
        /// </summary>
        private void CreateNode_Executed(Object sender, ExecutedRoutedEventArgs e) => CreateNode();

        /// <summary>
        /// Event raised to delete a node.
        /// </summary>
        private void DeleteNode_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            NodeViewModel node = (NodeViewModel)e.Parameter;
            ViewModel.DeleteNode(node);
        }

        /// <summary>
        /// Event raised to delete a connection.
        /// </summary>
        private void DeleteConnection_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            ConnectionViewModel connection = (ConnectionViewModel)e.Parameter;
            ViewModel.DeleteConnection(connection);
        }

        /// <summary>
        /// Creates a new node in the network at the current mouse location.
        /// </summary>
        private void CreateNode()
        {
            Point newNodePosition = Mouse.GetPosition(networkControl);
            ViewModel.CreateNode("New Node!", newNodePosition, true);
        }

        /// <summary>
        /// Event raised when the size of a node has changed.
        /// </summary>
        private void Node_SizeChanged(Object sender, SizeChangedEventArgs e)
        {
            //
            // The size of a node, as determined in the UI by the node's data-template,
            // has changed.  Push the size of the node through to the view-model.
            //
            FrameworkElement element = (FrameworkElement)sender;
            NodeViewModel node = (NodeViewModel)element.DataContext;
            node.Size = new Size(element.ActualWidth, element.ActualHeight);
        }
    }
}
