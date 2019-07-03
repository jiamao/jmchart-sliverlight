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
    public partial class ToolBar : UserControl
    {
        public ToolBar()
        {
            InitializeComponent();
        }

        public Model.BackGrid CanvasGrid { get; set; }

        public void LoadItems(IEnumerable<Model.Part> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    var tool = new ToolBarItem();
                    tool.DataContext = item;
                    var imgbinding = new System.Windows.Data.Binding();
                    imgbinding.Converter = new Converter.UnitImageConverter();
                    imgbinding.ConverterParameter = item.Id;
                    tool.ImageBinding = imgbinding;
                    itemContainer.Items.Add(tool);
                }
            }
        }
    }
}
