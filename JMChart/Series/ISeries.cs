using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Data;
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
    /// 图表线或图接口
    /// </summary>
    public abstract class ISeries : Common.IBaseControl
    {
        public ISeries(ChartCanvas canvas)
        {
            Canvas = canvas;
            
            Points = new System.Collections.ObjectModel.ObservableCollection<Model.DataPoint>();

            ItemMappings = new System.Collections.ObjectModel.ObservableCollection<Model.ItemMapping>();
        }

        /// <summary>
        /// 动画执行时间
        /// </summary>
        protected const int AnimateDurtion = 1000;

        /// <summary>
        /// 项单击事件
        /// </summary>
        public EventHandler<Model.ItemClickEventArg> ItemClick;

        /// <summary>
        /// 画布
        /// </summary>
        public ChartCanvas Canvas { get; set; }

        /// <summary>
        /// 当前颜色
        /// </summary>
        public Brush Stroke
        {
            get;
            set;
        }

        /// <summary>
        /// 填充色
        /// </summary>
        public Brush Fill
        {
            get;
            set;
        }

        /// <summary>
        /// 图例名
        /// </summary>
        public string LegendLabel
        {
            get;
            set;
        }

        /// <summary>
        /// 当前线的label格式
        /// /// #Y=当前值，#YName=当前Y轴名称,#C{列名}=表示绑定当前数据对象的指定列值
        /// </summary>
        public string ItemTooltipFormat { get; set; }

        /// <summary>
        /// 当前绑定的对象
        /// </summary>
        public object DataContext { get; set; }

        /// <summary>
        /// 当前索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 图点
        /// </summary>
        public System.Collections.ObjectModel.ObservableCollection<Model.DataPoint> Points { get; set; }

        /// <summary>
        /// 当前图型属性映射
        /// </summary>
        public System.Collections.ObjectModel.ObservableCollection<Model.ItemMapping> ItemMappings
        {
            get;
            internal set;
        }

        /// <summary>
        /// 获取对象的属性的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetValue(string name)
        {
            var mapping = GetMapping(name);
            if (mapping != null) name = mapping.MemberName;

            var obj = Common.Helper.GetPropertyName(DataContext, name);
            return obj;
        }

        /// <summary>
        /// 获取对象的值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns></returns>
        public double GetNumberValue(string name)
        {
            double value = 0;
            var obj = GetValue(name);
            if (obj != null)
            {
                if (Silverlight.Common.Data.TypeHelper.IsNumber(obj.GetType()))
                {
                    if (!double.TryParse(obj.ToString(), out value))
                    {
                        value = Index + 1;
                    }
                }
                else
                {
                    value = Index + 1;
                }
            }
            return value;
        }

        /// <summary>
        /// 获取指定的字段映射
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public Model.ItemMapping GetMapping(Model.ItemMapping.EnumDataMember dm)
        {
            foreach (var m in ItemMappings)
            {
                if (m.DataMember == dm) return m;
            }
            return null;
        }

        /// <summary>
        /// 获取指定的字段映射
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public Model.ItemMapping GetMapping(string name)
        {
            foreach (var m in ItemMappings)
            {
                if (name.Equals(m.OldName, StringComparison.OrdinalIgnoreCase) ||
                    name.Equals(m.MemberName, StringComparison.OrdinalIgnoreCase) ||
                    name.Equals(m.DisplayName, StringComparison.OrdinalIgnoreCase)) 
                    return m;
            }
            return null;
        }

        /// <summary>
        /// 获取指定的字段映射
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public System.Collections.Generic.IEnumerable<Model.ItemMapping> GetMappings(Model.ItemMapping.EnumDataMember dm)
        {
            var ms = (from m in ItemMappings
                      where m.DataMember == dm
                      select m).ToArray<Model.ItemMapping>();
            return ms;
        }

        /// <summary>
        /// 当前动画
        /// </summary>
        protected Storyboard storyboard;

        /// <summary>
        /// 展现
        /// </summary>
        public virtual void Draw()
        {
            var ps = CreatePath();
            foreach (var p in ps)
            {
                Canvas.AddChild(p);
            }
            if (storyboard != null && Canvas.IsAnimate)
            {
                storyboard.Begin();
            }
        }

        System.Collections.Generic.List<Shape> shaps=new System.Collections.Generic.List<Shape>();
        /// <summary>
        /// 当前线条
        /// </summary>
        public System.Collections.Generic.List<Shape> Shaps
        {
            get { return shaps; }
            protected set { shaps = value; }
        }

        /// <summary>
        /// 生成图形
        /// </summary>
        /// <returns></returns>
        public virtual System.Collections.Generic.IEnumerable<Shape> CreatePath()
        {
            return Shaps;
        }

        /// <summary>
        /// 生成图例
        /// </summary>
        /// <returns></returns>
        internal virtual StackPanel CreateLegend()
        {
            if (!string.IsNullOrWhiteSpace(LegendLabel))
            {
                var panel = new StackPanel();
                panel.Orientation = Orientation.Horizontal;
                var colorarea = new Rectangle();
                colorarea.Width = 20;
                colorarea.Height = 10;
                colorarea.Fill = this.Fill;
                colorarea.Stroke = this.Stroke;
                panel.Margin = new Thickness(2);
                panel.Children.Add(colorarea);

                var text = new TextBlock();
                text.Margin = new Thickness(2);

                var dic=new System.Collections.Generic.Dictionary<string,string>();
                foreach (var m in ItemMappings)
                {
                    if (!dic.ContainsKey("YName") && !string.IsNullOrWhiteSpace(m.DisplayName))
                    {
                        dic.Add("YName", m.DisplayName??m.MemberName);
                    }
                }

                text.Text = Common.Helper.DserLabelName(LegendLabel,dic ,
                        (string name) =>
                        {
                            return GetValue(name);
                        });
                text.Foreground = new SolidColorBrush(Colors.Black);
                panel.Children.Add(text);

                return panel;
            }
            return null;
        }

        /// <summary>
        /// 添加点的小圆圈，方便鼠标点中。并加提示
        /// </summary>
        /// <param name="center"></param>
        /// <param name="rotate"></param>
        protected Ellipse AddPoint(Point center, double rotate,object tooltip,Model.DataPoint p)
        {
            var circle = Common.Helper.CreateEllipse(center, rotate);
            circle.Stroke = this.Stroke;
            circle.Fill = this.Fill;
            ToolTipService.SetToolTip(circle, tooltip);

            if (this.ItemClick != null) {
                circle.Cursor = Cursors.Hand;
                circle.MouseLeftButtonUp += (sender, e) => {
                    var arg = new Model.ItemClickEventArg() { 
                     Data=this.DataContext,
                      Item=p
                    };
                    ItemClick(circle,arg);
                };
            }

            Canvas.AddChild(circle);

            System.Windows.Controls.Canvas.SetZIndex(circle, Common.BaseParams.TooltipZIndex);

            return circle;
        }

        /// <summary>
        /// 生成提示信息
        /// #Y=当前值，#YName=当前Y轴名称,#C{列名}=表示绑定当前数据对象的指定列值
        /// </summary>
        /// <returns></returns>
        protected string CreateTooltip(string yName)
        {
            if (!string.IsNullOrWhiteSpace(this.ItemTooltipFormat))
            {
                var yvalue = GetValue(yName);
                var tmp = Common.Helper.DserLabelName(this.ItemTooltipFormat,
                    new System.Collections.Generic.Dictionary<string, string>() { { "YName", yName }, { "Y", yvalue==null?"":yvalue.ToString() } }, 
                    (string name) =>
                    {
                        return GetValue(name);
                    });
                return tmp;
            }
            return this.ItemTooltipFormat;
        }

        public void Show()
        {
            throw new NotImplementedException();
        }

        public void Hide()
        {
            throw new NotImplementedException();
        }
    }
}
