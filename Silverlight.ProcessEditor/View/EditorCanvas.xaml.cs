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

namespace Silverlight.ProcessEditor.View
{
    public partial class EditorCanvas : UserControl
    {
        Model.BackGrid grid = null;
        /// <summary>
        /// 画布
        /// </summary>
        public Model.BackGrid GridCanvas
        {
            get { return grid; }
        }



        public EditorCanvas()
        {
            InitializeComponent();

            //实例化表格
            grid = new Model.BackGrid(this.canvasArea);           

            //this.canvasArea.Width = 200;
            //this.canvasArea.Height = 120;

            this.Loaded += new RoutedEventHandler(EditorCanvas_Loaded);
            //this.canvasArea.MouseEnter += new MouseEventHandler(canvasArea_MouseEnter);

            //this.canvasArea.MouseLeftButtonUp += new MouseButtonEventHandler(canvasArea_MouseLeftButtonUp);
        }

        //void canvasArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    var data = Helper.DragDrop.GetData();
        //    if (data is System.Windows.Controls.Primitives.Popup)
        //    {
        //        var pop = data as System.Windows.Controls.Primitives.Popup;

        //        var unit = pop.Child as Elements.Unit;
        //        if (unit != null)
        //        {
        //            pop.Child = null;
        //            unit.Position = grid.CheckPosition(e.GetPosition(canvasArea));
        //            var el = grid.GetElement(unit.Position);
        //            //必须是当前位置无元素方可放置
        //            if (!(el is Elements.Unit))
        //            {
        //                grid.AddElement(unit);
        //            }
        //        }

        //        pop.IsOpen = false;
        //        Helper.DragDrop.Clear();
        //    }            
        //}

        //void canvasArea_MouseEnter(object sender, MouseEventArgs e)
        //{
            
        //}

        void EditorCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            grid.DrawGrid();

            //grid.AddElement(StartPoint);
            //grid.AddElement(EndPoint);
        }

 
    }
}
