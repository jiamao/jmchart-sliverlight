using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Silverlight.Common.Controls
{
    /// <summary>
    /// 箭头
    /// </summary>
    public class Arrow:Canvas
    {
        Path path = new Path();
        PathGeometry geometry = new PathGeometry();
        PathFigure figure = new PathFigure();
        LineSegment[] points = new LineSegment[6];
        System.Windows.Media.CompositeTransform transform = new CompositeTransform();

        /// <summary>
        /// 箭头的头向
        /// </summary>
        public enum ArrowDirection
        {
            Up,
            Left,
            Right,
            Down,
            None
        }

        Color foreground;
        /// <summary>
        /// 当前前景色
        /// </summary>
        public Color Foreground
        {
            get {
                return foreground;
            }
            set {
                foreground = value;
                path.Stroke = new SolidColorBrush(foreground);
                //var fillcolor = Color.FromArgb(60, foreground.R, foreground.G, foreground.B);
                path.Fill = new SolidColorBrush(foreground);
            }
        }

        /// <summary>
        /// 旋转角度
        /// </summary>
        public double Rotation
        {
            get
            {
                return transform.Rotation;
            }
            set {
                transform.Rotation = value;
            }
        }

        ArrowDirection direction = ArrowDirection.Up;
        /// <summary>
        /// 箭头的头向
        /// </summary>
        public ArrowDirection Direction
        {
            get {
                return direction;
            }
            set { 
                direction=value;
                switch (direction)
                {
                    case ArrowDirection.Up:
                        {
                            Rotation = 0;
                            break;
                        }
                    case ArrowDirection.Left:
                        {
                            Rotation = -90;
                            break;
                        }
                    case ArrowDirection.Right:
                        {
                            Rotation = 90;
                            break;
                        }
                    case ArrowDirection.Down:
                        {
                            Rotation = 180;
                            break;
                        }
                }
            }
        }

        public Arrow()
        {
            this.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            this.Children.Add(path);

            this.RenderTransformOrigin = new Point(0.5, 0.5);
            this.RenderTransform = transform;

            path.Data = geometry;
            geometry.Figures.Add(figure);
            figure.IsClosed = true;

            Foreground = Colors.Blue;

            for (var i = 0; i < points.Length;i++ )
            {
                points[i]= new LineSegment();
                figure.Segments.Add(points[i]);
            }

            this.SizeChanged += new SizeChangedEventHandler(Arrow_SizeChanged);
        }

        void Arrow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Draw();
        }

        /// <summary>
        /// 开始画箭头
        /// </summary>
        public void Draw()
        {
            if (this.ActualHeight <=0 || this.ActualWidth <= 0) return;
           
            figure.StartPoint = new Point(this.ActualWidth / 2,0);
            var headtop = this.ActualHeight / 3;
            var marginw = this.ActualWidth / 4;

            points[0].Point = new Point(0, headtop);
            points[1].Point = new Point(marginw, headtop);
            points[2].Point = new Point(marginw, this.ActualHeight);
            points[3].Point = new Point(this.ActualWidth - marginw, this.ActualHeight);
            points[4].Point = new Point(this.ActualWidth - marginw, headtop);
            points[5].Point = new Point(this.ActualWidth, headtop);
        }
    }
}
