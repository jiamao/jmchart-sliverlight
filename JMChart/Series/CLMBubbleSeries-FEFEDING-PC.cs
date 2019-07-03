using System;
using System.Linq;
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
    /// CLM气泡图
    /// </summary>
    public class CLMBubbleSeries:ISeries
    {
        //中心大小
        int centerSize = 40;
        //边圆大小
        int circleSize = 25;

        public CLMBubbleSeries(CoorCanvas canvas)
            : base(canvas) {
                this.Stroke = new SolidColorBrush(Color.FromArgb(255, 51, 153, 255));
                this.Fill = new SolidColorBrush(Color.FromArgb(255, 188, 222, 255));
        }

        /// <summary>
        /// 中心圆指定的名称
        /// </summary>
        public string CenterName { get; set; }

        /// <summary>
        /// 生成当前图形
        /// </summary>
        /// <returns></returns>
        public override System.Collections.Generic.IEnumerable<Shape> CreatePath()
        {
            if (storyboard != null) storyboard.Stop();
            if (Canvas.IsAnimate) this.storyboard = new Storyboard();

            this.Shaps.Clear();
            if (DataContext == null) return base.CreatePath();

            var center=new Point() { X = this.Canvas.Width / 2, Y = centerSize * 2 };
            var left = (center.X - circleSize - centerSize) / 2;
            if (left <= circleSize / 2) left = circleSize + 2;
            var bottom = (center.Y + circleSize + centerSize);
            var maxbottom=Canvas.Height - Canvas.Margin.Bottom - circleSize - 4;
            //距离中心距离
            var radiacenter = Math.Min(center.X - left, maxbottom);

            var circleIndex = -1;
            var data = DataContext as System.Collections.ICollection;

            //小圆个数
            var circlecount = data.Count;
            var rotatestep = 3.8 / circlecount;//每个小圆的角度
            var mapping = GetMapping(Model.ItemMapping.EnumDataMember.Y);

            if (mapping == null) throw new Exception("至少需要指定一个Y轴字段映射");
            
            var tocentername="";
            //画泡泡
            foreach (var m in data)
            {
                if (m != null)
                {
                    var item = new Model.DataPoint();
                    
                    item.PotinShape= new Path();
                    var v = Common.Helper.GetPropertyName(m, mapping.MemberName); ;
                    item.PointType = Model.DataPoint.EnumPointType.ChartPoint;
                    item.StringValue = v==null?"":v.ToString();

                    var tooltip = CreateTooltip(mapping.MemberName, item.StringValue);
                    ToolTipService.SetToolTip(item.PotinShape, tooltip);

                    var el = new EllipseGeometry();
                    item.PotinShape.Data = el;

                    //画中心位置
                    if (circleIndex == -1)
                    {
                        item.Position = el.Center = center;
                        el.RadiusX = el.RadiusY = centerSize;
                        tocentername = item.StringValue;
                        item.StringValue =(CenterName??mapping.MemberName) + "\n" + item.StringValue;
                        //加入标签
                        Canvas.AddChild(item.CreateLabel());
                    }
                    //画边上的小圆
                    else
                    {
                        var position = new Point() { X = left };
                        //离最左的小圆斜角偏移量
                        //二圆直接偏移量的一半
                        var rotate = rotatestep * circleIndex / 2;
                        var rsin = Math.Sin(rotate);
                        var rcos = Math.Cos(rotate);
                        //二圆偏移量
                        var step = rsin * radiacenter * 2;
                        var ystep = step * rcos;
                        var xstep = step * rsin;

                        position.X = left + xstep;
                        position.Y = center.Y + ystep;

                        if (position.Y >= maxbottom) position.Y = maxbottom;
                        
                        item.Position = position;
                        el.RadiusX = el.RadiusY = circleSize;

                        if (Canvas.IsAnimate)
                        {
                            var anima = new PointAnimation();
                            anima.To = position;
                            anima.Duration = TimeSpan.FromMilliseconds(AnimateDurtion);

                            Storyboard.SetTarget(anima, el);
                            el.Center = center;
                            Storyboard.SetTargetProperty(anima, new PropertyPath("Center"));

                            anima.Completed += new EventHandler((sender, e) =>
                            {
                                Canvas.AddChild(item.CreateLabel());
                            });
                            this.storyboard.Children.Add(anima);
                        }
                        else
                        {
                            el.Center = position;
                            //加入标签
                            Canvas.AddChild(item.CreateLabel());
                        }

                        var arrow = new Controls.CLMArrow();
                        arrow.Fill = this.Fill;
                        arrow.Stroke = this.Stroke;
                        arrow.Rotate = rotate;
                        arrow.ToName = tocentername;
                        arrow.FromName = item.StringValue;
                        var startystep = circleSize * ((center.Y - item.Position.Y) / radiacenter);
                        var startxstep = circleSize * ((center.X - item.Position.X) / radiacenter);
                        arrow.StartPoint = new Point(item.Position.X + startxstep, item.Position.Y + startystep);
                        var endystep = centerSize *rsin ;
                        var endxstep = centerSize * rcos;
                        arrow.EndPoint = new Point(center.X + endxstep, center.Y + endystep);

                        arrow.Create(Canvas);
                    }
                    
                    if(Canvas.IsFillShape)item.PotinShape.Fill = this.Fill;
                    item.PotinShape.Stroke = this.Stroke;
                    item.PotinShape.StrokeThickness = Canvas.LineWidth;
                    this.Shaps.Add(item.PotinShape);

                    circleIndex++;                    
                }
            }

            return base.CreatePath();
        }

    }
}
