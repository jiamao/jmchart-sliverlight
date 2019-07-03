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
    public partial class CloseElement : UserControl
    {
        public CloseElement()
        {
            InitializeComponent();

            _fouceImg = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Silverlight.ProcessEditor;component/Images/closeFocus.png", UriKind.Relative));
            _subImg = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Silverlight.ProcessEditor;component/Images/close.png", UriKind.Relative));

            //this.Loaded += new RoutedEventHandler(CloseElement_Loaded);
           
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// 载入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CloseElement_Loaded(object sender, RoutedEventArgs e)
        {            
            
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        public void BindEvent()
        {
            if (this.TargetControl != null)
            {
                var element = this.TargetControl as UIElement;
                if (element != null)
                {
                    element.MouseEnter += new MouseEventHandler(element_MouseEnter);
                    element.MouseLeave += new MouseEventHandler(element_MouseLeave);

                    if (this.TargetControl is Path)
                    {
                        element.MouseLeftButtonUp += new MouseButtonEventHandler(imgClose_MouseLeftButtonUp);
                    }
                }
            }
        }

        void element_MouseLeave(object sender, MouseEventArgs e)
        {
            if (TargetControl == null) return;
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        void element_MouseEnter(object sender, MouseEventArgs e)
        {
            if (TargetControl == null) return;
            this.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// 绑定的目标控件
        /// </summary>
        public UIElement TargetControl = null;

        ImageSource _fouceImg=null;
        ImageSource _subImg = null;

        //感应鼠标进入
        private void imgClose_MouseEnter(object sender, MouseEventArgs e)
        {
            if (TargetControl == null) return;
            this.imgClose.Source = _fouceImg;
        }
        private void imgClose_MouseLeave(object sender, MouseEventArgs e)
        {
            if (TargetControl == null) return;
            this.imgClose.Source = _subImg;
        }

        /// <summary>
        /// 移除自身
        /// </summary>
        public void Remove()
        {
            var parent = this.Parent as Canvas;
            if (parent != null)
            {
                parent.Children.Remove(this);
            }
        }


        /// <summary>
        /// 执行移除事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.TargetControl == null) return;

                if (MessageBox.Show("确定要移除？", "确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    //this.TargetControl.Remove();
                    this.TargetControl = null;
                    this.Remove();//移除自已
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
