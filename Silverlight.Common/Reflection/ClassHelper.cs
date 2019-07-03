//////////////////////////////////////////////////
// Author   : jiamao
// Date     : 2010/09/15
// Usage    : 类型操作类
//////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Silverlight.Common.Reflection
{
    /// <summary>
    /// 类操作
    /// </summary>
    public class ClassHelper
    {
        /// <summary>
        /// 从DLL中获取类
        /// </summary>
        /// <param name="dllPath">DLL路径</param>
        /// <param name="className">类名</param>
        /// <returns></returns>
        public static Type GetClassType(string dllPath, string className)
        {
            Assembly ass = Assembly.Load(dllPath);
            if (!string.IsNullOrEmpty(className))
            {
                return ass.GetType(className);
            }
            else
            {
                Type[] ts = ass.GetTypes();
                if (ts.Length > 0) return ts[0];
                return null;
            }
        }

        /// <summary>
        /// 加载对象实例
        /// </summary>
        /// <param name="dllPath">DLL路径</param>
        /// <param name="className">类名</param>
        /// <returns></returns>
        public static object GetClassObject(string dllPath, string className)
        {
            Type t = GetClassType(dllPath, className);//获取类型

            return Activator.CreateInstance(t);
        }

        /// <summary>
        /// 生成对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="className">对象类路径</param>
        /// <returns></returns>
        public static T LoadInstance<T>(string namespaceName, string className)
        {
            return (T)Assembly.Load(namespaceName).CreateInstance(className);
        }

        /// <summary>
        /// 生成对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="className">对象类路径</param>
        /// <returns></returns>
        public static T LoadInstance<T>(string namespaceName, string className, params object[] pars)
        {
            return (T)Assembly.Load(namespaceName).CreateInstance(className);
        }

        /// <summary>
        /// 获取对象属性的值
        /// </summary>
        /// <param name="Instance">对象实体</param>
        /// <param name="ClassName">所在类</param>
        /// <param name="PropertyName">属性名</param>
        /// <returns></returns>
        public static object GetPropertyValue(object Instance, string PropertyName, bool isStatic=false)
        {
            Type t = Instance.GetType();
            PropertyInfo pi = t.GetProperty(PropertyName);
            if (pi != null)
            {
                return GetPropertyValue(Instance, pi, isStatic);
            }
            return null;
        }

        /// <summary>
        /// 获取对象属性值
        /// </summary>
        /// <param name="Instance"></param>
        /// <param name="property"></param>
        /// <param name="isStatic"></param>
        /// <returns></returns>
        public static object GetPropertyValue(object Instance, PropertyInfo property, bool isStatic = false)
        {
            if (isStatic) return property.GetValue(Instance, BindingFlags.Static, null, null, null);
            else return property.GetValue(Instance, null);
        }

        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <param name="instance">对象</param>
        /// <param name="PropertyName">属性名称</param>
        /// <param name="value">属性值</param>
        public static void SetPropertyValue(object instance, string PropertyName, object value, object[] indexs)
        {
            if (value == null || value.ToString() == "") return;

            Type t = instance.GetType();
            PropertyInfo pi = t.GetProperty(PropertyName);
            if (pi != null)
            {
                SetPropertyValue(instance, pi, value, indexs);
            }
        }

        /// <summary>
        /// 配置属性的值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pi"></param>
        /// <param name="obj"></param>
        public static void SetPropertyValue(object sender, System.Reflection.PropertyInfo pi, object obj, object[] indexs)
        {
            if (pi.CanWrite == false || obj == null || obj.ToString() == "") return;
            var setmethod = pi.GetSetMethod();
            if (setmethod == null || setmethod.IsPublic == false) return;

            if (pi.PropertyType != typeof(String))
            {
                if (pi.PropertyType == typeof(Enum) || pi.PropertyType.BaseType == typeof(Enum))
                {
                    pi.SetValue(sender,obj , indexs);//Convert.ChangeType(obj, typeof(int))
                }
                else if (pi.PropertyType == typeof(Boolean) || pi.PropertyType.BaseType == typeof(Boolean))
                {
                    pi.SetValue(sender, obj.Equals("true") || obj.Equals("1"), indexs);
                }
                else
                {
                    //if (!pi.PropertyType.FullName.Contains("System.")) //这里对自定义不做处理
                    //{
                    //    return;
                    //}

                    pi.SetValue(sender, Convert.ChangeType(obj, pi.PropertyType,null), indexs);
                }
            }
            else
            {
                pi.SetValue(sender, obj == DBNull.Value ? "" : obj.ToString(),
                            indexs);
            }
        }

        /// <summary>  
        /// 快速设置属性  
        /// </summary>  
        /// <param name="sender">对象</param>  
        /// <param name="pi">属性</param>  
        /// <param name="obj">值</param>  
        /// <param name="indexs">索引</param>  
        public static void FastSetPropertyValue(object sender, System.Reflection.PropertyInfo pi, object obj, object[] indexs)
        {
            if (obj == null || obj.ToString() == "") return;

            //获取属性的赋值方法  
            var setMethod = pi.GetSetMethod();
            var invoke = FastInvoke.GetMethodInvoker(setMethod);

            if (pi.PropertyType != typeof(String))
            {
                if (obj != DBNull.Value || obj.ToString() != "")
                {
                    if (pi.PropertyType == typeof(Enum) || pi.PropertyType.BaseType == typeof(Enum))
                    {
                        invoke(sender, new object[] { Convert.ChangeType(obj, typeof(int),null), indexs });
                    }
                    else if (pi.PropertyType == typeof(Boolean) || pi.PropertyType.BaseType == typeof(Boolean))
                    {
                        var strv = obj.ToString();
                        var v = 0;
                        var bv = false;
                        if (int.TryParse(strv, out v))
                        {
                            bv = v != 0;
                        }
                        else
                        {
                            bv = "true".Equals(strv.Trim(), StringComparison.OrdinalIgnoreCase);
                        }
                        invoke(sender, new object[] { bv, indexs });
                    }
                    else
                    {
                        invoke(sender, new object[] { Convert.ChangeType(obj, pi.PropertyType,null), indexs });
                    }
                }
            }
            else
            {
                invoke(sender, new object[] { obj == DBNull.Value ? "" : obj.ToString(), indexs });
            }
        }  

        /// <summary>
        /// 获取对象的所有属性        
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns></returns>
        public static PropertyInfo[] GetTypePropertys(Type type)
        {
            var ps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return ps;
        }

        /// <summary>
        /// 执行一个方法
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="methodName"></param>
        public static void RunMethod(object obj, string methodName, params object[] paras)
        {
            Type t = obj.GetType();
            if (t != null)
            {
                MethodInfo mi = t.GetMethod(methodName);
                if (mi != null)
                {
                    if (mi.IsStatic)
                    {
                        mi.Invoke(null, paras);
                    }
                    else
                    {
                        mi.Invoke(obj, paras);
                    }
                }
            }
        }

        /// <summary>
        /// 执行一个方法
        /// </summary>
        /// <param name="dllpath">DLL路径</param>
        /// <param name="className">所在类名</param>
        /// <param name="methodName">方法名</param>
        public static void RunMethod(string dllpath, string className, string methodName, params object[] paras)
        {
            if (string.IsNullOrEmpty(dllpath) || string.IsNullOrEmpty(methodName)) return;
            Type t = GetClassType(dllpath, className);
            if (t != null)
            {
                MethodInfo mi = t.GetMethod(methodName);
                if (mi != null)
                {
                    if (mi.IsStatic)
                    {
                        mi.Invoke(null, paras);
                    }
                    else
                    {
                        object obj = Activator.CreateInstance(t);
                        mi.Invoke(obj, paras);
                    }
                }
            }
        }

        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static string GetSystemVersion(Assembly asm)
        {
            //获取版本信息
            string VersionType = "";
            string VersionNum = "";
            string version = "";
            //Assembly asm = this.GetType().Assembly;
            Type aType = typeof(AssemblyDescriptionAttribute);
            object[] objarray = asm.GetCustomAttributes(aType, true);

            if (objarray.Length > 0)
            {
                VersionType = ((AssemblyDescriptionAttribute)objarray[0]).Description;

            }
            //获取版本号
            Type bType = typeof(AssemblyFileVersionAttribute);
            objarray = asm.GetCustomAttributes(bType, true);
            if (objarray.Length == 1)
            {
                VersionNum = ((AssemblyFileVersionAttribute)objarray[0]).Version;
            }

            version = VersionType + "V " + VersionNum;

            return version;
        }

        /// <summary>
        /// 拷贝一个对象
        /// </summary>
        /// <typeparam name="T">拷贝的类型</typeparam>
        /// <param name="obj">原对象</param>
        /// <returns></returns>
        public static T CloneObject<T>(T obj)           
        {
            return CloneObject<T, T>(obj);
        }

        /// <summary>
        /// 拷贝一个对象
        /// </summary>
        /// <typeparam name="T">拷贝的类型</typeparam>
        /// <param name="obj">原对象</param>
        /// <returns></returns>
        public static Result CloneObject<Source, Result>(Source obj)
        {
            return CloneObject<Source, Result>(obj, null);
        }

        /// <summary>
        /// 拷贝属性
        /// </summary>
        /// <typeparam name="Source"></typeparam>
        /// <typeparam name="Result"></typeparam>
        /// <param name="obj"></param>
        /// <param name="fun"></param>
        /// <returns></returns>
        public static Result CloneObject<Source, Result>(Source obj, Func<PropertyInfo, PropertyInfo> fun)
        {
            var t = typeof(Source);
            var propertys = GetTypePropertys(t);//获取其下的所有属性

            var newobj = Activator.CreateInstance<Result>();//生成新的实例

            foreach (var p in propertys)
            {
                var pvalue = GetPropertyValue(obj, p);//获取原对象的值

                PropertyInfo pi = fun != null ? fun(p) : p;//通过外部映射

                if (pi != null && pi.CanWrite)
                {
                    //把它赋给新对象
                    SetPropertyValue(newobj, pi, pvalue, null);
                }
            }

            return newobj;
        }
    }
}
