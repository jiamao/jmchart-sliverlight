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
    /// 雷达图坐标轴
    /// </summary>
    public class RadarAxis : IAxis
    {
        protected void Init()
        {
            var r = rotate * (Math.PI / 180);
            RotateSin = Math.Sin(r);
            RotateCos = Math.Cos(r);
        }

        double rotate;
        /// <summary>
        /// 当前轴的角度
        /// </summary>
        public double Rotate
        {
            get { return rotate; }
            set
            {
                rotate = value;
                Init();
            }
        }

        /// <summary>
        /// 角度的cos值
        /// </summary>
        public double RotateCos { get; set; }

        /// <summary>
        /// 角度的sin值
        /// </summary>
        public double RotateSin { get; set; }
    }
}
