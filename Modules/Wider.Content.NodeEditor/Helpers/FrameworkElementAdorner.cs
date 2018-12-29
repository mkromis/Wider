using System;
using System.Collections;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

//
// This code based on code available here:
//
//  http://www.codeproject.com/KB/WPF/WPFJoshSmith.aspx
//
namespace Wider.Content.NodeEditor.Helper
{
    //
    // This class is an adorner that allows a FrameworkElement derived class to adorn another FrameworkElement.
    //
    public class FrameworkElementAdorner : Adorner
    {

        //
        // Placement of the child.
        //
        private readonly AdornerPlacement horizontalAdornerPlacement = AdornerPlacement.Inside;
        private readonly AdornerPlacement verticalAdornerPlacement = AdornerPlacement.Inside;

        //
        // Offset of the child.
        //
        private readonly Double offsetX = 0.0;
        private readonly Double offsetY = 0.0;

        public FrameworkElementAdorner(FrameworkElement adornerChildElement, FrameworkElement adornedElement)
            : base(adornedElement)
        {
            if (adornedElement == null)
            {
                throw new ArgumentNullException("adornedElement");
            }

            Child = adornerChildElement ?? throw new ArgumentNullException("adornerChildElement");

            base.AddLogicalChild(adornerChildElement);
            base.AddVisualChild(adornerChildElement);
        }

        public FrameworkElementAdorner(FrameworkElement adornerChildElement, FrameworkElement adornedElement,
                                       AdornerPlacement horizontalAdornerPlacement, AdornerPlacement verticalAdornerPlacement,
                                       Double offsetX, Double offsetY)
            : base(adornedElement)
        {
            if (adornedElement == null)
            {
                throw new ArgumentNullException("adornedElement");
            }

            Child = adornerChildElement ?? throw new ArgumentNullException("adornerChildElement");
            this.horizontalAdornerPlacement = horizontalAdornerPlacement;
            this.verticalAdornerPlacement = verticalAdornerPlacement;
            this.offsetX = offsetX;
            this.offsetY = offsetY;

            adornedElement.SizeChanged += new SizeChangedEventHandler(AdornedElement_SizeChanged);

            base.AddLogicalChild(adornerChildElement);
            base.AddVisualChild(adornerChildElement);
        }

        /// <summary>
        /// Event raised when the adorned control's size has changed.
        /// </summary>
        private void AdornedElement_SizeChanged(Object sender, SizeChangedEventArgs e) => InvalidateMeasure();

        //
        // The framework element that is the adorner. 
        //
        public FrameworkElement Child { get; } = null;

        //
        // Position of the child (when not set to NaN).
        //
        public Double PositionX { get; set; } = Double.NaN;

        public Double PositionY { get; set; } = Double.NaN;

        protected override Size MeasureOverride(Size constraint)
        {
            Child.Measure(constraint);
            return Child.DesiredSize;
        }

        /// <summary>
        /// Determine the X coordinate of the child.
        /// </summary>
        private Double DetermineX()
        {
            switch (Child.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    {
                        if (horizontalAdornerPlacement == AdornerPlacement.Mouse)
                        {
                            Double adornerWidth = Child.DesiredSize.Width;
                            Point position = Mouse.GetPosition(AdornerLayer.GetAdornerLayer(AdornedElement));
                            return (position.X - adornerWidth) + offsetX;
                        }
                        else if (horizontalAdornerPlacement == AdornerPlacement.Outside)
                        {
                            return -Child.DesiredSize.Width + offsetX;
                        }
                        else
                        {
                            return offsetX;
                        }
                    }
                case HorizontalAlignment.Right:
                    {
                        if (horizontalAdornerPlacement == AdornerPlacement.Mouse)
                        {
                            Point position = Mouse.GetPosition(AdornerLayer.GetAdornerLayer(AdornedElement));
                            return position.X + offsetX;
                        }
                        else if (horizontalAdornerPlacement == AdornerPlacement.Outside)
                        {
                            Double adornedWidth = AdornedElement.ActualWidth;
                            return adornedWidth + offsetX;
                        }
                        else
                        {
                            Double adornerWidth = Child.DesiredSize.Width;
                            Double adornedWidth = AdornedElement.ActualWidth;
                            Double x = adornedWidth - adornerWidth;
                            return x + offsetX;
                        }
                    }
                case HorizontalAlignment.Center:
                    {
                        Double adornerWidth = Child.DesiredSize.Width;

                        if (horizontalAdornerPlacement == AdornerPlacement.Mouse)
                        {
                            Point position = Mouse.GetPosition(AdornerLayer.GetAdornerLayer(AdornedElement));
                            return (position.X - (adornerWidth / 2)) + offsetX;
                        }
                        else
                        {
                            Double adornedWidth = AdornedElement.ActualWidth;
                            Double x = (adornedWidth / 2) - (adornerWidth / 2);
                            return x + offsetX;
                        }
                    }
                case HorizontalAlignment.Stretch:
                    {
                        return 0.0;
                    }
            }

            return 0.0;
        }

