using System;
using System.Collections.Generic;
using System.Windows;
using Utils;

namespace NetworkModel
{
    /// <summary>
    /// Defines a node in the view-model.
    /// Nodes are connected to other nodes through attached connectors (aka anchor/connection points).
    /// </summary>
    public sealed class NodeViewModel : AbstractModelBase
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
        private Size size = Size.Empty;

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

        public NodeViewModel()
        {
        }

        public NodeViewModel(String name) => this.name = name;

        /// <summary>
        /// The name of the node.
        /// </summary>
        public String Name
        {
            get => name;
            set
            {
                if (name == value)
                {
                    return;
                }

                name = value;

                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// The X coordinate for the position of the node.
        /// </summary>
        public Double X
        {
            get => x;
            set
            {
                if (x == value)
                {
                    return;
                }

                x = value;

                OnPropertyChanged("X");
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
                if (y == value)
                {
                    return;
                }

                y = value;

                OnPropertyChanged("Y");
            }
        }

        /// <summary>
        /// The Z index of the node.
        /// </summary>
        public Int32 ZIndex
        {
            get => zIndex;
            set
            {
                if (zIndex == value)
                {
                    return;
                }

                zIndex = value;

                OnPropertyChanged("ZIndex");
            }
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
                if (size == value)
                {
                    return;
                }

                size = value;

                SizeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Event raised when the size of the node is changed.
        /// The size will change when the UI has determined its size based on the contents
        /// of the nodes data-template.  It then pushes the size through to the view-model
        /// and this 'SizeChanged' event occurs.
        /// </summary>
        public event EventHandler<EventArgs> SizeChanged;

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
            set
            {
                if (isSelected == value)
                {
                    return;
                }

                isSelected = value;

                OnPropertyChanged("IsSelected");
            }
        }

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

        #endregion Private Methods
    }
}
