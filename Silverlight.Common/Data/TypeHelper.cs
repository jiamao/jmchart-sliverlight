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

namespace Silverlight.Common.Data
{
    public static class TypeHelper
    {
        /// <summary>
        /// 检查类型是否为数值类型
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsNumber(Type t)
        {
            return t == typeof(decimal) ||
                                t == typeof(double) ||
                            t == typeof(float) ||
                                t == typeof(int) ||
                                t == typeof(long) ||
                                    t == typeof(uint) ||
                                    t == typeof(Single) ||
                                t == typeof(UInt32) ||
                                t == typeof(UInt64) ||
                                t == typeof(Int32) ||
                                t == typeof(Int64) ||
                                t == typeof(byte) ||
                                t == typeof(sbyte);
        }

        /// <summary>
        /// 检查类型是否为数值类型
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsNumber(string t)
        {
            return t.Equals("system.decimal", StringComparison.OrdinalIgnoreCase) ||
                               t.Equals("system.double", StringComparison.OrdinalIgnoreCase) ||
                           t.Equals("system.float", StringComparison.OrdinalIgnoreCase) ||
                                t.Equals("system.int", StringComparison.OrdinalIgnoreCase) ||
                                t.Equals("system.long", StringComparison.OrdinalIgnoreCase) ||
                               t.Equals("system.uint", StringComparison.OrdinalIgnoreCase) ||
                               t.Equals("system.UInt32", StringComparison.OrdinalIgnoreCase) ||
                               t.Equals("system.UInt64", StringComparison.OrdinalIgnoreCase) ||
                               t.Equals("system.Int32", StringComparison.OrdinalIgnoreCase) ||
                               t.Equals("system.Int64", StringComparison.OrdinalIgnoreCase) ||
                               t.Equals("system.byte", StringComparison.OrdinalIgnoreCase);
        }

    }
}
