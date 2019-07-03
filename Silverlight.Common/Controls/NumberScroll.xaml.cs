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

namespace Silverlight.Common.Controls
{
    public partial class NumberScroll : UserControl
    {
        List<NumberScrollItem> items = new List<NumberScrollItem>();
        public NumberScroll()
        {
            InitializeComponent();           
        }

        /// <summary>
        /// 设置滚动数字
        /// </summary>
        /// <param name="value"></param>
        public void SetNumber(double value)
        {
            var strvalue = value.ToString();
            var len = strvalue.Length - items.Count;
            if (len > 0)
            {
                AddNumber(len);
            }

            for (var i = 0; i < strvalue.Length; i++)
            {
                items[i].NumberChar = strvalue[i];
            }
        }

        /// <summary>
        /// 添加数值栏位
        /// </summary>
        /// <param name="count"></param>
        private void AddNumber(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var item = new NumberScrollItem() { Margin = new Thickness(2, 0, 2, 0), MinWidth = 16, VerticalAlignment = System.Windows.VerticalAlignment.Center };
                items.Add(item);
                if (items.Count > 0)
                {
                    var line = new Rectangle();
                    line.Fill = new SolidColorBrush(Silverlight.Common.Media.Convert.ToColor("#838383"));
                    line.Width = 1;
                    line.Margin = new Thickness(4);

                    numberPanel.Children.Insert(0, line);
                }
                numberPanel.Children.Insert(0, item);
            }
        }
    }
}
