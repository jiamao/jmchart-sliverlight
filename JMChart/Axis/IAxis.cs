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

namespace JMChart.Axis
{
    /// <summary>
    /// 坐标轴接口
    /// </summary>
    public abstract class IAxis
    {
        double maxValue;
        /// <summary>
        /// 此轴最大值
        /// </summary>
        public double MaxValue {

            get {

                return maxValue;
            }
            set {
                maxValue = value;
                ResetStep();
            }
        }

        double minValue;
        /// <summary>
        /// 此轴最小值
        /// </summary>
        public double MinValue {
            get
            {
                return minValue;
            }
            set
            {
                minValue = value;
                ResetStep();
            }
        }

        double? itemCount;
        /// <summary>
        /// 数据项个数
        /// 如果个数不为空
        /// 则表示为分段数据
        /// </summary>
        public double? ItemCount { 
            get { return itemCount; }
            set
            {
                itemCount = value;
                ResetStep();
            }
        }

        double step;
        /// <summary>
        /// 每个数值占多像素
        /// </summary>
        public double Step {
            get
            {
                return step;
            }
            set {
                step=value;
            }
        }

        double length;
        /// <summary>
        /// 当前的长度
        /// </summary>
        public double Length {
            get { return length; }
            set
            {
                length = value;
                ResetStep();
            }
        }

        //string label;
        ///// <summary>
        ///// 说明文字
        ///// </summary>
        //public string Label {
        //    get {
        //        if (string.IsNullOrWhiteSpace(label)) return BindName;
        //        else
        //        {
        //            return Common.Helper.DserLabelName(label, new System.Collections.Generic.Dictionary<string, string>() { { "YName", BindName } });
        //        }
        //    }
        //    set {
        //        label = value;
        //    }
        //}

        /// <summary>
        /// 绑定对应的字段名
        /// </summary>
        public string BindName { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 当前数据类型
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// 起点
        /// </summary>
        public Point StartPoint
        {
            get;
            set;
        }

        /// <summary>
        /// 终结点
        /// </summary>
        public Point EndPoint
        {
            get;
            set;
        }

        /// <summary>
        /// 轴类型
        /// </summary>
        public AxisType AType { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        public Brush Stroke
        {
            get;
            set;
        }

        Shape axisShap=null;
        /// <summary>
        /// 当前图形
        /// </summary>
        protected internal Shape AxisShap
        {
            get {
                if (axisShap != null) axisShap.Stroke = Stroke;
                return axisShap;
            }
            set {
                axisShap = value;
            }
        }

        /// <summary>
        /// 重新计算step
        /// </summary>
        private void ResetStep()
        {
            if (ItemCount.HasValue)
            {
                var l = Length - Length / 10;
                step = ItemCount.Value == 0 ? l : l / ItemCount.Value;
            }
            else
            {
                var v = MaxValue - MinValue;
                v += v / 10;
                if (v != 0) step = Length / v;
            }
        }
    }

    /// <summary>
    /// 轴类型
    /// </summary>
    public enum AxisType
    { 
        /// <summary>
        /// X轴
        /// </summary>
        XValue=0,
        /// <summary>
        /// Y轴
        /// </summary>
        YValue=1,
        /// <summary>
        /// 雷达图的X轴
        /// </summary>
        XRadar=2,
        /// <summary>
        /// 雷达图的Y
        /// 轴
        /// </summary>
        YRadar=3
    }
}
