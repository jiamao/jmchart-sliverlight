using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Silverlight.Common.Visual
{
    public static class TreeHelper
    {
        /// <summary>
        /// 查找当前项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        /// <summary>
        /// 查找当前项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static FrameworkElement FindAnchestorByDataContextType(FrameworkElement current, Type dataType)
        {
            do
            {
                if (current.DataContext != null && current.DataContext.GetType() == dataType)
                {
                    return current;
                }
                current = VisualTreeHelper.GetParent(current) as FrameworkElement;
            }
            while (current != null);
            return null;
        }

        /// <summary>
        /// 查找当前项子项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static T FindChild<T>(DependencyObject current) where T : class
        {

            if (current is T)
            {
                return current as T;
            }
            var count = VisualTreeHelper.GetChildrenCount(current);
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var tmp = VisualTreeHelper.GetChild(current, i);

                    var child = FindChild<T>(tmp);

                    if (child != null) return child;
                }
            }

            return null;
        }

        /// <summary>
        /// 查找当前项子项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindChildren<T>(DependencyObject current) where T : class
        {
            var list = new List<T>();

            if (current is T)
            {
                list.Add(current as T);
            }

            var count = VisualTreeHelper.GetChildrenCount(current);
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var obj = VisualTreeHelper.GetChild(current, i);
                    var ls = FindChildren<T>(obj);
                    list.AddRange(ls);
                }
            }

            return list;
        }
    }
}
