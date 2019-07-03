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
  public class ElementExpandos
  {
    public ElementExpandos()
    {
    }
    
    private int _yValueIndex;
    private int _seriesIndex;
    private Point _tooltipPreferedPoint;
    
    /// <summary>
    /// The YValueIndex associated with this element
    /// </summary>
    public int YValueIndex
    {
      get{return _yValueIndex;}
      set{_yValueIndex = value;}
    }

    /// <summary>
    /// The SeriesIndex associated with this element
    /// </summary>    
    public int SeriesIndex
    {
      get{return _seriesIndex;}
      set{_seriesIndex = value;}
    }

    /// <summary>
    /// The prefered location of the tooltip for this element
    /// </summary>
    public Point TooltipPreferedPoint
    {
      get { return _tooltipPreferedPoint; }
      set { _tooltipPreferedPoint = value; }
    }         
  }
}
