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
  public class ChartEventArgs : EventArgs
  {
    /// <summary>
    /// Chart Event
    /// </summary>
    /// <param name="seriesIndices"></param>
    /// <param name="yValueIndices"></param>
    /// <param name="yValues"></param>
    /// <param name="xValues"></param>
    public ChartEventArgs(int[]seriesIndices, int[] yValueIndices, double[]yValues, double [] xValues)
    {
      _seriesIndices = seriesIndices;
      _yValueIndices = yValueIndices;
      _yValues = yValues;
      _xValues = xValues;
    }
    
    private int[] _seriesIndices;
    private int[] _yValueIndices;
    private double[] _xValues;
    private double[] _yValues;

    public int[] SeriesIndices
    {
      get{return _seriesIndices;}
    }
    
    public int[] YValueIndices
    {
      get { return _yValueIndices; }
    }
    
    public double[] XValues
    {
      get{return _xValues;}
    }
    
    public double[] YValues
    {
      get{return _yValues;}
    }
    
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      if(_seriesIndices != null)
      {
        sb.Append("seriesIndices = ");
        _joinArray(_seriesIndices, sb);
      }
      if(_yValueIndices != null)
      {
        sb.Append("\nyValueIndices = ");
        _joinArray(_yValueIndices, sb);
      }
      sb.Append("\nyValues = ");
      _joinArray(_yValues, sb);
      
      if(_xValues != null)
      {
        sb.Append("\nxValues = ");
        _joinArray(_xValues, sb);
      }
      
      return sb.ToString();
    }
    
    private void _joinArray(Array array, StringBuilder sb)
    {
      for(int i = 0; i < array.Length; ++i)
      {
        if(i != 0)
          sb.Append(',');
        if(array is double [])
          sb.Append(((double)array.GetValue(i)).ToString("#.0"));
        else
          sb.Append(array.GetValue(i));
      }
    }
  }
}
