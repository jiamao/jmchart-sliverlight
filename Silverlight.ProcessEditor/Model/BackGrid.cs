using System;
using System.Net;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;

namespace Silverlight.ProcessEditor.Model
{
    public class BackGrid
    {
        /// <summary>
        /// 背景画布
        /// </summary>
        public Canvas BackCanvas
        { 
            get; 
            set;
        }

        /// <summary>
        /// 当前线路对象
        /// </summary>
        public Circuit Model { get; set; }

        /// <summary>
        /// 起始点
        /// </summary>
        public Elements.Start StartPoint { get; set; }
        /// <summary>
        /// 终结点
        /// </summary>
        public Elements.Start EndPoint { get; set; }

        public event EventHandler Change;

        /// <summary>
        /// 当前所有元素
        /// </summary>
        ObservableCollection<Elements.BaseElement> elements = new ObservableCollection<Elements.BaseElement>();

        public BackGrid(Canvas canvas)
        {
            BackCanvas = canvas;
            gridColor.Opacity = 0.5;

            StartPoint = new Elements.Start();
            EndPoint = new Elements.Start();

            StartPoint.Model = this.Model = new Model.Circuit() { Part = new Model.Part() { Id = "0", Name = "Start" } };
            StartPoint.Fill = Colors.Black;
            EndPoint.Model = new Model.Circuit() { Part = new Model.Part() { Id = "1", Name = "End" } };

            EndPoint.Fill = Colors.Black;

            this.AddElement(StartPoint);
            this.AddElement(EndPoint);
        }

        /// <summary>
        /// 当前网格坐标与宽高
        /// </summary>
        public Rect GridRect { get; set; }

        /// <summary>
        /// 画网格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawGrid();

            //ResetPosition();//重新定位
        }

        //网格颜色
        Brush gridColor = new SolidColorBrush(Colors.Gray);


        System.Collections.ObjectModel.ObservableCollection<Line> lines = new System.Collections.ObjectModel.ObservableCollection<Line>();

        /// <summary>
        /// 在画布上画网格
        /// </summary>
        public void DrawGrid()
        {
            //移除所有网格
            foreach (var l in lines)
            {
                this.BackCanvas.Children.Remove(l);
            }

            var w =  double.IsNaN(this.BackCanvas.Width) ? this.BackCanvas.MinWidth : this.BackCanvas.Width;
            var h = double.IsNaN(this.BackCanvas.Height ) ? this.BackCanvas.MinHeight : this.BackCanvas.Height;
            Helper.Config.ColumnCount = (int)(w / Helper.Config.GridSize.Width);//列数
            Helper.Config.RowCount = (int)(h / Helper.Config.GridSize.Height);//行数

            var vheight = Helper.Config.RowCount * Helper.Config.GridSize.Height;
            var hwidth = Helper.Config.ColumnCount * Helper.Config.GridSize.Width;
            var rectLeft = (w - hwidth) / 2;
            var rectTop = (h - vheight) / 2;

            //画列
            for (double i = 0; i <= Helper.Config.ColumnCount; i++)
            {
                var l = new Line();
                l.Width = 1;
                l.StrokeThickness = 1;

                l.Stroke = gridColor;
                l.StrokeDashArray.Add(2);
                var left = i * Helper.Config.GridSize.Width + rectLeft;
                l.SetValue(Canvas.LeftProperty, left);
                l.SetValue(Canvas.TopProperty, rectTop);
                l.Height = l.Y2 = vheight;
                l.SetValue(Canvas.ZIndexProperty, Helper.ShapZIndex.Grid.GetHashCode());
                this.BackCanvas.Children.Add(l);
                lines.Add(l);
            }
            //画行
            for (double i = 0; i <= Helper.Config.RowCount; i++)
            {
                var l = new Line();
                l.Height = 1;
                l.StrokeThickness = 1;
                l.Stroke = gridColor;
                l.StrokeDashArray.Add(2);
                var top = i * Helper.Config.GridSize.Height + rectTop;
                l.SetValue(Canvas.TopProperty, top);
                l.SetValue(Canvas.LeftProperty, rectLeft);

                l.Width = l.X2 = hwidth;
                l.SetValue(Canvas.ZIndexProperty, Helper.ShapZIndex.Grid.GetHashCode());
                this.BackCanvas.Children.Add(l);
                lines.Add(l);
            }

            ///网络距型
            GridRect = new Rect(rectLeft, rectTop, hwidth, vheight);

            ResetPosition();//重新定位
        }

