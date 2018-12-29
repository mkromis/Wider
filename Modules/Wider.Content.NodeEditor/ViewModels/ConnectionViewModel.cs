using Prism.Mvvm;
using System;
using System.Windows;
using System.Windows.Media;

namespace Wider.Content.NodeEditor.ViewModels
{
    /// <summary>
    /// Defines a connection between two connectors (aka connection points) of two nodes.
    /// </summary>
    public sealed class ConnectionViewModel : BindableBase
    {
        #region Internal Data Members

        /// <summary>
        /// The source connector the connection is attached to.
        /// </summary>
        private ConnectorViewModel sourceConnector = null;

        /// <summary>
        /// The destination connector the connection is attached to.
        /// </summary>
        private ConnectorViewModel destConnector = null;

        /// <summary>
        /// The source and dest hotspots used for generating connection points.
        /// </summary>
        private Point sourceConnectorHotspot;
        private Point destConnectorHotspot;

        /// <summary>
        /// Points that make up the connection.
        /// </summary>
        private PointCollection points = null;

        #endregion Internal Data Members

        /// <summary>
        /// The source connector the connection is attached to.
        /// </summary>
        public ConnectorViewModel SourceConnector
        {
            get => sourceConnector;
            set
            {
                if (sourceConnector == value)
                {
                    return;
                }

                if (sourceConnector != null)
                {
                    sourceConnector.AttachedConnections.Remove(this);
                    sourceConnector.HotspotUpdated -= new EventHandler<EventArgs>(SourceConnector_HotspotUpdated);
                }

                sourceConnector = value;

                if (sourceConnector != null)
                {
                    sourceConnector.AttachedConnections.Add(this);
                    sourceConnector.HotspotUpdated += new EventHandler<EventArgs>(SourceConnector_HotspotUpdated);
                    SourceConnectorHotspot = sourceConnector.Hotspot;
                }

                RaisePropertyChanged("SourceConnector");
                OnConnectionChanged();
            }
        }

        /// <summary>
        /// The destination connector the connection is attached to.
        /// </summary>
        public ConnectorViewModel DestConnector
        {
            get => destConnector;
            set
            {
                if (destConnector == value)
                {
                    return;
                }

                if (destConnector != null)
                {
                    destConnector.AttachedConnections.Remove(this);
                    destConnector.HotspotUpdated -= new EventHandler<EventArgs>(DestConnector_HotspotUpdated);
                }

                destConnector = value;

                if (destConnector != null)
                {
                    destConnector.AttachedConnections.Add(this);
                    destConnector.HotspotUpdated += new EventHandler<EventArgs>(DestConnector_HotspotUpdated);
                    DestConnectorHotspot = destConnector.Hotspot;
                }

                RaisePropertyChanged("DestConnector");
                OnConnectionChanged();
            }
        }

        /// <summary>
        /// The source and dest hotspots used for generating connection points.
        /// </summary>
        public Point SourceConnectorHotspot
        {
            get => sourceConnectorHotspot;
            set
            {
                sourceConnectorHotspot = value;

                ComputeConnectionPoints();

                RaisePropertyChanged("SourceConnectorHotspot");
            }
        }

        public Point DestConnectorHotspot
        {
            get => destConnectorHotspot;
            set
            {
                destConnectorHotspot = value;

                ComputeConnectionPoints();

                RaisePropertyChanged("DestConnectorHotspot");
            }
        }

        /// <summary>
        /// Points that make up the connection.
        /// </summary>
        public PointCollection Points
        {
            get => points;
            set
            {
                points = value;

                RaisePropertyChanged("Points");
            }
        }

        /// <summary>
        /// Event fired when the connection has changed.
        /// </summary>
        public event EventHandler<EventArgs> ConnectionChanged;

        #region Private Methods

        /// <summary>
        /// Raises the 'ConnectionChanged' event.
        /// </summary>
        private void OnConnectionChanged() => ConnectionChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Event raised when the hotspot of the source connector has been updated.
        /// </summary>
        private void SourceConnector_HotspotUpdated(Object sender, EventArgs e) => SourceConnectorHotspot = SourceConnector.Hotspot;

        /// <summary>
        /// Event raised when the hotspot of the dest connector has been updated.
        /// </summary>
        private void DestConnector_HotspotUpdated(Object sender, EventArgs e) => DestConnectorHotspot = DestConnector.Hotspot;

        /// <summary>
        /// Rebuild connection points.
        /// </summary>
        private void ComputeConnectionPoints()
        {
            PointCollection computedPoints = new PointCollection
            {
                SourceConnectorHotspot
            };

            Double deltaX = Math.Abs(DestConnectorHotspot.X - SourceConnectorHotspot.X);
            Double deltaY = Math.Abs(DestConnectorHotspot.Y - SourceConnectorHotspot.Y);
            if (deltaX > deltaY)
            {
                Double midPointX = SourceConnectorHotspot.X + ((DestConnectorHotspot.X - SourceConnectorHotspot.X) / 2);
                computedPoints.Add(new Point(midPointX, SourceConnectorHotspot.Y));
                computedPoints.Add(new Point(midPointX, DestConnectorHotspot.Y));
            }
            else
            {
                Double midPointY = SourceConnectorHotspot.Y + ((DestConnectorHotspot.Y - SourceConnectorHotspot.Y) / 2);
                computedPoints.Add(new Point(SourceConnectorHotspot.X, midPointY));
                computedPoints.Add(new Point(DestConnectorHotspot.X, midPointY));
            }

            computedPoints.Add(DestConnectorHotspot);
            computedPoints.Freeze();

            Points = computedPoints;
        }

        #endregion Private Methods
    }
}
