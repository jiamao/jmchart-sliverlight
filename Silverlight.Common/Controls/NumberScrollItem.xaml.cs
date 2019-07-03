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
    public partial class NumberScrollItem : UserControl
    {
        Dictionary<char, TextBlock> numberDic = new Dictionary<char, TextBlock>();
        char curChar = '0';
        /// <summary>
        /// 当并展示的字符
        /// </summary>
        public char NumberChar
        {
            set
            {
                if (curChar == value) return;
                ScrollToNumber(value);
            }
        }

        public NumberScrollItem()
        {
            InitializeComponent();
            this.Loaded += NumberScrollItem_Loaded;
        }

        void NumberScrollItem_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollToNumber(curChar);
        }

        /// <summary>
        /// 滚动向字符
        /// </summary>
        /// <param name="c"></param>
        private void ScrollToNumber(char c)
        {
            try
            {
                TextBlock txt = null;
                if (!numberDic.ContainsKey(c))
                {
                    txt = new TextBlock()
                    {
                        Style = this.Resources["numberStyle"] as Style,
                        Text = c.ToString(),
                        RenderTransform = new TranslateTransform()
                    };
                    numberDic.Add(c, txt);
                }
                else
                {
                    txt = numberDic[c];
                    txt.Visibility = System.Windows.Visibility.Visible;
                }

                if (!LayoutRoot.Children.Contains(txt))
                {
                    LayoutRoot.Children.Add(txt);
                }

                if (LayoutRoot.Children.Count > 0)
                {
                    txt.Opacity = 0;
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        var story = new Storyboard();
                        var ani = new DoubleAnimation();
                        var opacityani = new DoubleAnimation();
                        story.Children.Add(ani);
                        story.Children.Add(opacityani);

                        //opacityani.Duration = ani.Duration = new Duration(TimeSpan.FromMilliseconds(1500));
                        story.Duration = new Duration(TimeSpan.FromMilliseconds(1500));
                        Storyboard.SetTargetProperty(ani, new PropertyPath("Y"));
                        Storyboard.SetTarget(ani, txt.RenderTransform);
                        Storyboard.SetTargetProperty(opacityani, new PropertyPath("Opacity"));
                        Storyboard.SetTarget(opacityani, txt);

                        ani.From = ((TranslateTransform)(txt.RenderTransform)).Y = 0;
                        ani.To = -numberDic[curChar].ActualHeight;
                        opacityani.From = 0;
                        opacityani.To = 1;

                        if (numberDic.ContainsKey(curChar) && c != curChar)
                        {
                            var ani2 = new DoubleAnimation();
                            var opacityani2 = new DoubleAnimation();
                            story.Children.Add(opacityani2);
                            story.Children.Add(ani2);
                            //opacityani2.Duration = ani2.Duration = new Duration(TimeSpan.FromMilliseconds(1500));
                            Storyboard.SetTargetProperty(ani2, new PropertyPath("Y"));
                            Storyboard.SetTarget(ani2, numberDic[curChar].RenderTransform);
                            Storyboard.SetTargetProperty(opacityani2, new PropertyPath("Opacity"));
                            Storyboard.SetTarget(opacityani2, numberDic[curChar]);
                            ani2.From = 0;
                            ani2.To = -numberDic[curChar].ActualHeight;
                            opacityani2.From = 1;
                            opacityani2.To = 0;
                        }
                        story.Completed += (object sender, EventArgs e) =>
                        {
                            if (numberDic.ContainsKey(curChar) && c != curChar)
                            {
                                numberDic[curChar].Visibility = System.Windows.Visibility.Collapsed;
                                LayoutRoot.Children.Remove(numberDic[curChar]);
                            }
                            ((TranslateTransform)(txt.RenderTransform)).Y = 0;
                            curChar = c;
                        };
                        story.Begin();
                    });
                }
            }
            catch
            { }
        }
    }
}
