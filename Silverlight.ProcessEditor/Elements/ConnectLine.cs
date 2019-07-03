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

namespace Silverlight.ProcessEditor.Elements
{
    public class ConnectLine : Path
    {
        protected PathGeometry lineGeometry = new PathGeometry();
        
        //private CloseElement _closeButton = new CloseElement();//移除操作对象
        
        /// <summary>
        /// 移除委托
        /// </summary>
        public delegate void DelegateHide();

        /// <summary>
        /// 移除事件
        /// </summary>
        public event DelegateHide RemoveEventHandler;

        /// <summary>
        /// 编辑改变事件
        /// </summary>
        public event EventHandler Change;
            

        const int _defaultWidth = 1;
        const int _focusWidth = 3;

        ContextMenu menu = null;

        Model.BackGrid backGrid = null;

        public BaseElement StartElement { get; set; }

        public BaseElement EndElement { get; set; }

        /// <summary>
        /// 连线方位
        /// </summary>
        public enum PointDirection
        {
            /// <summary>
            /// 起始点在结束点左边，且为水平方向
            /// </summary>
            Left = 0,
            /// <summary>
            /// 起始点在结束点右边，且为水平方向
            /// </summary>
            Right = 1,
            /// <summary>
            /// 起始点在结束点垂直上边
            /// </summary>
            Top = 2,
            /// <summary>
            /// 起始点在结束点垂直下边
            /// </summary>
            Bottom = 3,
            /// <summary>
            /// 起始点在结束点左上边
            /// </summary>
            LTRB = 4,
            /// <summary>
            /// 起始点在结束点左下边
            /// </summary>
            LBRT = 5,
            /// <summary>
            /// 起始点在结束点右上边
            /// </summary>
            RTLB = 6,
            /// <summary>
            /// 起始点在结束点的右下边
            /// </summary>
            RBLT = 7
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public ConnectLine(Model.BackGrid grid)
        {
            SubBrush = new SolidColorBrush(Colors.Black);
            Selected = false;
            this.StrokeThickness = _defaultWidth;
            this.Cursor = Cursors.Hand;
            this.MouseEnter += new MouseEventHandler(_linePath_MouseEnter);
            this.MouseLeave += new MouseEventHandler(_linePath_MouseLeave);

            //_lineGeometry.Figures.Add(_linePoints);
            this.Data = lineGeometry;
            backGrid = grid;

            //添加移除菜单

            menu = new ContextMenu();
            var deleteMenu = new MenuItem();
            deleteMenu.Header = "移除";
            menu.Items.Add(deleteMenu);
            deleteMenu.Click += (m, arg) =>
            {
                this.Hide();
            };


            grid.BackCanvas.MouseMove += new MouseEventHandler(BackCanvas_MouseMove);
            grid.BackCanvas.MouseLeftButtonUp += new MouseButtonEventHandler(BackCanvas_MouseLeftButtonUp);
        }

        /// <summary>
        /// 连线完成工作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BackCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var data = Helper.DragDrop.GetData();
                if (data is ConnectLine)
                {
                    var line = data as ConnectLine;
                    var p = e.GetPosition(backGrid.BackCanvas);
                    var el = backGrid.GetElement(p.X, p.Y);
                    line.Connect(el);
                    //if (el != null && el != line.StartElement
                    //    && backGrid.CheckCanLine(line.StartElement, el))
                    //{
                    //    line.EndElement = el;
                    //    line.EndPoint = el.InPoint;
                    //    line.Position();
                    //    line.StartElement.Model.Outs.Add(el.Model);
                    //    el.Model.Ins.Add(line.StartElement.Model);

                    //    if (line.StartElement is Unit)
                    //    {
                    //        ((Unit)line.StartElement).RemoveUnitHandle += () =>
                    //        {
                    //            this.Hide();
                    //        };
                    //    }
                    //    if (el is Unit)
                    //    {
                    //        ((Unit)el).RemoveUnitHandle += () =>
                    //        {
                    //            this.Hide();
                    //        };
                    //    }

                    //    ContextMenuService.SetContextMenu(this, menu);//绑定菜单

                    //    if (Change != null) Change(this, null);
                    //    //var editor = Silverlight.Common.Visual.TreeHelper.FindAnchestor<Editor>(this);//查找到面板
                    //    //if (editor != null)
                    //    //{
                    //    //    editor.GetJson();
                    //    //}
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (EndElement == null)
                {
                    backGrid.RemoveElement(this);
                }
                Helper.DragDrop.Clear();
                backGrid.BackCanvas.MouseMove -= new MouseEventHandler(BackCanvas_MouseMove);
                backGrid.BackCanvas.MouseLeftButtonUp -= new MouseButtonEventHandler(BackCanvas_MouseLeftButtonUp);
            }
        }

