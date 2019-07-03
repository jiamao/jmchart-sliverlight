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
    /// 拖放操作类
    /// </summary>
    public static class DragDrop
    {
        /// <summary>
        /// 当前对象
        /// </summary>
        static object _currentData = null;

        /// <summary>
        /// 开始拖放对象
        /// </summary>
        /// <param name="data"></param>
        public static void DoDragDrop(object data)
        {
            _currentData = data;
        }

        /// <summary>
        /// 获取拖放数据
        /// </summary>
        /// <returns></returns>
        public static object GetData()
        {
            return _currentData;
        }

        /// <summary>
        /// 清除
        /// </summary>
        public static void Clear()
        {
            _currentData = null;
        }
    }
}
