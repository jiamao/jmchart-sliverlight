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

namespace JMChart.Series
{
    /// <summary>
    /// 雷达图线
    /// </summary>
    public class RadarSeries:ISeries
    {
        public RadarSeries(RadarCanvas canvas)
            :base(canvas)
        { 
        
        }

        /// <summary>
        /// 生成当前线条
        /// </summary>      
        /// <returns></returns>
        public override System.Collections.Generic.IEnumerable<Shape> CreatePath()
        {
            if (storyboard != null) storyboard.Stop();

            var canvas = Canvas as RadarCanvas;
            var index = 0;
            if (canvas.IsAnimate) this.storyboard = new Storyboard();

            Path path = Shaps.Count > 0 ? Shaps[0] as Path : new Path();
            if (Shaps.Count == 0) Shaps.Add(path);

            if (canvas.IsFillShape) path.Fill = this.Fill;
            System.Windows.Controls.Canvas.SetZIndex(path, Common.BaseParams.ShapZIndex);

            path.StrokeThickness = canvas.LineWidth;
            path.Stroke = this.Stroke;

            var geo = path.Data == null ? new PathGeometry() : path.Data as PathGeometry;
            path.Data = geo;
            PathFigure fig = null;
            if (geo.Figures.Count == 0)
            {
                fig = new PathFigure();
                geo.Figures.Add(fig);
                fig.IsClosed = true;
            }
            else
            {
                fig = geo.Figures[0];
                fig.Segments.Clear();
            }

            if (canvas.IsAnimate) this.storyboard = new Storyboard();
            if (Points == null) Points = new System.Collections.ObjectModel.ObservableCollection<Model.DataPoint>();
            Points.Clear();

            //起始点和线段点要分开处理
            foreach (var a in canvas.Axises)
            {
                if (a.AType != Axis.AxisType.YRadar) continue;
                var axis = a as Axis.RadarAxis;

                var p = new Model.DataPoint();
                Points.Add(p);

                //获取当前点的值
                p.NumberValue = this.GetNumberValue(a.BindName);
                var r = a.Step * (p.NumberValue.Value - a.MinValue);

                //生成提示信息
                var tooltip = CreateTooltip(a.BindName);

                //目标点
                p.Position = new Point(canvas.Center.X + axis.RotateCos * r, canvas.Center.Y + axis.RotateSin * r);

                var point = AddPoint(p.Position, 10, tooltip,p);
                if (canvas.IsAnimate)
                {
                    point.Visibility = Visibility.Collapsed;

                    var anima = new PointAnimation();
                    anima.To = p.Position;
                    anima.Duration = TimeSpan.FromMilliseconds(AnimateDurtion);

                    if (index != 0)
                    {
                        var seg = new LineSegment();
                        seg.Point = canvas.Center;
                        fig.Segments.Add(seg);
                        Storyboard.SetTarget(anima, seg);
                        Storyboard.SetTargetProperty(anima, new PropertyPath("Point"));
                    }
                    else
                    {
                        Storyboard.SetTarget(anima, fig);
                        fig.StartPoint = canvas.Center;
                        Storyboard.SetTargetProperty(anima, new PropertyPath("StartPoint"));
                        index++;
                    }

                    //动画展示完后加入点
                    anima.Completed += new EventHandler((object sender, EventArgs e) =>
                    {
                        var panima = sender as PointAnimation;
                        point.Visibility = Visibility.Visible;
                    });

                    this.storyboard.Children.Add(anima);
                }
                else
                {
                    if (index != 0)
                    {
                        var seg = new LineSegment();
                        seg.Point = p.Position;
                        fig.Segments.Add(seg);
                    }
                    else
                    {
                        fig.StartPoint = p.Position;
                        index++;
                    }
                }
            }
            return base.CreatePath();
        }
    }
}
