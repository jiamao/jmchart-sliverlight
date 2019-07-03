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

namespace Silverlight.Common
{
    public class DatePickerExpand : Canvas
    {
        bool onlyone = false;
        public DateTime datatime;

        public DatePickerExpand()
        {
            this.Loaded += new RoutedEventHandler(DatePickerExpand_Loaded);
        }
        void DatePickerExpand_Loaded(object sender, RoutedEventArgs e)
        {
           TextBox txt = new TextBox();
            txt.TextChanged += new TextChangedEventHandler(txt_TextChanged);
            datatime = DateTime.Now;
            txt.Text = datatime.ToString("yyyy-MM-dd HH:mm:ss");
            txt.Width = 125;
            txt.Height = 30;
            this.Children.Add(txt);
            Button btn = new Button();
            btn.Content = "请选择";
            btn.Click += new RoutedEventHandler(btn_Click);

            this.Children.Add(btn);
            btn.SetValue(MarginProperty, new Thickness(txt.Width, 0, 0, 0));
        }

        void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = this.Children[0] as TextBox;
            try
            {
                Convert.ToDateTime(txt.Text);
                datatime = Convert.ToDateTime(txt.Text);
                txt.Foreground = new SolidColorBrush(Colors.Black);
               
            }
            catch (Exception)
            {
                txt.Foreground = new SolidColorBrush(Colors.Red);
            }
        }



        void btn_Click(object sender, RoutedEventArgs e)
        {
            if (onlyone == false)
            {
                Border border = new Border();
                border.BorderBrush = new SolidColorBrush(Colors.Black);
                Thickness tk = new Thickness(1.0);
                border.BorderThickness = tk;
                Canvas can = new Canvas();
                border.Child = can;
                //can.SetValue(Border.BorderBrushProperty, new SolidColorBrush(Colors.Black));

                //can.SetValue(Border.BorderThicknessProperty, 1);
                can.Width = 190;
                can.Height = 203;
                can.Background = new SolidColorBrush(Colors.White);
                can.Opacity = 1.0;
                can.SetValue(ZIndexProperty, 200);
                Calendar calendar = new Calendar();
                calendar.BorderThickness = new Thickness(0.0);
                //calendar.Width = 100;
                TimePicker timePicker = new TimePicker();
                timePicker.Background = new SolidColorBrush(Colors.White);
                timePicker.Opacity = 1.0;
                timePicker.Value = datatime;
                //timePicker.Width = 50;
                Button btn1 = new Button();
                btn1.Opacity = 1.0;
                btn1.Background = new SolidColorBrush(Colors.White);
                btn1.Click += new RoutedEventHandler(btn1_Click);
                btn1.Content = "确定";
                can.Children.Add(calendar);
                can.Children.Add(timePicker);
                can.Children.Add(btn1);
                calendar.SetValue(MarginProperty, new Thickness(7, 0, 0, 0));
                timePicker.SetValue(MarginProperty, new Thickness(5, 175, 0, 0));
                btn1.SetValue(MarginProperty, new Thickness(140, 175, 0, 0));
                //throw new NotImplementedException();
                this.Children.Add(border);
                can.SetValue(MarginProperty, new Thickness(0, 30, 0, 0));
                onlyone = true;
            }
        }



        void btn1_Click(object sender, RoutedEventArgs e)
        {
            Border border = this.Children[2] as Border;
            TextBox txt = this.Children[0] as TextBox;
            Canvas can = border.Child as Canvas;
            Calendar calendar = can.Children[0] as Calendar;
            TimePicker timePicker = can.Children[1] as TimePicker;
            string time1 = "";
            string time2 = "";
            if (calendar.SelectedDate == null)
            {
                time1 = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                time1 = Convert.ToDateTime(calendar.SelectedDate).ToString("yyyy-MM-dd");
            }
            if (timePicker.Value == null)
            {
                time2 = DateTime.Now.ToString("HH:mm:ss");
            }
            else
            {
                time2 = Convert.ToDateTime(timePicker.Value).ToString("HH:mm:ss");
            }
            txt.Text = time1 + " " + time2;
            this.Children.RemoveAt(Children.Count - 1);
            onlyone = false;
            datatime = Convert.ToDateTime(txt.Text);
        }
    }
}
