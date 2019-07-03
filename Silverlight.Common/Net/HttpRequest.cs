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
    /// http请求
    /// </summary>
    public static class HttpRequest
    {
        /// <summary>
        /// http请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public static void Request(string uri, byte[] data,Action<byte[]> callback = null)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(uri);
            if (data != null)
            {
                request.Method = "post";
                request.AllowReadStreamBuffering = true;
                request.AllowWriteStreamBuffering = true;
                request.BeginGetRequestStream((r) =>
                {
                    var s = request.EndGetRequestStream(r);
                    s.Write(data, 0, data.Length);
                    s.Close();
                    if (callback != null)
                    {
                        request.BeginGetResponse((ire) =>
                        {
                            if (request.HaveResponse)
                            {
                                var response = request.EndGetResponse(ire) as HttpWebResponse;
                                {
                                    var stream = response.GetResponseStream();
                                    var result = new System.Collections.Generic.List<byte>();
                                    var b = stream.ReadByte();
                                    while (b != -1)
                                    {
                                        result.Add((byte)b);
                                        b = stream.ReadByte();
                                    }
                                    callback(result.ToArray());
                                }
                            }
                        }, null);
                    }
                }, null);
            }
        }

        /// <summary>
        /// http请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public static void Request(string uri,string data, Action<string> callback = null)
        {
            var bdata = System.Text.Encoding.UTF8.GetBytes(data);
            if (callback != null)
            {
                Request(uri, bdata, (r) =>
                {
                    var result = System.Text.Encoding.UTF8.GetString(r, 0, r.Length);
                    callback(result);
                });
            }
            else
            {
                Request(uri, bdata);
            }
        }
    }
}
