using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class Circuit
    {
        public Circuit() {
            Ins = new List<Circuit>();
            Outs = new List<Circuit>();
        }

        /// <summary>
        /// 对应的元件
        /// </summary>
        public Part Part { get; set; }

        /// <summary>
        /// 对应环路结束元件
        /// </summary>
        public Part EndPart { get; set; }

        /// <summary>
        /// 所有入口
        /// </summary>
        public List<Circuit> Ins { get; set; }

        /// <summary>
        /// 所有出口
        /// </summary>
        public List<Circuit> Outs { get; set; }

        /// <summary>
        /// 转为json格式
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            if (this.Part.Id == "1") return "";

            var json = new System.Text.StringBuilder();

            json.Append(this.Part.Id);

            if (Outs != null && Outs.Count > 0)
            {
                json.Append(",");
                if (Outs.Count > 1)
                {
                    json.Append("[");
                    foreach (var c in Outs)
                    {
                        if (c.Outs.Count > 1)
                        {
                            json.Append(c.ToJson());
                        }
                        else
                        {
                            json.Append(c.GetGroupJson());
                        }
                        json.Append(",");
                    }
                    if (json[json.Length - 1] == ',') json.Remove(json.Length - 1, 1);
                    json.Append("]");

                    var end = GetEndPart(Outs[0], Outs);
                    if (end != null)
                    {
                        json.Append(",");
                        json.Append(end.ToJson());
                    }
                }
                else
                {
                    if (Outs.Count > 0)
                    {
                        foreach (var c in Outs)
                        {
                            if (c.Ins.Count == 1)
                            {
                                json.Append(c.ToJson());
                            }
                        }
                    }
                }
            }

            var str = json.ToString().Trim(',');
            //if (this.Part.IsStart) str += "," + Part.EndId;
            if ((str.Length > 0 && 
                str.Contains(",") &&
                Outs.Count > 0  && 
                !Outs[0].Part.IsEnd) &&
                !(Ins.Count == 1 && Ins[0].Outs.Count == 1))//&& Ins.Count != 1
            {
                return "{" + str + "}";
            }
            else
            {
                return str;
            }
        }


        public string GetGroupJson()
        {
            var json = new System.Text.StringBuilder();
            if (Ins.Count < 2)
            {
                json.Append(this.Part.Id);

                if (Outs != null && Outs.Count > 0)
                {
                    json.Append(",");
                    if (Outs.Count > 1)
                    {
                        json.Append("[");
                        foreach (var c in Outs)
                        {
                            if (c.Outs.Count > 0 && c.Outs[0].Ins.Count == 1)
                            {
                                json.Append(c.ToJson());
                            }
                            else
                            {
                                json.Append(c.GetGroupJson());
                            }
                            json.Append(",");
                        }
                        if (json[json.Length - 1] == ',') json.Remove(json.Length - 1, 1);
                        json.Append("]");

                    }
                    else
                    {
                        if (Outs.Count > 0)
                        {
                            foreach (var c in Outs)
                            {
                                json.Append(c.GetGroupJson());
                            }
                        }
                    }
                }
            }

            var str = json.ToString().Trim(',');
            if (str.Length > 0 && 
                Outs.Count > 0 && 
                str.Contains(",") &&
                (Ins.Count != 1 || (Ins.Count == 1 && Ins[0].Outs.Count > 1 && Outs[0].Ins.Count == 1)) //&& Outs.Count == 1 
                && !Outs[0].Part.IsEnd)
            {
                return "{" + str + "}";
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// 获取多个线路的共同终结点
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        private Model.Circuit GetEndPart(Model.Circuit c,IEnumerable <Model.Circuit> cs)
        {
            if (c.Ins.Count > 0)
            {
                Model.Circuit end = null;
                foreach (var mc in cs)
                {
                    //如果这条路与提供的结点相同，则查找另一条路、、保证所有子通路结点一至
                    if ((mc.Outs.Count > 1 ||
                        mc.Outs.Count == 0 ||
                        mc.Outs[0].Part != c.Part) && !CheckEndPart(c, mc.Outs))
                    {
                        end = null;
                        break;
                    }
                    else
                    {
                        end = c;
                    }
                }
                if (end == null && c.Outs.Count > 0)
                {
                    end = GetEndPart(c.Outs[0], cs);
                }
                return end;
            }
            return null;
        }

        /// <summary>
        /// 检查是否当前元素在指定的集合中所有线路中都存在
        /// 如果是表示他们都以此元件为终结点
        /// </summary>
        /// <param name="c"></param>
        /// <param name="cs"></param>
        /// <returns></returns>
        private bool CheckEndPart(Model.Circuit c, IEnumerable<Model.Circuit> cs)
        {
            bool isexists = false;
            foreach (var mcs in cs)
            {
                isexists = true;
                if (mcs.Part == c.Part)
                {
                    isexists = true;
                }
                else if (!CheckEndPart(c, mcs.Outs))
                {
                    isexists = false;
                    break;
                }
            }

            return isexists;
        }

        /// <summary>
        /// 生成线路
        /// </summary>
        /// <returns></returns>
        public CircuitGroup CreateGroup()
        {
            //if (this.Part.IsStart || Ins.Count > 1)                
            //{
            //    var group = new CircuitGroup();
            //    group.StartParts.Add(this);
            //    if (this.Outs.Count > 1)
            //    {
            //        group.EndPart = this;
            //    }
            //}
            //else if (this.Ins.Count == 1 && Ins[0].Outs.Count > 1)
            //{
            //    var group = new CircuitGroup();
            //    group.StartParts = Ins[0].Outs;
            //}
            return null;
        }
    }

    /// <summary>
    /// 线路串路或并路
    /// </summary>
    public class CircuitGroup
    {
        public CircuitGroup()
        {
            Parts = new List<Circuit>();
            Groups = new List<CircuitGroup>();
        }

        /// <summary>
        /// 起始元件
        /// </summary>
        public List<Circuit> Parts { get; set; }

        /// <summary>
        /// 紧接着的后面的线路
        /// </summary>
        public List<CircuitGroup> Groups { get; set; }
    }
}
