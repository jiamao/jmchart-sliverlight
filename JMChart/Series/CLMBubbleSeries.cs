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
        const double centerSize = 40;
        //边圆大小
        const double circleSize = 25;

        /// <summary>
        /// 当前显示的标识
        /// </summary>
        Controls.CLMArrow currentShowedArrow = null;

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
        /// 提示信息鼠标点击事件
        /// </summary>
        public EventHandler<Model.CLMArrowClickEventArg> ArrowTooltipClick;

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
            var data = DataContext as System.Collections.ICollection;

            var circlesize = data.Count > 20 ? circleSize / data.Count * 20 : circleSize;
            var center=new Point() { X = this.Canvas.Width / 2, Y = centerSize * 2.3 };
            var left = Canvas.Margin.Left + circlesize * 2;
            if (left <= circlesize / 2) left = circlesize + 2;
            var bottom = (center.Y + circlesize + centerSize);
            var maxbottom = Canvas.Height - Canvas.Margin.Bottom - circlesize - 4;
            //距离中心距离
            var radiacenter = Math.Min(center.X - left, maxbottom);
            var circleIndex = -1;            

            //小圆个数
            var circlecount = data.Count;
            var rotatestep = 3.78 / circlecount;//每个小圆的角度
            var mapping = GetMapping(Model.ItemMapping.EnumDataMember.Y);

            if (mapping == null) throw new Exception("至少需要指定一个Y轴字段映射");
            //与中心点关联设置
            var links = GetMappings(Model.ItemMapping.EnumDataMember.CLMLink);

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
                    System.Windows.Controls.Canvas.SetZIndex(item.PotinShape, Common.BaseParams.ShapZIndex);
                   
                    var el = new EllipseGeometry();
                    item.PotinShape.Data = el;

                    //画中心位置
                    if (circleIndex == -1)
                    {
                        item.Position = el.Center = center;
                        el.RadiusX = el.RadiusY = centerSize;
                        item.Width = item.Height = centerSize * 2;
                        tocentername = item.StringValue;
                        item.StringValue =(CenterName??mapping.MemberName) + "\n" + item.StringValue;

                        var label = item.CreateLabel();
                        //加入标签
                        Canvas.AddChild(label);

                        if (ItemClick != null)
                        {
                            label.Cursor = Cursors.Hand;
                            var centerdata = m;
                            label.MouseLeftButtonUp += (sender, e) =>
                            {
                                var arg = new Model.ItemClickEventArg()
                                {
                                    Data = centerdata,
                                    Item = item
                                };
                                ItemClick(sender, arg);
                            };
                        }

                        var tootip = CreateTooltip(m);
                        ToolTipService.SetToolTip(label,tootip);
                    }
                    //画边上的小圆
                    else
                    {
                        //初始化小圆点
                        InitPoint(el, item, rotatestep, circleIndex, radiacenter, center, maxbottom, circlesize, tocentername,circlecount,links,m);
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

        void PotinShape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 初始化项
        /// </summary>
        /// <param name="el"></param>
        /// <param name="item"></param>
        /// <param name="rotatestep"></param>
        /// <param name="circleIndex"></param>
        /// <param name="radiacenter"></param>
        /// <param name="center"></param>
        /// <param name="maxbottom"></param>
        /// <param name="circlesize"></param>
        /// <param name="tocentername"></param>
        private void InitPoint(EllipseGeometry el,Model.DataPoint item,
            double rotatestep, int circleIndex, double radiacenter, Point center,
            double maxbottom, double circlesize, string tocentername, int circlecount,
            System.Collections.Generic.IEnumerable<Model.ItemMapping> links,object data)
        {
            var position = new Point();            
            var rotate = rotatestep * circleIndex + 2.95;
            var rsin = Math.Sin(rotate);
            var rcos = Math.Cos(rotate);
            //二圆偏移量           
            var ystep = rsin * radiacenter;
            var xstep = rcos * radiacenter;

            position.X = center.X + xstep;
            position.Y = center.Y - ystep;

            if (position.Y >= maxbottom) position.Y = maxbottom;

            item.Position = position;
            el.RadiusX = el.RadiusY = circlesize;
            item.Width = item.Height = circlesize * 2;

            var arrow = new Controls.CLMArrow(Canvas);
            arrow.Fill = this.Fill;
            arrow.Stroke = this.Stroke;
            arrow.Rotate = rotate;
            arrow.ToName = tocentername;
            arrow.FromName = item.StringValue;
            arrow.RotateSin = rsin;
            arrow.RotateCos = rcos;

            var startystep = (circlesize) * arrow.RotateSin;
            var startxstep = (circlesize) * arrow.RotateCos;
            arrow.StartPoint = new Point(item.Position.X - startxstep, item.Position.Y + startystep);
            var endystep = centerSize * arrow.RotateSin;
            var endxstep = centerSize * arrow.RotateCos;
            arrow.EndPoint = new Point(center.X + endxstep, center.Y - endystep);

            if (links != null)
            {
                var count = links.Count<Model.ItemMapping>();
                if (count > 0)
                {
                    var lnk = links.ElementAt<Model.ItemMapping>(0);
                    var tmp = Common.Helper.GetPropertyName(data, lnk.MemberName);
                    if (!string.IsNullOrWhiteSpace(lnk.MarkName)) arrow.FromMarkName = lnk.MarkName;
                    arrow.FromValue = tmp == null ? "" : tmp.ToString();
                }
                if (count > 1)
                {
                    var lnk = links.ElementAt<Model.ItemMapping>(1);
                    var tmp = Common.Helper.GetPropertyName(data, lnk.MemberName);
                    if (!string.IsNullOrWhiteSpace(lnk.MarkName)) arrow.ToMarkName = lnk.MarkName;
                    arrow.ToValue = tmp == null ? "" : tmp.ToString();
                }
            }

            //设置箭头提示事件
            if (ArrowTooltipClick != null) arrow.SetClickEvent(ArrowTooltipClick);

            arrow.Draw();
            item.TargetControl = arrow;

            var label = item.CreateLabel();
            Canvas.AddChild(label);

            if (ItemClick != null)
            {
                label.Cursor = Cursors.Hand;
                label.MouseLeftButtonUp += (sender, e) =>
                {
                    var arg = new Model.ItemClickEventArg()
                    {
                        Data = data,
                        Item = item
                    };
                    ItemClick(sender, arg);
                };
            }

            if (Canvas.IsAnimate)
            {
                label.Visibility = Visibility.Collapsed;
                var anima = new PointAnimation();
                anima.To = position;
                anima.Duration = TimeSpan.FromMilliseconds(AnimateDurtion);

                Storyboard.SetTarget(anima, el);
                el.Center = center;
                Storyboard.SetTargetProperty(anima, new PropertyPath("Center"));

                var sizeanimax = new DoubleAnimation();
                sizeanimax.From = 0;
                sizeanimax.To = circlesize;
                Storyboard.SetTarget(sizeanimax, el);
                Storyboard.SetTargetProperty(sizeanimax, new PropertyPath("RadiusX"));

                var sizeanimay = new DoubleAnimation();
                sizeanimay.From = 0;
                sizeanimay.To = circlesize;
                Storyboard.SetTarget(sizeanimay, el);
                Storyboard.SetTargetProperty(sizeanimay, new PropertyPath("RadiusY"));

                anima.Completed += new EventHandler((sender, e) =>
                {
                    label.Visibility = Visibility.Visible;
                    InitMouseEvent(label, arrow);
                    if (circleIndex == circlecount / 2 - 1) { 
                        arrow.Show();
                        currentShowedArrow = arrow;
                    }
                });
                this.storyboard.Children.Add(anima);
                this.storyboard.Children.Add(sizeanimax);
                this.storyboard.Children.Add(sizeanimay);
            }
            else
            {
                el.Center = position;
                //加入标签
                //var label = item.CreateLabel();
                //Canvas.AddChild(label);
                InitMouseEvent(label, arrow);
                if (circleIndex == circlecount / 2 - 1) { 
                    arrow.Show();
                    currentShowedArrow = arrow;
                }
            }
        }

        /// <summary>
        /// 生成中心圆提示信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private FrameworkElement CreateTooltip(object data)
        {
            if (!string.IsNullOrWhiteSpace(ItemTooltipFormat))
            {
                var margin = new Thickness(8);

                var tooltip = Common.Helper.DserLabelName(ItemTooltipFormat, null,
                                (string name) =>
                                {
                                    var mapping = GetMapping(name);
                                    if (mapping != null) name = mapping.MemberName;
                                    return Common.Helper.GetPropertyName(data, name);
                                });

                var panel = new StackPanel() { Orientation = Orientation.Vertical };

                //替换换行
                tooltip = tooltip.Replace("[br]", "\n");

                var reg = new System.Text.RegularExpressions.Regex("\\[hr\\]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                var index = 0;
                if (reg.IsMatch(tooltip))
                {                    
                    foreach (System.Text.RegularExpressions.Match m in reg.Matches(tooltip))
                    {
                        if (m.Index > index)
                        {
                            var starttext = tooltip.Substring(index, m.Index - index);
                            var txt = new TextBlock();
                            txt.Margin = margin;
                            txt.Text = starttext;
                            panel.Children.Add(txt);
                        }
                        var border = new Border();
                        border.BorderBrush = new SolidColorBrush(Colors.LightGray);
                        border.BorderThickness = new Thickness(0, 1, 0, 0);
                        border.HorizontalAlignment = HorizontalAlignment.Stretch;
                        border.Height = 1;
                        panel.Children.Add(border);
                        //当前索引位置
                        index = m.Index + 4;
                    }
                }
                if (tooltip.Length > index)
                {
                    var txt = new TextBlock();
                    txt.Margin = margin;
                    txt.Text = tooltip.Substring(index);
                    panel.Children.Add(txt);
                }
                return panel;
            }
            return null;
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        /// <param name="fe"></param>
        /// <param name="arrow"></param>
        private void InitMouseEvent(FrameworkElement fe, Controls.CLMArrow arrow)
        {
            fe.MouseEnter += (sender,e) => {
                if (currentShowedArrow == arrow) return;
                if (currentShowedArrow != null) currentShowedArrow.Hide();
                arrow.Show();
                currentShowedArrow = arrow;
            };            
        }
    }
}
