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

namespace Silverlight.Common.Net
{
    /// <summary>
    /// 网络url地址帮助类
    /// </summary>
    public static class Url
    {
        /// <summary>
        /// 是否为正常的url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Uri Convert(string url)
        {
            Uri uri = null;
            Uri.TryCreate(url, UriKind.Absolute, out uri);
            return uri;
        }
    }
}
