using System;
using System.Text;
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
  public class CylinderBarChart : BarChart
  {
    public CylinderBarChart(ChartType type, ChartModel model)
      : base(type, model)
    {
    }
    
    /// <summary>
    /// Appends the path data to a stringBuilder for a 3D bar
    /// </summary>
    /// <param name="sb"> stringbuilder to which the path data needs to be appended</param>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    /// <param name="xOffset"></param>
    /// <param name="yOffset"></param>
    /// <param name="barWidth"></param>
    /// <param name="barHeight"></param>
    protected override void Get3DBarPathData(
      StringBuilder sb,
      double dx,
      double dy,
      double xOffset,
      double yOffset,
      double barWidth,
      double barHeight
      )
    {

      double ry = Math.Min(yOffset * .707 / 2.0, barWidth / 2.0);
      double rx = barWidth / 2.0;
      // start of the arc
      double sx = dx + xOffset / 2.0;
      double sy = dy - yOffset / 2.0;

      sb.Append("M").Append(sx).Append(",").Append(sy);

      sb.Append(" A").Append(rx).Append(",").Append(ry);
      sb.Append(" 180 1,1 ").Append(sx + barWidth).Append(",").Append(sy);
      sb.Append(" A").Append(rx).Append(",").Append(ry);
      sb.Append(" 180 0,1 ").Append(sx).Append(",").Append(sy);

      sb.Append("M").Append(sx).Append(",").Append(sy);
      sb.Append(" v").Append(barHeight);

      sb.Append("A").Append(rx).Append(",").Append(ry);
      sb.Append(" 180 1,0 ").Append(sx + barWidth).Append(",").Append(sy + barHeight);

      sb.Append(" L").Append(sx + barWidth).Append(",").Append(sy);
      sb.Append("M").Append(sx + barWidth).Append(",").Append(sy);
      sb.Append(" A").Append(rx).Append(",").Append(ry);
      sb.Append(" 180 0,1 ").Append(sx).Append(",").Append(sy);

    }
  }
}
