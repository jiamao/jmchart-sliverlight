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

namespace JMChart.Common
{
    public class BaseParams
    {
        /// <summary>
        /// 基线层级
        /// </summary>
        public const int BaseLineZIndex = 10;

        /// <summary>
        /// 图形层级
        /// </summary>
        public const int ShapZIndex = 500;

        /// <summary>
        /// 标签层级
        /// </summary>
        public const int LabelZIndex = 1000;

        /// <summary>
        /// 提示层级 
        /// </summary>
        public const int TooltipZIndex = 2000;

    }
}
