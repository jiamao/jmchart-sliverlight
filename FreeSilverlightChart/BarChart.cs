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
  public class BarChart: Chart
  {
    public BarChart(ChartType type, ChartModel model) : base(type, model)
    {
    }

    public override void DrawChartData()
    {
      int mod = 1, rem = 0;
      bool isCombo = false;      
      
      ChartType type = Type;
      if(type != ChartType.VBAR && type != ChartType.VBAR_STACKED)
      {
        _getModForCombo(ref mod, type);
        isCombo = true;
      }
      
      DrawChartData(mod, rem);
        
      // delegate to the line chart for combos
      if(isCombo)
      {
        // delegate to the other chart type for combo graphs
        for(int i = 0; i<_delegates.Count; i++)
        {
          _delegates[i].DrawChartData(mod, ++rem);
        }
      }
    }
    
    private void _getModForCombo(ref int mod, ChartType type)
    {
      _delegates = new List<Chart>();
      
      if(type == ChartType.BAR_LINE_COMBO)
      {
        mod = 2;
        
        Chart lineChart = new LineChart(ChartType.LINE, Model);
        SetupDelegateChart(this, lineChart);
        _delegates.Add(lineChart);       
      }
      else if(type == ChartType.BAR_AREA_COMBO)
      {
        mod = 2;

        Chart areaChart = new AreaChart(ChartType.AREA, Model);
        SetupDelegateChart(this, areaChart);
        _delegates.Add(areaChart);
      }
      else if(type == ChartType.BAR_LINE_AREA_COMBO)
      {
        mod = 3;

        Chart lineChart = new LineChart(ChartType.LINE, Model);
        SetupDelegateChart(this, lineChart);
        _delegates.Add(lineChart);

        Chart areaChart = new AreaChart(ChartType.AREA, Model);
        SetupDelegateChart(this, areaChart);
        _delegates.Add(areaChart);
      }
    }

    public override void DrawChartData(int mod, int rem)
    {
      if (IsPerspective)
        _drawPerspectiveBars(mod, rem);
      else
        _drawBars(mod, rem);
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
      
      if(animate)
      {
        dataElems = DataElements;
      }
      
      int barItemPadding = _BARITEM_PADDING;
      bool isStacked = (Type == ChartType.VBAR_STACKED);

      ChartModel model = Model;

      string[] groupLabels = model.GroupLabels;
      int groupCount = groupLabels.Length;

      string[] seriesLabels = model.SeriesLabels;
      int seriesCount = seriesLabels.Length;

      Color[] seriesColors = model.SeriesColors;
      double[,] yValues = model.YValues;

      double minValue = model.MinYValue, maxValue = model.MaxYValue;

      // For combo graphs we display every once in mod
      double barDivider = isStacked ? 1 : ((mod > 1) ? Math.Ceiling(seriesCount / (double)mod) : seriesCount);
      int yValueCount = yValues.GetUpperBound(0)+1;      
      double barWidth = (gridWidth/Math.Max(yValueCount,groupCount)-2*barItemPadding)/barDivider;
      
      double dx = marginLeft, dy, barHeight, stackBase = minValue;
      string gradientXAML = lf.GetElementGradientXAML();
      
      for (int i = 0; i<yValueCount; ++i)
      {
        dx += barItemPadding;
        dy = gridHeight + marginTop;
       
        for (int j = 0; j < seriesCount; ++j)
        {
          // for combo charts we draw a bar every once in a mod.
          if ((mod > 1) && (j % mod) != rem)
            continue;

          // If we use non zero min and it is a stacked graph, we need to remove the min for only
          // the first series.
          if(isStacked)
            stackBase = (j==0?minValue:0);

          rectElem = XamlReader.Load(rectXAML) as Path;
          if(animate)
          {
            dataElems.Add(rectElem);
            
            // FIXTHIS: This is inefficient. However Silverlight currently does not allow sharing transform attribute
            defaultTransform = new MatrixTransform();
            Matrix m = new Matrix();
            m.M22 = 0.0;
            defaultTransform.Matrix = m;
            rectElem.SetValue(UIElement.RenderTransformProperty, defaultTransform); 
          }
          
          barHeight = gridHeight*(yValues[i,j]-stackBase)/(maxValue-minValue);
          if(isStacked)
            dy -= barHeight;
          else
            dy = gridHeight + marginTop - barHeight;
          
          RectangleGeometry rectGeometry = new RectangleGeometry();
          rectGeometry.RadiusX = rectGeometry.RadiusY = 2;
          rectGeometry.Rect = new Rect(dx, dy, barWidth, barHeight);
          
          rectElem.SetValue(Path.DataProperty, rectGeometry);

          if (gradientXAML != null)
          {
            SetGradientOnElement(rectElem, gradientXAML, seriesColors[j], 0xe5);
          }
          else
            SetFillOnElement(rectElem, seriesColors[j]);
          
          rectElem.Stroke = new SolidColorBrush(seriesColors[j]);

          SetExpandosOnElement(rectElem, i, j, new Point(dx+barWidth/2.0, dy));

          if (DisplayToolTip)
          {
            rectElem.MouseEnter += new MouseEventHandler(ShowToolTip);
            rectElem.MouseLeave += new MouseEventHandler(HideToolTip);
          }

          rectElem.MouseLeftButtonUp += new MouseButtonEventHandler(ChartDataClicked);
          ChartCanvas.Children.Add(rectElem);
          
          if(!isStacked)
            dx += barWidth;
        }
        if(isStacked)
          dx += barWidth;
        dx += barItemPadding;
      }  
    }

    private void _drawPerspectiveBars(int mod, int rem)
    {
      bool animate = (this.AnimationDuration>0);
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
      bool isStacked = (Type == ChartType.VBAR_STACKED);
      
      ChartModel model = Model;
      
      string [] groupLabels = model.GroupLabels;
      int groupCount = groupLabels.Length;
      
      string []seriesLabels = model.SeriesLabels;
      int seriesCount = seriesLabels.Length;
      
      Color [] seriesColors = model.SeriesColors;
      double[,] yValues = model.YValues;
      
      double minValue = model.MinYValue, maxValue = model.MaxYValue;
      
      // For combo graphs we display every once in mod
      int seriesBars = (mod > 1) ? (int)Math.Ceiling(seriesCount/(double)mod) : seriesCount;
      double barWidth;

      int yValueCount = yValues.GetUpperBound(0)+1;
      
      if(isStacked)
        barWidth = gridWidth/Math.Max(yValueCount,groupCount)-2*barItemPadding;
      else
        barWidth = (gridWidth/Math.Max(yValueCount,groupCount) -2*barItemPadding - (seriesBars)*barItemPadding)/seriesBars;
      
      double dx = marginLeft, dy, barHeight, stackBase = minValue;
      string gradientXAML = lf.GetElementGradientXAML();
      
      for (int i = 0; i< yValueCount; ++i)
      {
        dx += barItemPadding;
        dy = gridHeight + marginTop+yOffset;
        for (int j = 0; j < seriesCount; ++j)
        {
          // for combo charts we draw a bar every once in a mod.
          if ((mod > 1) && j % mod != rem)
            continue;
            
          // If we use non zero min and it is a stacked graph, we need to remove the min for only
          // the first series.
          if(isStacked)
            stackBase = (j==0?minValue:0);

          barHeight = gridHeight*(yValues[i,j]-stackBase)/(maxValue-minValue);
          if(isStacked)
            dy -= barHeight;
          else
            dy = gridHeight + yOffset + marginTop - barHeight;

          

          StringBuilder sb = new StringBuilder();
          Get3DBarPathData(sb, dx, dy, xOffset, yOffset, barWidth, barHeight);
          pathElem = CreatePathFromXAMLAndData(lf.GetBarPathXAML(), sb);

          if (animate)
          {
            dataElems.Add(pathElem);
            defaultTransform = new MatrixTransform();
            Matrix m = new Matrix();
            m.M22 = 0.0;
            defaultTransform.Matrix = m;
            pathElem.SetValue(UIElement.RenderTransformProperty, defaultTransform);
          }

          pathElem.SetValue(Path.StrokeProperty, new SolidColorBrush(seriesColors[j]));
          
          if(gradientXAML!= null)
          {
            SetGradientOnElement(pathElem, gradientXAML, seriesColors[j], 0xe5);
          }
          else
          {
            SetFillOnElement(pathElem, seriesColors[j]);
          }

          (pathElem.Data as PathGeometry).FillRule = FillRule.Nonzero;
          
          SetExpandosOnElement(pathElem, i, j, new Point(dx+(xOffset+barWidth)/2.0, dy-yOffset/2.0));
          
          if(DisplayToolTip)
          {
            pathElem.MouseEnter += new MouseEventHandler(ShowToolTip);
            pathElem.MouseLeave += new MouseEventHandler(HideToolTip);      
          }
          
          pathElem.MouseLeftButtonUp += new MouseButtonEventHandler(ChartDataClicked); 

          ChartCanvas.Children.Add(pathElem);
          if(!isStacked)
          {
            dx += barWidth;
            dx += barItemPadding;
          }
        }
        
        if(isStacked)
           dx += barWidth;
        dx += barItemPadding;
      }
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
    protected virtual void Get3DBarPathData(
      StringBuilder sb,
      double dx,
      double dy,
      double xOffset,
      double yOffset,
      double barWidth,
      double barHeight
      )
    {
      sb.Append("M").Append(dx).Append(",").Append(dy);
      sb.Append(" l").Append(xOffset).Append(",").Append(-yOffset);
      sb.Append(" h").Append(barWidth);
      sb.Append(" l").Append(-xOffset).Append(",").Append(yOffset);
      sb.Append(" h").Append(-barWidth);
      sb.Append(" v").Append(barHeight);
      sb.Append(" h").Append(barWidth);
      sb.Append(" v").Append(-barHeight);
      sb.Append(" h").Append(-barWidth);
      sb.Append(" m").Append(barWidth).Append(",0");
      sb.Append(" l").Append(xOffset).Append(",").Append(-yOffset);
      sb.Append(" v").Append(barHeight);
      sb.Append(" l").Append(-xOffset).Append(",").Append(yOffset);      
    }
    
    public override void SetDataAnimStep(double ratio)
    {
      base.SetDataAnimStep(ratio);
      
      if(_delegates != null)
      {
        for (int i = 0; i < _delegates.Count; i++)
        {
          _delegates[i].SetDataAnimStep(ratio);
        }
      }
    }
    
    List<Chart> _delegates;
    private int _BARITEM_PADDING = 2;    
  }
}
