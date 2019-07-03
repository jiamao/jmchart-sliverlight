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

namespace Silverlight.ProcessEditor.Helper
{
    /// <summary>
    /// 图层
    /// </summary>
    public enum ShapZIndex
    {
        Grid = 1,
        Line = 25,
        Shap = 20,
        Move = 50
    }

    /// <summary>
    /// 当前编辑器配置
    /// </summary>
    public static class Config
    {
        static Config() 
        {
            GridSize=new Size(60,60);           
        }       

        /// <summary>
        /// 网格大小
        /// </summary>
        public static Size GridSize { get; set; }

        /// <summary>
        /// 网格列个数
        /// </summary>
        public static int ColumnCount { get; set; }

        /// <summary>
        /// 网格行数
        /// </summary>
        public static int RowCount { get; set; }

        /// <summary>
        /// 元素放入网格中的margin
        /// </summary>
        public static Thickness ElementMargin = new Thickness(10);
    }
}
