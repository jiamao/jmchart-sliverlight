using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Markup;

namespace FreeSilverlightChart
{
  public class CandleStickChart : Chart
  {
    public CandleStickChart(ChartType type, ChartModel model)
      : base(type, model)
    {
    }

    protected override int GetVLineCount()
    {
      var xMajorCount = XMajorGridCount;
      if (xMajorCount >= 0)
        return xMajorCount;
      else
      {
        // For candle stick chart the first line is a value
        return Model.GroupLabels.Length - 1;
      }
    }

    protected override void ComputeMinMaxValues()
    {
      CandleStickChartModel model = Model as CandleStickChartModel;
      double[][][] candleStickYValues = model.CandleStickYValues;

      double maxYValue = model.MaxYValue;
      double minYValue = model.MinYValue;

      string[] seriesLabels = model.SeriesLabels;

      if (candleStickYValues != null && (double.IsNegativeInfinity(maxYValue) || double.IsPositiveInfinity(minYValue)))
      {
        double value;
        double curMaxValue = Double.NegativeInfinity;
        double curMinValue = Double.PositiveInfinity;

        int groupsCount = candleStickYValues.GetUpperBound(0) + 1;
        int seriesSize = seriesLabels.Length;

        for (int i = 0; i < groupsCount; ++i)
        {
          for (int j = 0; j < seriesSize; ++j)
          {
            value = candleStickYValues[i][j][1]; // Use the high value
            curMaxValue = Math.Max(curMaxValue, value);
            curMinValue = Math.Min(curMinValue, value);
          }
        }

        double maxMult = curMaxValue > 0 ? 1.05 : .95;
        double minMult = curMinValue > 0 ? .95 : 1.05;

        if (double.IsNegativeInfinity(maxYValue))
          model.MaxYValue = curMaxValue * maxMult;
        if (double.IsPositiveInfinity(minYValue))
          model.MinYValue = curMinValue * minMult;
      }
    }

    /// <summary>
    /// Overridden to indicate that the group label is edge aligned instead of center aligned
    /// </summary>    
    protected override bool IsGroupLabelCentered()
    {
      return false;
    }


    public override void DrawChartData()
    {
      bool animate = (this.AnimationDuration > 0);

      double marginLeft = MarginLeft, marginTop = MarginTop;
      double marginRight = MarginRight, marginBottom = MarginBottom;

      double gridWidth = (ChartCanvas.Width - marginLeft - marginRight);
      double gridHeight = (ChartCanvas.Height - marginTop - marginBottom);

      LookAndFeel lf = CurrentLookAndFeel;
      string rectXAML = lf.GetBarPathXAML();
      Path rectElem;
      List<UIElement> dataElems = null;
      MatrixTransform defaultTransform = null;

      if (animate)
      {
        dataElems = DataElements;
      }

      if(!(Model is CandleStickChartModel))
        throw new Exception("model is not a CandleStickChartModel");

      CandleStickChartModel model = Model as CandleStickChartModel;

      string[] groupLabels = model.GroupLabels;
      int groupCount = groupLabels.Length;

      string[] seriesLabels = model.SeriesLabels;
      int seriesCount = seriesLabels.Length;

      Color[] seriesColors = model.SeriesColors;
      double[][][] candleStickYValues = model.CandleStickYValues;


      double minValue = model.MinYValue, maxValue = model.MaxYValue;
      int yValueCount = candleStickYValues.Length;
      double barWidth = ((gridWidth - _PADDING_LEFT) / Math.Max(yValueCount, groupCount));

      double dx = marginLeft + _PADDING_LEFT, dy, barStart, barEnd, stackBase = minValue;
      string gradientXAML = lf.GetElementGradientXAML();

      for (int i = 0; i < yValueCount; ++i)
      {
        dy = gridHeight + marginTop;

        // skip over missing values
        if (candleStickYValues[i] != null)
        {
          for (int j = 0; j < seriesCount; ++j)
          {
            rectElem = XamlReader.Load(rectXAML) as Path;
            double openValue = candleStickYValues[i][j][0];
            double closeValue = candleStickYValues[i][j][3];
            double highValue = candleStickYValues[i][j][1];
            double lowValue = candleStickYValues[i][j][2];

            // use the stock open value
            barStart = (gridHeight + marginTop) - (gridHeight * (openValue - stackBase) / (maxValue - minValue));
            // use the stock close value
            barEnd = (gridHeight + marginTop) - (gridHeight * (closeValue - stackBase) / (maxValue - minValue));

            bool doFill = openValue > closeValue;

            RectangleGeometry rectGeometry = new RectangleGeometry();
            rectGeometry.Rect = new Rect(dx, Math.Min(barStart, barEnd), _CANDLE_WIDTH, Math.Abs(barStart - barEnd));
            rectElem.SetValue(Path.DataProperty, rectGeometry);

            if (doFill)
            {
              if (gradientXAML != null)
              {
                SetGradientOnElement(rectElem, gradientXAML, seriesColors[j], 0xe5);
              }
              else
                SetFillOnElement(rectElem, seriesColors[j]);
            }

            SolidColorBrush scb = new SolidColorBrush(seriesColors[j]);
            rectElem.SetValue(Rectangle.StrokeProperty, scb);

            SetExpandosOnElement(rectElem, i, j,
                                 new Point(dx + _CANDLE_WIDTH / 2.0, Math.Min(barStart, barEnd)));

            if (DisplayToolTip)
            {
              rectElem.MouseEnter += new MouseEventHandler(ShowToolTip);
              rectElem.MouseLeave += new MouseEventHandler(HideToolTip);
            }

            rectElem.MouseLeftButtonUp += new MouseButtonEventHandler(ChartDataClicked);
            ChartCanvas.Children.Add(rectElem);

            // Draw the sticks for the candle
            Line lineTop = new Line(), lineBottom = new Line();
            if (animate)
            {
              dataElems.Add(lineTop);
              dataElems.Add(lineBottom);
              dataElems.Add(rectElem);
              defaultTransform = new MatrixTransform();
              Matrix m = new Matrix();
              m.M22 = 0.0;
              defaultTransform.Matrix = m;

              lineTop.SetValue(UIElement.RenderTransformProperty, defaultTransform);
              lineBottom.SetValue(UIElement.RenderTransformProperty, defaultTransform);
              rectElem.SetValue(UIElement.RenderTransformProperty, defaultTransform);
            }

            lineTop.SetValue(Line.StrokeProperty, scb);
            lineBottom.SetValue(Line.StrokeProperty, scb);

            lineTop.X1 = lineTop.X2 = lineBottom.X1 = lineBottom.X2 = dx + _CANDLE_WIDTH / 2.0;
            lineTop.Y1 = doFill ? barStart : barEnd;
            lineTop.Y2 = (gridHeight + marginTop) - (gridHeight * (highValue - stackBase) / (maxValue - minValue));
            lineBottom.Y1 = doFill ? barEnd : barStart;
            lineBottom.Y2 = (gridHeight + marginTop) - (gridHeight * (lowValue - stackBase) / (maxValue - minValue));

            ChartCanvas.Children.Add(lineTop);
            ChartCanvas.Children.Add(lineBottom);
          }
        }
        dx += barWidth;
      }
    }

