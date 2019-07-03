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

namespace JMChart.Common
{
    /// <summary>
    /// 辅助类
    /// </summary>
    static class Helper
    {
        /// <summary>
        /// 获取对象属性的值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object GetPropertyName(object instance, string name)
        {
            var tmpControl = new PropertyBindingControl();
            tmpControl.SetBinding(PropertyBindingControl.PropertyValueProperty, new System.Windows.Data.Binding(name));
            tmpControl.DataContext = instance;

            //var p = instance.GetType().GetProperty(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreCase);
            //if (p != null)
            //{
            //    return p.GetValue(instance, null);
            //}
            //return null;
            return tmpControl.PropertyValue;
        }

        /// <summary>
        /// 获取对象中的指定方法
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static System.Reflection.MethodInfo GetMethod(object instance, string name)
        { 
            var m = instance.GetType().GetMethod(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreCase);
            return m;
        }

        /// <summary>
        /// 执行指定的方法
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="paramers"></param>
        /// <returns></returns>
        public static object RunMethod(object instance, string name,object[] paramers)
        {
            var m = GetMethod(instance, name);
            return m.Invoke(instance, paramers);
        }

        /// <summary>
        /// 获取对象的值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns></returns>
        public static double? GetNumberValue(object instance, string name)
        {
            var obj = GetPropertyName(instance,name);
            if (obj != null)
            {
                double value = 0;
                double.TryParse(obj.ToString(), out value);
                return value;
            }
            return null;
        }

        /// <summary>
        /// 获取集合指定属生的最大值与最小值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static System.Collections.Generic.Dictionary<string, double?[]> GetMaxandNinValue(System.Collections.ICollection data, string[] names)
        {
            var dic = new System.Collections.Generic.Dictionary<string, double?[]>();
            foreach (var name in names)
            {                
                var values = new double?[2];
                foreach (var d in data)
                {
                    var v = GetPropertyName(d, name);
                    if (v != null && Silverlight.Common.Data.TypeHelper.IsNumber(v.GetType()))
                    {
                        var n = double.Parse(v.ToString());
                        if (values[0] == null || values[0] > n)
                        {
                            values[0] = n;
                        }
                        if (values[1] == null || values[1] < n)
                        {
                            values[1] = n;
                        }
                    }
                        //如果为字符型
                        //第0个值为空表示为非数字型
                    else
                    {
                        values[1] = data.Count;
                    }
                }
                if(!dic.ContainsKey(name))dic.Add(name, values);
            }
            return dic;
        }

        /// <summary>
        /// 获取集合指定属生的最大值与最小值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static System.Collections.Generic.Dictionary<string, double?[]> GetMaxandNinValue(System.Collections.ICollection data, 
            System.Collections.Generic.IEnumerable<Model.ItemMapping> mappings)
        {
            var dic = new System.Collections.Generic.Dictionary<string, double?[]>();
            foreach (var name in mappings)
            {
                var values = new double?[2];
                foreach (var d in data)
                {
                    var v = GetNumberValue(d, name.MemberName);
                    if (v != null)
                    {
                        if (values[0] == null || values[0] > v)
                        {
                            values[0] = v;
                        }
                        if (values[1] == null || values[1] < v)
                        {
                            values[1] = v;
                        }
                    }
                }
                if (!dic.ContainsKey(name.MemberName)) dic.Add(name.MemberName, values);
            }
            return dic;
        }

        /// <summary>
        /// 处理最小值规则
        /// </summary>
        /// <param name="minv"></param>
        /// <returns></returns>
        public static double CheckMinValue(double minv)
        {
            if (minv >= 0) minv = 0;
            else
            {
                minv = minv - 10;
            }
            return minv;
        }

        /// <summary>
        /// 生成一个圆
        /// </summary>
        /// <param name="center"></param>
        /// <param name="rotate"></param>
        /// <returns></returns>
        public static Ellipse CreateEllipse(Point center, double rotate)
        {
            var ell = new Ellipse();
            ell.Width = ell.Height = rotate;

            var x = center.X - rotate / 2;
            var y = center.Y - rotate / 2;
            System.Windows.Controls.Canvas.SetLeft(ell, x);
            System.Windows.Controls.Canvas.SetTop(ell, y);

            return ell;
        }

        /// <summary>
        /// 生成一个圆 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="rotate"></param>
        /// <returns></returns>
        public static EllipseGeometry CreateEllipseGeometry(Point center, double rotate)
        {
            var ell = new EllipseGeometry();
            ell.Center = center;
            ell.RadiusX = ell.RadiusY = rotate;
            return ell;
        }

        static System.Text.RegularExpressions.Regex ColumnReg = new System.Text.RegularExpressions.Regex("#C\\{(?<column>[^\\}]*)\\}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        public delegate object DelegateGetValue(string name);
        /// <summary>
        /// 处理标签名称，替换关健值
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        internal static string DserLabelName(string source, System.Collections.Generic.Dictionary<string, string> pars, DelegateGetValue getValueAction = null)
        {
            if (string.IsNullOrWhiteSpace(source)) return source;
            if (pars == null) pars=new System.Collections.Generic.Dictionary<string,string>();

            if (pars.ContainsKey("YName"))
            {                
                source = source.Replace("#YName", pars["YName"]);
            }
            if (pars.ContainsKey("Y"))
            {
                source = source.Replace("#Y", pars["Y"]);
            }
           
            var ms = ColumnReg.Matches(source);
            foreach (System.Text.RegularExpressions.Match m in ms)
            {
                var col = m.Groups["column"].Value;
                if (pars.ContainsKey(col))
                {
                    source = source.Replace(m.Value, pars[col]);
                }
                else if (getValueAction != null)
                {
                    var obj = getValueAction(col);
                    source = source.Replace(m.Value, obj == null?"":obj.ToString());
                }
            }
            return source;
        }
    }
}
