using Prism.Commands;
using Prism.Ioc;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Wider.Content.VirtualCanvas.Gestures;
using Wider.Content.VirtualCanvas.Models;
using Wider.Core.Services;
using WiderRibbonDemo.Models;

namespace WiderRibbonDemo.ViewModels
{
    /// <summary>
    /// This demo shows the VirtualCanvas managing up to 50,000 random WPF shapes providing smooth scrolling and
    /// zooming while creating those shapes on the fly.  This helps make a WPF canvas that is a lot more
    /// scalable.
    /// </summary>
    public class VirtualCanvasViewModel : Wider.Content.VirtualCanvas.ViewModels.VirtualCanvasViewModel
    {
        MapZoom zoom;
        Pan pan;
        RectangleSelectionGesture rectZoom;
        AutoScroll autoScroll;

        bool _showGridLines;
        bool _animateStatus = true;

        double _tileWidth = 50;
        double _tileHeight = 30;
        double _tileMargin = 10;
        int _totalVisuals = 0;
        int rows = 100;
        int cols = 100;

        public VirtualCanvasViewModel(IContainerExtension containerExtension) : base(containerExtension)
        {
            Model = new EmptyModel();

            grid = Graph;// new VirtualCanvas();
            grid.SmallScrollIncrement = new Size(_tileWidth + _tileMargin, _tileHeight + _tileMargin);

            //Scroller.Content = grid;
            //Object v = Scroller.GetValue(ScrollViewer.CanContentScrollProperty);

            Canvas target = grid.ContentCanvas;
            zoom = new MapZoom(target);
            pan = new Pan(target, zoom);
            rectZoom = new RectangleSelectionGesture(target, zoom, ModifierKeys.Control);
            rectZoom.ZoomSelection = true;
            autoScroll = new AutoScroll(target, zoom);
            zoom.ZoomChanged += new EventHandler(OnZoomChanged);

#warning fix zoom slider
            //grid.VisualsChanged += new EventHandler<VisualChangeEventArgs>(OnVisualsChanged);
            //ZoomSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(OnZoomSliderValueChanged);

            grid.Scale.Changed += new EventHandler(OnScaleChanged);
            grid.Translate.Changed += new EventHandler(OnScaleChanged);

            grid.Background = new SolidColorBrush(Color.FromRgb(0xd0, 0xd0, 0xd0));
            grid.ContentCanvas.Background = Brushes.White;

            AllocateNodes();
        }

        private void AllocateNodes()
        {
            zoom.Zoom = 1;
            zoom.Offset = new Point(0, 0);

            // Fill a sparse grid of rectangular color palette nodes with each tile being 50x30.    
            // with hue across x-axis and saturation on y-axis, brightness is fixed at 100;
            Random r = new Random(Environment.TickCount);
            grid.VirtualChildren.Clear();
            double w = _tileWidth + _tileMargin;
            double h = _tileHeight + _tileMargin;
            int count = (rows * cols) / 20;
            double width = (w * (cols - 1));
            double height = (h * (rows - 1));
            while (count > 0)
            {
                double x = r.NextDouble() * width;
                double y = r.NextDouble() * height;

                Point pos = new Point(_tileMargin + x, _tileMargin + y);
                Size s = new Size(r.Next((int)_tileWidth, (int)_tileWidth * 5),
                                    r.Next((int)_tileHeight, (int)_tileHeight * 5));
                TestShapeType type = (TestShapeType)r.Next(0, (int)TestShapeType.Last);

                //Color color = HlsColor.ColorFromHLS((x * 240) / cols, 100, 240 - ((y * 240) / rows));                    
                TestShape shape = new TestShape(new Rect(pos, s), type, r);
                SetRandomBrushes(shape, r);
                grid.AddVirtualChild(shape);
                count--;
            }
        }

        string[] _colorNames = new string[10];
        Brush[] _strokeBrushes = new Brush[10];
        Brush[] _fillBrushes = new Brush[10];

        void SetRandomBrushes(TestShape s, Random r)
        {
            int i = r.Next(0, 10);
            if (_strokeBrushes[i] == null)
            {
                Color color = Color.FromRgb((byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255));
                HlsColor hls = new HlsColor(color);
                Color c1 = hls.Darker(0.25f);
                Color c2 = hls.Lighter(0.25f);
                Brush fill = new LinearGradientBrush(Color.FromArgb(0x80, c1.R, c1.G, c1.B),
                    Color.FromArgb(0x80, color.R, color.G, color.B), 45);
                Brush stroke = new LinearGradientBrush(Color.FromArgb(0x80, color.R, color.G, color.B),
                    Color.FromArgb(0x80, c2.R, c2.G, c2.B), 45);

                _colorNames[i] = "#" + color.R.ToString("X2", CultureInfo.InvariantCulture) +
                    color.G.ToString("X2", CultureInfo.InvariantCulture) +
                    color.B.ToString("X2", CultureInfo.InvariantCulture);
                _strokeBrushes[i] = stroke;
                _fillBrushes[i] = fill;
            }

            s.Label = _colorNames[i];
            s.Stroke = _strokeBrushes[i];
            s.Fill = _fillBrushes[i];
        }