        /// <summary>
        /// 检查网格大小
        /// //如果有超出的元素。则重置大小
        /// </summary>
        public void CheckCanvasGrid()
        {
            var w = 0d;
            var h = 0d;
            foreach (var e in elements)
            {
                if (e is Elements.Start) continue;

                var l = Canvas.GetLeft(e);
                var t = Canvas.GetTop(e);
                if (l + Helper.Config.GridSize.Width * 2 > w) { 
                    w = l + Helper.Config.GridSize.Width * 2;
                    if (l <= Helper.Config.GridSize.Width) w += Helper.Config.GridSize.Width;
                }
                if (t + Helper.Config.GridSize.Height * 2 > h) h = t + Helper.Config.GridSize.Height * 2;               
            }

            var resetW = w > 0 && BackCanvas.Width != w && w > BackCanvas.MinWidth;
            var resetH = h > 0 && BackCanvas.Height != h && h > BackCanvas.MinHeight;
            if (resetW) BackCanvas.Width = w;
            if (resetH) BackCanvas.Height = h;

            if (resetW || resetH)
            {
                DrawGrid();
            }
        }

        /// <summary>
        /// 获取指定网格中的元素
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Elements.BaseElement GetElement(Point p)
        {
            foreach (var e in elements)
            {
                if (e.Position == p)
                {
                    return e;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取指定坐标的元素
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Elements.BaseElement GetElement(double x,double y)
        {
            foreach (var e in elements)
            {
                var l = Canvas.GetLeft(e);
                var t = Canvas.GetTop(e);
                if (x >= l && x <= l + e.ActualWidth &&
                    y >= t && y <= t + e.ActualHeight)
                    return e;
            }
            return null;
        }

        /// <summary>
        /// 根据坐标获取控件所在的网格
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Point CheckPosition(Point p)
        {
            var offsetx = p.X - GridRect.Left;
            var offsety = p.Y - GridRect.Top;

            var position = new Point();
            position.X = (int)(offsetx / Helper.Config.GridSize.Width);
            position.Y = (int)(offsety / Helper.Config.GridSize.Height);

            if (position.X < 1) position.X = 1;
            //else if (position.X > Helper.Config.ColumnCount - 1) position.X = Helper.Config.ColumnCount - 1;
            if (position.Y < 0) position.Y = 0;
            //else if (position.Y > Helper.Config.RowCount - 1) position.Y = Helper.Config.RowCount - 1;

            return position;
        }

        /// <summary>
        /// 添加控件
        /// </summary>
        /// <param name="el"></param>
        public void AddControl(UIElement el)
        {
            if (!this.BackCanvas.Children.Contains(el))
            {
                this.BackCanvas.Children.Add(el);
            }
        }

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="el"></param>
        public void AddElement(Elements.BaseElement el)
        {            
            if (!elements.Contains(el))
            {
                this.BackCanvas.Children.Add(el);
                elements.Add(el);
                el.CanvasGrid = this;

                el.ResetPosition(this);
                if (Change != null)
                {
                    el.Change += Change;
                }                               

                CheckCanvasGrid();

                //如果是出口，则直接与终结点连接
                if (el.Model.Part.IsEnd)
                {
                    Connect(el, EndPoint);
                }
            }
        }

        /// <summary>
        /// 连接二个结点
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void Connect(Elements.BaseElement start, Elements.BaseElement end)
        {
            var line = new Elements.ConnectLine(this);
            line.StartElement = start;
            line.Connect(end);
        }

        public void Editor_MouseMove(object sender, MouseEventArgs e)
        {
            var data = Helper.DragDrop.GetData();
            if (data is System.Windows.Controls.Primitives.Popup)
            {
                var pop = data as System.Windows.Controls.Primitives.Popup;

                var unit = pop.Child as Elements.Unit;
                if (unit != null)
                {
                    unit.CanvasGrid = this;
                    var p = e.GetPosition(null);

                    pop.HorizontalOffset = p.X - unit.ActualWidth / 2;
                    pop.VerticalOffset = p.Y - unit.ActualHeight / 2;
                }
            }
        }

        /// <summary>
        /// 检查是否存在指定的元素
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public bool Contain(UIElement el)
        {
            return this.BackCanvas.Children.Contains(el);
        }


        /// <summary>
        /// 移除元素
        /// </summary>
        /// <param name="el"></param>
        public void RemoveElement(UIElement el)
        {
            if (el is Elements.BaseElement)
            {
                elements.Remove((Elements.BaseElement)el);
            }
            this.BackCanvas.Children.Remove(el);
        }

        /// <summary>
        /// 重新定位所有元素
        /// </summary>
        public void ResetPosition()
        {
            foreach (var el in elements)
            {
                el.ResetPosition(this);
            }
        }

        /// <summary>
        /// 检查二个元素是否可以相连
        /// </summary>
        /// <param name="srcElement"></param>
        /// <param name="tarElement"></param>
        /// <returns></returns>
        public bool CheckCanLine(Elements.BaseElement srcElement,Elements.BaseElement tarElement)
        {
            if (tarElement.Model.Part.IsStart) return false;

            return !srcElement.Model.Outs.Contains(tarElement.Model);
        }

        /// <summary>
        /// 获取当前图形所有元件信息
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Model.Part> GetAllParts()
        {
            var parts = (from p in elements
                         select p.Model.Part).ToList<Model.Part>();

            return parts;
        }
    }
}
