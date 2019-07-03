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
    public class CLMArrow:Common.IBaseControl
    {
        //生成双向箭头
        Path arrowPath = new Path() { StrokeThickness = 3 };
        PathGeometry arrowgeo = new PathGeometry();
        Border txtRect = new Border();        
        ChartCanvas currentCanvas;

        public CLMArrow(ChartCanvas canvas)
        {
            currentCanvas = canvas;
            Hide();//默认隐藏
        }

        /// <summary>
        /// 填充色
        /// </summary>
        public Brush Fill
        {
            get { return this.arrowPath.Fill; } 
            set {
                arrowPath.Fill = value; 
            } }

        /// <summary>
        /// 边框色
        /// </summary>
        public Brush Stroke
        {
            get { return this.arrowPath.Stroke; }
            set {
                arrowPath.Stroke = value; 
            } }

        /// <summary>
        /// 箭头偏移
        /// </summary>
        public double ArroThickness = 5;

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

        public double RotateSin { get; set; }

        public double RotateCos { get; set; }

        /// <summary>
        /// 起始点名称
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// 目标名称
        /// </summary>
        public string ToName { get; set; }

        /// <summary>
        /// 值标识名称
        /// </summary>
        public string FromMarkName { get; set; }

        /// <summary>
        /// 值标识名称
        /// </summary>
        public string ToMarkName { get; set; }

        /// <summary>
        /// 起始点到目标点值
        /// </summary>
        public string FromValue { get; set; }

        /// <summary>
        /// 目标值到起始点值
        /// </summary>
        public string ToValue { get; set; }

        /// <summary>
        /// 显示
        /// </summary>
        public void Show()
        {
            txtRect.Visibility = arrowPath.Visibility = Visibility.Visible;
            if (txtRect.Opacity == 0)
            {                
                var story = new Storyboard();
                story.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
                //txtRect.Opacity = 0;
                var ani = new DoubleAnimation();
                ani.From = 0;
                ani.To = 1;
                Storyboard.SetTarget(ani, txtRect);
                Storyboard.SetTargetProperty(ani, new PropertyPath("Opacity"));
                story.Children.Add(ani);

                var ani2 = new DoubleAnimation();
                ani2.From = 0;
                ani2.To = 1;
                Storyboard.SetTarget(ani2, arrowPath);
                Storyboard.SetTargetProperty(ani2, new PropertyPath("Opacity"));
                story.Children.Add(ani2);
                story.Begin();
            }
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hide()
        {
            //var story = new Storyboard();
            //story.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            ////txtRect.Opacity = 0;
            //var ani = new DoubleAnimation();           
            //ani.To = 0;
            //Storyboard.SetTarget(ani, txtRect);
            //Storyboard.SetTargetProperty(ani, new PropertyPath("Opacity"));
            //story.Children.Add(ani);

            //var ani2 = new DoubleAnimation();           
            //ani2.To = 0;
            //Storyboard.SetTarget(ani2, arrowPath);
            //Storyboard.SetTargetProperty(ani2, new PropertyPath("Opacity"));
            //story.Children.Add(ani2);
            //story.Begin();

            //story.Completed += (sender,e) => {
            //    txtRect.Visibility = arrowPath.Visibility = Visibility.Collapsed;                
            //};
            txtRect.Visibility = arrowPath.Visibility = Visibility.Collapsed;
            txtRect.Opacity = 0;
        }

        /// <summary>
        /// 设置箭头弹出事件
        /// </summary>
        /// <param name="handler"></param>
        public void SetClickEvent(EventHandler<Model.CLMArrowClickEventArg> handler)
        {
            if (handler != null)
            {
                txtRect.Cursor = Cursors.Hand;
                var arg = new Model.CLMArrowClickEventArg()
                {
                    Arrow = this
                };
                txtRect.MouseLeftButtonUp += (sender, e) =>
                {
                    handler(this, arg);
                };
            }
        }

        /// <summary>
        /// 展现
        /// </summary>
        public void Draw()
        {
            //生成双向箭头           
            this.arrowPath.Data = arrowgeo;
            arrowgeo.Figures.Clear();
            var fig = new PathFigure();
            arrowgeo.Figures.Add(fig);
            Canvas.SetZIndex(arrowPath, Common.BaseParams.BaseLineZIndex);

            var r = Rotate - Math.Atan2(ArroThickness, ArroThickness);
            var rsin = Math.Sin(r);
            var rcos = Math.Cos(r);
            var sq = Math.Sqrt(ArroThickness * ArroThickness * 2);
            var ystep = rsin * sq;
            var xstep = rcos * sq;
            //画起始箭头
            fig.StartPoint = StartPoint;
            var blp = new Point();
            //var blpystep = arrowMargin * RotateSin;
            blp.Y = StartPoint.Y + ystep;
            blp.X = StartPoint.X - xstep;

            fig.Segments.Add(new LineSegment() { Point = blp });            
            var brp = new Point();
            brp.Y = StartPoint.Y + xstep;
            brp.X = StartPoint.X + ystep;
            fig.Segments.Add(new LineSegment() { Point = brp });
            fig.Segments.Add(new LineSegment() { Point = StartPoint });

            fig.Segments.Add(new LineSegment() { Point = EndPoint });

            var tlp = new Point() { X = EndPoint.X - ystep, Y = EndPoint.Y - xstep };
            fig.Segments.Add(new LineSegment() { Point = tlp });
            var trp = new Point() { Y = EndPoint.Y - ystep, X = EndPoint.X + xstep };
            fig.Segments.Add(new LineSegment() { Point = trp });
            fig.Segments.Add(new LineSegment() { Point = EndPoint });

            this.currentCanvas.AddChild(arrowPath);

            //初始化说明
            InitBlock();
        }

        /// <summary>
        /// 初始化说明
        /// </summary>
        private void InitBlock()
        {
            txtRect.Background = this.Fill;
            txtRect.BorderBrush = this.Stroke;
            txtRect.BorderThickness = new Thickness(2);
           
            var mainpanel = new StackPanel() { Orientation = Orientation.Vertical, HorizontalAlignment= HorizontalAlignment.Stretch};
            txtRect.Child = mainpanel;
            if (!string.IsNullOrWhiteSpace(this.FromValue))
            {
                var p = CreateValueBlock(string.Format("{0,-5}->{1,5} {2}",this.FromName, this.ToName,this.FromMarkName), this.FromValue);
                mainpanel.Children.Add(p);
                mainpanel.MinWidth = p.MinWidth;
                mainpanel.MinHeight = p.MinHeight;
            }
            if (!string.IsNullOrWhiteSpace(this.ToValue))
            {
                var p = CreateValueBlock(string.Format("{0,-5}->{1,5} {2}", this.ToName, this.FromName, this.ToMarkName), this.ToValue);
                mainpanel.Children.Add(p);

                mainpanel.MinHeight += p.MinHeight;
                mainpanel.MinWidth = Math.Max(p.MinWidth, mainpanel.MinWidth);
            }

            Canvas.SetZIndex(txtRect, Common.BaseParams.LabelZIndex + 1);

            var txtcenter = new Point()
            {
                X=(StartPoint.X - EndPoint.X) / 2 + EndPoint.X,
                Y=(StartPoint.Y - EndPoint.Y) /2 + EndPoint.Y
            };
            
            this.currentCanvas.AddChild(txtRect);

            Canvas.SetLeft(txtRect, txtcenter.X - mainpanel.MinWidth / 2);
            Canvas.SetTop(txtRect, txtcenter.Y - mainpanel.MinHeight / 2);
        }

        /// <summary>
        /// 生成说明标签 值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private StackPanel CreateValueBlock(string name, string value)
        {
            var lpanel = new StackPanel() { Orientation = Orientation.Horizontal };
            lpanel.Margin = new Thickness(2,1,2,1);

            var txt = new TextBlock();            
            txt.Text = name;
            lpanel.Children.Add(txt);
            var txtvalue = new TextBlock() { Foreground = new SolidColorBrush(Colors.Red) };
            txtvalue.Text = string.Format("  {0,-5}", value);
            lpanel.Children.Add(txtvalue);

            lpanel.MinWidth = txt.ActualWidth + txtvalue.ActualWidth;
            lpanel.MinHeight = txt.ActualHeight;
            return lpanel;
        }
    }
}