        void OnSaveLog(object sender, RoutedEventArgs e)
        {
#if DEBUG_DUMP
                    SaveFileDialog s = new SaveFileDialog();
                    s.FileName = "quadtree.xml";
                    if (s.ShowDialog() == true)
                    {
                        grid.Dump(s.FileName);
                    }
#else
            MessageBox.Show("You need to build the assembly with 'DEBUG_DUMP' to get this feature");
#endif
        }

        void OnScaleChanged(object sender, EventArgs e)
        {
            // Make the grid lines get thinner as you zoom in
            double t = _gridLines.StrokeThickness = 0.1 / grid.Scale.ScaleX;
            grid.Backdrop.BorderThickness = new Thickness(t);
        }

        int lastTick = Environment.TickCount;
        int addedPerSecond = 0;
        int removedPerSecond = 0;

        void OnVisualsChanged(object sender, VisualChangeEventArgs e)
        {
#warning fix status
            //if (_animateStatus)
            //{
            //    StatusText.Text = string.Format(CultureInfo.InvariantCulture, "{0} live visuals of {1} total", grid.LiveVisualCount, _totalVisuals);

            //    int tick = Environment.TickCount;
            //    if (e.Added != 0 || e.Removed != 0)
            //    {
            //        addedPerSecond += e.Added;
            //        removedPerSecond += e.Removed;
            //        if (tick > lastTick + 100)
            //        {
            //            Created.BeginAnimation(Rectangle.WidthProperty, new DoubleAnimation(
            //                Math.Min(addedPerSecond, 450),
            //                new Duration(TimeSpan.FromMilliseconds(100))));
            //            CreatedLabel.Text = addedPerSecond.ToString(CultureInfo.InvariantCulture) + " created";
            //            addedPerSecond = 0;

            //            Destroyed.BeginAnimation(Rectangle.WidthProperty, new DoubleAnimation(
            //                Math.Min(removedPerSecond, 450),
            //                new Duration(TimeSpan.FromMilliseconds(100))));
            //            DestroyedLabel.Text = removedPerSecond.ToString(CultureInfo.InvariantCulture) + " disposed";
            //            removedPerSecond = 0;
            //        }
            //    }
            //    if (tick > lastTick + 1000)
            //    {
            //        lastTick = tick;
            //    }
            //}
        }

        void OnAnimateStatus(object sender, RoutedEventArgs e)
        {
#warning fix animate status
            //MenuItem item = (MenuItem)sender;
            //_animateStatus = item.IsChecked = !item.IsChecked;

            //StatusText.Text = "";
            //Created.BeginAnimation(Rectangle.WidthProperty, null);
            //Created.Width = 0;
            //CreatedLabel.Text = "";
            //Destroyed.BeginAnimation(Rectangle.WidthProperty, null);
            //Destroyed.Width = 0;
            //DestroyedLabel.Text = "";
        }

        delegate void BooleanEventHandler(bool arg);

        void OnShowQuadTree(object sender, RoutedEventArgs e)
        {
#warning fix quad tree
            //MenuItem item = (MenuItem)sender;
            //item.IsChecked = !item.IsChecked;
            //if (item.IsChecked)
            //{
            //    if (MessageBoxResult.OK == MessageBox.Show("This could take a while...please be patient", "Warning",
            //        MessageBoxButton.OKCancel, MessageBoxImage.Exclamation))
            //    {
            //        StatusText.Text = "Building quad tree visuals...";
            //        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
            //            new BooleanEventHandler(ShowQuadTree), true);
            //    }
            //}
            //else
            //{
            //    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
            //        new BooleanEventHandler(ShowQuadTree), false);
            //}
        }

        void ShowQuadTree(bool arg)
        {
#if DEBUG_DUMP
                    grid.ShowQuadTree(arg);
                    StatusText.Text = "Done.";
#else
            MessageBox.Show("You need to build the assembly with 'DEBUG_DUMP' to get this feature");
#endif
        }

        void OnRowColChange(object sender, RoutedEventArgs e)
        {
#warning fix row col change
            //MenuItem item = sender as MenuItem;
            //int d = int.Parse((string)item.Tag, CultureInfo.InvariantCulture);
            //rows = cols = d;
            //AllocateNodes();
        }