        void BackCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            this.EndPoint = e.GetPosition(backGrid.BackCanvas);

            this.Draw();
        }

        /// <summary>
        /// 如果鼠标移开则消选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _linePath_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Selected = false;
        }

        /// <summary>
        /// 是否可以显示关闭按钮
        /// </summary>
        public bool CanShowRemoveButton { get; set; }

        /// <summary>
        /// 鼠标移入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _linePath_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Selected = true;//选中当前

            //if (CanShowRemoveButton)
            //{
            ////显示关闭按钮
            //if (!backGrid.Contain(_closeButton))
            //{
            //    _closeButton.Width = 36;
            //    _closeButton.Height = 36;
            //    _closeButton.TargetControl = this;

            //    this._closeButton.BindEvent();//绑定事件
            //    backGrid.AddControl(_closeButton);
            //}
            //_closeButton.Visibility = Visibility.Visible;
            //var p = e.GetPosition(backGrid.BackCanvas);
            //Canvas.SetLeft(_closeButton, p.X - 16);
            //Canvas.SetTop(_closeButton, p.Y - 16);

            //_closeButton.Focus();//焦点交给关闭按钮
            //}
        }

        /// <summary>
        /// 获得焦点的背景
        /// </summary>
        public Brush ForcusBrush = new SolidColorBrush(Colors.Red);
        /// <summary>
        /// 失去焦点时的背景
        /// </summary>
        public Brush SubBrush { get; set; }

        /// <summary>
        /// 获取或设置选中状态
        /// </summary>
        public bool Selected
        {
            get
            {
                return this.Stroke == ForcusBrush;
            }
            set
            {
                if (value == true)
                {
                    this.Stroke = ForcusBrush;
                    this.StrokeThickness = _focusWidth;
                }
                else
                {
                    this.Stroke = SubBrush;
                    this.StrokeThickness = _defaultWidth;
                }
            }
        }        

        /// <summary>
        /// 前景色
        /// </summary>
        public Brush ForeBrush
        {
            get { return this.Stroke; }
            set { this.Stroke = value; }
        }

        /// <summary>
        /// 线条起始坐标
        /// </summary>
        public Point StartPoint
        {
            get {
                if (StartElement.Model.Part.IsStart && EndElement != null)
                {
                    return new Point(this.StartElement.OutPoint.X, EndElement.InPoint.Y);
                }
                else
                {
                    return this.StartElement.OutPoint;
                }
            }            
        }

        Point _endpoint;
        /// <summary>
        /// 线条终止坐标
        /// </summary>
        public Point EndPoint
        {
            get
            {
                return _endpoint;
            }
            set
            {
                _endpoint = value;
            }
        }

        public const int StartOffset = 20;
        public const int EndOffset = 60;

        //箭头偏移量
        const int EndPointYOffset = 3;
        const int EndPointXOffset = 6;

        /// <summary>
        /// 获取连线二点的方位
        /// 从起始点到结束点
        /// </summary>
        /// <returns></returns>
        public PointDirection GetDirection()
        {
            if (StartPoint.X == EndPoint.X)
            {
                return this.StartPoint.Y > this.EndPoint.Y ? PointDirection.Bottom : PointDirection.Top;//垂直方向
            }
            if (StartPoint.Y == EndPoint.Y)
            {
                return StartPoint.X > EndPoint.X ? PointDirection.Right : PointDirection.Left;//水平方向
            }
            if (StartPoint.X > EndPoint.X)
            {
                //起始点X,Y都大于结束点则为左上右下
                if (StartPoint.Y > EndPoint.Y)
                {
                    return PointDirection.RBLT;
                }
                else
                {
                    return PointDirection.RTLB;
                }
            }
            else
            {
                //起始点X小于结束点,Y大于结束点则为左下右上
                if (StartPoint.Y > EndPoint.Y)
                {
                    return PointDirection.LBRT;
                }
                else
                {
                    return PointDirection.LTRB;
                }
            }
        }

        /// <summary>
        /// 连接终结点
        /// </summary>
        /// <param name="el"></param>
        public void Connect(Elements.BaseElement el)
        {
            if (el != null && el != StartElement
                            && backGrid.CheckCanLine(StartElement, el))
            {
                EndElement = el;
                EndPoint = el.InPoint;
                
                StartElement.Model.Outs.Add(el.Model);
                el.Model.Ins.Add(StartElement.Model);
                Position();

                if (StartElement is Unit)
                {
                    ((Unit)StartElement).RemoveUnitHandle += () =>
                    {
                        this.Hide();
                    };
                }
                if (el is Unit)
                {
                    ((Unit)el).RemoveUnitHandle += () =>
                    {
                        this.Hide();
                    };
                }

                if (!StartElement.Model.Part.IsEnd)
                {
                    ContextMenuService.SetContextMenu(this, menu);//绑定菜单
                }
                if (Change != null) Change(this, null);

                backGrid.BackCanvas.MouseMove -= new MouseEventHandler(BackCanvas_MouseMove);
                backGrid.BackCanvas.MouseLeftButtonUp -= new MouseButtonEventHandler(BackCanvas_MouseLeftButtonUp);
            }
        }


        /// <summary>
        /// 初始化参数
        /// </summary>
        public void Draw()
        {
            if (EndPoint == null) return;

            //曲线
            var lineBezier = new BezierSegment();

            var pointOffset = EndPoint.X - StartPoint.X;
            if (pointOffset > 140)
            {
                lineBezier.Point1 = new Point(StartPoint.X + 140, StartPoint.Y + StartOffset);
                lineBezier.Point2 = new Point(EndPoint.X - 70, EndPoint.Y - EndOffset);
            }
            else if (pointOffset < -140)
            {
                lineBezier.Point1 = new Point(StartPoint.X - 140, StartPoint.Y + StartOffset);
                lineBezier.Point2 = new Point(EndPoint.X + 70, EndPoint.Y - EndOffset);
            }
            else if (EndPoint.Y > StartPoint.Y)
            {
                lineBezier.Point1 = new Point(StartPoint.X, StartPoint.Y + StartOffset);
                lineBezier.Point2 = new Point(EndPoint.X, EndPoint.Y - EndOffset);
            }
            else if (pointOffset > 0)
            {
                lineBezier.Point1 = new Point(StartPoint.X + 140, StartPoint.Y + StartOffset);
                lineBezier.Point2 = new Point(EndPoint.X - 70, EndPoint.Y - EndOffset);
            }
            else
            {
                lineBezier.Point1 = new Point(StartPoint.X - 140, StartPoint.Y + StartOffset);
                lineBezier.Point2 = new Point(EndPoint.X + 70, EndPoint.Y - EndOffset);
            }


            lineBezier.Point3 = EndPoint;

            var linePoints = new PathFigure();
            linePoints.StartPoint = StartPoint;

            linePoints.Segments.Add(lineBezier);
            lineGeometry.Figures.Clear();
            lineGeometry.Figures.Add(linePoints);

            var ps = CreateEndPoints(EndPoint, 0);//获取向下的箭头点
            foreach (var p in ps)
            {
                linePoints.Segments.Add(new LineSegment() { Point = p });
            }

            Canvas.SetZIndex(this, Helper.ShapZIndex.Move.GetHashCode());
        }

        /// <summary>
        /// 定位
        /// </summary>
        public void Position()
        {
            var linePoints = new PathFigure();
            linePoints.StartPoint = StartPoint;

            ////获取出线第一个转点
            //var secpoint = new Point();
            //secpoint.X = StartPoint.X;
            //var sp = StartPoint.Y % Helper.Config.GridSize.Height;
            ////网格底部           
            //secpoint.Y = ((int)StartPoint.Y / Helper.Config.GridSize.Height + 1) * Helper.Config.GridSize.Height;


            ////生成第三个转折点
            //var thpoint = new Point();
            //thpoint.Y = secpoint.Y;            

            //    //如果起始点在结束点右边
            //    if (StartPoint.X > EndPoint.X)
            //    {
            //        thpoint.X = ((int)EndPoint.X / Helper.Config.GridSize.Width + 1) * Helper.Config.GridSize.Width;
            //    }
            //    else if (StartPoint.X == EndPoint.Y && StartPoint.Y < EndPoint.Y)
            //    {
            //        thpoint.X = secpoint.X;
            //    }
            //    else
            //    {
            //        thpoint.X = ((int)EndPoint.X / Helper.Config.GridSize.Width) * Helper.Config.GridSize.Width;
            //    }

            ////第四个转折点
            //    var frpoint = new Point(thpoint.X, EndPoint.Y);
            //    if (StartPoint.Y < EndPoint.Y)
            //    {
            //        frpoint.Y = EndPoint.Y - common.UIHelper.StatusGridTop;
            //    }
            //    else
            //    {
            //        if (EndPoint.X % Helper.Config.GridSize.Width > Helper.Config.GridSize.Width / 2)
            //        {
            //            frpoint.X = ((int)EndPoint.X / Helper.Config.GridSize.Width + 1) * Helper.Config.GridSize.Width;
            //        }
            //        else
            //        {
            //            frpoint.X = ((int)EndPoint.X / Helper.Config.GridSize.Width) * Helper.Config.GridSize.Width;
            //        }
            //    }

            ////第五个转折点
            //var fipoint = new Point(thpoint.X,EndPoint.Y);
            //if (StartPoint.Y < EndPoint.Y)
            //{
            //    fipoint.X = EndPoint.X;
            //    fipoint.Y = EndPoint.Y - common.UIHelper.StatusGridTop;
            //}
            //else
            //{
            //    if (EndPoint.X % Helper.Config.GridSize.Width > Helper.Config.GridSize.Width / 2)
            //    {
            //        fipoint.X = ((int)EndPoint.X / Helper.Config.GridSize.Width + 1) * Helper.Config.GridSize.Width;
            //    }
            //    else
            //    {
            //        fipoint.X = ((int)EndPoint.X / Helper.Config.GridSize.Width) * Helper.Config.GridSize.Width;
            //    }
            //}

            //linePoints.Segments.Add(new LineSegment() { Point = StartPoint });
            //linePoints.Segments.Add(new LineSegment() { Point = secpoint });
            //linePoints.Segments.Add(new LineSegment() { Point = thpoint });
            //linePoints.Segments.Add(new LineSegment() { Point = frpoint });
            //linePoints.Segments.Add(new LineSegment() { Point = fipoint });
            //linePoints.Segments.Add(new LineSegment() { Point = EndPoint });


            if (EndElement.Model.Part.Id == "1")
            {
                EndPoint = new Point(EndElement.InPoint.X, StartElement.OutPoint.Y);
            }
            else
            {
                EndPoint = EndElement.InPoint;
            }
            //获取当前二点在网格中的方位
            var direction = GetDirection();
            var points = CreatePoints(direction);//生成连线点集合
            foreach (var p in points)
            {
                var s = new LineSegment() { Point = p };

                linePoints.Segments.Add(s);
            }

            //linePoints.Segments.Add(new LineSegment() { Point = EndPoint });

            //DrawEnd(EndPoint, linePoints);//画最后的箭头

            this.SetValue(Canvas.ZIndexProperty, Helper.ShapZIndex.Line.GetHashCode());

            lineGeometry.Figures.Clear();
            lineGeometry.Figures.Add(linePoints);

            Show();

            StartElement.MoveHandle -= new BaseElement.MoveElementDelegate(Element_MoveHandle);

            EndElement.MoveHandle -= new BaseElement.MoveElementDelegate(Element_MoveHandle);

            StartElement.MoveHandle += new BaseElement.MoveElementDelegate(Element_MoveHandle);

            EndElement.MoveHandle += new BaseElement.MoveElementDelegate(Element_MoveHandle);
            //this.backGrid.BackCanvas.MouseMove -= new MouseEventHandler(BackCanvas_MouseMove);
        }

        void Element_MoveHandle()
        {
            this.Position();
        }

        /// <summary>
        /// 椐据方位生成连线点
        /// </summary>
        /// <param name="der">当前方位</param>
        /// <returns></returns>
        private System.Collections.Generic.List<Point> CreatePoints(PointDirection der)
        {
            //连线的转折点
            var result = new System.Collections.Generic.List<Point>();

            ////获取出线第一个转点
            //var fp = new Point();
            ////固定从底部出线。因此第一个点是固定的
            //fp.X = StartPoint.X;
            //fp.Y = ((int)StartPoint.Y / Helper.Config.GridSize.Height + 1) * Helper.Config.GridSize.Height;
            result.Add(StartPoint);

            switch (der)
            {
                //起始点在水平左边
                case PointDirection.Left:
                //起始点位于左下方
                case PointDirection.LBRT:
                case PointDirection.LTRB:
                    {
                        var sp = new Point();
                        sp.Y = StartPoint.Y;
                        //X取中点
                        sp.X = (EndPoint.X - StartPoint.X) / 2 + StartPoint.X;//((int)EndPoint.X / Helper.Config.GridSize.Width) * Helper.Config.GridSize.Width;
                        result.Add(sp);
                        //第三个转点
                        var tp = new Point();
                        tp.Y = EndPoint.Y;
                        tp.X = sp.X;
                        result.Add(tp);

                        result.Add(EndPoint);

                        result.AddRange(CreateEndPoints(EndPoint, 1));//生成向右的箭头
                        break;
                    }
                //超始点在结束点水平右边
                case PointDirection.Right:
                case PointDirection.Top:
                case PointDirection.Bottom:
                //起始点位于右下方
                case PointDirection.RBLT:
                case PointDirection.RTLB:
                    {
                        var sp = new Point();
                        sp.Y = StartPoint.Y;
                        //X取中间点
                        sp.X = StartPoint.X + StartElement.GridMargin.Right;//((int)EndPoint.X / Helper.Config.GridSize.Width + 1) * Helper.Config.GridSize.Width;
                        result.Add(sp);
                        //第三个转点
                        var tp = new Point();
                        var offsety = (StartPoint.Y - EndPoint.Y) / 2;
                        if (Math.Abs(offsety) < Helper.Config.GridSize.Height / 2)
                        {
                            offsety = Helper.Config.GridSize.Height / 2;
                        }
                        tp.Y = EndPoint.Y + offsety;

                        tp.X = sp.X;
                        result.Add(tp);

                        var fp = new Point();
                        fp.X = EndPoint.X - EndElement.GridMargin.Left;
                        fp.Y = tp.Y;
                        result.Add(fp);

                        var fp2 = new Point();
                        fp2.X = fp.X;
                        fp2.Y = EndPoint.Y;
                        result.Add(fp2);

                        result.Add(EndPoint);
                        result.AddRange(CreateEndPoints(EndPoint, 1));//生成向左 的箭头
                        break;
                    }
                ////起始点在垂直上边
                //case PointDirection.Top:
                //    {
                //        var sp = new Point();
                //        sp.Y = StartPoint.Y;
                //        //var yc1 = (int)StartPoint.Y / Helper.Config.GridSize.Height;
                //        //var yc2 = (int)EndPoint.Y / Helper.Config.GridSize.Height;
                //        ////如果起始点高于结束点一个网格以上
                //        //if (yc1 < yc2 - 1)
                //        //{
                //        //    var l = StartPoint.X % Helper.Config.GridSize.Width;
                //        //    //如果起始点偏右，则从右走线
                //        //    //否则从左走线
                //        //    if (l > Helper.Config.GridSize.Width / 2)
                //        //    {
                //        //        sp.X = ((int)StartPoint.X / Helper.Config.GridSize.Width + 1) * Helper.Config.GridSize.Width;
                //        //    }
                //        //    else
                //        //    {
                //        //        sp.X = ((int)StartPoint.X / Helper.Config.GridSize.Width) * Helper.Config.GridSize.Width;
                //        //    }
                //        //    result.Add(sp);
                //        //    //第三个转点
                //        //    var tp = new Point();
                //        //    tp.X = sp.X;
                //        //    tp.Y = (int)EndPoint.Y / Helper.Config.GridSize.Height * Helper.Config.GridSize.Height;
                //        //    result.Add(tp);

                //        //    var frp = new Point() { X = EndPoint.X, Y = tp.Y };
                //        //    result.Add(frp);
                //        //}
                //        ////起始点就在结束点上一个网格
                //        //else
                //        //{
                //        //    sp.X = EndPoint.X;
                //        //    result.Add(sp);
                //        //}

                //        sp.X = StartPoint.X + StartElement.GridMargin.Right;
                //        result.Add(sp);

                //        var tp = new Point();
                //        tp.X = sp.X;
                //        tp.Y = StartPoint.Y + (EndPoint.Y - StartPoint.Y) / 2;
                //        result.Add(tp);

                //        var fp = new Point();
                //        fp.Y = tp.Y;
                //        fp.X = EndPoint.X - EndElement.GridMargin.Left;
                //        result.Add(fp);

                //        var fp2 = new Point();
                //        fp2.X = fp.X;
                //        fp2.Y = EndPoint.Y;
                //        result.Add(fp2);

                //        result.Add(EndPoint);
                //        result.AddRange(CreateEndPoints(EndPoint, 1));//生成向下的箭头
                //        break;
                //    }
                ////起始点在结束点下边
                //case PointDirection.Bottom:
                //    {
                //        var sp = new Point();
                //        sp.Y = StartPoint.Y;
                //        var l = StartPoint.X % Helper.Config.GridSize.Width;
                //        var flag = 2;//默认为向右的箭头
                //        //如果起始点偏右，则从右走线
                //        //否则从左走线
                //        if (l > Helper.Config.GridSize.Width / 2)
                //        {
                //            sp.X = ((int)StartPoint.X / Helper.Config.GridSize.Width + 1) * Helper.Config.GridSize.Width;
                //        }
                //        else
                //        {
                //            flag = 1;//表示箭头向左
                //            sp.X = ((int)StartPoint.X / Helper.Config.GridSize.Width) * Helper.Config.GridSize.Width;
                //        }
                //        result.Add(sp);
                //        //第三个转点
                //        var tp = new Point();
                //        tp.X = sp.X;
                //        tp.Y = EndPoint.Y;
                //        result.Add(tp);

                //        result.Add(EndPoint);
                //        result.AddRange(CreateEndPoints(EndPoint, flag));//生成向flag的箭头
                //        break;
                //    }
                ////起始点位于左上方
                //case PointDirection.LTRB:
                //    {
                //        var sp = new Point();
                //        sp.Y = StartPoint.Y;
                //        //从右边走线
                //        sp.X = ((int)EndPoint.X / Helper.Config.GridSize.Width) * Helper.Config.GridSize.Width;
                //        result.Add(sp);

                //        //第三个转点
                //        var tp = new Point();
                //        tp.X = sp.X;
                //        tp.Y = ((int)EndPoint.Y / Helper.Config.GridSize.Height) * Helper.Config.GridSize.Height;
                //        result.Add(tp);

                //        //第四个转点
                //        var frp = new Point();
                //        frp.X = EndPoint.X;
                //        frp.Y = tp.Y;
                //        result.Add(frp);

                //        result.Add(EndPoint);
                //        result.AddRange(CreateEndPoints(EndPoint, 0));//生成向下的箭头
                //        break;
                //    }
                //起始点位于左下方
                //case PointDirection.LBRT:
                //    {
                //        var sp = new Point();
                //        sp.Y = fp.Y;
                //        //从右边走线
                //        sp.X = ((int)EndPoint.X / Helper.Config.GridSize.Width) * Helper.Config.GridSize.Width;   
                //        result.Add(sp);                       

                //        //第四个转点
                //        var frp = new Point();
                //        frp.X = sp.X;
                //        frp.Y = EndPoint.Y;
                //        result.Add(frp);
                //        break;
                //    }                 
                ////起始点位于右上方
                //case PointDirection.RTLB:
                //    {
                //        var sp = new Point();
                //        sp.Y = StartPoint.Y;
                //        //从左边走线
                //        sp.X = ((int)EndPoint.X / Helper.Config.GridSize.Width + 1) * Helper.Config.GridSize.Width;
                //        result.Add(sp);

                //        //第三个转点
                //        var tp = new Point();
                //        tp.X = sp.X;
                //        tp.Y = ((int)EndPoint.Y / Helper.Config.GridSize.Height) * Helper.Config.GridSize.Height;
                //        result.Add(tp);

                //        //第四个转点
                //        var frp = new Point();
                //        frp.X = EndPoint.X;
                //        frp.Y = tp.Y;
                //        result.Add(frp);

                //        result.Add(EndPoint);
                //        result.AddRange(CreateEndPoints(EndPoint, 0));//生成向下的箭头
                //        break;
                //    }
            }

            return result;
        }

        /// <summary>
        /// 画箭头
        /// 根据方位生成箭头
        /// 参数er:0=向下，1表示向右，2表示向左
        /// </summary>
        private Point[] CreateEndPoints(Point ep, int er = 0)
        {
            var points = new Point[5];
            switch (er)
            {
                //向下的箭头
                case 0:
                    {
                        points[0] = new Point() { Y = ep.Y - EndPointYOffset, X = ep.X + EndPointXOffset };
                        points[1] = new Point() { Y = ep.Y - EndPointYOffset, X = ep.X + EndPointXOffset };
                        points[2] = ep;
                        points[3] = new Point() { Y = ep.Y - EndPointYOffset, X = ep.X - EndPointXOffset };
                        points[4] = new Point() { Y = ep.Y - EndPointYOffset, X = ep.X - EndPointXOffset };
                        break;
                    }
                //向右的箭头
                case 1:
                    {
                        points[0] = new Point() { Y = ep.Y - EndPointYOffset, X = ep.X - EndPointXOffset };
                        points[1] = new Point() { Y = ep.Y - EndPointYOffset, X = ep.X - EndPointXOffset };
                        points[2] = ep;
                        points[3] = new Point() { Y = ep.Y + EndPointYOffset, X = ep.X - EndPointXOffset };
                        points[4] = new Point() { Y = ep.Y + EndPointYOffset, X = ep.X - EndPointXOffset };
                        break;
                    }
                //向左的箭头
                case 2:
                    {
                        points[0] = new Point() { Y = ep.Y - EndPointYOffset, X = ep.X + EndPointXOffset };
                        points[1] = new Point() { Y = ep.Y - EndPointYOffset, X = ep.X + EndPointXOffset };
                        points[2] = ep;
                        points[3] = new Point() { Y = ep.Y + EndPointYOffset, X = ep.X + EndPointXOffset };
                        points[4] = new Point() { Y = ep.Y + EndPointYOffset, X = ep.X + EndPointXOffset };
                        break;
                    }
            }
            return points;
            //pf.Segments.Add(new LineSegment() { Point = new Point() { Y = ep.Y - EndPointYOffset, X = ep.X + EndPointXOffset } });
            //pf.Segments.Add(new LineSegment() { Point = new Point() { Y = ep.Y - EndPointYOffset, X = ep.X + EndPointXOffset } });
            //pf.Segments.Add(new LineSegment() { Point = ep });
            //pf.Segments.Add(new LineSegment() { Point = new Point() { Y = ep.Y - EndPointYOffset, X = ep.X - EndPointXOffset } });
            //pf.Segments.Add(new LineSegment() { Point = new Point() { Y = ep.Y - EndPointYOffset, X = ep.X - EndPointXOffset } });
        }

        /// <summary>
        /// 显示线条
        /// </summary>
        public void Show()
        {
            if (!backGrid.Contain(this))
            {
                backGrid.AddControl(this);
            }

            this.Visibility = Visibility.Visible;
            this.ForeBrush = this.SubBrush;
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hide()
        {
            if (backGrid == null) return;
           
            this.Visibility = Visibility.Collapsed;
            //从起始点中移除
            this.StartElement.OutLines.Remove(this);
            this.StartElement.Model.Outs.Remove(this.EndElement.Model);
            StartElement.MoveHandle -= new BaseElement.MoveElementDelegate(Element_MoveHandle);

            if (this.EndElement != null)
            {
                this.EndElement.Model.Ins.Remove(this.StartElement.Model);
                EndElement.MoveHandle -= new BaseElement.MoveElementDelegate(Element_MoveHandle);
                //var editor = Silverlight.Common.Visual.TreeHelper.FindAnchestor<Editor>(this);//查找到面板
                //if (editor != null)
                //{
                //    editor.GetJson();
                //}
                if (Change != null) Change(this, null);
            }
            backGrid.RemoveElement(this);

            if (RemoveEventHandler != null) RemoveEventHandler();//触发移除事件
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void Remove()
        {
            this.Hide();//移除
        }
    
    }
}
