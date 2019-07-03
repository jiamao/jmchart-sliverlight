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
  public interface LookAndFeel
  {
    /// <summary>
    /// Returns the XAML used to create the border for the chart
    /// </summary>
    /// <returns>XAML representing the string for the border of the chart</returns>
    string GetBorderXAML();

    /// <summary>
    /// Returns the XAML used to create the 2D grid rect for the chart
    /// </summary>
    string GetGridRectXAML();
    
    /// <summary>
    /// Returns the XAML used to create a path with the lines in the grid 
    /// </summary>    
    string GetGridPathXAML();

    /// <summary>
    /// Returns the XAML used to create the path for the perspective grid 
    /// </summary>
    string GetGridPath3DXAML();
    
    /// <summary>
    /// Returns the XAML used to create the 2D grid circle for radar charts.
    /// The XAML element must be a path with ellipse geometry
    /// </summary>
    string GetRadarCircleXAML();
    
    /// <summary>
    /// Returns the XAML used to create the inner grid circles for radar charts.
    /// The XAML element must be a path with ellipse geometry
    /// </summary>
    string GetRadarInnerCircleXAML();

    /// <summary>
    /// Returns the XAML used to create a path with the lines in the grid for radar chart
    /// </summary>    
    string GetRadarGridPathXAML();
    
    /// <summary>
    /// Returns the XAML used to create the bars for 2D and perspective bar charts
    /// </summary>
    string GetBarPathXAML();

    /// <summary>
    /// Returns the XAML used to create the pies for 2D and perspective pie charts
    /// </summary>
    string GetPiePathXAML();

    /// <summary>
    /// Returns the XAML used to create the areas for 2D and perspective area charts
    /// </summary>
    string GetAreaPathXAML();

    /// <summary>
    /// Returns the XAML used to create the lines for 2D line charts
    /// </summary>
    string GetLinePathXAML();

    /// <summary>
    /// Returns the XAML used to create the lines for perspective line charts
    /// </summary>
    string GetLinePath3DXAML();

    /// <summary>
    /// Returns the XAML used to create the dots for line charts. The XAML must contain a Path with ellipse geometry
    /// </summary>
    string GetLineDotXAML();

    /// <summary>
    /// Returns the XAML used to create the dots for 2D scatter plots.
    /// The XAML element must be a path with ellipse geometry
    /// </summary>
    string GetScatterDotXAML();

    /// <summary>
    /// Returns the XAML used to create the dots for perspective scatter plots. 
    /// The XAML element must be a path with ellipse geometry
    /// </summary>
    string GetScatterDot3DXAML();

    /// <summary>
    /// Returns the XAML used to create the funnels for 2D and perspective funnel charts
    /// </summary>
    string GetFunnelPathXAML();

    /// <summary>
    /// Returns the XAML used to create the Title text of the chart
    /// </summary>
    string GetTitleXAML();

    /// <summary>
    /// Returns the XAML used to create the Sub-title text of the chart
    /// </summary>
    string GetSubTitleXAML();

    /// <summary>
    /// Returns the XAML used to create the Footnote text of the chart
    /// </summary>
    string GetFootNoteXAML();

    /// <summary>
    /// Returns the XAML used to create the group labels
    /// </summary>
    string GetGroupLabelXAML();

    /// <summary>
    /// Returns the XAML used to create the yValue labels
    /// </summary>
    string GetYValueLabelXAML();

    /// <summary>
    /// Returns the XAML used to create the legend text
    /// </summary>
    string GetLegendTextXAML();

    /// <summary>
    /// Returns the XAML used to create the icon
    /// </summary>
    string GetLegendIconXAML();

    /// <summary>
    /// Returns the XAML used to gradient inside the grid. 
    /// A return of null string indicates no gradient
    /// </summary>
    string GetGridGradientXAML();

    /// <summary>
    /// Returns the XAML used to gradient for the elements
    /// Return null value for no gradient.
    /// </summary>
    string GetElementGradientXAML();

    /// <summary>
    /// Returns the XAML used to create the tooltip
    /// </summary>
    string GetTooltipXAML();

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
    string GetCircularGauageXAML();

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
    string GetSemiCircularGauageXAML();
    
    /// <summary>
    /// Returns the XAML for drawing the text inside the gauge
    /// </summary>
    string GetGauageTextXAML();

    /// <summary>
    /// Returns the XAML for drawing the marjor markers inside the gauge
    /// </summary>
    string GetGauageMarkerMajorXAML();

    /// <summary>
    /// Returns the XAML for drawing the minor markers inside the gauge
    /// </summary>
    string GetGauageMarkerMinorXAML();
            
    /// <summary>
    /// returns the size of the border. Recommended value is 6
    /// </summary>
    /// <returns></returns>
    double GetBorderSize();
        
    /// <summary>
    /// returns the identifier associated with this Look And Feel
    /// </summary>
    /// <returns></returns>
    string GetId();
  }
}
