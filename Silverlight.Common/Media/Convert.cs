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

namespace Silverlight.Common.Media
{
    /// <summary>
    /// 多媒体转换
    /// </summary>
    public static class Convert
    {
        /// <summary>
        /// 转换字符串格工的颜色
        /// </summary>
        /// <param name="source">#000000</param>
        /// <returns></returns>
        public static Color ToColor(string source)
        {
            byte a=255;
            byte r=0;
            byte g=0;
            byte b=0;
            if (!string.IsNullOrWhiteSpace(source))
            {
                source = source.TrimStart('#');
                if (source.Length > 1)
                {
                    var bv = source.Substring(source.Length - 2);
                    b = System.Convert.ToByte(bv, 16);
                }
                if (source.Length > 3)
                {
                    var bv = source.Substring(source.Length - 4,2);
                    g = System.Convert.ToByte(bv, 16);
                }
                if (source.Length > 5)
                {
                    var bv = source.Substring(source.Length - 6, 2);
                    r = System.Convert.ToByte(bv, 16);
                }
                if (source.Length > 7)
                {
                    var bv = source.Substring(source.Length - 8, 2);
                    a = System.Convert.ToByte(bv, 16);
                }

                return Color.FromArgb(a, r, g, b);
            }

            return Colors.Transparent;
        }
    }
}
