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

namespace JMChart.Model
{
    /// <summary>
    /// 图点
    /// </summary>
    public class DataPoint:IComparable<DataPoint>
    {
        //当前点的图形
        public Path PotinShape
        {
            get;
            internal set;
        }

        /// <summary>
        /// 图点类型
        /// </summary>
        public enum EnumPointType
        {
            XPoint = 0,
            YPoint = 1,
            ChartPoint=2
        }

        /// <summary>
        /// 所关联的控件
        /// </summary>
        public Common.IBaseControl TargetControl { get; set; }

        /// <summary>
        /// 数值
        /// </summary>
        public double? NumberValue { get; set; }

        /// <summary>
        /// 分类值
        /// </summary>
        public string StringValue { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public EnumPointType PointType { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 字幕颜色
        /// </summary>
        public Color? ForeColor { get; set; }

        /// <summary>
        /// 生成标签
        /// </summary>
        /// <returns></returns>
        public FrameworkElement CreateLabel(string value = "")
        {
            var grid = new Grid();
            grid.Background = new SolidColorBrush(Colors.Transparent);
            grid.Height = Height;            
            grid.Width = Width;
            try
            {
                var txt = new TextBlock();
                //txt.Width = Width;
                //txt.Height = Height;
                if (string.IsNullOrWhiteSpace(value))
                {
                    txt.Text = NumberValue.HasValue ? NumberValue.Value.ToString() : StringValue;
                }
                else
                {
                    txt.Text = value;
                }
                txt.TextAlignment = TextAlignment.Center;
                txt.TextWrapping = TextWrapping.Wrap;
                if (ForeColor.HasValue) txt.Foreground = new SolidColorBrush(ForeColor.Value);

                txt.HorizontalAlignment = HorizontalAlignment.Center;
                txt.VerticalAlignment = VerticalAlignment.Center;

                var l = Position.X - Width / 2;
                var t = Position.Y - Height / 2;
                Canvas.SetZIndex(grid, Common.BaseParams.LabelZIndex);
                Canvas.SetLeft(grid, l >0?l:0);
                Canvas.SetTop(grid, t >0?t:0);

                grid.Children.Add(txt);
            }
            catch { }
            return grid;
        }



        public int CompareTo(DataPoint other)
        {
            if (this.NumberValue.HasValue && other.NumberValue.HasValue)
            {
                if (this.NumberValue.Value == other.NumberValue.Value) return 0;
                return this.NumberValue.Value >other.NumberValue.Value?-1:1;
            }
            return this.NumberValue.HasValue ?-1:1;
        }
    }
}
