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
    public class CLMArrowClickEventArg:EventArgs
    {
        /// <summary>
        /// 当前箭头信息
        /// </summary>
        public Controls.CLMArrow Arrow
        {
            get;
            internal set;
        }
    }
}
