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
  public class HorizontalBarChart : Chart
  {
    public HorizontalBarChart(ChartType type, ChartModel model) : base(type, model)
    {
    }

    public override void DrawChartData()
    {
      DrawChartData(1, 0);
    }

    public override void DrawChartData(int mod, int rem)
    {
      if (IsPerspective)
        _drawPerspectiveBars(mod, rem);
      else
        _drawBars(mod, rem);
    }
    
    protected override bool AnimateAlongXAxis()
    {
      // horizontal bar animates around x axis
      return true;  
    }

    /// <summary>
    /// Overridden to Indicate that the chart is a horizontal chart
    /// </summary>
    /// <returns></returns>
    protected override bool IsHorizontalChart()
    {
      return true;
    }

    protected override bool IsGroupLabelCentered()
    {
      return false;
    }
        
    protected override int GetVLineCount()
    {
      return YMajorGridCount;
    }

    protected override int GetHLineCount()
    {
      var xMajorCount = XMajorGridCount;
      if(xMajorCount >= 0)
        return xMajorCount;
      else
        return Model.GroupLabels.Length;
    }
    
    private void _drawBars(int mod, int rem)
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

      int barItemPadding = _BARITEM_PADDING;
      bool isStacked = (Type == ChartType.HBAR_STACKED);

      ChartModel model = Model;

      string[] groupLabels = model.GroupLabels;
      int groupCount = groupLabels.Length;

      string[] seriesLabels = model.SeriesLabels;
      int seriesCount = seriesLabels.Length;

      Color[] seriesColors = model.SeriesColors;
      double[,] yValues = model.YValues;

      double minValue = model.MinYValue, maxValue = model.MaxYValue;

      // For combo graphs we display every once in mod
      double barDivider = isStacked ? 1 : ((mod > 1) ? Math.Ceiling(seriesCount / mod) : seriesCount);
      double stackBase = minValue;
      int yValueCount = yValues.GetUpperBound(0)+1;
      
      double barHeight = (gridHeight / Math.Max(yValueCount, groupCount) - 2 * barItemPadding) / barDivider;
      double dx = marginLeft, dy = gridHeight + marginTop, barWidth;
      string gradientXAML = lf.GetElementGradientXAML();

      for (int i = 0; i < yValueCount; ++i)
      {
        dy -= barItemPadding;
        dx = marginLeft;
        for (int j = 0; j < seriesCount; ++j)
        {
          // for combo charts we draw a bar every once in a mod.
          if ((mod > 1) && (j % mod) != rem)
            continue;
                  
          // If we use non zero min and it is a stacked graph, we need to remove the min for only
          // the first series.
          if (isStacked)
            stackBase = (j == 0 ? minValue : 0);

          rectElem = XamlReader.Load(rectXAML) as Path;
          if (animate)
          {
            dataElems.Add(rectElem);

            // FIXTHIS: This is inefficient. However Silverlight currently does not allow sharing transform attribute
            defaultTransform = new MatrixTransform();
            Matrix m = new Matrix();
            m.M11 = 0.0;
            defaultTransform.Matrix = m;
            rectElem.SetValue(UIElement.RenderTransformProperty, defaultTransform);
          }
          
          barWidth = gridWidth * (yValues[i,j] - stackBase) / (maxValue - minValue);
          RectangleGeometry rectGeometry = new RectangleGeometry();
          rectGeometry.RadiusX = rectGeometry.RadiusY = 2;
          rectGeometry.Rect = new Rect(dx, dy - barHeight, barWidth, barHeight);
          rectElem.SetValue(Path.DataProperty, rectGeometry);

          if (gradientXAML != null)
          {
            SetGradientOnElement(rectElem, gradientXAML, seriesColors[j], 0xe5);
          }
          else
            SetFillOnElement(rectElem, seriesColors[j]);

          rectElem.SetValue(Rectangle.StrokeProperty, new SolidColorBrush(seriesColors[j]));

          if (isStacked)
            dx += barWidth;

          SetExpandosOnElement(rectElem, i, j, new Point(dx + barWidth, dy-barHeight/2.0));

          if (DisplayToolTip)
          {
            rectElem.MouseEnter += new MouseEventHandler(ShowToolTip);
            rectElem.MouseLeave += new MouseEventHandler(HideToolTip);
          }

          rectElem.MouseLeftButtonUp += new MouseButtonEventHandler(ChartDataClicked);

          ChartCanvas.Children.Add(rectElem);
          if (!isStacked)
            dy -= barHeight;
        }
        if (isStacked)
          dy -= barHeight;
        dy -= barItemPadding;
      }
    }
    
    private void _drawPerspectiveBars(int mod, int rem)
    {
      bool animate = (this.AnimationDuration > 0);
      double xOffset = XOffsetPerspective, yOffset = YOffsetPerspective;

      double marginLeft = MarginLeft, marginTop = MarginTop;
      double marginRight = MarginRight, marginBottom = MarginBottom;

      double gridWidth = (ChartCanvas.Width - marginLeft - marginRight - xOffset);
      double gridHeight = (ChartCanvas.Height - marginTop - marginBottom - yOffset);
      LookAndFeel lf = CurrentLookAndFeel;
      string pathXAML = lf.GetBarPathXAML();
      Path pathElem;
      List<UIElement> dataElems = null;
      MatrixTransform defaultTransform = null;

      if (animate)
      {
        dataElems = DataElements;
      }

      int barItemPadding = _BARITEM_PADDING;
      bool isStacked = (Type == ChartType.HBAR_STACKED);

      ChartModel model = Model;

      string[] groupLabels = model.GroupLabels;
      int groupCount = groupLabels.Length;

      string[] seriesLabels = model.SeriesLabels;
      int seriesCount = seriesLabels.Length;

      Color[] seriesColors = model.SeriesColors;
      double[,] yValues = model.YValues;

      double minValue = model.MinYValue, maxValue = model.MaxYValue;

      // For combo graphs we display every once in mod
      int seriesBars = (mod > 1) ? (int)Math.Ceiling(1 /(double)mod) : seriesCount;
      int yValueCount = yValues.GetUpperBound(0)+1;
      double barHeight, stackBase = minValue;

      if (isStacked)
      {
        barHeight = gridHeight/Math.Max(yValueCount, groupCount) - 2*barItemPadding;
      }
      else
      {
        barHeight = (gridHeight/Math.Max(yValueCount, groupCount) 
                     - 2*barItemPadding - (seriesCount)*barItemPadding)/seriesBars;
      }
      
      double dx = marginLeft, dy = gridHeight + marginTop + yOffset, barWidth;
      string gradientXAML = lf.GetElementGradientXAML();

      for (int i = 0; i < yValueCount; ++i)
      {
        dy -= barItemPadding;
        dx = marginLeft;
        for (var j = 0; j < seriesCount; ++j)
        {
          // for combo charts we draw a bar every once in a mod.
          if ((mod > 1) && j % mod != rem)
            continue;
            
          // If we use non zero min and it is a stacked graph, we need to remove the min for only
          // the first series.
          if (isStacked)
            stackBase = (j == 0 ? minValue : 0);

          barWidth = gridWidth * (yValues[i,j] - stackBase) / (maxValue - minValue);
          

          StringBuilder sb = new StringBuilder();
          sb.Append("M").Append(dx).Append(",").Append(dy);
          sb.Append(" h").Append(barWidth);
          sb.Append(" v").Append(-barHeight);
          sb.Append(" h").Append(-barWidth);
          sb.Append(" v").Append(barHeight);

          sb.Append(" M").Append(dx).Append(",").Append(dy - barHeight);
          sb.Append(" l").Append(xOffset).Append(",").Append(-yOffset);
          sb.Append(" h").Append(barWidth);
          sb.Append(" l").Append(-xOffset).Append(",").Append(yOffset);
          sb.Append(" z");
          sb.Append(" M").Append(dx + barWidth).Append(",").Append(dy);
          sb.Append(" v").Append(-barHeight);
          sb.Append(" l").Append(xOffset).Append(",").Append(-yOffset);
          sb.Append(" v").Append(barHeight);
          sb.Append(" z");

          pathElem = CreatePathFromXAMLAndData(lf.GetBarPathXAML(), sb);

          if (animate)
          {
            dataElems.Add(pathElem);
            defaultTransform = new MatrixTransform();
            Matrix m = new Matrix();
            m.M11 = 0.0;
            defaultTransform.Matrix = m;
            pathElem.SetValue(UIElement.RenderTransformProperty, defaultTransform);
          }

          pathElem.SetValue(Path.StrokeProperty, new SolidColorBrush(seriesColors[j]));

          if (gradientXAML != null)
          {
            SetGradientOnElement(pathElem, gradientXAML, seriesColors[j], 0xe5);
          }
          else
          {
            SetFillOnElement(pathElem, seriesColors[j]);
          }

          SetExpandosOnElement(pathElem, i, j, new Point(dx + barWidth + (xOffset)/2.0, dy - (barHeight+yOffset)/2.0));

          if (DisplayToolTip)
          {
            pathElem.MouseEnter += new MouseEventHandler(ShowToolTip);
            pathElem.MouseLeave += new MouseEventHandler(HideToolTip);
          }

          pathElem.MouseLeftButtonUp += new MouseButtonEventHandler(ChartDataClicked);
          
          ChartCanvas.Children.Add(pathElem);
          if (isStacked)
            dx += barWidth;
          else
          {
            dy -= barHeight;
            dy -= barItemPadding;
          }
        }
        if (isStacked)
          dy -= barHeight;
        dy -= barItemPadding;
      }
    }
    
    private int _BARITEM_PADDING = 2;
  }
}
