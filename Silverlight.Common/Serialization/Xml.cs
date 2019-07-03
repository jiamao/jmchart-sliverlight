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
using System.Xml.Serialization;

namespace Silverlight.Common.Serialization
{
    /// <summary>
    /// xml序列化辅助类
    /// </summary>
    public static class Xml
    {
        public static object Parse<T>(string source)
        {
            var bs = System.Text.Encoding.UTF8.GetBytes(source);
            return Parse<T>(bs);
        }

        public static object Parse<T>(byte[] data)
        {
            var ms = new System.IO.MemoryStream(data);
            var xml = new XmlSerializer(typeof(T));
            return xml.Deserialize(ms);
        }

        /// <summary>
        /// XML反序列化
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string Parse(object target)
        {
            var xml = new XmlSerializer(target.GetType());
            var ms = new System.IO.MemoryStream();
            xml.Serialize(ms,target);
            return System.Text.Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
        }
    }
}
