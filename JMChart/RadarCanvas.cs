using System;
using System.Linq;
using System.Collections.ObjectModel;
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
    /// 雷达图基线
    /// </summary>
    public class RadarCanvas : ChartCanvas
    {
        public RadarCanvas() {
            IsCircle = false;

            ItemMappings = new System.Collections.ObjectModel.ObservableCollection<Model.ItemMapping>();
        }

        /// <summary>
        /// 雷达图外形
        /// </summary>
        public enum RadarType
        { 
            /// <summary>
            /// 线开型
            /// </summary>
            Line=0,
            /// <summary>
            /// 圆型
            /// </summary>
            Circle=1
        }

        //string[] yvalueNames;
        ///// <summary>
        ///// 绑定到Y轴的属性名
        ///// </summary>
        //public string[] YValueNames {
        //    get {
        //        return yvalueNames;
        //    }
        //    set {
        //        yvalueNames = value;
        //        VerticalCount = value.Length;                
        //    }
        //}

        /// <summary>
        /// 字段映射
        /// </summary>
        public System.Collections.ObjectModel.ObservableCollection<Model.ItemMapping> ItemMappings { get; set; }

        /// <summary>
        /// 点的提示说明格式
        /// #Y=当前值，#YName=当前Y轴名称,#C{列名}=表示绑定当前数据对象的指定列值
        /// </summary>
        public string[] ItemTooltips { get; set; }

        /// <summary>
        /// 图例格式，
        /// 默认为字段名
        /// </summary>
        public string LegendLabel { get; set; }

        /// <summary>
        /// 显示在Y轴附上的标签
        /// </summary>
        public string[] YLabels { get; set; }

        /// <summary>
        /// 项单击事件
        /// </summary>
        public EventHandler<Model.ItemClickEventArg> ItemClick;


        double _angle = 0;
        /// <summary>
        /// 纵线之间的角度
        /// </summary>
        public double Angle
        {
            get {
                _angle = 360 / VerticalCount;
                return _angle;
            }
        }

        /// <summary>
        /// 中心点
        /// </summary>
        public Point Center
        {
            get;
            internal set;
        }

        /// <summary>
        /// 是否为圆型图
        /// </summary>
        public bool IsCircle { get; set; }

        /// <summary>
        /// 半径
        /// </summary>
        public double Radius
        {
            get;
            internal set;
        }

        /// <summary>
        /// 展现
        /// </summary>
        public override void Draw(bool isClear = false)
        {
            Reset();

            //画基线
            DrawLine();

            base.Draw(false);
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="data"></param>
        internal override void InitSeries(object data)
        {
            Serieses.Clear();

            if (data == null) return;

            //获取集合接口
            var enumerator = data as System.Collections.ICollection;

            if (enumerator != null)
            {
                var modelindex = 0;
                foreach (var d in enumerator)
                {
                    var item = new Series.RadarSeries(this);
                    item.DataContext = d;
                    item.Index = modelindex;
                    var color = modelindex < SeriesColors.Length ? SeriesColors[modelindex] : SeriesColors[modelindex % SeriesColors.Length];
                    item.LegendLabel = LegendLabel;
                    item.Stroke = new SolidColorBrush(color);
                    if (this.ItemTooltips != null && this.ItemTooltips.Length > 0)
                    {
                        item.ItemTooltipFormat = this.ItemTooltips.Length > modelindex ? this.ItemTooltips[modelindex] : this.ItemTooltips[this.ItemTooltips.Length - 1];
                    }
                    else
                    {
                        //默认采用Y名称加Y值的格式
                        item.ItemTooltipFormat = "#YName:#Y";
                    }

                    item.ItemClick += ItemClick;
                    item.ItemMappings = ItemMappings;

                    color.A = 60;
                    item.Fill = new SolidColorBrush(color);

                    modelindex++;
                    this.Serieses.Add(item);
                }
            }
        }

        /// <summary>
        /// 计算画布位置
        /// </summary>
        protected override void SetCanvasLayout()
        {
            //base.SetCanvasLayout();

            Radius = Math.Min(Width, Height) / 2;

            //中心点
            Center = new Point(Radius, Radius);           
        }

        /// <summary>
        /// 以中间为点画线
        /// </summary>
        /// <param name="center"></param>
        private void DrawLine()
        {
            var mappings = ItemMappings.Where<Model.ItemMapping>(p => p.DataMember == Model.ItemMapping.EnumDataMember.Y).ToArray<Model.ItemMapping>();

            VerticalCount = mappings.Length;
            var ynames = mappings.Select<Model.ItemMapping, string>(p => p.MemberName).ToArray<string>();

            var angle = Angle;

            var center = Center;
            //横轴个数
            var hcount = this.HorizontalCount;


            //x轴每一小格的长度
            var xstep = Radius / hcount;
            var yaxises = new Axis.RadarAxis[VerticalCount];

            var sources = DataContext as System.Collections.ICollection;
            var dataDic = Common.Helper.GetMaxandNinValue(sources, ynames);

            //从90度起开始画线条
            //画星状线
            for (var i = 0; i < yaxises.Length; i++)
            {
                var axis = yaxises[i] = new Axis.RadarAxis();
                var mapping = mappings[i];

                var path = new Path();
                //path.StrokeDashArray.Add(4);
                axis.AxisShap = path;
                axis.BindName = mapping.MemberName;
                axis.DisplayName = mapping.DisplayName;

                var dic = dataDic[axis.BindName];
                if (dic[0].HasValue)
                {
                    axis.MaxValue = dic[1].Value;
                    axis.MinValue = Common.Helper.CheckMinValue(dic[0].Value);
                }
                else
                {
                    axis.ItemCount = sources.Count;//数值个数
                }

                axis.Length = Radius;
                axis.Rotate = i * angle;

                /////设定Y标签
                //if (this.YLabels != null && i < this.YLabels.Length)
                //{
                //    axis.Label = this.YLabels[i];
                //}

                //计算纵轴离中心的偏移量
                var offsety = axis.RotateSin * Radius;
                var offsetx = axis.RotateCos * Radius;

                axis.StartPoint = center;
                axis.EndPoint = new Point(center.X + offsetx, center.Y + offsety);
                axis.AType = Axis.AxisType.YRadar;
                axis.Stroke = ForeColor;

                //如果画基线
                if (IsDrawBaseLine)
                {
                    var geo = new PathGeometry();
                    path.Data = geo;
                    var fig = new PathFigure();
                    geo.Figures.Add(fig);
                    fig.StartPoint = axis.StartPoint;
                    fig.Segments.Add(new LineSegment() { Point = axis.EndPoint });

                    AddChild(axis.AxisShap);
                    //生成Y轴标签
                    var label = CreateYLabel(axis);
                    AddChild(label);
                }
                this.Axises.Add(axis);
            }

            //如果画基线
            if (IsDrawBaseLine)
            {
                //当线条少于5条时。无法画多边形。因为四条就是一个正方形。所以会默认用圆形。
                if (IsCircle == false && VerticalCount > 4)
                {
                    for (var i = 0; i < hcount; i++)
                    {
                        var xaxis = new Axis.RadarAxis() { AType = Axis.AxisType.XRadar, Stroke = ForeColor };
                        var shap = new Polygon();
                        shap.Stroke = xaxis.Stroke;
                        //shap.StrokeDashArray.Add(4);
                        xaxis.AxisShap = shap;

                        this.Axises.Add(xaxis);
                        var step = xstep * (i + 1);

                        foreach (var yaxis in yaxises)
                        {
                            //纵坐标上的点偏移量
                            var xoffsetx = yaxis.RotateCos * step;
                            var xoffsety = yaxis.RotateSin * step;

                            var p = new Point(Center.X + xoffsetx, Center.Y + xoffsety);
                            shap.Points.Add(p);
                        }

                        AddChild(shap);
                    }
                }
                else
                {
                    for (var i = 0; i < hcount; i++)
                    {
                        var xaxis = new Axis.RadarAxis() { AType = Axis.AxisType.XRadar, Stroke = ForeColor };
                        var path = new Path();
                        xaxis.AxisShap = path;
                        //path.StrokeDashArray.Add(4);
                        this.Axises.Add(xaxis);

                        var step = xstep * (i + 1);
                        var cirl = new EllipseGeometry();
                        path.Data = cirl;
                        cirl.Center = Center;
                        cirl.RadiusX = cirl.RadiusY = step;

                        AddChild(xaxis.AxisShap);
                    }
                }
            }
        }

        /// <summary>
        /// 生成Y轴标签
        /// </summary>
        /// <param name="label">标签值</param>
        /// <param name="rotate">旋转角度</param>
        /// <returns></returns>
        protected TextBlock CreateYLabel(Axis.RadarAxis axis)
        {
            var textBlock = new TextBlock();
           
            textBlock.Text = axis.DisplayName??axis.BindName;
            textBlock.Foreground = new SolidColorBrush(Colors.Black);
            textBlock.TextAlignment = TextAlignment.Center;
            var transformgroup = new TransformGroup();

            //旋转角度
            var rotatetransform = new RotateTransform();            
            rotatetransform.Angle = axis.Rotate <= 180 && axis.Rotate > 0 ? axis.Rotate - 90 : axis.Rotate + 90;
            rotatetransform.CenterX = 0.5;
            rotatetransform.CenterY = 0.5;
            transformgroup.Children.Add(rotatetransform);

            //平移位置
            var r = (rotatetransform.Angle % 360) * (Math.PI / 180);
            var cos = -Math.Abs(Math.Cos(r));
            var sin = r > 0 && r < Math.PI ? -Math.Abs(Math.Sin(r)) : Math.Abs(Math.Sin(r));
            var translatetransform = new TranslateTransform();
            translatetransform.X = cos * (textBlock.ActualWidth / 2);
            translatetransform.Y = sin * (textBlock.ActualWidth / 2);
            transformgroup.Children.Add(translatetransform);

            var x = rotatetransform.Angle > axis.Rotate ? axis.EndPoint.X + axis.RotateCos * textBlock.ActualHeight : axis.EndPoint.X -2;// -Math.Abs(axis.RotateCos) * textBlock.ActualWidth / 2;
            var y = rotatetransform.Angle > axis.Rotate ? axis.EndPoint.Y + axis.RotateSin * textBlock.ActualHeight : axis.EndPoint.Y + 2;// -Math.Abs(axis.RotateCos) * textBlock.ActualHeight / 2;
            textBlock.SetValue(Canvas.LeftProperty, x);
            textBlock.SetValue(Canvas.TopProperty, y);          
            textBlock.RenderTransformOrigin = new Point(0.5, 0.5);
            textBlock.RenderTransform = transformgroup;
            return textBlock;
        }
    }
}
