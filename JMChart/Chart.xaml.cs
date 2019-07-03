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

namespace JMChart
{
    public partial class Chart : UserControl
    {
        /// <summary>
        /// 当前图表的画布
        /// </summary>
        Canvas chartCanvas;

        const int MarginSize = 24;

        public Chart()
        {
            InitializeComponent();

            chartCanvas = new Canvas();
            
            chartCanvas.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            chartCanvas.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            //chartCanvas.Margin = new Thickness(2);

            IsShowLegend = true;
            //默认为坐标图
            CurrentCanvas = new ChartCanvas();
            
            this.Loaded += Chart_Loaded;
            this.SizeChanged += new SizeChangedEventHandler(Chart_SizeChanged);
           
            //this.chartCanvas.SizeChanged += new SizeChangedEventHandler(chartCanvas_SizeChanged);
        }

        void Chart_LayoutUpdated(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(this.DesiredSize.Width);
        }

        /// <summary>
        /// 当前画布类型
        /// </summary>
        public ChartCanvas CurrentCanvas
        {
            get;
            set;
        }

        /// <summary>
        /// 是否显示图例
        /// </summary>
        public bool IsShowLegend { get; set; }

        /// <summary>
        /// 图例位置
        /// </summary>
        public EnumLegendLabelPosition LegendLabelPosition
        {
            get {
                return CurrentCanvas.LegendLabelPosition;
            }
            set
            {
                CurrentCanvas.LegendLabelPosition = value;
            }
        }

        ///// <summary>
        ///// 绑定的数据
        ///// </summary>
        //public new object DataContext
        //{
        //    get {
        //        return CurrentCanvas.DataContext;
        //    }
        //    set
        //    {                
        //        CurrentCanvas.DataContext = value;
        //    }
        //}

        /// <summary>
        /// 当大小发生变化时重画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Chart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (CurrentCanvas != null)
            //{
            //    CurrentCanvas.Width = this.LayoutRoot.ActualWidth;
            //    CurrentCanvas.Height = this.LayoutRoot.ActualHeight;
            //}

            if (CurrentCanvas != null && DataContext != null)
            {
                Draw();
            }            
        }

        /// <summary>
        /// 展示当前图表
        /// </summary>
        public void Draw()
        {
            if (CurrentCanvas != null)
            {
                if (CurrentCanvas.Parent == null)
                {
                    CurrentCanvas.Parent = this.chartCanvas;
                }

                CurrentCanvas.DataContext = this.DataContext;
            
                this.LayoutRoot.Children.Clear();
                this.chartCanvas.Children.Clear();

                if (IsShowLegend)
                {
                    this.CurrentCanvas.LegendPanel.Visibility = System.Windows.Visibility.Visible;
                    ///初始化图例容器
                    /////根据图例位置，排例图表与图例的位置
                    switch (CurrentCanvas.LegendLabelPosition)
                    {
                        case EnumLegendLabelPosition.Bottom:
                            {
                                this.LayoutRoot.Children.Add(this.chartCanvas);
                                this.CurrentCanvas.LegendPanel.Orientation = Orientation.Horizontal;
                                this.CurrentCanvas.LegendPanel.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                                this.CurrentCanvas.LegendPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                this.LayoutRoot.Children.Add(this.CurrentCanvas.LegendPanel);
                                chartCanvas.Margin = new Thickness(MarginSize);

                                CurrentCanvas.Width = this.ActualWidth - MarginSize * 2;
                                CurrentCanvas.Height = this.ActualHeight - CurrentCanvas.LegendSize.Height - MarginSize;
                                break;
                            }
                        case EnumLegendLabelPosition.Right:
                            {
                                this.LayoutRoot.Children.Add(this.chartCanvas);
                                this.CurrentCanvas.LegendPanel.Orientation = Orientation.Vertical;
                                this.CurrentCanvas.LegendPanel.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                                this.CurrentCanvas.LegendPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                                this.LayoutRoot.Children.Add(this.CurrentCanvas.LegendPanel);
                                chartCanvas.Margin = new Thickness(24, 24, 24, 30);

                                CurrentCanvas.Width = this.ActualWidth - CurrentCanvas.LegendSize.Width - MarginSize;
                                CurrentCanvas.Height = this.ActualHeight - MarginSize * 2;
                                break;
                            }
                        case EnumLegendLabelPosition.Top:
                            {
                                this.CurrentCanvas.LegendPanel.Orientation = Orientation.Horizontal;
                                this.CurrentCanvas.LegendPanel.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                                this.CurrentCanvas.LegendPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                this.LayoutRoot.Children.Add(this.CurrentCanvas.LegendPanel);
                                this.LayoutRoot.Children.Add(this.chartCanvas);

                                CurrentCanvas.Width = this.ActualWidth - MarginSize * 2;
                                CurrentCanvas.Height = this.ActualHeight - CurrentCanvas.LegendSize.Height - MarginSize;

                                chartCanvas.Margin = new Thickness(24, CurrentCanvas.LegendSize.Height, MarginSize, MarginSize);
                                break;
                            }
                        case EnumLegendLabelPosition.Left:
                            {
                                this.CurrentCanvas.LegendPanel.Orientation = Orientation.Vertical;
                                this.CurrentCanvas.LegendPanel.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                                this.CurrentCanvas.LegendPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                this.LayoutRoot.Children.Add(this.CurrentCanvas.LegendPanel);
                                this.LayoutRoot.Children.Add(this.chartCanvas);

                                CurrentCanvas.Width = this.ActualWidth - CurrentCanvas.LegendSize.Width - MarginSize;
                                CurrentCanvas.Height = this.ActualHeight - MarginSize - MarginSize;

                                chartCanvas.Margin = new Thickness(CurrentCanvas.LegendSize.Width, MarginSize, MarginSize, MarginSize);
                                break;
                            }

                    }
                }
                else
                {
                    CurrentCanvas.Width = this.ActualWidth - MarginSize * 2;
                    CurrentCanvas.Height = this.ActualHeight - MarginSize;
                    this.CurrentCanvas.LegendPanel.Visibility = System.Windows.Visibility.Collapsed;
                    this.LayoutRoot.Children.Add(this.chartCanvas);
                }
                if (CurrentCanvas.Width > 0 && CurrentCanvas.Height > 0)
                {
                    CurrentCanvas.Draw();
                }
            }
        }
        

        void Chart_Loaded(object sender, RoutedEventArgs e)
        {
            this.LayoutRoot.Background = this.Background;
        }
    }
}
