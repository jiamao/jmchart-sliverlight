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

namespace JMChart.Model
{
    /// <summary>
    /// 图关联属性
    /// </summary>
    public class ItemMapping
    {
        /// <summary>
        /// 当前属性绑定类型
        /// </summary>
        public enum EnumDataMember
        {
            /// <summary>
            /// X轴
            /// </summary>
            X = 0,
            /// <summary>
            /// X轴分类属性
            /// </summary>
            XCat = 1,
            /// <summary>
            /// Y轴
            /// </summary>
            Y = 2,
            /// <summary>
            /// Y轴分类
            /// </summary>
            YCat=3,
            /// <summary>
            /// clm图与中心点关联值
            /// </summary>
            CLMLink=4,
            /// <summary>
            /// 标签
            /// </summary>
            Lengend=5,
            /// <summary>
            /// 普通属性
            /// </summary>
            Mebmer=10
        }

        /// <summary>
        /// 当前属性绑定类型
        /// </summary>
        public EnumDataMember DataMember
        {
            get;
            set;
        }

        /// <summary>
        /// 绑定名称
        /// </summary>
        public string MemberName
        {
            get;
            set;
        }

        /// <summary>
        /// 原列名
        /// ///membername有可能已是改变了的名称
        /// </summary>
        public string OldName { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 标识名称。预留用
        /// </summary>
        public string MarkName { get; set; }
    }
}
