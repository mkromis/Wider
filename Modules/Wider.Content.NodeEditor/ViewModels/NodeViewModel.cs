using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows;
using Wider.Content.NodeEditor.Events;
using Wider.Content.NodeEditor.Helper;
using Wider.Content.VirtualCanvas.Controls;

namespace Wider.Content.NodeEditor.ViewModels
{
    /// <summary>
    /// Defines a node in the view-model.
    /// Nodes are connected to other nodes through attached connectors (aka anchor/connection points).
    /// </summary>
    public sealed class NodeViewModel : BindableBase, IVirtualChild
    {
        #region Private Data Members

        /// <summary>
        /// The name of the node.
        /// </summary>
        private String name = String.Empty;

        /// <summary>
        /// The X coordinate for the position of the node.
        /// </summary>
        private Double x = 0;

        /// <summary>
        /// The Y coordinate for the position of the node.
        /// </summary>
        private Double y = 0;

        /// <summary>
        /// The Z index of the node.
        /// </summary>
        private Int32 zIndex = 0;

        /// <summary>
        /// The size of the node.
        /// 
        /// Important Note: 
        ///     The size of a node in the UI is not determined by this property!!
        ///     Instead the size of a node in the UI is determined by the data-template for the Node class.
        ///     When the size is computed via the UI it is then pushed into the view-model
        ///     so that our application code has access to the size of a node.
        /// </summary>
        private Size size = new Size(160,100);

        /// <summary>
        /// List of input connectors (connections points) attached to the node.
        /// </summary>
        private ImpObservableCollection<ConnectorViewModel> inputConnectors = null;

        /// <summary>
        /// List of output connectors (connections points) attached to the node.
        /// </summary>
        private ImpObservableCollection<ConnectorViewModel> outputConnectors = null;

        /// <summary>
        /// Set to 'true' when the node is selected.
        /// </summary>
        private Boolean isSelected = false;

        #endregion Private Data Members

        /// <summary>
        /// The name of the node.
        /// </summary>
        public String Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        /// <summary>
        /// The X coordinate for the position of the node.
        /// </summary>
        public Double X
        {
            get => x;
            set
            {
                if (SetProperty(ref x, value))
                {
                    BoundsChanged?.Invoke(this, null);
                }
            }
        }

        /// <summary>
        /// The Y coordinate for the position of the node.
        /// </summary>
        public Double Y
        {
            get => y;
            set
            {
                if (SetProperty(ref y, value))
                {
                    BoundsChanged?.Invoke(this, null);
                }
            }
        }

        /// <summary>
        /// The Z index of the node.
        /// </summary>
        public Int32 ZIndex
        {
            get => zIndex;
            set => SetProperty(ref zIndex, value);
        }

        /// <summary>
        /// The size of the node.
        /// 
        /// Important Note: 
        ///     The size of a node in the UI is not determined by this property!!
        ///     Instead the size of a node in the UI is determined by the data-template for the Node class.
        ///     When the size is computed via the UI it is then pushed into the view-model
        ///     so that our application code has access to the size of a node.
        /// </summary>
        public Size Size
        {
            get => size;
            set
            {
                if (SetProperty(ref size, value))
                {
                    BoundsChanged?.Invoke(this, null);
                }
            }
        }

        /// <summary>
        /// Event raised when the size of the node is changed.
        /// The size will change when the UI has determined its size based on the contents
        /// of the nodes data-template.  It then pushes the size through to the view-model
        /// and this 'SizeChanged' event occurs.
        /// </summary>
        public event EventHandler BoundsChanged;

        /// <summary>
        /// List of input connectors (connections points) attached to the node.
        /// </summary>
        public ImpObservableCollection<ConnectorViewModel> InputConnectors
        {
            get
            {
                if (inputConnectors == null)
                {
                    inputConnectors = new ImpObservableCollection<ConnectorViewModel>();
                    inputConnectors.ItemsAdded += new EventHandler<CollectionItemsChangedEventArgs>(InputConnectors_ItemsAdded);
                    inputConnectors.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(InputConnectors_ItemsRemoved);
                }

                return inputConnectors;
            }
        }

        /// <summary>
        /// List of output connectors (connections points) attached to the node.
        /// </summary>
        public ImpObservableCollection<ConnectorViewModel> OutputConnectors
        {
            get
            {
                if (outputConnectors == null)
                {
                    outputConnectors = new ImpObservableCollection<ConnectorViewModel>();
                    outputConnectors.ItemsAdded += new EventHandler<CollectionItemsChangedEventArgs>(OutputConnectors_ItemsAdded);
                    outputConnectors.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(OutputConnectors_ItemsRemoved);
                }

                return outputConnectors;
            }
        }

        /// <summary>
        /// A helper property that retrieves a list (a new list each time) of all connections attached to the node. 
        /// </summary>
        public ICollection<ConnectionViewModel> AttachedConnections
        {
            get
            {
                List<ConnectionViewModel> attachedConnections = new List<ConnectionViewModel>();

                foreach (ConnectorViewModel connector in InputConnectors)
                {
                    attachedConnections.AddRange(connector.AttachedConnections);
                }

                foreach (ConnectorViewModel connector in OutputConnectors)
                {
                    attachedConnections.AddRange(connector.AttachedConnections);
                }

                return attachedConnections;
            }
        }

        /// <summary>
        /// Set to 'true' when the node is selected.
        /// </summary>
        public Boolean IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        public Rect Bounds => new Rect(X, Y, Size.Width, Size.Height);
        public UIElement Visual { get; private set; }

        #region Private Methods

        /// <summary>
        /// Event raised when connectors are added to the node.
        /// </summary>
        private void InputConnectors_ItemsAdded(Object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectorViewModel connector in e.Items)
            {
                connector.ParentNode = this;
                connector.Type = ConnectorType.Input;
            }
        }

        /// <summary>
        /// Event raised when connectors are removed from the node.
        /// </summary>
        private void InputConnectors_ItemsRemoved(Object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectorViewModel connector in e.Items)
            {
                connector.ParentNode = null;
                connector.Type = ConnectorType.Undefined;
            }
        }

        /// <summary>
        /// Event raised when connectors are added to the node.
        /// </summary>
        private void OutputConnectors_ItemsAdded(Object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectorViewModel connector in e.Items)
            {
                connector.ParentNode = this;
                connector.Type = ConnectorType.Output;
            }
        }

        /// <summary>
        /// Event raised when connectors are removed from the node.
        /// </summary>
        private void OutputConnectors_ItemsRemoved(Object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectorViewModel connector in e.Items)
            {
                connector.ParentNode = null;
                connector.Type = ConnectorType.Undefined;
            }
        }

        public UIElement CreateVisual(VirtualCanvas.Controls.VirtualCanvas parent)
        {
            if (Visual == null)
            {
                Views.Node item = new Views.Node
                {
                    DataContext = this
                };
                Size = new Size(item.Width, item.Height);
                Visual = item;
            }
            return Visual;
        }
        public void DisposeVisual() { }

        #endregion Private Methods
    }
}
