using System;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Json;

namespace Silverlight.Common.Serialization
{
    /// <summary>
    /// JSON序列化
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Parse<T>(string source)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(source)))
                {
                    var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
                    var obj = ser.ReadObject(ms);
                    return (T)obj;
                }
            }
            return default(T);
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static object Parse(Type t, string source)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(source)))
                {
                    var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(t);
                    var obj = ser.ReadObject(ms);
                    return obj;
                }
            }
            return null;
        }

        /// <summary>
        /// JSON字符串解析
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static JsonValue Parse(string source)
        {
            var obj = JsonObject.Parse(source);
            return obj;
        }

        /// <summary>
        /// 转换json对象为json字符串
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string Parse(JsonValue json)
        {
            var ms = new MemoryStream();
            json.Save(ms);
            var source = System.Text.Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
            return source;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string Parse(object instance)
        {
            if (instance != null)
            {
                using (var ms = new MemoryStream())
                {
                    var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(instance.GetType());
                    ser.WriteObject(ms,instance);

                    return System.Text.Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
                }
            }
            return null;
        }
    }
}
