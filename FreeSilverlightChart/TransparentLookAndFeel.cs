using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace FreeSilverlightChart
{
  public class TransparentLookAndFeel : LookAndFeel
  {
    /// <summary>
    /// returns the size of the border. 
    /// </summary>
    /// <returns></returns>
    public double GetBorderSize()
    {
      return 6;
    }

    /// <summary>
    /// returns the identifier associated with this Look And Feel
    /// </summary>
    /// <returns></returns>
    public string GetId()
    {
      return _TRANSPARENT_LOOK_AND_FEEL;
    }

    /// <summary>
    /// Returns the XAML used to create the border for the chart
    /// </summary>
    /// <returns>XAML representing the string for the border of the chart</returns>
    public string GetBorderXAML()
    {
      return _BORDER;
    }

    /// <summary>
    /// Returns the XAML used to create the 2D grid rect for the chart
    /// </summary>
    public string GetGridRectXAML()
    {
      return _GRID_RECT;
    }

    /// <summary>
    /// Returns the XAML used to create a path with the lines in the grid 
    /// </summary>    
    public string GetGridPathXAML()
    {
      return _GRID_PATH;
    }

    /// <summary>
    /// Returns the XAML used to create the path for the perspective grid 
    /// </summary>
    public string GetGridPath3DXAML()
    {
      return _GRID_PATH_3D;
    }

    /// <summary>
    /// Returns the XAML used to create the 2D grid circle for radar charts
    /// </summary>
    public string GetRadarCircleXAML()
    {
      return _RADAR_CIRCLE;
    }

    /// <summary>
    /// Returns the XAML used to create the inner grid circles for radar charts
    /// </summary>
    public string GetRadarInnerCircleXAML()
    {
      return _RADAR_INNER_CIRCLE;
    }

    /// <summary>
    /// Returns the XAML used to create a path with the lines in the grid for radar chart
    /// </summary>    
    public string GetRadarGridPathXAML()
    {
      return _RADAR_GRID_PATH;
    }

    /// <summary>
    /// Returns the XAML used to create the bars for 2D and perspective bar charts
    /// </summary>
    public string GetBarPathXAML()
    {
      return _BAR_PATH;
    }

    /// <summary>
    /// Returns the XAML used to create the pies for 2D and perspective pie charts
    /// </summary>
    public string GetPiePathXAML()
    {
      return _PIE_PATH;
    }

    /// <summary>
    /// Returns the XAML used to create the areas for 2D and perspective area charts
    /// </summary>
    public string GetAreaPathXAML()
    {
      return _AREA_PATH;
    }

    /// <summary>
    /// Returns the XAML used to create the lines for 2D line charts
    /// </summary>
    public string GetLinePathXAML()
    {
      return _LINE_PATH;
    }

    /// <summary>
    /// Returns the XAML used to create the lines for perspective line charts
    /// </summary>
    public string GetLinePath3DXAML()
    {
      return _LINE_PATH_3D;
    }

    /// <summary>
    /// Returns the XAML used to create the dots for line charts
    /// </summary>
    public string GetLineDotXAML()
    {
      return _LINE_DOT;
    }

    /// <summary>
    /// Returns the XAML used to create the dots for 2D scatter plots
    /// </summary>
    public string GetScatterDotXAML()
    {
      return _SCATTER_DOT;
    }

    /// <summary>
    /// Returns the XAML used to create the dots for perspective scatter plots
    /// </summary>
    public string GetScatterDot3DXAML()
    {
      return _SCATTER_DOT_3D;
    }

    /// <summary>
    /// Returns the XAML used to create the funnels for 2D and perspective funnel charts
    /// </summary>
    public string GetFunnelPathXAML()
    {
      return _FUNNEL_PATH;
    }

    public string GetTitleXAML()
    {
      return _TITLE_TEXT;
    }

    public string GetSubTitleXAML()
    {
      return _SUBTITLE_TEXT;
    }

    public string GetFootNoteXAML()
    {
      return _FOOT_NOTE_TEXT;
    }

    /// <summary>
    /// Returns the XAML used to create the group labels
    /// </summary>
    public string GetGroupLabelXAML()
    {
      return _GROUP_LABEL;
    }

    /// <summary>
    /// Returns the XAML used to create the yValue labels
    /// </summary>
    public string GetYValueLabelXAML()
    {
      return _Y_LABEL;
    }

    /// <summary>
    /// Returns the XAML used to create the legend text
    /// </summary>
    public string GetLegendTextXAML()
    {
      return _LEGEND_TEXT;
    }

    /// <summary>
    /// Returns the XAML used to create the icon
    /// </summary>
    public string GetLegendIconXAML()
    {
      return _LEGEND_RECT;
    }

    /// <summary>
    /// Returns the XAML used to gradient inside the grid
    /// </summary>
    public string GetGridGradientXAML()
    {
      return null;
    }

    /// <summary>
    /// Returns the XAML used to gradient for the elements
    /// </summary>
    public string GetElementGradientXAML()
    {
      return null;
    }

    /// <summary>
    /// Returns the XAML used to create the tooltip
    /// </summary>
    public string GetTooltipXAML()
    {
      return _TOOLTIP;
    }

    /// <summary>
    /// <para>Returns the XAML used to draw the template for the circular gauge.
    /// The root canvas gives the bounds of the circular gauage. 
    /// </para>
    /// <para>
    /// The Canvas name "indicator" is gauge indicator. It's Center is the center of rotation.
    /// </para>
    /// <para>
    /// The Canvas with name "markerContainer" is the Gauge Marker container. 
    /// Its  Width is used to compute Marker radius
    /// </para>
    /// </summary>
    public string GetCircularGauageXAML()
    {
      return _CIRCULAR_GAUGE;
    }

    /// <summary>
    /// <para>Returns the XAML used to draw the template for the semi-circular gauge.
    /// The root canvas gives the bounds of the circular gauage. 
    /// </para>
    /// <para>
    /// The Canvas name "indicator" is gauge indicator. It's Center is the center of rotation.
    /// </para>
    /// <para>
    /// The Canvas with name "markerContainer" is the Gauge Marker container. 
    /// Its  Width is used to compute Marker radius
    /// </para>
    /// </summary>
    public string GetSemiCircularGauageXAML()
    {
      return _SEMI_CIRCULAR_GAUGE;
    }
    
    /// <summary>
    /// Returns the XAML for drawing the text inside the gauge
    /// </summary>
    public string GetGauageTextXAML()
    {
      return _GAUGE_TEXT;
    }

    /// <summary>
    /// Returns the XAML for drawing the marjor markers inside the gauge
    /// </summary>
    public string GetGauageMarkerMajorXAML()
    {
      return _GAUGE_MARKER_MAJOR;
    }

    /// <summary>
    /// Returns the XAML for drawing the minor markers inside the gauge
    /// </summary>
    public string GetGauageMarkerMinorXAML()
    {
      return _GAUGE_MARKER_MINOR;
    }
    
    private const string _TRANSPARENT_LOOK_AND_FEEL = "TransparentLookAndFeel";

    private const string _BORDER = @"<Rectangle xmlns=""http://schemas.microsoft.com/client/2007"" Stroke=""#DFA726"">
                                    <Rectangle.Fill>
                                    <LinearGradientBrush StartPoint=""0,0"" EndPoint=""0,1"">
                                    <GradientStop Color=""#B23F2F83"" Offset=""0.0""/>
                                    <GradientStop Color=""#FF3F2F83"" Offset=""0.1""/>
                                    <GradientStop Color=""#CC3F2F83"" Offset=""0.9""/>
                                    <GradientStop Color=""#FF3F2F83"" Offset=""1.0""/>
                                    </LinearGradientBrush>
                                    </Rectangle.Fill>
                                    </Rectangle>";
  
    // chart grid
    private const string _GRID_RECT = "<Path xmlns=\"http://schemas.microsoft.com/client/2007\" Stroke=\"#DFA726\" StrokeThickness=\"1\"/>";
    private const string _GRID_PATH = "<Path xmlns=\"http://schemas.microsoft.com/client/2007\" Stroke=\"#DFA726\" StrokeThickness=\".5\"/>";
    private const string _GRID_PATH_3D = "<Path xmlns=\"http://schemas.microsoft.com/client/2007\" Stroke=\"#DFA726\" StrokeThickness=\"1\"/>";

    // radar chart
    private const string _RADAR_CIRCLE = @"<Path xmlns=""http://schemas.microsoft.com/client/2007"" Stroke=""#000000"" StrokeThickness=""1"">
                                        <Path.Data>
                                          <EllipseGeometry />
                                          </Path.Data>
                                        </Path>";

    private const string _RADAR_INNER_CIRCLE = @"<Path xmlns=""http://schemas.microsoft.com/client/2007"" Stroke=""#A0A0A0"" StrokeThickness=""1"">
                                        <Path.Data>
                                          <EllipseGeometry />
                                          </Path.Data>
                                        </Path>";
    private const string _RADAR_GRID_PATH = "<Path xmlns=\"http://schemas.microsoft.com/client/2007\" Stroke=\"#A0A0A0\" StrokeThickness=\"1\"/>";

    // bar charts
    private const string _BAR_PATH = @"<Path xmlns=""http://schemas.microsoft.com/client/2007"" StrokeThickness=""1"">
                                        <Path.Fill>
                                          <SolidColorBrush Opacity=""0.5""/>
                                        </Path.Fill>
                                        </Path>";

    // pie
    private const string _PIE_PATH = @"<Path xmlns=""http://schemas.microsoft.com/client/2007"" StrokeThickness=""1"">
                                        <Path.Fill>
                                          <SolidColorBrush Opacity=""0.7""/>
                                        </Path.Fill>
                                        </Path>";

    // area
    private const string _AREA_PATH = @"<Path xmlns=""http://schemas.microsoft.com/client/2007"" StrokeThickness=""1"">
                                        <Path.Fill>
                                          <SolidColorBrush Opacity=""0.3""/>
                                        </Path.Fill>
                                        </Path>";

    // line charts
    private const string _LINE_PATH = "<Path xmlns=\"http://schemas.microsoft.com/client/2007\" StrokeThickness=\"2\"/>";
    private const string _LINE_PATH_3D = @"<Path xmlns=""http://schemas.microsoft.com/client/2007"" StrokeThickness=""1"">
                                        <Path.Fill>
                                          <SolidColorBrush Opacity=""0.5""/>
                                        </Path.Fill>
                                        </Path>";

    private const string _LINE_DOT = @"<Path xmlns=""http://schemas.microsoft.com/client/2007"" StrokeThickness=""1"">
                                        <Path.Data>
                                          <EllipseGeometry Center=""0,0"" RadiusX=""4"" RadiusY=""4""/>
                                          </Path.Data>
                                        <Path.Fill>
                                          <SolidColorBrush Opacity=""0.5""/>
                                        </Path.Fill>
                                        </Path>";

    // scatter plots
    private const string _SCATTER_DOT_3D = @"<Path xmlns=""http://schemas.microsoft.com/client/2007"" StrokeThickness=""1"">
                                        <Path.Data>
                                          <EllipseGeometry Center=""0,0"" RadiusX=""4"" RadiusY=""4""/>
                                        </Path.Data>
                                        <Path.Fill>
                                          <SolidColorBrush Opacity=""0.5""/>
                                        </Path.Fill>
                                        </Path>";
    private const string _SCATTER_DOT = @"<Path xmlns=""http://schemas.microsoft.com/client/2007"" StrokeThickness=""1"">
                                        <Path.Data>
                                          <EllipseGeometry Center=""0,0"" RadiusX=""2"" RadiusY=""2""/>
                                          </Path.Data>
                                          <Path.Fill>
                                          <SolidColorBrush Opacity=""0.5""/>
                                          </Path.Fill>
                                        </Path>";

    // funnel 
    private const string _FUNNEL_PATH = @"<Path xmlns=""http://schemas.microsoft.com/client/2007"" StrokeThickness=""1"">
                                        <Path.Fill>
                                        <SolidColorBrush Opacity=""0.5""/>
                                        </Path.Fill>
                                        </Path>";

    private const string _LEGEND_RECT = @"<Rectangle xmlns=""http://schemas.microsoft.com/client/2007"" Width=""8"" Height=""8"" Stroke=""#000000"" StrokeThickness=""1"">
                                          <Rectangle.Fill>
                                          <SolidColorBrush Opacity=""0.5""/>
                                          </Rectangle.Fill>
                                          </Rectangle>
                                          ";

    // Labels
    private const string _Y_LABEL = "<TextBlock xmlns=\"http://schemas.microsoft.com/client/2007\" FontSize=\"11\" FontFamily=\"Ariel\" Foreground=\"#aaaaaa\"/>";
    private const string _GROUP_LABEL = "<TextBlock xmlns=\"http://schemas.microsoft.com/client/2007\" FontSize=\"11\" FontFamily=\"Ariel\" Foreground=\"#aaaaaa\"/>";

    private const string _LEGEND_TEXT = "<TextBlock xmlns=\"http://schemas.microsoft.com/client/2007\" FontSize=\"11\" FontFamily=\"Ariel\" Foreground=\"#aaaaaa\"/>";

    private const string _TITLE_TEXT = "<TextBlock xmlns=\"http://schemas.microsoft.com/client/2007\" FontSize=\"14\" FontFamily=\"Ariel\" Foreground=\"#FF0000\"/>";
    private const string _SUBTITLE_TEXT = "<TextBlock xmlns=\"http://schemas.microsoft.com/client/2007\" FontSize=\"11\" FontFamily=\"Ariel\" Foreground=\"#cccccc\"/>";
    private const string _FOOT_NOTE_TEXT = "<TextBlock xmlns=\"http://schemas.microsoft.com/client/2007\" FontSize=\"9\" FontFamily=\"Ariel\" Foreground=\"#cccccc\"/>";


    // tooltip
    private const string _TOOLTIP = @"<Canvas xmlns=""http://schemas.microsoft.com/client/2007"">
                          <Path Fill=""#fefee6"" Stroke=""Black"" StrokeThickness=""1"">
                            <Path.Data>
                              <EllipseGeometry Center=""0,20"" RadiusX=""3"" RadiusY=""3""/>
                            </Path.Data>
                          </Path>
                          <Path Opacity=""0.8"" Fill=""#fefee6"" StrokeThickness=""1"">
                            <Path.Data>
                              <RectangleGeometry  Rect=""0,0,0,25"" RadiusX=""2"" RadiusY=""2"" />
                            </Path.Data>
                          </Path>
                          <TextBlock Canvas.Left=""2"" Canvas.Top=""2"" FontSize=""9"" FontFamily=""Verdana"" Foreground=""#000000""/>
                          <TextBlock Canvas.Top=""12"" FontSize=""9"" FontFamily=""Verdana"" Foreground=""#000000""/>
                          </Canvas>";
                          
    // XAML for the circular gauge
    private const string _CIRCULAR_GAUGE = 
    // The root canvas gives the bounds of the circular gauage. 
    // Its center is the center of rotation for the indicator
    @"<Canvas xmlns=""http://schemas.microsoft.com/client/2007"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Width=""450"" Height=""450"">
      <!-- The dial shell -->
      <Canvas>
        <Path>
          <Path.Data>
            <EllipseGeometry Center=""225,225"" RadiusX=""225"" RadiusY=""225""/>
          </Path.Data>
          <Path.Fill>
          <LinearGradientBrush StartPoint=""0,0"" EndPoint=""0,1"">
            <GradientStop Offset=""0.0"" Color=""#FFF299""/>
            <GradientStop Offset=""0.2"" Color=""#DB8827""/>
            <GradientStop Offset=""0.8"" Color=""#DB8827""/>
            <GradientStop Offset=""1.0"" Color=""#FFF299""/>
          </LinearGradientBrush>
          </Path.Fill>
        </Path>
        <Path Fill=""#203D57"" Opacity="".4"">
        <Path.Data>
          <EllipseGeometry Center=""225,225"" RadiusX=""215"" RadiusY=""215""/>
          </Path.Data>
        </Path>
        <Path>
          <Path.Data>
            <EllipseGeometry Center=""225,225"" RadiusX=""210"" RadiusY=""210""/>
          </Path.Data>
          <Path.Fill>
          <RadialGradientBrush GradientOrigin=""0.5,0.5"" Center=""0.5,0.5"" RadiusX=""0.5"" RadiusY=""0.5"">
            <GradientStop Offset=""0.0"" Color=""#B2333333""/>
            <GradientStop Offset=""1.0"" Color=""#E6000000""/>
          </RadialGradientBrush>
          </Path.Fill>
        </Path>          
      </Canvas>
        
      <!-- The container can be customized for e.g. GreenLine -->
      <Path Fill=""#00FF00"" Opacity="".6"" Data=""M427,225 l6,0 A208,208 0 0,1 329,405 L326,400 A202,202 0 0,0 427,225"">
      </Path>

      <TextBlock Text=""Silver Rocks!"" Canvas.Left=""160"" Canvas.Top=""396"" FontSize=""20"" FontFamily=""Ariel"" Foreground=""#aaaaaa""/>
              
      <!-- The Canvas with name ""markerContainer"" is the Gauge Marker container. 
           Its  Width is used to compute Marker radius -->
      <Canvas x:Name=""markerContainer"" Width=""330"">
        <Path Data=""M143,368 L127,394 A195,195 0 1,1 323,394 L308,368 A165,165 0 1,0 143,368"">
          <Path.Fill>
            <LinearGradientBrush StartPoint=""0,0"" EndPoint=""0,1"">
              <GradientStop Offset=""0.0"" Color=""#FFFF90""/>
              <GradientStop Offset=""0.2"" Color=""#FFF335""/>
              <GradientStop Offset=""0.8"" Color=""#FFF335""/>
              <GradientStop Offset=""1.0"" Color=""#FFFF90""/>    
            </LinearGradientBrush>
          </Path.Fill>      
        </Path> 
      </Canvas>
                        
      <!-- The Canvas name ""indicator"" is gauge indicator. 
           It's Center is the center of rotation-->
      <Canvas x:Name=""indicator"" Width=""450"">
        <Canvas.RenderTransform>
          <RotateTransform Angle=""300"" CenterX=""225"" CenterY=""225""/>
        </Canvas.RenderTransform>
        <Path Stroke=""#000000"" StrokeThickness=""1"">
          <Path.Data>
            <RectangleGeometry Rect=""40,221,185,6"" RadiusX=""2"" RadiusY=""2""/>
          </Path.Data>
          <Path.Fill>
          <LinearGradientBrush Opacity="".5"" StartPoint=""0,0"" EndPoint=""0,1"">
            <GradientStop Offset=""0.0"" Color=""#EE9999""/>
            <GradientStop Offset=""1.0"" Color=""#FF0000""/>
          </LinearGradientBrush>
          </Path.Fill>
        </Path>
        <Path Fill=""#eeeeee"" Opacity="".5"" StrokeThickness=""0"">
          <Path.Data>
            <EllipseGeometry Center=""227, 227"" RadiusX=""16"" RadiusY=""16""/>
          </Path.Data>
        </Path>
        <Path Stroke=""#000000"" StrokeThickness=""1"">
          <Path.Data>
            <EllipseGeometry Center=""225, 225"" RadiusX=""16"" RadiusY=""16""/>
          </Path.Data>
          <Path.Fill>
          <RadialGradientBrush GradientOrigin=""0.5,0.5"" Center=""0.5,0.5"" RadiusX=""0.8"" RadiusY=""0.8"">
            <GradientStop Offset=""0.0"" Color=""#CACADA""/>
            <GradientStop Offset=""1.0"" Color=""#EAEAFA""/>
          </RadialGradientBrush>
          </Path.Fill>
        </Path> 
      </Canvas>
    </Canvas>";

    // XAML for the semi circular gauge
    // The root canvas gives the bounds of the semi-circular gauage. 
    private const string _SEMI_CIRCULAR_GAUGE = @"<Canvas xmlns=""http://schemas.microsoft.com/client/2007"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" Width=""450"" Height=""255"">
      <!-- The dial shell -->
      <Canvas>
        <Path Stroke=""#cccccc"" StrokeThickness=""4""  StrokeLineJoin=""Round"" Data=""M0,225A225,225 0 1,1 450,225 A30,30 0 0,1 420,255 
            l-150,0 a80,80 0 0,1 -90,0 l-150,0 a30,30 0 0,1 -30,-30"">
          <Path.Fill>
          <RadialGradientBrush GradientOrigin=""0.5,0.5"" Center=""0.5,0.5"" RadiusX=""0.5"" RadiusY=""0.5"">
            <GradientStop Offset=""0.0"" Color=""#B2333333""/>
            <GradientStop Offset=""1.0"" Color=""#E6000000""/>
          </RadialGradientBrush>
          </Path.Fill>
        </Path>
      </Canvas>
        
      <!-- The container can be customized for e.g. Red Line -->
      <Path Data=""M370,80 l10,0 A215,215 0 0,1 440,225 l-10 0 A205,205 0 0,0 370,80"">
        <Path.Fill>
          <LinearGradientBrush StartPoint=""0,0"" EndPoint=""0,1"">
            <GradientStop Offset=""0.0"" Color=""#B2F0F3F8""/>
            <GradientStop Offset=""1.0"" Color=""#80E3656C""/>
          </LinearGradientBrush>
        </Path.Fill>
       </Path>
      
      <!-- The Canvas with name ""markerContainer"" is the Gauge Marker container. 
           Its  Width is used to compute Marker radius -->
      <Canvas x:Name=""markerContainer"" Width=""330"">
        <Path Data=""M60,225 l-30,0 A195,195 0 1,1 420,225 l-30 0 A165,165 0 1,0 60,225"">
          <Path.Fill>
            <LinearGradientBrush StartPoint=""0,0"" EndPoint=""0,1"">
              <GradientStop Offset=""0.0"" Color=""#FFFF90""/>
              <GradientStop Offset=""0.2"" Color=""#FFF335""/>
              <GradientStop Offset=""0.8"" Color=""#FFF335""/>
              <GradientStop Offset=""1.0"" Color=""#FFFF90""/>    
            </LinearGradientBrush>
          </Path.Fill>      
        </Path> 
      </Canvas>
                        
      <!-- The Canvas name ""indicator"" is gauge indicator. 
           It's Center is the center of rotation-->
      <Canvas x:Name=""indicator"" Width=""450"">
        <Path Stroke=""#000000"" StrokeThickness=""1"">
          <Path.Data>
            <RectangleGeometry Rect=""40,221,185,6"" RadiusX=""2"" RadiusY=""2""/>
          </Path.Data>
          <Path.Fill>
          <LinearGradientBrush Opacity="".5"" StartPoint=""0,0"" EndPoint=""0,1"">
            <GradientStop Offset=""0.0"" Color=""#EE9999""/>
            <GradientStop Offset=""1.0"" Color=""#FF0000""/>
          </LinearGradientBrush>
          </Path.Fill>
        </Path>
        <Path Fill=""#eeeeee"" Opacity="".5"" StrokeThickness=""0"">
          <Path.Data>
            <EllipseGeometry Center=""227, 227"" RadiusX=""16"" RadiusY=""16""/>
          </Path.Data>
        </Path>
        <Path Stroke=""#000000"" StrokeThickness=""1"">
          <Path.Data>
            <EllipseGeometry Center=""225, 225"" RadiusX=""16"" RadiusY=""16""/>
          </Path.Data>
          <Path.Fill>
          <RadialGradientBrush GradientOrigin=""0.5,0.5"" Center=""0.5,0.5"" RadiusX=""0.8"" RadiusY=""0.8"">
            <GradientStop Offset=""0.0"" Color=""#CACADA""/>
            <GradientStop Offset=""1.0"" Color=""#EAEAFA""/>
          </RadialGradientBrush>
          </Path.Fill>
        </Path> 
      </Canvas>
    </Canvas>";
    
    // gauge text  
    private const string _GAUGE_TEXT = "<TextBlock xmlns=\"http://schemas.microsoft.com/client/2007\" FontSize=\"20\" FontFamily=\"Ariel\" Foreground=\"#aaaaaa\"/>";

    //guage markers
    private const string _GAUGE_MARKER_MAJOR = "<Path xmlns=\"http://schemas.microsoft.com/client/2007\" Data=\"M0,0 l-30,2 l0,-6 l30,2z\" Opacity=\".9\" Fill=\"#008984\"/>";
    private const string _GAUGE_MARKER_MINOR = "<Path xmlns=\"http://schemas.microsoft.com/client/2007\" Data=\"M-8,1 l-15,0 l0,2 l15,0z\" Opacity=\".8\" Fill=\"#008984\"/>";
  }
}
