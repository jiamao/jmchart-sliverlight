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

namespace JMChart.Common
{
    public partial class PropertyBindingControl : UserControl
    {
        public PropertyBindingControl()
        {
            InitializeComponent();
        }

        public readonly static DependencyProperty PropertyValueProperty =
            DependencyProperty.Register(
           "PropertyValue",
           typeof(object),
           typeof(PropertyBindingControl), new PropertyMetadata(new PropertyChangedCallback(PropertyValuePropertyChangedCallback))
           );

        static void PropertyValuePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ct = sender as PropertyBindingControl;
            ct.PropertyValue = e.NewValue;
        }

        /// <summary>
        /// 属性值
        /// </summary>
        public object PropertyValue { get; set; }
    }
}
