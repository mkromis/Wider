using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Wider.Content.VirtualCanvas.Controls;
using Wider.Content.VirtualCanvas.Models;

namespace WiderRibbonDemo.Models
{

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
        Double _fontSize;

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
