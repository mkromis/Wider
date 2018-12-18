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
using Wider.Content.VirtualCanvas.Controls;
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
        public readonly MapZoom zoom;
        private readonly Pan pan;
        private readonly RectangleSelectionGesture rectZoom;
        private readonly AutoScroll autoScroll;

        private Boolean _showGridLines;
        private readonly Boolean _animateStatus = true;

        private readonly Double _tileWidth = 50;
        private readonly Double _tileHeight = 30;
        private readonly Double _tileMargin = 10;
        private readonly Int16 _totalVisuals = 0;
        private readonly Int16 rows = 100;
        private readonly Int16 cols = 100;

        public Boolean ShowContextRibbon => true;

        public ICommand OnHelpCommand => new DelegateCommand(() =>
        {
            MessageBox.Show(
                "Click left mouse button and drag to pan the view " +
                "Hold Control-Key and run mouse wheel to zoom in and out " +
                "Click middle mouse button to turn on auto-scrolling " +
                "Hold Control-Key and drag the mouse with left button down to draw a rectangle to zoom into that region.",
                "User Interface", MessageBoxButton.OK, MessageBoxImage.Information);
        });

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
            rectZoom = new RectangleSelectionGesture(target, zoom, ModifierKeys.Control)
            {
                ZoomSelection = true
            };
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
            Double w = _tileWidth + _tileMargin;
            Double h = _tileHeight + _tileMargin;
            Int32 count = (rows * cols) / 20;
            Double width = (w * (cols - 1));
            Double height = (h * (rows - 1));
            while (count > 0)
            {
                Double x = r.NextDouble() * width;
                Double y = r.NextDouble() * height;

                Point pos = new Point(_tileMargin + x, _tileMargin + y);
                Size s = new Size(r.Next((Int32)_tileWidth, (Int32)_tileWidth * 5),
                                    r.Next((Int32)_tileHeight, (Int32)_tileHeight * 5));
                TestShapeType type = (TestShapeType)r.Next(0, (Int32)TestShapeType.Last);

                //Color color = HlsColor.ColorFromHLS((x * 240) / cols, 100, 240 - ((y * 240) / rows));                    
                TestShape shape = new TestShape(new Rect(pos, s), type, r);
                SetRandomBrushes(shape, r);
                grid.AddVirtualChild(shape);
                count--;
            }
        }

        private readonly String[] _colorNames = new String[10];
        private readonly Brush[] _strokeBrushes = new Brush[10];
        private readonly Brush[] _fillBrushes = new Brush[10];

        void SetRandomBrushes(TestShape s, Random r)
        {
            Int32 i = r.Next(0, 10);
            if (_strokeBrushes[i] == null)
            {
                Color color = Color.FromRgb((Byte)r.Next(0, 255), (Byte)r.Next(0, 255), (Byte)r.Next(0, 255));
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

        void OnSaveLog(Object sender, RoutedEventArgs e)
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

        void OnScaleChanged(Object sender, EventArgs e)
        {
            // Make the grid lines get thinner as you zoom in
            Double t = _gridLines.StrokeThickness = 0.1 / grid.Scale.ScaleX;
            grid.Backdrop.BorderThickness = new Thickness(t);
        }

        private readonly Int32 lastTick = Environment.TickCount;
        private readonly Int32 addedPerSecond = 0;
        private readonly Int32 removedPerSecond = 0;

        void OnVisualsChanged(Object sender, VisualChangeEventArgs e)
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

        void OnAnimateStatus(Object sender, RoutedEventArgs e)
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

        delegate void BooleanEventHandler(Boolean arg);

        void OnShowQuadTree(Object sender, RoutedEventArgs e)
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

        void ShowQuadTree(Boolean arg)
        {
#if DEBUG_DUMP
                    grid.ShowQuadTree(arg);
                    StatusText.Text = "Done.";
#else
            MessageBox.Show("You need to build the assembly with 'DEBUG_DUMP' to get this feature");
#endif
        }

        void OnRowColChange(Object sender, RoutedEventArgs e)
        {
#warning fix row col change
            //MenuItem item = sender as MenuItem;
            //int d = int.Parse((string)item.Tag, CultureInfo.InvariantCulture);
            //rows = cols = d;
            //AllocateNodes();
        }

        void OnShowGridLines(Object sender, RoutedEventArgs e)
        {
#warning fix show grid lines
            //MenuItem item = (MenuItem)sender;
            //this.ShowGridLines = item.IsChecked = !item.IsChecked;
        }

        private readonly Polyline _gridLines = new Polyline();
        private readonly VirtualCanvas grid;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702")]
        public Boolean ShowGridLines
        {
            get => _showGridLines;
            set
            {
                _showGridLines = value;
                if (value)
                {
                    Double width = _tileWidth + _tileMargin;
                    Double height = _tileHeight + _tileMargin;

                    Double numTileToAccumulate = 16;

                    Polyline gridCell = _gridLines;
                    gridCell.Margin = new Thickness(_tileMargin);
                    gridCell.Stroke = Brushes.Blue;
                    gridCell.StrokeThickness = 0.1;
                    gridCell.Points = new PointCollection(new Point[] { new Point(0, height-0.1),
                        new Point(width-0.1, height-0.1), new Point(width-0.1, 0) });
                    VisualBrush gridLines = new VisualBrush(gridCell)
                    {
                        TileMode = TileMode.Tile,
                        Viewport = new Rect(0, 0, 1.0 / numTileToAccumulate, 1.0 / numTileToAccumulate),
                        AlignmentX = AlignmentX.Center,
                        AlignmentY = AlignmentY.Center
                    };

                    VisualBrush outerVB = new VisualBrush();
                    Rectangle outerRect = new Rectangle
                    {
                        Width = 10.0,  //can be any size
                        Height = 10.0,
                        Fill = gridLines
                    };
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

        void OnZoom(Object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            String tag = item.Tag as String;
            if (tag == "Fit")
            {
                Double scaleX = grid.ViewportWidth / grid.Extent.Width;
                Double scaleY = grid.ViewportHeight / grid.Extent.Height;
                zoom.Zoom = Math.Min(scaleX, scaleY);
                zoom.Offset = new Point(0, 0);
            }
            else
            {
                if (Double.TryParse(tag, out Double zoomPercent))
                {
                    zoom.Zoom = zoomPercent / 100;
                }
            }

        }

        void OnZoomChanged(Object sender, EventArgs e)
        {
#warning fix zoom slider
            //if (ZoomSlider.Value != zoom.Zoom)
            //{
            //    ZoomSlider.Value = zoom.Zoom;
            //}
        }


        void OnZoomSliderValueChanged(Object sender, RoutedPropertyChangedEventArgs<Double> e)
        {
            if (zoom.Zoom != e.NewValue)
            {
                zoom.Zoom = e.NewValue;
            }
        }
    }
}
