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

namespace JMChart
{
    /// <summary>
    /// 坐标画布
    /// </summary>
    public class CoorCanvas : ChartCanvas
    {
        //坐标图路线组件
        Path coorPath = new Path();
        PathGeometry coorGeometry = new PathGeometry();
 
        //箭头偏移量
        double arrowMargin = 4;
       
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void init()
        {
            base.init();
            VerticalCount = 10;
            HorizontalCount = 10;
            coorPath.Stroke = ForeColor;
          
            coorPath.Data = coorGeometry;          
        }

        /// <summary>
        /// 画图
        /// </summary>
        public override void Draw(bool isClear = false)
        {
            base.Reset();

            DrawCoor();


            base.Draw(false);
        }

        /// <summary>
        /// 计算布局
        /// </summary>
        protected override void SetCanvasLayout()
        {
            //base.SetCanvasLayout();
        }

        /// <summary>
        /// 画坐标图
        /// </summary>
        private void DrawCoor()
        {
            if (!IsDrawBaseLine) return;

            coorGeometry.Figures.Clear();

            var xaxis = new Axis.CoorAxis();
            xaxis.AxisShap = coorPath;
            xaxis.AType = Axis.AxisType.XValue;

            var yaxis = new Axis.CoorAxis();
            yaxis.AType = Axis.AxisType.YValue;
            yaxis.AxisShap = coorPath;
            
            this.Axises.Add(xaxis);
            this.Axises.Add(yaxis);

            var coorfigure = new PathFigure();
            coorGeometry.Figures.Add(coorfigure);
            
            //画上箭头
            yaxis.StartPoint = coorfigure.StartPoint = new Point(Margin.Left, Margin.Top - arrowMargin);
            var tlp = new Point() { X = Margin.Left - arrowMargin, Y = Margin.Top + arrowMargin };
            coorfigure.Segments.Add(new LineSegment() { Point = tlp });
            coorfigure.Segments.Add(new LineSegment() { Point = tlp });
            coorfigure.Segments.Add(new LineSegment() { Point = coorfigure.StartPoint });
            var trp = new Point() { X = Margin.Left + arrowMargin, Y = Margin.Top + arrowMargin };
            coorfigure.Segments.Add(new LineSegment() { Point = trp });
            coorfigure.Segments.Add(new LineSegment() { Point = trp });
            coorfigure.Segments.Add(new LineSegment() { Point = coorfigure.StartPoint });

            //左侧Y轴
            yaxis.EndPoint = xaxis.StartPoint = new Point() { X = Margin.Left, Y = this.Height - Margin.Bottom };
            coorfigure.Segments.Add(new LineSegment() { Point = xaxis.StartPoint });
            //x轴
            xaxis.EndPoint = new Point() { X = this.Width - Margin.Right + arrowMargin, Y = xaxis.StartPoint.Y };
            coorfigure.Segments.Add(new LineSegment() { Point = xaxis.EndPoint });
            
            //画右箭头
            var brtp = new Point() { X = this.Width - Margin.Right - arrowMargin, Y = xaxis.EndPoint.Y - arrowMargin };
            var brbp = new Point() { X = brtp.X, Y = xaxis.EndPoint.Y + arrowMargin };
            coorfigure.Segments.Add(new LineSegment() { Point = brtp });
            coorfigure.Segments.Add(new LineSegment() { Point = brtp });
            coorfigure.Segments.Add(new LineSegment() { Point = xaxis.EndPoint });
            coorfigure.Segments.Add(new LineSegment() { Point = brbp });
            coorfigure.Segments.Add(new LineSegment() { Point = brbp });

            AddChild(coorPath);

            DrawLine();//画虚线
        }

        /// <summary>
        /// 画虚线
        /// </summary>
        private void DrawLine()
        {
            var w = this.Width - Margin.Left - Margin.Right;
            var h = this.Height - Margin.Top - Margin.Bottom;

            var vstep = h / HorizontalCount;
         
            for (var i = 1; i <= HorizontalCount; i++)
            {
                var l = new Line();
                l.StrokeLineJoin = PenLineJoin.Round;
                l.StrokeDashArray.Add(4);
                l.Stroke = DashColor;
                l.StrokeThickness = 1;
                l.X1 = Margin.Left;
                l.Y1 = this.Height - Margin.Bottom - (vstep * i);
                l.X2 = this.Width - Margin.Right;
                l.Y2 = l.Y1;
                AddChild(l);
            }

            var xstep = w / VerticalCount;
            for (var i = 1; i <= VerticalCount; i++)
            {
                var l = new Line();
                l.Stroke = DashColor;
                l.StrokeDashArray.Add(4);
                l.StrokeThickness = 1;
                l.X1 = Margin.Left + xstep * i;
                l.Y1 = Margin.Top;
                l.X2 = l.X1;
                l.Y2 = this.Height - Margin.Bottom;
                AddChild(l);
            }
        }
    }
}
