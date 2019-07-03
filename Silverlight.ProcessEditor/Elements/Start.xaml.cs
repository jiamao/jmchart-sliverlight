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
    public partial class Start : BaseElement
    {
        public Start()
        {
            InitializeComponent();

            centerRect.MouseLeftButtonDown += new MouseButtonEventHandler(centerRect_MouseLeftButtonDown);
        }

        void centerRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.Model.Part.IsEnd)
            {
                StartConnect();
            }
        }

        /// <summary>
        /// 终结点位置
        /// 0=起始位置
        /// 1=结束位置
        /// </summary>
        public int PointType { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        public Color Fill
        {
            set
            {
                centerRect.Fill = new SolidColorBrush(value);
                value.A = 120;
                lineRect.Fill = new SolidColorBrush(value);
            }
        }

        /// <summary>
        /// 重新定位
        /// </summary>
        /// <param name="grid"></param>
        public override void ResetPosition(Model.BackGrid grid = null)
        {           
          
            if (this.Model.Part.IsStart) this.Position = new Point(0, 0);
            else if (this.Model.Part.Id == "1") this.Position = new Point(Helper.Config.ColumnCount - 1, 0); 

            base.ResetPosition(grid);

            //Canvas.SetZIndex(this, Helper.ShapZIndex.Shap.GetHashCode());

            //var x = this.Position.X * Helper.Config.GridSize.Width + Helper.Config.ElementMargin.Left + grid.GridRect.Left;
            //var y = this.Position.Y * Helper.Config.GridSize.Height + Helper.Config.ElementMargin.Top + grid.GridRect.Top;

            //Canvas.SetLeft(this, x);
            //Canvas.SetTop(this, y);

            //CreateInOutPosition();

            this.Height = CanvasGrid.GridRect.Height - CanvasGrid.GridRect.Top * 2;

            var ml = (Helper.Config.GridSize.Width - centerRect.ActualWidth) / 2;
            var mh = (Helper.Config.GridSize.Height - centerRect.ActualHeight) / 2;
            this.GridMargin = new Thickness(ml, mh, ml, mh);

            var x= Canvas.GetLeft(this);
            var y= Canvas.GetTop(this);

            this.InPoint = new Point(x + (this.Width - centerRect.ActualWidth) / 2, y + (this.Height) / 2);

            this.OutPoint = new Point(x + (this.Width + centerRect.ActualWidth) / 2, y + (this.Height) / 2);
        }

    }
}
