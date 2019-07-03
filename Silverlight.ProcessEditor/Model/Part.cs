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

namespace Silverlight.ProcessEditor.Model
{
    /// <summary>
    /// 元件model
    /// </summary>
    public class Part
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 组件类型
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// 复制新对象
        /// </summary>
        /// <returns></returns>
        public Part Clone()
        {
            var p = new Part()
            {
                Id = this.Id,
                Name = this.Name,
                TypeId = this.TypeId
            };
            return p;
        }

        public readonly static string StartId = "0";

        public readonly static string EndId = "100";

        public bool IsStart { get {
            return StartId == Id;
        } }

        public bool IsEnd {
            get {
                return EndId == Id || TypeId == EndId;
            }
        }
    }
}
