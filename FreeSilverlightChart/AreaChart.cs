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
  public class AreaChart : Chart
  {
    public AreaChart(ChartType type, ChartModel model) : base(type, model)
    {
    }

    public override void clear()
    {
      base.clear();
      _tooltips = null;
    }

    public override void DrawChartData()
    {
      DrawChartData(1, 0);
    }

    public override void DrawChartData(int mod, int rem)
    {
      ChartType type = Type;
      
      _mod = mod;
      _rem = rem;
      
      if(IsPerspective)
        this._drawPerspectiveAreas(mod, rem);
      else
        this._drawAreas(mod, rem);    
    }

    protected override bool AnimateAlongXAxis()
    {
      // horizontal bar animates around x axis
      return true;
    }

    public override void SetDataAnimStep(double ratio)
    {
      if(Type != ChartType.AREA_STACKED)
      {
        base.SetDataAnimStep(ratio);
        return;
      }
      
      List<UIElement> animElems = DataElements;      
            
      // make sure that everything is scaled properly at the end
      if(ratio == 1)
      {
        for (var i = 0; i < animElems.Count; ++i)
        {
          MatrixTransform mt = animElems[i].GetValue(UIElement.RenderTransformProperty) as MatrixTransform;
          mt.Matrix = Matrix.Identity;
        }
      }
      else
      {
        int seriesCount = Model.SeriesLabels.Length;
        double newRatio = ratio * seriesCount;
        int animSeriesIndex = 0;
      
        if(newRatio > 1)
        {
          animSeriesIndex = (int)Math.Floor(newRatio);
          if(animSeriesIndex >= seriesCount)
            animSeriesIndex = seriesCount - 1;
          newRatio = newRatio - Math.Floor(newRatio);
        }

        // We will make each series appear separately
        double tx = (1-newRatio)*MarginLeft;
        MatrixTransform mt = animElems[animSeriesIndex].GetValue(UIElement.RenderTransformProperty) as MatrixTransform;
        Matrix m = mt.Matrix;
        m.OffsetX = tx;
        m.M11 = newRatio;
        mt.Matrix = m;
                    
        if(animSeriesIndex>0)
        {
          mt = animElems[animSeriesIndex-1].GetValue(UIElement.RenderTransformProperty) as MatrixTransform;
          mt.Matrix = Matrix.Identity;
        }
      
      }
    }


    private void _drawAreas(int mod, int rem)
    {
      bool animate = (this.AnimationDuration>0);
      
      double marginLeft = MarginLeft, marginTop = MarginTop;
      double marginRight = MarginRight, marginBottom = MarginBottom;
      
      double gridWidth = (ChartCanvas.Width - marginLeft - marginRight);
      double gridHeight = (ChartCanvas.Height - marginTop - marginBottom);
      LookAndFeel lf = CurrentLookAndFeel;
      string pathXAML = lf.GetAreaPathXAML();
      Path pathElem;
      List<UIElement> dataElems = null;
      MatrixTransform defaultTransform = null;

      if (animate)
      {
        dataElems = DataElements;
      }
              
      bool isStacked = (Type == ChartType.AREA_STACKED);
      ChartModel model = Model;
      
      string [] groupLabels = model.GroupLabels;
      int groupCount = groupLabels.Length;
      
      string []seriesLabels = model.SeriesLabels;
      int seriesCount = seriesLabels.Length;
      
      Color [] seriesColors = model.SeriesColors;
      double[,] yValues = model.YValues;
      
      double minValue = model.MinYValue, maxValue = model.MaxYValue;
      
      int yValueCount = yValues.GetUpperBound(0)+1;
          
      double barWidth = (gridWidth/(Math.Max(yValueCount,groupCount))), stackBase;
      double dx, dy;
      double[] cumYs = null;

      if (isStacked)
      {
        cumYs = new double[yValueCount];
        for (int j = 0; j < yValueCount; ++j)
          cumYs[j] = double.NaN;
      }

      string gradientXAML = lf.GetElementGradientXAML();
            
      for (int i = 0; i< seriesCount; ++i)
      {
        // for combo charts we draw a bar every once in a mod.
        if ((mod > 1) && (i % mod) != rem)
          continue;
          
        dx = marginLeft+barWidth/2;
        dy = marginTop + gridHeight;

        StringBuilder sb = new StringBuilder();
        
        if(i == 0 || !isStacked)
          sb.Append("M").Append(dx).Append(",").Append(dy);
        else if(isStacked)
          sb.Append("M").Append(dx).Append(",").Append(cumYs[0]);
        
        // If we use non zero min and it is a stacked graph, we need to remove the min for only
        // the first series.
        stackBase = (i==0?minValue:0);
        
        for (int j = 0; j < yValueCount; ++j)
        {
          if(isStacked)
          {
            if (double.IsNaN(cumYs[j]))
              cumYs[j] = gridHeight + marginTop;              
            dy = (cumYs[j] -= gridHeight*(yValues[j,i]-stackBase)/(maxValue-minValue));
          }
          else
            dy = gridHeight + marginTop - gridHeight*(yValues[j,i]- minValue)/(maxValue-minValue);
          
          sb.Append(" L").Append(dx).Append(",").Append(dy);    

          if(j != yValueCount - 1)      
            dx += barWidth;
        }

                  
        if(i == 0 || !isStacked)
        {
          sb.Append(" L").Append(dx).Append(",").Append(gridHeight + marginTop);
          sb.Append(" Z");
        }
        else
        {
          for (int j = yValueCount-1; j>=0; --j)
          {
            var prevY = cumYs[j]+gridHeight*(yValues[j,i]-stackBase)/(maxValue-minValue);
            sb.Append(" L").Append(dx).Append(",").Append(prevY);
            dx -= barWidth;
          }
        }

        pathElem = CreatePathFromXAMLAndData(pathXAML, sb);
        SetExpandosOnElement(pathElem, -1, i, new Point());

        if (DisplayToolTip)
        {
          pathElem.MouseMove += new MouseEventHandler(ShowToolTip);
          pathElem.MouseLeave += new MouseEventHandler(HideToolTip);
        }

        pathElem.MouseLeftButtonUp += new MouseButtonEventHandler(ChartDataClicked); 

        if (gradientXAML != null)
        {
          SetGradientOnElement(pathElem, gradientXAML, seriesColors[i], 0x7F);
        }
        else
        {
          SetFillOnElement(pathElem, seriesColors[i]);
        }
        pathElem.SetValue(Path.StrokeProperty, new SolidColorBrush(seriesColors[i]));

        (pathElem.Data as PathGeometry).FillRule = FillRule.Nonzero;

        if (animate)
        {
          dataElems.Add(pathElem);
          defaultTransform = new MatrixTransform();
          Matrix m = new Matrix();
          m.M11 = 0.0;
          defaultTransform.Matrix = m;
          pathElem.SetValue(UIElement.RenderTransformProperty, defaultTransform);
        }
        ChartCanvas.Children.Add(pathElem);
      }
    }

    private void _drawPerspectiveAreas(int mod, int rem)
    {
      bool animate = (this.AnimationDuration>0);
      double xOffset = XOffsetPerspective, yOffset = YOffsetPerspective;
      
      double marginLeft = MarginLeft, marginTop = MarginTop;
      double marginRight = MarginRight, marginBottom = MarginBottom;
      
      double gridWidth = (ChartCanvas.Width - marginLeft - marginRight - xOffset);
      double gridHeight = (ChartCanvas.Height - marginTop - marginBottom - yOffset);
      LookAndFeel lf = CurrentLookAndFeel;
      string pathXAML = lf.GetAreaPathXAML();
      Path pathElem;
      List<UIElement> dataElems = null;
      MatrixTransform defaultTransform = null;

      if (animate)
      {
        dataElems = DataElements;
      }
              
      bool isStacked = (Type == ChartType.AREA_STACKED);
      ChartModel model = Model;
      
      string [] groupLabels = model.GroupLabels;
      int groupCount = groupLabels.Length;
      
      string []seriesLabels = model.SeriesLabels;
      int seriesCount = seriesLabels.Length;
      
      Color [] seriesColors = model.SeriesColors;
      double[,] yValues = model.YValues;
      
      double minValue = model.MinYValue, maxValue = model.MaxYValue;
      
      int yValueCount = yValues.GetUpperBound(0)+1;
          
      double barWidth = (gridWidth/(Math.Max(yValueCount,groupCount))), stackBase;
      double barHeight;
      double gridBottom = gridHeight + marginTop + yOffset, dx, dy;
      double[] cumYs = isStacked? new double[yValueCount] : null;
      
      if(isStacked)
      {
        cumYs = new double[yValueCount];
        for(int j = 0; j < yValueCount; ++j)
          cumYs[j] = double.NaN;
      }
              
      string gradientXAML = lf.GetElementGradientXAML();
      
      for (int i = 0; i< seriesCount; ++i)
      {
        // for combo charts we draw a bar every once in a mod.
        if ((mod > 1) && (i % mod) != rem)
          continue;
            
        dx = marginLeft+barWidth/2.0;
        
        // If we use non zero min and it is a stacked graph, we need to remove the min for only
        // the first series.
        stackBase = (i==0?minValue:0);
        
        StringBuilder sb = new StringBuilder();
        
        for (int j = 0; j < yValueCount; ++j)
        {            
          barHeight = gridHeight*(yValues[j,i]-stackBase)/(maxValue-minValue);
          if(isStacked)
          {
            if(double.IsNaN(cumYs[j]))
              cumYs[j] = gridBottom;
              
            dy = (cumYs[j] -= barHeight);
          }
          else
            dy = gridBottom - barHeight;

          if (j == yValueCount - 1)
            break;

          sb.Append("M").Append(dx).Append(",").Append(dy);
          sb.Append(" l").Append(xOffset).Append(",").Append(-yOffset);
          
          if(i==0 || !isStacked)
            sb.Append(" L").Append(dx + xOffset).Append(",").Append(gridHeight + marginTop);
          else
            sb.Append(" v").Append(barHeight);
          
          sb.Append(" l").Append(-xOffset).Append(",").Append(yOffset);
          sb.Append(" z");
          
          sb.Append("M").Append(dx).Append(",").Append(dy);
          sb.Append(" l").Append(xOffset).Append(",").Append(-yOffset);
          
          double nextdy, nextdx = dx+barWidth;
          
          if(isStacked)
          {
            if(double.IsNaN(cumYs[j+1]))
              cumYs[j+1] = gridBottom;
              
            nextdy = (cumYs[j+1] - gridHeight*(yValues[j+1,i]-stackBase)/(maxValue-minValue));
          }
          else
            nextdy = gridBottom - gridHeight*(yValues[j+1,i]-minValue)/(maxValue-minValue);
          
          sb.Append(" L").Append(nextdx+xOffset).Append(",").Append(nextdy-yOffset);
          sb.Append(" l").Append(-xOffset).Append(",").Append(yOffset);
          sb.Append(" L").Append(dx).Append(",").Append(dy);
          sb.Append(" M").Append(nextdx).Append(",").Append(nextdy);
          sb.Append(" l").Append(xOffset).Append(",").Append(-yOffset);
          
          if(i == 0 || !isStacked)
          {
            sb.Append(" L").Append(nextdx+xOffset).Append(",").Append(gridHeight + marginTop);  
          }
          else
          {
            sb.Append(" L").Append(nextdx+xOffset).Append(",").Append(cumYs[j+1]-yOffset);
          }
          
          sb.Append(" l").Append(-xOffset).Append(",").Append(yOffset);
          sb.Append(" L").Append(nextdx).Append(",").Append(nextdy);
          
          sb.Append(" M").Append(dx).Append(",").Append(dy);
          sb.Append(" L").Append(nextdx).Append(",").Append(nextdy);
          
          if(i == 0 || !isStacked)
          {
            sb.Append(" L").Append(nextdx).Append(",").Append(gridBottom);
            sb.Append(" L").Append(dx).Append(",").Append(gridBottom);
          }
          else
          {
            sb.Append(" L").Append(nextdx).Append(",").Append(cumYs[j+1]);
            sb.Append(" L").Append(dx).Append(",").Append(
              cumYs[j]+gridHeight*(yValues[j,i]-stackBase)/(maxValue-minValue));
          }
          
          sb.Append(" L").Append(dx).Append(",").Append(dy);
          
          dx += barWidth;
        }
        
        pathElem =  CreatePathFromXAMLAndData(pathXAML, sb);
        
        SetExpandosOnElement(pathElem, -1, i, new Point());

        if(DisplayToolTip)
        {
          pathElem.MouseMove += new MouseEventHandler(ShowToolTip);
          pathElem.MouseLeave += new MouseEventHandler(HideToolTip);
        }
        pathElem.MouseLeftButtonUp += new MouseButtonEventHandler(ChartDataClicked); 
         
        if (gradientXAML != null)
        {
          SetGradientOnElement(pathElem, gradientXAML, seriesColors[i], 0x7F);
        }
        else
        {
          SetFillOnElement(pathElem, seriesColors[i]);
        }
        pathElem.SetValue(Path.StrokeProperty, new SolidColorBrush(seriesColors[i]));
        (pathElem.Data as PathGeometry).FillRule = FillRule.Nonzero;

        if (animate)
        {
          dataElems.Add(pathElem);
          defaultTransform = new MatrixTransform();
          Matrix m = new Matrix();
          m.M11 = 0.0;
          defaultTransform.Matrix = m;
          pathElem.SetValue(UIElement.RenderTransformProperty, defaultTransform);
        }
        ChartCanvas.Children.Add(pathElem);
        
      }
    }
    
    protected override ChartEventArgs GetChartEvent(object sender, MouseEventArgs e)
    {
      Point pt = e.GetPosition(ChartCanvas);
      
      bool isStacked = (Type == ChartType.AREA_STACKED);
      ChartModel model = Model;
      bool isPerspective = IsPerspective;
      
      double xOffset = XOffsetPerspective, yOffset = YOffsetPerspective;
      string [] groupLabels = model.GroupLabels;
      int groupCount = groupLabels.Length;
      
      double[,] yValues = model.YValues;      
      double minValue = model.MinYValue, maxValue = model.MaxYValue;
      int yValueCount = yValues.GetUpperBound(0)+1;
                
      double marginLeft = MarginLeft, marginTop = MarginTop; 
      double gridWidth = (ChartCanvas.Width - marginLeft - MarginRight);
      double gridHeight = (ChartCanvas.Height - marginTop - MarginBottom);
      double barWidth = (gridWidth / (Math.Max(yValueCount, groupCount)));

      if(isPerspective)
      {
        gridWidth -= xOffset;
        gridHeight -= yOffset;
      }
      
      if(pt.X < marginLeft || 
         pt.X>(marginLeft + gridWidth + (isPerspective?xOffset:0)) ||
         pt.Y < marginTop || 
         pt.Y>(marginTop + gridHeight + (isPerspective?yOffset:0)))
      {
        return null;
      }

      int seriesCount = model.SeriesLabels.Length;
      double stackBase;
      bool insideStacked = false;
      double dx, dy, dy1, dy2, value;
      List<int> seriesIndices = new List<int>(seriesCount);
      List<double> seriesValues = new List<double>(seriesCount);
      
      double[] cumYs = isStacked? new double[yValueCount] : null;
      
      if(isStacked)
      {
        cumYs = new double[yValueCount];
        for(int j = 0; j < yValueCount; ++j)
          cumYs[j] = double.NaN;
      }
      
      double gridBottom = gridHeight + marginTop +(isPerspective?yOffset:0);

      _seriesYs = new List<double>(seriesCount);
      
      for (int i = 0; i< seriesCount && !insideStacked; ++i)
      {
        // for combo charts we draw a bar every once in a mod.
        if ((_mod > 1) && (i % _mod) != _rem)
          continue;
          
        dx = marginLeft + barWidth/2;
        
        stackBase = (i==0?minValue:0);      
        
        for (int j = 0; j < yValueCount; ++j)
        {
          if(isStacked)
          {
            if(double.IsNaN(cumYs[j]))
              cumYs[j] = gridBottom;

            if ((j != yValueCount - 1) && double.IsNaN(cumYs[j + 1]))
              cumYs[j+1] = gridBottom;
              
            cumYs[j] -= gridHeight*(yValues[j,i]-stackBase)/(maxValue-minValue);
          }
          
          if(j == yValueCount - 1)
            continue;

          if (pt.X > dx && pt.X < (dx + barWidth))
          {
            if(isStacked)
            {
              dy1 = cumYs[j];
              dy2 = (cumYs[j+1] - gridHeight*(yValues[j+1,i]-stackBase)/(maxValue-minValue));
              dy = dy1 - (dy1 - dy2) * (pt.X - dx) / barWidth;
              
              if(pt.Y >= dy)
              {
                value = yValues[j,i] + (yValues[j + 1, i] - yValues[j, i]) * (pt.X - dx) / barWidth;
                seriesValues.Add(value);
                seriesIndices.Add(i);
                _seriesYs.Add(dy);
                insideStacked = true;
                break;
              }
            }
            else 
            {
              dy1 = gridBottom - 
                    gridHeight*(yValues[j,i]-minValue)/(maxValue-minValue);
              
              dy = dy1 - (gridHeight*(yValues[j+1,i]-yValues[j,i])/(maxValue-minValue))*(pt.X-dx)/barWidth;

              // find all the series that the y point matches
              if (dy <= pt.Y)
              {
                value = yValues[j,i] + (yValues[j+1,i]-yValues[j,i])*(pt.X - dx)/barWidth;
                seriesValues.Add(value);
                seriesIndices.Add(i);
                _seriesYs.Add(dy);
              }
              break;
            }
          }
          dx += barWidth; 
        }
      }
      return new ChartEventArgs(seriesIndices.ToArray(),null, seriesValues.ToArray(),null);
    }

    protected override void ShowToolTip(object sender, MouseEventArgs e)
    {  
      
      // Hide any existing tooltips
      HideToolTip(sender, e);
      
      ChartEventArgs chartEvent = this.GetChartEvent(sender, e);
      
      if(chartEvent == null || chartEvent.YValues.Length == 0)
      {
        return;
      }
      
      this._displayToolTips(sender, e, chartEvent.YValues, chartEvent.SeriesIndices);
    }

    private void _displayToolTips(
      object sender, 
      MouseEventArgs e,
      double[] seriesValues, 
      int []  seriesIndices
      )
    {
      ChartModel model = Model;
      string[] seriesLabels = model.SeriesLabels;
      int seriesCount = seriesLabels.Length;
      
      Color[] seriesColors = model.SeriesColors;

      int tooltipCount = seriesIndices.Length;;
      Point pt = e.GetPosition(ChartCanvas);
      double dx, dy;

      if(_tooltips == null)
        _tooltips = new Canvas[seriesCount];
        
      for(int i = 0; i<tooltipCount; ++i)
      { 
        int seriesIndex = seriesIndices[i];
        
        Canvas toolTip = _tooltips[seriesIndex];
        
        bool resizeOnInit = false;
        
        if (toolTip == null)
        {
          toolTip = XamlReader.Load(CurrentLookAndFeel.GetTooltipXAML()) as Canvas;
          ChartCanvas.Children.Add(toolTip);
          _tooltips[seriesIndex] = toolTip;
          resizeOnInit = true;
        }
        
        toolTip.Visibility = Visibility.Visible;

        Path circleElem = toolTip.Children[0] as Path;
        Path boundingRectElem = toolTip.Children[1] as Path;
        
        //append all the series values as labels
        TextBlock textElem = toolTip.Children[2] as TextBlock;
        
        int textElemCount = seriesValues.Length;
        textElem.Text = seriesLabels[seriesIndex] + ":  " + seriesValues[i].ToString(Format);

        double rectWidth = textElem.ActualWidth;        

        RectangleGeometry rg = boundingRectElem.Data as RectangleGeometry;
        Rect rect = rg.Rect;
        // Initially the template tooltip has an extra text node
        if(resizeOnInit)
        {
          dy = textElem.ActualHeight;
          rg.Rect = new Rect(rect.X, rect.Y, rect.Width, rect.Height-dy);
          textElem = toolTip.Children[3] as TextBlock;
          textElem.Text = "";
        }
        
        rectWidth += 2*_TEXT_MARGIN;
        rect = rg.Rect;
        rg.Rect = new Rect(rect.X, rect.Y, rectWidth, rect.Height);

        EllipseGeometry eg = circleElem.Data as EllipseGeometry;        
        dx = pt.X;
        dy = _seriesYs[i]- rect.Height;

        if(IsPerspective)
        {
          dy -= YOffsetPerspective/2.0;
        }
        
        double cx, cy;
        if (dx + rectWidth > ChartCanvas.Width)
        {
          dx -= rectWidth;
          cx = rectWidth;
        }
        else
        {
          cx = 0;
        }

        if (dy - rect.Height < 0)
        {
          dy += rect.Height;
          cy = 0;
        }
        else
        {
          cy = rect.Height;
        }

        eg.Center = new Point(cx, cy);
        boundingRectElem.SetValue(Path.StrokeProperty, new SolidColorBrush(seriesColors[seriesIndex]));
        circleElem.SetValue(Path.StrokeProperty, new SolidColorBrush(seriesColors[seriesIndex]));

        TranslateTransform tTrans = new TranslateTransform();
        tTrans.X = dx;
        tTrans.Y = dy;
        toolTip.RenderTransform = tTrans;                
      }
    }

    protected virtual void HideToolTip(object sender, MouseEventArgs e)
    {
      int tooltipCount = _tooltips != null ? _tooltips.Length:0;

      for(var i = 0; i<tooltipCount; ++i)
      { 
        Canvas toolTip = _tooltips[i];
        if(toolTip != null)
          toolTip.Visibility = Visibility.Collapsed;
      }
    }
    
    private List<double> _seriesYs;
    private Canvas[] _tooltips;
    private int _mod, _rem;
  }
}
