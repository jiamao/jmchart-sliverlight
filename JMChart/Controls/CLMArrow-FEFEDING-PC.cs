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

namespace JMChart.Controls
{
    /// <summary>
    /// CLM图中的中间箭头
    /// </summary>
    public class CLMArrow
    {
        //箭头偏移量
        const int arrowMargin = 4;

        //生成双向箭头
        Path arrowPath = new Path();
        PathGeometry arrowgeo = new PathGeometry();

        /// <summary>
        /// 填充色
        /// </summary>
        public Brush Fill { get { return this.arrowPath.Fill; } set { this.arrowPath.Fill = value; } }

        /// <summary>
        /// 边框色
        /// </summary>
        public Brush Stroke { get { return this.arrowPath.Stroke; } set { this.arrowPath.Stroke = value; } }

        /// <summary>
        /// 起始点
        /// </summary>
        public Point StartPoint { get; set; }

        /// <summary>
        /// 结束点
        /// </summary>
        public Point EndPoint { get; set; }

        /// <summary>
        /// 当前角度
        /// </summary>
        public double Rotate { get; set; }

        /// <summary>
        /// 起始点名称
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// 目标名称
        /// </summary>
        public string ToName { get; set; }

        /// <summary>
        /// 起始点到目标点值
        /// </summary>
        public string FromValue { get; set; }

        /// <summary>
        /// 目标值到起始点值
        /// </summary>
        public string ToValue { get; set; }

        /// <summary>
        /// 生成显示的元素
        /// </summary>
        /// <returns></returns>
        public void Create(ChartCanvas canvas)
        {
            //生成双向箭头           
            this.arrowPath.Data = arrowgeo;
            arrowgeo.Figures.Clear();
            var fig = new PathFigure();
            arrowgeo.Figures.Add(fig);
            Canvas.SetZIndex(arrowPath, Common.BaseParams.ShapZIndex);

            var rsin = Math.Sin(Rotate);
            var rcos = Math.Cos(Rotate);

            //画起始箭头
            fig.StartPoint = StartPoint;
            var blp = new Point() { X = StartPoint.X - arrowMargin * rsin, Y = StartPoint.Y + arrowMargin + rcos };
            fig.Segments.Add(new LineSegment() { Point = blp });
            fig.Segments.Add(new LineSegment() { Point = blp });
            fig.Segments.Add(new LineSegment() { Point = StartPoint });

            fig.Segments.Add(new LineSegment() { Point = EndPoint });

            var brp = new Point() { X = EndPoint.X + arrowMargin * rsin, Y = EndPoint.Y + arrowMargin * rcos };
            fig.Segments.Add(new LineSegment() { Point = brp });
            fig.Segments.Add(new LineSegment() { Point = brp });
            fig.Segments.Add(new LineSegment() { Point = EndPoint });

            canvas.AddChild(arrowPath);
        }
    }
}