        /// <summary>
        /// Determine the Y coordinate of the child.
        /// </summary>
        private Double DetermineY()
        {
            switch (Child.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    {
                        if (verticalAdornerPlacement == AdornerPlacement.Mouse)
                        {
                            Double adornerWidth = Child.DesiredSize.Width;
                            Point position = Mouse.GetPosition(AdornerLayer.GetAdornerLayer(AdornedElement));
                            return (position.Y - adornerWidth) + offsetY;
                        }
                        else if (verticalAdornerPlacement == AdornerPlacement.Outside)
                        {
                            return -Child.DesiredSize.Height + offsetY;
                        }
                        else
                        {
                            return offsetY;
                        }
                    }
                case VerticalAlignment.Bottom:
                    {
                        if (verticalAdornerPlacement == AdornerPlacement.Mouse)
                        {
                            Point position = Mouse.GetPosition(AdornerLayer.GetAdornerLayer(AdornedElement));
                            return position.Y + offsetY;
                        }
                        else if (verticalAdornerPlacement == AdornerPlacement.Outside)
                        {
                            Double adornedHeight = AdornedElement.ActualHeight;
                            return adornedHeight + offsetY;
                        }
                        else
                        {
                            Double adornerHeight = Child.DesiredSize.Height;
                            Double adornedHeight = AdornedElement.ActualHeight;
                            Double x = adornedHeight - adornerHeight;
                            return x + offsetY;
                        }
                    }
                case VerticalAlignment.Center:
                    {
                        Double adornerHeight = Child.DesiredSize.Height;

                        if (verticalAdornerPlacement == AdornerPlacement.Mouse)
                        {
                            Point position = Mouse.GetPosition(AdornerLayer.GetAdornerLayer(AdornedElement));
                            return (position.Y - (adornerHeight / 2)) + offsetY;
                        }
                        else
                        {
                            Double adornedHeight = AdornedElement.ActualHeight;
                            Double y = (adornedHeight / 2) - (adornerHeight / 2);
                            return y + offsetY;
                        }
                    }
                case VerticalAlignment.Stretch:
                    {
                        return 0.0;
                    }
            }

            return 0.0;
        }

        /// <summary>
        /// Determine the width of the child.
        /// </summary>
        private Double DetermineWidth()
        {
            if (!Double.IsNaN(PositionX))
            {
                return Child.DesiredSize.Width;
            }

            switch (Child.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    {
                        return Child.DesiredSize.Width;
                    }
                case HorizontalAlignment.Right:
                    {
                        return Child.DesiredSize.Width;
                    }
                case HorizontalAlignment.Center:
                    {
                        return Child.DesiredSize.Width;
                    }
                case HorizontalAlignment.Stretch:
                    {
                        return AdornedElement.ActualWidth;
                    }
            }

            return 0.0;
        }

        /// <summary>
        /// Determine the height of the child.
        /// </summary>
        private Double DetermineHeight()
        {
            if (!Double.IsNaN(PositionY))
            {
                return Child.DesiredSize.Height;
            }

            switch (Child.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    {
                        return Child.DesiredSize.Height;
                    }
                case VerticalAlignment.Bottom:
                    {
                        return Child.DesiredSize.Height;
                    }
                case VerticalAlignment.Center:
                    {
                        return Child.DesiredSize.Height;
                    }
                case VerticalAlignment.Stretch:
                    {
                        return AdornedElement.ActualHeight;
                    }
            }

            return 0.0;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Double x = PositionX;
            if (Double.IsNaN(x))
            {
                x = DetermineX();
            }
            Double y = PositionY;
            if (Double.IsNaN(y))
            {
                y = DetermineY();
            }
            Double adornerWidth = DetermineWidth();
            Double adornerHeight = DetermineHeight();
            Child.Arrange(new Rect(x, y, adornerWidth, adornerHeight));
            return finalSize;
        }

        protected override Int32 VisualChildrenCount => 1;

        protected override Visual GetVisualChild(Int32 index) => Child;

        protected override IEnumerator LogicalChildren
        {
            get
            {
                ArrayList list = new ArrayList
                {
                    Child
                };
                return (IEnumerator)list.GetEnumerator();
            }
        }

        /// <summary>
        /// Disconnect the child element from the visual tree so that it may be reused later.
        /// </summary>
        public void DisconnectChild()
        {
            base.RemoveLogicalChild(Child);
            base.RemoveVisualChild(Child);
        }

        /// <summary>
        /// Override AdornedElement from base class for less type-checking.
        /// </summary>
        public new FrameworkElement AdornedElement => (FrameworkElement)base.AdornedElement;
    }
}
