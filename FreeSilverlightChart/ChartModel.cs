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
  public class ChartModel
  {
    ///<Summary> 
    /// Abstract Data Structure representing the model for drawing a chart control
    /// The chart is draw using values from the yValues 2D array. The yValues are returned by
    /// yValues. The maximum and the minimum are controlled by maxYValue and minyValue. 
    /// The default maximum value is 120% of the maximum of the yValues if no value is specified. 
    /// The default minimum value is 0. For XYLine and Scatter plots xValues are also required. 
    /// The xValues are controlled by xValues, maxXValue and minXValue.
    /// 
    /// The labels on y-axis of the graph are calculated from the yValues. However the labels on the
    /// x-axis are controlled by the group labels array returned by  groupLabels.
    /// 
    /// Each group of values in the chart may contain multiple series. The number of series in a
    /// group are controlled by seriesLabels. 
    /// 
    /// The colors of the series are controlled by seriesColors.
    /// 
    /// The chart title, sub-title and the footnote can be specified using 
    /// title, subTitle and footNote.
    /// </Summary>
    public ChartModel(
      string [] seriesLabels, 
      string [] groupLabels, 
      double [,] yValues, 
      double [,] xValues,
      string title,
      string subTitle,
      string footNote,
      Color[] seriesColors)
    {
      _seriesLabels = seriesLabels;
      _groupLabels = groupLabels;
      _yValues = yValues;
      _xValues = xValues;

      // default max/min values
      _maxYValue = double.NegativeInfinity;
      _maxXValue = double.NegativeInfinity;
      _minYValue = double.PositiveInfinity;
      _minXValue = double.PositiveInfinity;
      
      _title = title;
      _subTitle = subTitle;
      _footNote = footNote;
      
      _seriesColors = (seriesColors == null ? _DEFAULT_COLORS : seriesColors);
      
      _init();
    }

    static ChartModel()
    {
      _DEFAULT_COLORS = new Color[]{ 
        Color.FromArgb(255,231,109,72),
        Color.FromArgb(255,110,166,243),
        Color.FromArgb(255,157,206,110),
        Color.FromArgb(255,252,196,111),
        Color.FromArgb(255,114,126,142),
        Color.FromArgb(255,109,44,145)
      };
    }
        
    private void _init()
    {
      int colorCount = _seriesColors.Length;
      int labelCount = _seriesLabels.Length;

      if (colorCount < labelCount)
      {
        Color[] newColors = new Color[labelCount];

        Array.Copy(_seriesColors, newColors, colorCount);
        _seriesColors = newColors;
        
        Random random = new Random();
        for (int i = colorCount; i < labelCount; i++)
        {
          // generate random colors
          byte rVal = (byte)(random.Next() % 255);
          byte gVal = (byte)(random.Next() % 255);
          byte bVal = (byte)(random.Next() % 255);
          _seriesColors[i] = Color.FromArgb(255, rVal, gVal, bVal);
        }
      }    
    }

    static private Color[] _DEFAULT_COLORS;

    private string[] _seriesLabels;
    private string[] _groupLabels;
    private double[,] _yValues;
    private double[,] _xValues;
    private Color[] _seriesColors;
    private double _maxYValue;
    private double _minYValue;
    private double _maxXValue;
    private double _minXValue;
    private string _title;
    private string _subTitle;
    private string _footNote;

    public string[] SeriesLabels
    {
      get { return _seriesLabels; }
      set { _seriesLabels = value; }
    }

    public string[] GroupLabels
    {
      get { return _groupLabels; }
      set { _groupLabels = value; }
    }

    public double[,] YValues
    {
      get { return _yValues; }
      set { _yValues = value; }
    }

    public double[,] XValues
    {
      get { return _xValues; }
      set { _xValues = value; }
    }

    public Color[] SeriesColors
    {
      get { return _seriesColors; }
      set { _seriesColors = value; }
    }

    public double MaxYValue
    {
      get { return _maxYValue; }
      set { _maxYValue = value; }
    }

    public double MinYValue
    {
      get { return _minYValue; }
      set { _minYValue = value; }
    }

    public double MaxXValue
    {
      get { return _maxXValue; }
      set { _maxXValue = value; }
    }

    public double MinXValue
    {
      get { return _minXValue; }
      set { _minXValue = value; }
    }

    public string Title
    {
      get { return _title; }
      set { _title = value; }
    }

    public string SubTitle
    {
      get { return _subTitle; }
      set { _subTitle = value; }
    }

    public string FootNote
    {
      get { return _footNote; }
      set { _footNote = value; }
    }
  }
}
