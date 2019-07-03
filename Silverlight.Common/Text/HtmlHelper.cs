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

namespace Silverlight.Common.Text
{
/// <summary>
/// html字符处理
/// </summary>
    public class HtmlHelper
    {
        /// <summary>
        /// 转换xaml的TEXT
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Encode(string source)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                return source.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
            }
            return source;
        }
    }
}
