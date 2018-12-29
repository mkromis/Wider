using System;
using System.Collections.Generic;
using Wider.Content.NodeEditor.Events;

namespace Wider.Content.NodeEditor.Controls
{
    /// <summary>
    /// Partial definition of the NetworkView class.
    /// This file only contains private members related to dragging nodes.
    /// </summary>
    public partial class NetworkView
    {
        #region Private Methods

        /// <summary>
        /// Event raised when the user starts to drag a node.
        /// </summary>
        private void NodeItem_DragStarted(Object source, NodeDragStartedEventArgs e)
        {
            e.Handled = true;

            IsDragging = true;
            IsNotDragging = false;
            IsDraggingNode = true;
            IsNotDraggingNode = false;

            NodeDragStartedEventArgs eventArgs = new NodeDragStartedEventArgs(NodeDragStartedEvent, this, SelectedNodes);
            RaiseEvent(eventArgs);

            e.Cancel = eventArgs.Cancel;
        }

        /// <summary>
        /// Event raised while the user is dragging a node.
        /// </summary>
        private void NodeItem_Dragging(Object source, NodeDraggingEventArgs e)
        {
            e.Handled = true;

            //
            // Cache the NodeItem for each selected node whilst dragging is in progress.
            //
            if (cachedSelectedNodeItems == null)
            {
                cachedSelectedNodeItems = new List<NodeItem>();

                foreach (Object selectedNode in SelectedNodes)
                {
                    NodeItem nodeItem = FindAssociatedNodeItem(selectedNode);
                    if (nodeItem == null)
                    {
                        throw new ApplicationException("Unexpected code path!");
                    }

                    cachedSelectedNodeItems.Add(nodeItem);
                }
            }

            // 
            // Update the position of the node within the Canvas.
            //
            foreach (NodeItem nodeItem in cachedSelectedNodeItems)
            {
                nodeItem.X += e.HorizontalChange;
                nodeItem.Y += e.VerticalChange;
            }

            NodeDraggingEventArgs eventArgs = new NodeDraggingEventArgs(NodeDraggingEvent, this, SelectedNodes, e.HorizontalChange, e.VerticalChange);
            RaiseEvent(eventArgs);
        }

        /// <summary>
        /// Event raised when the user has finished dragging a node.
        /// </summary>
        private void NodeItem_DragCompleted(Object source, NodeDragCompletedEventArgs e)
        {
            e.Handled = true;

            NodeDragCompletedEventArgs eventArgs = new NodeDragCompletedEventArgs(NodeDragCompletedEvent, this, SelectedNodes);
            RaiseEvent(eventArgs);

            if (cachedSelectedNodeItems != null)
            {
                cachedSelectedNodeItems = null;
            }

            IsDragging = false;
            IsNotDragging = true;
            IsDraggingNode = false;
            IsNotDraggingNode = true;
        }

        #endregion Private Methods
    }
}
