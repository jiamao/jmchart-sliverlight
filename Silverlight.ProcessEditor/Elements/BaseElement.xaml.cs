using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Silverlight.ProcessEditor.Elements
{
    public partial class BaseElement : UserControl
    {
        public BaseElement()
        {
            InitializeComponent();

            OutLines = new List<ConnectLine>();
        }

        /// <summary>
        /// 画布
        /// </summary>
        public Model.BackGrid CanvasGrid { get; set; }

        public delegate void MoveElementDelegate();
        /// <summary>
        /// 位置发生改变事件
        /// </summary>
        public event MoveElementDelegate MoveHandle;

        public event EventHandler Change;

        /// <summary>
        /// 所在网格索引
        /// </summary>
        public Point Position
        {
            get { return Model.Part.Position; }
            set { Model.Part.Position = value; }
        }

        /// <summary>
        /// 入口坐标
        /// </summary>
        public Point InPoint { get; set; }

        /// <summary>
        /// 所有的出口线路
        /// </summary>
        public List<ConnectLine> OutLines { get; set; }

        /// <summary>
        /// 出品坐标
        /// </summary>
        public Point OutPoint { get; set; }

        /// <summary>
        /// 距网格边距
        /// </summary>
        public Thickness GridMargin { get; set; }

        /// <summary>
        /// 部件信息
        /// </summary>
        public Model.Circuit Model { get {
            return this.DataContext as Model.Circuit;
        }
            set {
                this.DataContext = value;
            }
        }
        
        /// <summary>
        /// 重新定位
        /// </summary>
        public virtual void ResetPosition(Model.BackGrid grid = null)
        {
            if (grid == null) grid = CanvasGrid;
            else CanvasGrid = grid;

            this.Height = Helper.Config.GridSize.Height - Helper.Config.ElementMargin.Top - Helper.Config.ElementMargin.Bottom;
            this.Width = Helper.Config.GridSize.Width - Helper.Config.ElementMargin.Left - Helper.Config.ElementMargin.Right;

            Canvas.SetZIndex(this, Helper.ShapZIndex.Shap.GetHashCode());

            var x = this.Position.X * Helper.Config.GridSize.Width + Helper.Config.ElementMargin.Left + grid.GridRect.Left;
            var y = this.Position.Y * Helper.Config.GridSize.Height + Helper.Config.ElementMargin.Top + grid.GridRect.Top;

            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);

            CreateInOutPosition();

            MoveCallback();
        }

        /// <summary>
        /// 生成输入输出坐标
        /// </summary>
        public void CreateInOutPosition()
        {
            var x = Canvas.GetLeft(this);
            var y = Canvas.GetTop(this);

            var w = Math.Max(this.Width, this.ActualWidth);
            var h = Math.Max(this.Height, this.ActualHeight);
            this.InPoint = new Point(x + 2, y + h / 2);

            this.OutPoint = new Point(x + w - 2, y + h / 2);
        }

        /// <summary>
        /// 元素位置变更
        /// </summary>
        public void MoveCallback()
        {
            if (MoveHandle != null) MoveHandle();
        }

        /// <summary>
        /// 开始连接
        /// </summary>
        public void StartConnect()
        {
            try
            {
                var line = new ConnectLine(this.CanvasGrid);
                line.StartElement = this;
                Helper.DragDrop.DoDragDrop(line);
                line.Show();

                if (Change != null)
                {
                    line.Change -= Change;
                    line.Change += Change;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
