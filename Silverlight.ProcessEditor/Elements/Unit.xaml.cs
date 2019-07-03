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
    public partial class Unit : BaseElement
    {
        public Unit()
        {
            InitializeComponent();

            //绑定移动事件
            thumb.DragDelta += new System.Windows.Controls.Primitives.DragDeltaEventHandler(thumb_DragDelta);

            thumb.DragStarted += new System.Windows.Controls.Primitives.DragStartedEventHandler(thumb_DragStarted);

            thumb.DragCompleted += new System.Windows.Controls.Primitives.DragCompletedEventHandler(thumb_DragCompleted);

            thumb.MouseLeftButtonDown += new MouseButtonEventHandler(thumb_MouseLeftButtonDown);

            this.MouseEnter += new MouseEventHandler(Unit_MouseEnter);
            this.MouseLeave += new MouseEventHandler(Unit_MouseLeave);
        }

        void Unit_MouseLeave(object sender, MouseEventArgs e)
        {
            Blur();
        }

        void Unit_MouseEnter(object sender, MouseEventArgs e)
        {
            Hover();
        }

        /// <summary>
        /// 父容器
        /// </summary>
        Editor backEditor = null;

        ContextMenu menu = new ContextMenu();

        public ImageSource IcoSource
        {
            get {
                return ico.Source;
            }
            set {
                ico.Source = value;
            }
        }

        System.Windows.Data.Binding imageBinding = null;
        /// <summary>
        /// 关联的图片binding
        /// </summary>
        public System.Windows.Data.Binding ImageBinding
        {
            set
            {
                imageBinding = value;
                this.ico.SetBinding(Image.SourceProperty, value);
            }
            get
            {
                return imageBinding;
            }
        }

        /// <summary>
        /// 当前元件移除事件
        /// </summary>
        public delegate void RemoveDelegate();
        public event RemoveDelegate RemoveUnitHandle;

        void BindMenu()
        {
            var item = new MenuItem();
            item.Header = "移除";
            menu.Items.Add(item);

            item.Click += (m, a) =>
            {
                this.CanvasGrid.RemoveElement(this);
                if (RemoveUnitHandle != null)
                {
                    RemoveUnitHandle();
                }
            };

            ContextMenuService.SetContextMenu(this, menu);
        }

        //Point firstClickPosition = new Point();
        void thumb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            //firstClickPosition = e.GetPosition(CanvasGrid.BackCanvas);
        }

        //移除容器
        //System.Windows.Controls.Primitives.Popup dragPop = new System.Windows.Controls.Primitives.Popup();


        void thumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            //dragPop.Child = this;

            //this.CanvasGrid.AddControl(dragPop);
           
            this.Opacity = 0.5;           
        }

        /// <summary>
        /// 拖放完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void thumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (CanvasGrid == null) return;

            var p = new Point();
            p.X = Canvas.GetLeft(this);
            if (p.X == double.NaN) p.X = 0;
            p.Y = Canvas.GetTop(this);
            if (p.Y == double.NaN) p.Y = 0;

            CanvasGrid.CheckCanvasGrid();

            //var p = new Point(firstClickPosition.X + e.HorizontalChange, firstClickPosition.Y + e.VerticalChange);
            var newp = CanvasGrid.CheckPosition(p);

            //如果当前位置已有元素
            //则放弃落地
            var el = CanvasGrid.GetElement(newp);
            if (el == null && newp.X != 0) this.Position = newp;

            this.Opacity = 1;

            //重新定位。
            this.ResetPosition();
        }

        /// <summary>
        /// 鼠标移动方块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (CanvasGrid == null) return;

            //活动对象
            this.SetValue(Canvas.ZIndexProperty, Helper.ShapZIndex.Move.GetHashCode());

            var left = Canvas.GetLeft(this);
            if (left == double.NaN) left = 0;
            var top = Canvas.GetTop(this);
            if (top == double.NaN) top = 0;
            left += e.HorizontalChange;
            top += e.VerticalChange;

            //保证不从左边和上边出界
            if (left < 0) left = 0;
            if (top < 0) top = 0;

            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);

            CreateInOutPosition();

            MoveCallback();
        }

        public override void ResetPosition(Model.BackGrid grid = null)
        {
            base.ResetPosition(grid);

            var ml = (Helper.Config.GridSize.Width - this.ActualWidth) / 2;
            var mh = (Helper.Config.GridSize.Height - this.ActualHeight) / 2;

            this.GridMargin = new Thickness(ml,mh,ml,mh);
        }

        /// <summary>
        /// 开始拖放操作
        /// </summary>
        public void StartDrag(Point p)
        {
            //backEditor = editor;

            var pop = new System.Windows.Controls.Primitives.Popup();           
            pop.Child = this;
            Helper.DragDrop.DoDragDrop(pop);
            pop.HorizontalOffset = p.X;// -this.Width / 2;
            pop.VerticalOffset = p.Y;// -this.Height / 2;

            //editor.MouseMove += new MouseEventHandler(editor_MouseMove);
           
            Helper.DragDrop.DoDragDrop(pop);
            pop.IsOpen = true;

        }

        private void StopDrag()
        {
            //backEditor.MouseMove -= new MouseEventHandler(editor_MouseMove);
            //backEditor.MouseRightButtonUp -= new MouseButtonEventHandler(editor_MouseRightButtonUp);
            //backEditor.MouseLeave -= new MouseEventHandler(editor_MouseLeave);
        }

        void editor_MouseLeave(object sender, MouseEventArgs e)
        {
            //var data = Helper.DragDrop.GetData();
            //if (data is System.Windows.Controls.Primitives.Popup)
            //{
            //    var pop = data as System.Windows.Controls.Primitives.Popup;
            //    pop.IsOpen = false;

            //    Helper.DragDrop.Clear();
            //    StopDrag();
            //}
        }

        //void editor_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    var data = Helper.DragDrop.GetData();
        //    if (data is System.Windows.Controls.Primitives.Popup)
        //    {
        //        var pop = data as System.Windows.Controls.Primitives.Popup;
        //        pop.IsOpen = false;
        //        StopDrag();
        //    }
        //}

        //void editor_MouseMove(object sender, MouseEventArgs e)
        //{
        //    var data = Helper.DragDrop.GetData();
        //    if (data is System.Windows.Controls.Primitives.Popup)
        //    {
        //        var pop = data as System.Windows.Controls.Primitives.Popup;
        //        var p = e.GetPosition(null);

        //        pop.HorizontalOffset = p.X - this.ActualWidth / 2;
        //        pop.VerticalOffset = p.Y - this.ActualHeight / 2;
        //    }
        //}

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CanvasGrid == null) return;

            this.StartConnect();

            e.Handled = true;
        }

        /// <summary>
        /// 释放拖放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var data = Helper.DragDrop.GetData();
            if (data is System.Windows.Controls.Primitives.Popup)
            {
                var pop = data as System.Windows.Controls.Primitives.Popup;

                var unit = pop.Child as Elements.Unit;
                if (unit != null)
                {                    
                    pop.Child = null;
                    unit.Position = CanvasGrid.CheckPosition(e.GetPosition(CanvasGrid.BackCanvas));
                    var el = CanvasGrid.GetElement(unit.Position);
                    //必须是当前位置无元素方可放置
                    if (!(el is Elements.Unit))
                    {
                        CanvasGrid.AddElement(unit);
                        BindMenu();//绑定菜单
                    }                   
                }

                pop.IsOpen = false;
                Helper.DragDrop.Clear();
                StopDrag();
            }
        }

        //private void Grid_MouseMove(object sender, MouseEventArgs e)
        //{
        //    var data = Helper.DragDrop.GetData();
        //    if (data is System.Windows.Controls.Primitives.Popup)
        //    {
        //        var pop = data as System.Windows.Controls.Primitives.Popup;
        //        var p = e.GetPosition(null);

        //        pop.HorizontalOffset = p.X - this.ActualWidth / 2;
        //        pop.VerticalOffset = p.Y - this.ActualHeight / 2;
        //    }
        //}

        //private void Grid_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    var data = Helper.DragDrop.GetData();
        //    if (data is System.Windows.Controls.Primitives.Popup)
        //    {
        //        var pop = data as System.Windows.Controls.Primitives.Popup;
        //        pop.IsOpen = false;

        //        Helper.DragDrop.Clear();
        //    }
        //}

        public void Hover()
        {
            thumb.Visibility = System.Windows.Visibility.Visible;
            grid.Background = new SolidColorBrush(Silverlight.Common.Media.Convert.ToColor("#E2E6E9"));
            statusBorder.BorderBrush = new SolidColorBrush(Colors.Black);
        }

        public void Blur()
        {
            thumb.Visibility = System.Windows.Visibility.Collapsed;
            grid.Background = new SolidColorBrush(Colors.Transparent);
            statusBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }
    }
}
