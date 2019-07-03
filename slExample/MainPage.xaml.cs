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
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var c = new JMChart.RadarCanvas();
            radarchart.CurrentCanvas = c;
            clmchart.IsShowLegend = false;
            clmchart.CurrentCanvas = new JMChart.CoorCanvas();
            button1_Click(null, null);//首次画出默认的           

            
        }

       

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var data = new List<DataClass>();
            var rnd = new Random();
            var count = 10;
            
            if (!int.TryParse(txtitemcount.Text, out count))
            {
                count = 10;
            }
            for (var i = 0; i < count; i++)
            {
                var d = new DataClass();
                d.我是测试的属性0 = i + rnd.Next(400);
                d.我是测试的属性 = i + rnd.Next(400);
                d.我是测试的属性2 = i + rnd.Next(400);
                d.c4 = i + rnd.Next(400);
                d.c5 = i + rnd.Next(400);
                d.测试分段属性 = "p_" + i;

                data.Add(d);

                //var item = new TabItem();
                //item.DataContext = d;
                //tsTab.Items.Add(item);
            }

            radarchart.CurrentCanvas.IsAnimate = clmchart.CurrentCanvas.IsAnimate = chkAni.IsChecked.Value;
            radarchart.CurrentCanvas.IsFillShape = clmchart.CurrentCanvas.IsFillShape = chkFill.IsChecked.Value;
            radarchart.CurrentCanvas.LegendLabelPosition = clmchart.CurrentCanvas.LegendLabelPosition = (JMChart.EnumLegendLabelPosition)cmbLegendPosition.SelectedIndex;
            radarchart.CurrentCanvas.LineWidth = clmchart.CurrentCanvas.LineWidth = cmbLineW.SelectedIndex + 1;

            var radar = radarchart.CurrentCanvas as JMChart.RadarCanvas;
            radar.ItemMappings.Clear();
            //radar.IsDrawBaseLine = false;
            radar.ItemMappings.Add(
                new JMChart.Model.ItemMapping()
                {
                    DataMember = JMChart.Model.ItemMapping.EnumDataMember.Y,
                    DisplayName = "我是测试的属性0",
                    MemberName = "我是测试的属性0"
                }
                );
            radar.ItemMappings.Add(
                new JMChart.Model.ItemMapping()
                {
                    DataMember = JMChart.Model.ItemMapping.EnumDataMember.Y,
                    DisplayName = "我是测试的属性",
                    MemberName = "我是测试的属性"
                }
                );
            radar.ItemMappings.Add(
               new JMChart.Model.ItemMapping()
               {
                   DataMember = JMChart.Model.ItemMapping.EnumDataMember.Y,
                   DisplayName = "我是测试的属性",
                   MemberName = "我是测试的属性"
               }
               );
            /*radar.ItemMappings.Add(
               new JMChart.Model.ItemMapping()
               {
                   DataMember = JMChart.Model.ItemMapping.EnumDataMember.Y,
                   DisplayName = "我是测试的属性",
                   MemberName = "我是测试的属性"
               }
               );*/
           
           

            //radar.ItemTooltipFormat = txttooltip.Text;
            //radar.YValueNames = new string[] { "我是测试的属性0", "我是测试的属性", "我是测试的属性2", "c4", "c5" };
            //radar.YLabels = new string[] { "测试：#YName", "随机:#YName" };

            //数据点的tooltip,如果只有一个就所有的都按这个格式。否则为对应索引的
            radar.ItemTooltips = new string[] { txttooltip.Text };
            radar.LegendLabel = txtlegend.Text;
           // radar.Serieses.Add(radarseries);

            //当线条少于5条时。无法画多边形。因为四条就是一个正方形。所以会默认用圆形。
            radar.IsCircle = chkCircle.IsChecked.Value;

            var coor = clmchart.CurrentCanvas as JMChart.CoorCanvas;
            coor.Serieses.Clear();
            //coor.IsDrawBaseLine = false;
            var serie = new JMChart.Series.CLMBubbleSeries(coor);
            var centermapping = new JMChart.Model.ItemMapping();
            serie.CenterName = "目标簇";
            centermapping.MemberName = "c4";
            centermapping.DataMember = JMChart.Model.ItemMapping.EnumDataMember.Y;
            coor.Serieses.Add(serie);
            serie.ItemMappings.Add(centermapping);
            var v1mapping = new JMChart.Model.ItemMapping();
            v1mapping.DataMember = JMChart.Model.ItemMapping.EnumDataMember.CLMLink;
            v1mapping.MemberName = "我是测试的属性";
            serie.ItemMappings.Add(v1mapping);

            var v2mapping = new JMChart.Model.ItemMapping();
            v2mapping.DataMember = JMChart.Model.ItemMapping.EnumDataMember.CLMLink;
            v2mapping.MemberName = "我是测试的属性0";
            serie.ItemMappings.Add(v2mapping);

            clmchart.DataContext = data;
            radarchart.DataContext = data;// data.Take<DataClass>(4).ToList<DataClass>();
            funnelchart.DataContext = data;
            //事件
            radar.ItemClick = (obj, arg) => {
                System.Windows.Browser.HtmlPage.PopupWindow(new Uri("http://www.jm47.com", UriKind.Absolute), "_blank", new System.Windows.Browser.HtmlPopupWindowOptions());
            };

            serie.ItemTooltipFormat = "用户簇信息[hr]簇ID:#C{c4}\n用户付费转化率:#C{我是测试的属性}\n用户规模:#C{c5}";
            //事件
           serie.ItemClick = (obj, arg) =>
            {
                System.Windows.Browser.HtmlPage.PopupWindow(new Uri("http://www.jm47.com", UriKind.Absolute), "_blank", new System.Windows.Browser.HtmlPopupWindowOptions());
            };
           serie.ArrowTooltipClick = (obj,arg)=>{
               System.Windows.Browser.HtmlPage.PopupWindow(new Uri("http://www.jm47.com", UriKind.Absolute), "_blank", new System.Windows.Browser.HtmlPopupWindowOptions());
           };



           funnelchart.CurrentCanvas.Serieses.Clear();
           var itemmapp = new JMChart.Model.ItemMapping();          
           itemmapp.MemberName = "c4";
           itemmapp.DisplayName = "测试分段属性";
           itemmapp.DataMember = JMChart.Model.ItemMapping.EnumDataMember.Y;
           var ser = new JMChart.Series.FunnelSeries(funnelchart.CurrentCanvas);
           funnelchart.CurrentCanvas.Serieses.Add(ser);
           ser.ItemMappings.Add(itemmapp);

           var leapp = new JMChart.Model.ItemMapping();
           leapp.MemberName = "c4";
           leapp.DisplayName = "测试分段属性";
           leapp.DataMember = JMChart.Model.ItemMapping.EnumDataMember.Lengend;
           ser.ItemMappings.Add(leapp);

            clmchart.Draw();
            radarchart.Draw();
            funnelchart.Draw();

            ////the default type of the chart is vertical bar-chart.
            //Chart.ChartType chartType = Chart.ChartType.VBAR;

            ////set all the information of the chart.
            //ChartModel model = _loadChartModel();

            //// create the chart 
            //Chart chart = Chart.CreateChart(chartType, model);

            ////set the chart to be in 2D or in 3D
            //chart.IsPerspective = false;

            ////set the position of the series labels
            //chart.LegendPosition = Chart.LegendLocation.BOTTOM;

            ////set the duration of the animation
            //chart.AnimationDuration = 2.0;

            ////set the formate of the 
            //chart.Format = "$#,##0.00;($#,##0.00);0";

            ////set the size of the chart area.
            //chart.SetBounds(new Rect(0, 0, chartpanel.Width, chartpanel.Height));
            //// add the custom component
            //this.chartpanel.Children.Add(chart);

            //// setup the event handler so that when mouse enter show show the information of the bar. 
            ////chart.ChartClicked += new ChartEventHandler(ChartClicked);

            //// now draw it
            //chart.Draw();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
           //textcmb.Text += ",a";
        }

        //// load the data of the chart here.
        //private ChartModel _loadChartModel()
        //{
        //    // the name of the each group
        //    string[] _groupLabels = { "G1", "G2", "G3", "G4", "G5", "G6", "G7", "G8", "G9", "G10" };
        //    // the name of each series.
        //    string[] _seriesLabels = { "S1", "S2", "S3" };

        //    double[,] _chartYValues = {{135235, 155535, 141725},
        //                      {106765, 131725, 127868},
        //                      {108456, 119326, 139326},
        //                      {136765, 147265, 184349}, 
        //                      {107868, 113968, 174349},
        //                      {103019, 119145, 182080},
        //                      {149037, 156071, 187799},
        //                      {133013, 169140, 186010},
        //                      {139994, 186871, 158003}, 
        //                      {191246, 180196, 153177}};

        //    double[,] _chartXValues = {{6.1, 6.3, 6.5},
        //                        {6.8, 7.1,7.3},
        //                        {7.6, 7.8, 8.0},
        //                        {7.9, 8.4, 8.9},
        //                        {8.15, 8.45, 9.1},
        //                        {8.45, 8.55, 9.2}, 
        //                        {8.76, 8.75, 9.4},
        //                        {8.98, 8.95, 9.6},
        //                        {9.14, 9.25, 9.7},
        //                        {9.23, 9.48,9.88}};


        //    string[] _seriesColors = { "#E76D48", "#6EA6F3", "#9DCE6E", "#FCC46F", "#FF7FFF", "#AEAEAE" };


        //    Color[] colors = new Color[_seriesColors.Length];
        //    for (var i = 0; i < _seriesColors.Length; ++i)
        //    {


        //        if (_seriesColors[i].Length != 7)
        //        {
        //            throw new Exception("Invalid color format");
        //        }
        //        else
        //        {
        //            byte r = byte.Parse(_seriesColors[i].Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
        //            byte g = byte.Parse(_seriesColors[i].Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
        //            byte b = byte.Parse(_seriesColors[i].Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
        //            colors[i] = Color.FromArgb(255, r, g, b);
        //        }
        //    }

        //    double minYValue = 0;
        //    double maxYValue = 200000;
        //    double minXValue = 6;
        //    double maxXValue = 10;

        //    string title = "This is the title of chart";
        //    string subTitle = "This is sub-tilte";
        //    string footNote = "This is footnote";

        //    ChartModel model = new ChartModel(_seriesLabels, _groupLabels, _chartYValues, _chartXValues, title, subTitle, footNote, colors);

        //    model.MaxYValue = maxYValue;
        //    model.MaxXValue = maxXValue;
        //    model.MinYValue = minYValue;
        //    model.MinXValue = minXValue;

        //    return model;
        //}
    }

    public class DataClass
    {
        public double 我是测试的属性0 { get; set; }

        public double 我是测试的属性 { get; set; }

        public double 我是测试的属性2 { get; set; }

        public double c4 { get; set; }
        public double c5 { get; set; }

        public string 测试分段属性 { get; set; }
    }
}