        void OnShowGridLines(object sender, RoutedEventArgs e)
        {
#warning fix show grid lines
            //MenuItem item = (MenuItem)sender;
            //this.ShowGridLines = item.IsChecked = !item.IsChecked;
        }

        Polyline _gridLines = new Polyline();
        private VirtualCanvas grid;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702")]
        public bool ShowGridLines
        {
            get { return _showGridLines; }
            set
            {
                _showGridLines = value;
                if (value)
                {
                    double width = _tileWidth + _tileMargin;
                    double height = _tileHeight + _tileMargin;

                    double numTileToAccumulate = 16;

                    Polyline gridCell = _gridLines;
                    gridCell.Margin = new Thickness(_tileMargin);
                    gridCell.Stroke = Brushes.Blue;
                    gridCell.StrokeThickness = 0.1;
                    gridCell.Points = new PointCollection(new Point[] { new Point(0, height-0.1),
                        new Point(width-0.1, height-0.1), new Point(width-0.1, 0) });
                    VisualBrush gridLines = new VisualBrush(gridCell);
                    gridLines.TileMode = TileMode.Tile;
                    gridLines.Viewport = new Rect(0, 0, 1.0 / numTileToAccumulate, 1.0 / numTileToAccumulate);
                    gridLines.AlignmentX = AlignmentX.Center;
                    gridLines.AlignmentY = AlignmentY.Center;

                    VisualBrush outerVB = new VisualBrush();
                    Rectangle outerRect = new Rectangle();
                    outerRect.Width = 10.0;  //can be any size
                    outerRect.Height = 10.0;
                    outerRect.Fill = gridLines;
                    outerVB.Visual = outerRect;
                    outerVB.Viewport = new Rect(0, 0,
                        width * numTileToAccumulate, height * numTileToAccumulate);
                    outerVB.ViewportUnits = BrushMappingMode.Absolute;
                    outerVB.TileMode = TileMode.Tile;

                    grid.Backdrop.Background = outerVB;

                    Border border = grid.Backdrop;
                    border.BorderBrush = Brushes.Blue;
                    border.BorderThickness = new Thickness(0.1);
                    grid.InvalidateVisual();
                }
                else
                {
                    grid.Backdrop.Background = null;
                }
            }
        }

        void OnZoom(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            string tag = item.Tag as string;
            if (tag == "Fit")
            {
                double scaleX = grid.ViewportWidth / grid.Extent.Width;
                double scaleY = grid.ViewportHeight / grid.Extent.Height;
                zoom.Zoom = Math.Min(scaleX, scaleY);
                zoom.Offset = new Point(0, 0);
            }
            else
            {
                double zoomPercent;
                if (double.TryParse(tag, out zoomPercent))
                {
                    zoom.Zoom = zoomPercent / 100;
                }
            }

        }

        void OnZoomChanged(object sender, EventArgs e)
        {
#warning fix zoom slider
            //if (ZoomSlider.Value != zoom.Zoom)
            //{
            //    ZoomSlider.Value = zoom.Zoom;
            //}
        }


        void OnZoomSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (zoom.Zoom != e.NewValue)
            {
                zoom.Zoom = e.NewValue;
            }
        }

        enum TestShapeType { Ellipse, Curve, Rectangle, Last };

        class TestShape : IVirtualChild
        {
            Rect _bounds;
            public Brush Fill { get; set; }
            public Brush Stroke { get; set; }
            public string Label { get; set; }
            UIElement _visual;
            TestShapeType _shape;
            Point[] _points;

            public event EventHandler BoundsChanged;

            public TestShape(Rect bounds, TestShapeType s, Random r)
            {
                _bounds = bounds;
                _shape = s;
                if (s == TestShapeType.Curve)
                {
                    _bounds.Width *= 2;
                    _bounds.Height *= 2;
                    _points = new Point[3];

                    bounds = new Rect(0, 0, _bounds.Width, _bounds.Height);
                    switch (r.Next(0, 8))
                    {
                        case 0:
                            _points[0] = bounds.TopLeft;
                            _points[1] = bounds.TopRight;
                            _points[2] = bounds.BottomRight;
                            break;
                        case 1:
                            _points[0] = bounds.TopRight;
                            _points[1] = bounds.BottomRight;
                            _points[2] = bounds.BottomLeft;
                            break;
                        case 2:
                            _points[0] = bounds.BottomRight;
                            _points[1] = bounds.BottomLeft;
                            _points[2] = bounds.TopLeft;
                            break;
                        case 3:
                            _points[0] = bounds.BottomLeft;
                            _points[1] = bounds.TopLeft;
                            _points[2] = bounds.TopRight;
                            break;
                        case 4:
                            _points[0] = bounds.TopLeft;
                            _points[1] = new Point(bounds.Right, bounds.Height / 2);
                            _points[2] = bounds.BottomLeft;
                            break;
                        case 5:
                            _points[0] = bounds.TopRight;
                            _points[1] = new Point(bounds.Left, bounds.Height / 2);
                            _points[2] = bounds.BottomRight;
                            break;
                        case 6:
                            _points[0] = bounds.TopLeft;
                            _points[1] = new Point(bounds.Width / 2, bounds.Bottom);
                            _points[2] = bounds.TopRight;
                            break;
                        case 7:
                            _points[0] = bounds.BottomLeft;
                            _points[1] = new Point(bounds.Width / 2, bounds.Top);
                            _points[2] = bounds.BottomRight;
                            break;
                    }
                }
            }

