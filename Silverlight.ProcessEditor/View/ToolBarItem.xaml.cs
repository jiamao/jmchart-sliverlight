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
    public partial class ToolBarItem : UserControl
    {
        public ToolBarItem()
        {
            InitializeComponent();

            this.MouseLeftButtonDown += new MouseButtonEventHandler(ToolBarItem_MouseLeftButtonDown);
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
            get {
                return imageBinding;
            }
        }

        /// <summary>
        /// 开始拖放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolBarItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var part=this.DataContext as Model.Part;
            var item= new Model.Circuit(){ Part=part.Clone()};
            item.Part.Id = Guid.NewGuid().ToString("n");
            item.Part.TypeId = part.Id;
           
            var unit = new Elements.Unit() { Model = item };
            unit.Height = Helper.Config.GridSize.Height - Helper.Config.ElementMargin.Top - Helper.Config.ElementMargin.Bottom;
            unit.Width = Helper.Config.GridSize.Width - Helper.Config.ElementMargin.Left - Helper.Config.ElementMargin.Right;
            unit.IcoSource = this.ico.Source;

            var p = e.GetPosition(null);
            //var editor = Silverlight.Common.Visual.TreeHelper.FindAnchestor<Editor>(this);

            unit.StartDrag(p);
        }
    }
}
