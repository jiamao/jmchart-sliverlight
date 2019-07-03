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
  /// <summary>
  /// Summary description for CandleStickChartModel
  /// </summary>

  public class CandleStickChartModel : ChartModel
  {
    ///<Summary> 
    /// Abstract Data Structure representing the model for drawing a candlestick chart.
    /// The CandleStickChartModel is similar to the ChartModel. The only difference is that
    /// the yValues are replaced by candleStickYValues. The candleStickYValues is a three dimensional 
    /// array. The extra dimension adds open, high, low, close values required for candleStick
    /// </Summary>

    public CandleStickChartModel(      
      string [] seriesLabels, 
      string [] groupLabels, 
      double [][][] candleStickYValues, 
      string title,
      string subTitle,
      string footNote,
      Color[] seriesColors)
      : base(seriesLabels, groupLabels, null, null, title, subTitle, footNote, seriesColors)
    {
      _candleStickYValues = candleStickYValues;
      
    }

    private double[][][] _candleStickYValues;

    public double[][][] CandleStickYValues
    {
      get { return _candleStickYValues; }
      set { _candleStickYValues = value; }
    }
  }
}