    protected override Size FillToolTipData(
      object sender,
      MouseEventArgs e,
      Path boundingRectElem,
      Path circleElem)
    {
      ChartEventArgs chartEvent = GetChartEvent(sender, e);

      CandleStickChartModel model = Model as CandleStickChartModel;

      var j = chartEvent.SeriesIndices[0];
      var i = chartEvent.YValueIndices[0];

      string[] groupLabels = model.GroupLabels,
                seriesLabels = model.SeriesLabels;

      double[][][] candleStickYValues = model.CandleStickYValues;

      Color[] seriesColors = model.SeriesColors;

      //top label
      TextBlock textElem = ToolTip.Children[2] as TextBlock;
      textElem.Text = seriesLabels[j];

      var labelWidth = textElem.ActualWidth;

      //Show the open-high-low-close values as comma separated
      StringBuilder sb = new StringBuilder();
      textElem = ToolTip.Children[3] as TextBlock;
      sb.Append(candleStickYValues[i][j][0].ToString(Format));
      sb.Append(", ");
      sb.Append(candleStickYValues[i][j][1].ToString(Format));
      sb.Append(", ");
      sb.Append(candleStickYValues[i][j][2].ToString(Format));
      sb.Append(", ");
      sb.Append(candleStickYValues[i][j][3].ToString(Format));
      textElem.Text = sb.ToString();

      var dataWidth = textElem.ActualWidth;

      // leave a  clearance on either end of the text
      double xMargin = _TEXT_MARGIN, dx = xMargin;

      if (labelWidth > dataWidth)
        dx = (labelWidth - dataWidth) / 2 + xMargin;

      textElem.SetValue(Canvas.LeftProperty, dx);

      var rectWidth = Math.Max(labelWidth, dataWidth) + 2 * xMargin;

      RectangleGeometry rg = boundingRectElem.Data as RectangleGeometry;
      Rect rect = rg.Rect;
      rg.Rect = new Rect(0, 0, rectWidth, rect.Height);

      SolidColorBrush scb = new SolidColorBrush(seriesColors[j]);
      boundingRectElem.SetValue(Path.StrokeProperty, scb);
      circleElem.SetValue(Path.StrokeProperty, scb);

      return new Size(rectWidth, 2 * (textElem.ActualHeight + _TEXT_MARGIN));
    }


    protected override ChartEventArgs GetChartEvent(object sender, MouseEventArgs e)
    {
      ElementExpandos ee = Expandos[sender];
      int i = ee.YValueIndex;
      int j = ee.SeriesIndex;

      int[] seriesIndices = { j };
      int[] yValueIndices = { i };

      // For now use null for the yValue since there are open-high-low-close values at that location
      return new ChartEventArgs(seriesIndices, yValueIndices, null, null);
    }

    // The width of the candle
    static private int _CANDLE_WIDTH = 4;
    static private int _PADDING_LEFT = 2;
  }
}