            public UIElement Visual
            {
                get { return _visual; }
            }

            public UIElement CreateVisual(VirtualCanvas parent)
            {
                if (_visual == null)
                {
                    switch (_shape)
                    {
                        case TestShapeType.Curve:
                            {
                                PathGeometry g = new PathGeometry();
                                PathFigure f = new PathFigure();
                                f.StartPoint = _points[0];
                                g.Figures.Add(f);
                                for (int i = 0, n = _points.Length; i < n; i += 3)
                                {
                                    BezierSegment s = new BezierSegment(_points[i], _points[i + 1], _points[i + 2], true);
                                    f.Segments.Add(s);
                                }
                                Path p = new Path();
                                p.Data = g;

                                p.Stroke = Stroke;
                                p.StrokeThickness = 2;

                                //DropShadowBitmapEffect effect = new DropShadowBitmapEffect();
                                //effect.Opacity = 0.8;
                                //effect.ShadowDepth = 3;
                                //effect.Direction = 270;
                                //c.BitmapEffect = effect;
                                _visual = p;
                                break;
                            }
                        case TestShapeType.Ellipse:
                            {
                                Canvas c = new Canvas();

                                Ellipse e = new Ellipse();
                                c.Width = e.Width = _bounds.Width;
                                c.Height = e.Height = _bounds.Height;
                                c.Children.Add(e);

                                Size s = MeasureText(parent, Label);
                                double x = (_bounds.Width - s.Width) / 2;
                                double y = (_bounds.Height - s.Height) / 2;

                                TextBlock text = new TextBlock();
                                text.Text = Label;
                                Canvas.SetLeft(text, x);
                                Canvas.SetTop(text, y);
                                c.Children.Add(text);

                                e.StrokeThickness = 2;
                                e.Stroke = Stroke;
                                e.Fill = Fill;

                                //DropShadowBitmapEffect effect = new DropShadowBitmapEffect();
                                //effect.Opacity = 0.8;
                                //effect.ShadowDepth = 3;
                                //effect.Direction = 270;
                                //c.BitmapEffect = effect;
                                _visual = c;
                                break;
                            }
                        case TestShapeType.Rectangle:
                            {
                                Border b = new Border();
                                b.CornerRadius = new CornerRadius(3);
                                b.Width = _bounds.Width;
                                b.Height = _bounds.Height;
                                TextBlock text = new TextBlock();
                                text.Text = Label;
                                text.VerticalAlignment = VerticalAlignment.Center;
                                text.HorizontalAlignment = HorizontalAlignment.Center;
                                b.Child = text;
                                b.Background = Fill;
                                //DropShadowBitmapEffect effect = new DropShadowBitmapEffect();
                                //effect.Opacity = 0.8;
                                //effect.ShadowDepth = 3;
                                //effect.Direction = 270;
                                //b.BitmapEffect = effect;
                                _visual = b;
                                break;
                            }
                    }
                }
                return _visual;
            }

            public void DisposeVisual()
            {
                _visual = null;
            }

            public Rect Bounds
            {
                get { return _bounds; }
            }

            VirtualCanvas _parent;
            Typeface _typeface;
            double _fontSize;

            public Size MeasureText(VirtualCanvas parent, string label)
            {
                if (_parent != parent)
                {
                    FontFamily fontFamily = (FontFamily)parent.GetValue(TextBlock.FontFamilyProperty);
                    FontStyle fontStyle = (FontStyle)parent.GetValue(TextBlock.FontStyleProperty);
                    FontWeight fontWeight = (FontWeight)parent.GetValue(TextBlock.FontWeightProperty);
                    FontStretch fontStretch = (FontStretch)parent.GetValue(TextBlock.FontStretchProperty);
                    _fontSize = (double)parent.GetValue(TextBlock.FontSizeProperty);
                    _typeface = new Typeface(fontFamily, fontStyle, fontWeight, fontStretch);
                    _parent = parent;
                }
                FormattedText ft = new FormattedText(label, CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight, _typeface, _fontSize, Brushes.Black);
                return new Size(ft.Width, ft.Height);
            }
        }
    }
}
