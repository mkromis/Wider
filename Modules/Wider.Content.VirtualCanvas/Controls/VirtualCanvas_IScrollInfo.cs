using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Wider.Content.VirtualCanvas.Controls
{
    public partial class VirtualCanvas : IScrollInfo
    {
        #region IScrollInfo Members

        /// <summary>
        /// Return whether we are allowed to scroll horizontally.
        /// </summary>
        public Boolean CanHorizontallyScroll { get; set; } = false;

        /// <summary>
        /// Return whether we are allowed to scroll vertically.
        /// </summary>
        public Boolean CanVerticallyScroll { get; set; } = false;

        /// <summary>
        /// The height of the canvas to be scrolled.
        /// </summary>
        public Double ExtentHeight => _extent.Height * Scale.ScaleY;

        /// <summary>
        /// The width of the canvas to be scrolled.
        /// </summary>
        public Double ExtentWidth => _extent.Width * Scale.ScaleX;

        /// <summary>
        /// Scroll down one small scroll increment.
        /// </summary>
        public void LineDown() => SetVerticalOffset(VerticalOffset + (SmallScrollIncrement1.Height * Scale.ScaleX));

        /// <summary>
        /// Scroll left by one small scroll increment.
        /// </summary>
        public void LineLeft() => SetHorizontalOffset(HorizontalOffset - (SmallScrollIncrement1.Width * Scale.ScaleX));

        /// <summary>
        /// Scroll right by one small scroll increment
        /// </summary>
        public void LineRight() => SetHorizontalOffset(HorizontalOffset + (SmallScrollIncrement1.Width * Scale.ScaleX));

        /// <summary>
        /// Scroll up by one small scroll increment
        /// </summary>
        public void LineUp() => SetVerticalOffset(VerticalOffset - (SmallScrollIncrement1.Height * Scale.ScaleX));

        /// <summary>
        /// Make the given visual at the given bounds visible.
        /// </summary>
        /// <param name="visual">The visual that will become visible</param>
        /// <param name="rectangle">The bounds of that visual</param>
        /// <returns>The bounds that is actually visible.</returns>
        public Rect MakeVisible(System.Windows.Media.Visual visual, Rect rectangle)
        {
            if (Zoom != null && visual != this)
            {
                return Zoom.ScrollIntoView(visual as FrameworkElement);
            }
            return rectangle;
        }

        /// <summary>
        /// Scroll down by one mouse wheel increment.
        /// </summary>
        public void MouseWheelDown() => SetVerticalOffset(VerticalOffset + (SmallScrollIncrement1.Height * Scale.ScaleX));

        /// <summary>
        /// Scroll left by one mouse wheel increment.
        /// </summary>
        public void MouseWheelLeft() => SetHorizontalOffset(HorizontalOffset + (SmallScrollIncrement1.Width * Scale.ScaleX));

        /// <summary>
        /// Scroll right by one mouse wheel increment.
        /// </summary>
        public void MouseWheelRight() => SetHorizontalOffset(HorizontalOffset - (SmallScrollIncrement1.Width * Scale.ScaleX));

        /// <summary>
        /// Scroll up by one mouse wheel increment.
        /// </summary>
        public void MouseWheelUp() => SetVerticalOffset(VerticalOffset - (SmallScrollIncrement1.Height * Scale.ScaleX));

        /// <summary>
        /// Page down by one view port height amount.
        /// </summary>
        public void PageDown() => SetVerticalOffset(VerticalOffset + _viewPortSize.Height);

        /// <summary>
        /// Page left by one view port width amount.
        /// </summary>
        public void PageLeft() => SetHorizontalOffset(HorizontalOffset - _viewPortSize.Width);

        /// <summary>
        /// Page right by one view port width amount.
        /// </summary>
        public void PageRight() => SetHorizontalOffset(HorizontalOffset + _viewPortSize.Width);

        /// <summary>
        /// Page up by one view port height amount.
        /// </summary>
        public void PageUp() => SetVerticalOffset(VerticalOffset - _viewPortSize.Height);

        /// <summary>
        /// Return the ScrollViewer that contains this object.
        /// </summary>
        public ScrollViewer ScrollOwner { get; set; }

        /// <summary>
        /// Scroll to the given absolute horizontal scroll position.
        /// </summary>
        /// <param name="offset">The horizontal position to scroll to</param>
        public void SetHorizontalOffset(Double offset)
        {
            Double xoffset = Math.Max(Math.Min(offset, ExtentWidth - ViewportWidth), 0);
            Translate.X = -xoffset;
            OnScrollChanged();
        }

        /// <summary>
        /// Scroll to the given absolute vertical scroll position.
        /// </summary>
        /// <param name="offset">The vertical position to scroll to</param>
        public void SetVerticalOffset(Double offset)
        {
            Double yoffset = Math.Max(Math.Min(offset, ExtentHeight - ViewportHeight), 0);
            Translate.Y = -yoffset;
            OnScrollChanged();
        }

        /// <summary>
        /// Get the current horizontal scroll position.
        /// </summary>
        public Double HorizontalOffset => -Translate.X;

        /// <summary>
        /// Return the current vertical scroll position.
        /// </summary>
        public Double VerticalOffset => -Translate.Y;

        /// <summary>
        /// Return the height of the current viewport that is visible in the ScrollViewer.
        /// </summary>
        public Double ViewportHeight => _viewPortSize.Height;

        /// <summary>
        /// Return the width of the current viewport that is visible in the ScrollViewer.
        /// </summary>
        public Double ViewportWidth => _viewPortSize.Width;

        public Size SmallScrollIncrement1 { get => _smallScrollIncrement; set => _smallScrollIncrement = value; }
        public Int32 Removed { get; set; }

        #endregion
    }
}
