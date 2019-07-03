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

namespace slExample
{
    public partial class scrollnumberpage : UserControl
    {
        public scrollnumberpage()
        {
            InitializeComponent();

            this.Dispatcher.BeginInvoke(() => {
                ScrollNumber();
            });
            

            this.Loaded += scrollnumberpage_Loaded;

            lan.SelectionChanged += lan_SelectionChanged;
        }

        void lan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lan.SelectedIndex == 0)
            {
                res.ui.Culture=System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            }
            else
            {

                res.ui.Culture=System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko");
            }
            txttest.Text = res.ui.ResourceManager.GetString("test", res.ui.Culture);
            //txttest.Text = res.ui.test;
        }

        void scrollnumberpage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void ScrollNumber()
        {
            //延时后载入数据
            var story = new Storyboard();
            story.Duration = new Duration(TimeSpan.FromMilliseconds(3000));
            story.Completed += (s, er) =>
            {
                var value = new Random().NextDouble() * 10000;
                scrollnumber.SetNumber(value);
                ScrollNumber();
            };
            story.Begin();
        }
    }
}
