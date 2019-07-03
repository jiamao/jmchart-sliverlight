using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace JMChart.Series
{
    /// <summary>
    /// 漏斗图形
    /// </summary>
    public class FunnelSeries:ISeries
    {
        public FunnelSeries(ChartCanvas canvas)
            : base(canvas) { }

        public override System.Collections.Generic.IEnumerable<Shape> CreatePath()
        {
            this.Shaps.Clear();
            if (DataContext == null) return base.CreatePath();
            var data = DataContext as System.Collections.ICollection;
            //获取绑定的属性
            var mapping = GetMapping(Model.ItemMapping.EnumDataMember.Y);
            if (mapping == null) throw new Exception("请设定一个默认的绑定属性.");
            var lengendmapping = GetMapping(Model.ItemMapping.EnumDataMember.Lengend);

            var lst = new System.Collections.Generic.List<Model.DataPoint>();
            foreach (var d in data)
            {
                var p = new Model.DataPoint();
                double v=0;
                var obj = Silverlight.Common.Reflection.ClassHelper.GetPropertyValue(d, mapping.MemberName);
                if (obj != null && double.TryParse(obj.ToString(), out v))
                {
                    p.NumberValue = v;
                }
                if (lengendmapping != null)
                {
                    obj = Silverlight.Common.Reflection.ClassHelper.GetPropertyValue(d, lengendmapping.MemberName);
                    if (obj != null) p.StringValue = obj.ToString();
                }
                lst.Add(p);
                Points.Add(p);
            }
            lst.Sort();
            

            var legwidth = Canvas.Width * 0.3;
            Rect rec = new Rect(Canvas.Margin.Left, Canvas.Margin.Top, Canvas.Width - legwidth,Canvas.Height);

            var itemHeight = rec.Height / lst.Count - lst.Count * 2;
            //var curWidth = rec.Width;
            double maxValue = lst[0].NumberValue.Value;
            var index=0;
            PathFigure lastFig = null;
            foreach (var p in lst)
            {                
                p.Height = itemHeight;                

                p.PotinShape = new Path();
                Shaps.Add(p.PotinShape);

                var color =index >= Canvas.SeriesColors.Length?Canvas.SeriesColors[0] : Canvas.SeriesColors[index];
                color.A = 200;
                p.PotinShape.Fill = new SolidColorBrush(color);
                System.Windows.Controls.Canvas.SetZIndex(p.PotinShape, Common.BaseParams.ShapZIndex);

                p.PotinShape.StrokeThickness = Canvas.LineWidth;
                p.PotinShape.Stroke = new SolidColorBrush(Colors.Black);

                var geo = new PathGeometry() ;
                p.PotinShape.Data = geo;
                var fig = new PathFigure();
                geo.Figures.Add(fig);
                fig.IsClosed = true;

                var per = p.NumberValue.Value / maxValue;
                p.Width  = per * rec.Width;
                var top =rec.Top + index * itemHeight + 2 * index;
                var left = rec.Left + (rec.Width - p.Width) / 2;
                
                p.Position = fig.StartPoint = new Point(left, top);
                var l1 = new LineSegment() { Point = new Point(left + p.Width, top) };
                fig.Segments.Add(l1);
                //确定上一个图的下边框
                if (lastFig != null)
                {
                    var l2 = new LineSegment() { Point = new Point(l1.Point.X, lastFig.StartPoint.Y + itemHeight) };
                    lastFig.Segments.Add(l2);
                    var l3 = new LineSegment() { Point = new Point(left, l2.Point.Y) };
                    lastFig.Segments.Add(l3);                    
                }
                //当为最后一个
                if (index == lst.Count - 1)
                {
                    var bottomwidth = p.Width * 0.7;
                    var l2 = new LineSegment() { Point = new Point(l1.Point.X - (p.Width - bottomwidth) / 2, top + itemHeight) };
                    fig.Segments.Add(l2);
                    var l3 = new LineSegment() { Point = new Point(l2.Point.X - bottomwidth, l2.Point.Y) };
                    fig.Segments.Add(l3);
                }
                //var l2 = new LineSegment() { Point = new Point(l1.Point.X, top) };
                //fig.Segments.Add(l2);
                //var l3 = new LineSegment() { Point = new Point(left + curWidth, top) };
                //fig.Segments.Add(l3);
                //var l4 = new LineSegment() { Point = new Point(left + curWidth, top) };
                //fig.Segments.Add(l4);

                p.ForeColor = Colors.Black;
                var label = p.CreateLabel(p.NumberValue.Value.ToString() + "\n" + (per * 100).ToString("0.#") + "%");
                label.Width = rec.Width;
                label.SetValue(System.Windows.Controls.Canvas.LeftProperty, rec.Left);
                label.SetValue(System.Windows.Controls.Canvas.TopProperty, p.Position.Y );
                Canvas.AddChild(label);

                if (!string.IsNullOrWhiteSpace(p.StringValue))
                {
                    var txt = new TextBlock() { TextWrapping = TextWrapping.Wrap, TextAlignment = TextAlignment.Left, Text = p.StringValue, MaxWidth = legwidth ,
                                                VerticalAlignment = VerticalAlignment.Center,
                                                HorizontalAlignment = HorizontalAlignment.Left,
                                                FontWeight = FontWeights.Bold,
                                                Foreground = new SolidColorBrush(p.ForeColor.Value)
                    };
                    var grid = new Grid() { Width = legwidth, Height = itemHeight };
                    grid.Children.Add(txt);
                    grid.SetValue(System.Windows.Controls.Canvas.LeftProperty, p.Position.X + p.Width + 8);
                    grid.SetValue(System.Windows.Controls.Canvas.TopProperty, p.Position.Y);
                    Canvas.AddChild(grid);
                }
                index++;
                lastFig = fig;
            }
            
            return base.CreatePath();
        }
    }
}
