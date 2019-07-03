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
using System.Threading;
using System.Reflection;
using System.IO;
using System.Windows.Resources;
using System.Windows.Markup;

namespace Silverlight.Common.Reflection
{
    public class XamlHelper
    {
        /// <summary>
        /// 转换xaml为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="source">xaml字符</param>
        /// <returns></returns>
        public static T LoadXaml<T>(string source) where T : DependencyObject
        {
            var el = (T)XamlReader.Load(source);
            return el;
        }

        /// <summary>
        /// 解析信息为xaml
        /// 支持标签[b][/b]粗体
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static System.Collections.Generic.List<string> CreateXamlInfo(string source, string color = "Black", int lineHeight = 0, int width = 0, int fontsize = 12)
        {
            var lines = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrWhiteSpace(source))
            {
                var boldReg = new System.Text.RegularExpressions.Regex(@"\[b\](?<bold>.*)\[\/b\]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                var reader = new System.IO.StringReader(source);
                var strlineheight = (lineHeight > 0 ? "LineHeight=\"" + lineHeight + "\"" : "");
                var strwidth = width > 0 ? "Width=\"" + width + "\"" : "";
                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    var ms = boldReg.Matches(line);
                    var temp = string.Empty;
                    var index = 0;
                    foreach (System.Text.RegularExpressions.Match m in ms)
                    {
                        //第一次处理此行
                        if (m.Index > index)
                        {
                            temp += "<TextBlock HorizontalAlignment=\"Stretch\" Foreground=\"" + color + "\" TextWrapping=\"Wrap\" " + 
                                strlineheight + " FontSize=\"" + fontsize + "\" Text=\"" + Text.HtmlHelper.Encode(line.Substring(index, m.Index - index)) + "\" />";
                            //temp += line.Substring(index, m.Index - index);
                        }

                        temp += "<TextBlock HorizontalAlignment=\"Stretch\" Foreground=\"" + color + "\" FontWeight=\"Bold\" TextWrapping=\"Wrap\" " + 
                            strlineheight + " FontSize=\"" + fontsize + "\"  Text=\"" + Text.HtmlHelper.Encode(m.Groups["bold"].Value) + "\" />";
                        //temp+=string.Format("<b>{0}</b>",m.Groups["bold"].Value);
                        index = m.Index + m.Value.Length;
                    }
                    //第一次处理此行
                    if (string.IsNullOrWhiteSpace(line) || index < line.Length - 1)
                    {
                        temp += "<TextBlock HorizontalAlignment=\"Stretch\" Foreground=\"" + color + "\" FontSize=\"" + fontsize + "\"  TextWrapping=\"Wrap\" " + 
                            strlineheight + " Text=\"" +
                            (string.IsNullOrWhiteSpace(line) ? " " : Text.HtmlHelper.Encode(line.Substring(index))) + "\" />";
                    }

                    lines.Add(temp);
                }
            }
            return lines;
        }
    }
}
