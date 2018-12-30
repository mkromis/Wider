using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Wider.Content.NodeEditor.ViewModels;

namespace Wider.Content.NodeEditor.Views
{
    /// <summary>
    /// Interaction logic for OverviewWindow.xaml
    /// </summary>
    public partial class OverviewWindow : Window
    {
        public OverviewWindow() => InitializeComponent();

        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        public NodeEditorViewModel ViewModel => (NodeEditorViewModel)DataContext;

        /// <summary>
        /// Event raised when the ZoomAndPanControl is loaded.
        /// </summary>
        private void Overview_Loaded(Object sender, RoutedEventArgs e)
        {
            //
            // Update the scale so that the entire content fits in the window.
            //
            overview.ScaleToFit();
        }

        /// <summary>
        /// Event raised when the size of the ZoomAndPanControl changes.
        /// </summary>
        private void Overview_SizeChanged(Object sender, SizeChangedEventArgs e)
        {
            //
            // Update the scale so that the entire content fits in the window.
            //
            overview.ScaleToFit();
        }

        /// <summary>
        /// Event raised when the user drags the overview zoom rect.
        /// </summary>
        private void OverviewZoomRectThumb_DragDelta(Object sender, DragDeltaEventArgs e)
        {
            //
            // Update the position of the overview rect as the user drags it around.
            //
            Double newContentOffsetX = Math.Min(Math.Max(0.0, Canvas.GetLeft(overviewZoomRectThumb) + e.HorizontalChange), ViewModel.ContentWidth - ViewModel.ContentViewportWidth);
            Canvas.SetLeft(overviewZoomRectThumb, newContentOffsetX);

            Double newContentOffsetY = Math.Min(Math.Max(0.0, Canvas.GetTop(overviewZoomRectThumb) + e.VerticalChange), ViewModel.ContentHeight - ViewModel.ContentViewportHeight);
            Canvas.SetTop(overviewZoomRectThumb, newContentOffsetY);
        }

        /// <summary>
        /// Event raised on mouse down.
        /// </summary>
        private void Window_MouseDown(Object sender, MouseButtonEventArgs e)
        {
            //
            // Update the position of the overview rect to the point that was clicked.
            //
            Point clickedPoint = e.GetPosition(networkControl);
            Double newX = clickedPoint.X - (overviewZoomRectThumb.Width / 2);
            Double newY = clickedPoint.Y - (overviewZoomRectThumb.Height / 2);
            Canvas.SetLeft(overviewZoomRectThumb, newX);
            Canvas.SetTop(overviewZoomRectThumb, newY);
        }

    }
}
