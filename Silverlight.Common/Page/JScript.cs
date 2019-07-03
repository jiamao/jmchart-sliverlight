using System;
using System.Net;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;

namespace Silverlight.Common.Page
{
    public static class JScript
    {
        static Dictionary<string, System.Windows.Browser.ScriptObject> jsObjectCache = new Dictionary<string, ScriptObject>();
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="code"></param>
        /// <param name="par"></param>
        public static object RunScript(string code, string method, params object[] pars)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(code))
                {
                    System.Windows.Browser.ScriptObject jsobj;
                    if (!jsObjectCache.TryGetValue(code, out jsobj))
                    {
                        jsobj = System.Windows.Browser.HtmlPage.Window.Eval(string.Format("({0})", code)) as System.Windows.Browser.ScriptObject;
                        if (jsobj != null) jsObjectCache.Add(code, jsobj);
                    }

                    if (jsobj != null && !string.IsNullOrWhiteSpace(method))
                    {
                        var m = jsobj.GetProperty(method) as System.Windows.Browser.ScriptObject;
                        if (m != null)
                        {
                            return m.InvokeSelf(pars);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return null;
        }
    }
}
