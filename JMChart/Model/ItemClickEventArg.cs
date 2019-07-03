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
    /// 报表中点击事件参数
    /// </summary>
    public class ItemClickEventArg:EventArgs
    {
        /// <summary>
        /// 当前项
        /// </summary>
        public DataPoint Item { get; set; }

        /// <summary>
        /// 当前数据
        /// </summary>
        public object Data { get; set; }
    }
}
