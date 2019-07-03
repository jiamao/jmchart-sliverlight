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
using System.Reflection;
using System.ComponentModel;

namespace Silverlight.Common.Reflection
{
    public static class PropertyManager
    {
        /// <summary>
        /// 获取指定的说明
        /// </summary>
        /// <param name="t"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetFieldDescription<T>(string name)
        {
            return GetFieldDescription(typeof(T), name);
        }

        /// <summary>
        /// 获取指定的说明
        /// </summary>
        /// <param name="t"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetFieldDescription(Type t, string name)
        {
            var finfo = t.GetField(name);
            var cAttr = finfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (cAttr.Length > 0)
            {
                var desc = cAttr[0] as DescriptionAttribute;
                if (desc != null)
                {
                    return desc.Description;
                }
            }
            return name;
        }
    }
}
