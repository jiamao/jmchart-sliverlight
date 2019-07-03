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
  public class ScatterPlotChart : Chart
  {
    public ScatterPlotChart(ChartType type, ChartModel model) : base(type, model)
    {
    }

    public override void DrawChartData()
    {
      DrawChartData(1, 0);
    }

    public override void DrawChartData(int mod, int rem)
    {
      if (IsPerspective)
        _drawPerspectivePoints();
      else
        _drawPoints();
    }
    
    public override void SetDataAnimStep(double ratio)
    {
      if(AnimateAlongXAxis())
      {
        base.SetDataAnimStep(ratio);
        return;
      }
       
      bool isPerspective = IsPerspective;
      double gridCx, gridCy;
      
      double marginLeft = MarginLeft, marginTop = MarginTop;
      double gridWidth = (ChartCanvas.Width - marginLeft - MarginRight);
      double gridHeight = (ChartCanvas.Height - marginTop - MarginBottom);
      List<UIElement> animElems = DataElements;
      int animCount = _cxs.Length, elemIndex = 0;
      
      if(isPerspective)
        marginLeft += this.XOffsetPerspective;
      
      gridCx = gridWidth/2 + marginLeft;
      gridCy = gridHeight/2 + marginTop;
      
      // we are going to animate by starting the dot at the middle and work towards its destination
      for(int i = 0; i < animCount; ++i)
      {
        double cx = gridCx - (gridCx - _cxs[i])*ratio;
        double cy = gridCy - (gridCy - _cys[i])*ratio;
        
        Path elem = animElems[elemIndex++] as Path;
        EllipseGeometry eg = elem.Data as EllipseGeometry;
        eg.Center = new Point(cx, cy);
        
        if(isPerspective)
        {
          elem = animElems[elemIndex++] as Path;
          eg = elem.Data as EllipseGeometry;
          eg.Center = new Point(cx, cy);
        }
      }
    }

    protected override void SetGridAnimStep(double ratio)
    {
      List<UIElement> animElems = GridElements;
      int animCount = animElems.Count;
      
      for(int i = 0; i < animCount; ++i)
      {
        animElems[i].Opacity = ratio;
        MatrixTransform mt = animElems[i].GetValue(UIElement.RenderTransformProperty) as MatrixTransform;
        mt.Matrix = Matrix.Identity;
      }
    }

    /// <summary>
    /// Overridden to indicate that the group label is edge aligned instead of center aligned
    /// </summary>    
    protected override bool IsGroupLabelCentered()
    {
      return false;
    }

    /// <summary>
    /// Overridden to return the x value
    /// </summary>
    /// <param name="index"></param>
    /// <returns>string representing the label text</returns>
    protected override string GetGroupLabelAtIndex(int index)
    {
      if(index == 0)
        return Model.MinXValue.ToString(Format);
      
      int vLineCount = GetVLineCount();
      if(index == vLineCount)
        return Model.MaxXValue.ToString(Format);
        
      double minValue = Model.MinXValue, maxValue = Model.MaxXValue;

      double value = ((maxValue - minValue) * (index) / (double)vLineCount) + minValue;
      return value.ToString(Format);
    }
    
    protected override int GetVLineCount()
    {
      var xMajorCount = XMajorGridCount;
      if(xMajorCount >= 0)
        return xMajorCount;
      else
      {
        // Area Chart has one vertical line less since the 
        // first line represents a value
        return Model.GroupLabels.Length-1;
      }  
    }

    private void _drawPoints()
    {
      bool animate = (this.AnimationDuration>0);
      
      double marginLeft = MarginLeft, marginTop = MarginTop;
      double marginRight = MarginRight, marginBottom = MarginBottom;
      
      double gridWidth = (ChartCanvas.Width - marginLeft - marginRight);
      double gridHeight = (ChartCanvas.Height - marginTop - marginBottom);
      LookAndFeel lf = CurrentLookAndFeel;
      string dotXAML = lf.GetScatterDotXAML();
      Path dotElem;
      List<UIElement> dataElems = null;

      if (animate)
      {
        dataElems = DataElements;
      }
              
      ChartModel model = Model;
      
      string [] groupLabels = model.GroupLabels;
      int groupCount = groupLabels.Length;
      
      string []seriesLabels = model.SeriesLabels;
      int seriesCount = seriesLabels.Length;
      
      Color [] seriesColors = model.SeriesColors;
      
      double[,] yValues = model.YValues;      
      double minYValue = model.MinYValue, maxYValue = model.MaxYValue;
      int nValues = yValues.GetUpperBound(0) + 1;
      double[,] xValues = model.XValues;
      double minXValue = model.MinXValue, maxXValue = model.MaxXValue;
          
      double barWidth = (gridWidth/(double)(groupCount-1));
      double dx, dy, gridCx=0, gridCy=0;
      
      string gradientXAML = lf.GetElementGradientXAML();
      
      if(animate)
      {
        _cxs = new double[seriesCount*nValues];
        _cys = new double[seriesCount * nValues];
        gridCx = gridWidth/2 + marginLeft;
        gridCy = gridHeight/2 + marginTop;
      }
            
      for (int i = 0; i< seriesCount; ++i)
      {
        for (int j = 0; j < nValues; ++j)
        {
          dy = gridHeight + marginTop - gridHeight*(yValues[j,i]-minYValue)/(maxYValue-minYValue);
          dx = marginLeft + gridWidth*(xValues[j,i]-minXValue)/(maxXValue-minXValue);

          dotElem = XamlReader.Load(dotXAML) as Path;

          EllipseGeometry eg = dotElem.Data as EllipseGeometry;
          
          if (gradientXAML != null)
          {
            SetGradientOnElement(dotElem, gradientXAML, seriesColors[i], 0xe5);
          }
          else
          {
            SetFillOnElement(dotElem, seriesColors[i]);
          }
          dotElem.SetValue(Path.StrokeProperty, new SolidColorBrush(seriesColors[i]));

          SetExpandosOnElement(dotElem, j, i, new Point(dx, dy));
          
          if(DisplayToolTip)
          {
            dotElem.MouseEnter += new MouseEventHandler(ShowToolTip);
            dotElem.MouseLeave += new MouseEventHandler(HideToolTip);
          }

          dotElem.MouseLeftButtonUp += new MouseButtonEventHandler(ChartDataClicked);    

          if (animate)
          {
            dataElems.Add(dotElem);
            eg.Center = new Point(gridCx, gridCy);

            // we will use it during animation
            int cIndex = (i * nValues + j);
            _cxs[cIndex] = dx;
            _cys[cIndex] = dy;
          }
          else
          {
            eg.Center = new Point(dx, dy);
          }
          ChartCanvas.Children.Add(dotElem);
        }    
      }
    }

    private void _drawPerspectivePoints()
    {
      bool animate = (this.AnimationDuration>0);
      double xOffset = XOffsetPerspective, yOffset = YOffsetPerspective;
      
      double marginLeft = MarginLeft, marginTop = MarginTop;
      double marginRight = MarginRight, marginBottom = MarginBottom;
      
      double gridWidth = (ChartCanvas.Width - marginLeft - marginRight - xOffset);
      double gridHeight = (ChartCanvas.Height - marginTop - marginBottom - yOffset);
      LookAndFeel lf = CurrentLookAndFeel;
      string dotXAML = lf.GetScatterDot3DXAML();
      Path dotElem;
      List<UIElement> dataElems = null;

      if (animate)
      {
        dataElems = DataElements;
      }
              
      ChartModel model = Model;
      
      string [] groupLabels = model.GroupLabels;
      int groupCount = groupLabels.Length;
      
      string []seriesLabels = model.SeriesLabels;
      int seriesCount = seriesLabels.Length;
      
      Color [] seriesColors = model.SeriesColors;
      
      double[,] yValues = model.YValues;      
      double minYValue = model.MinYValue, maxYValue = model.MaxYValue;
      int nValues = yValues.GetUpperBound(0) + 1;
      double[,] xValues = model.XValues;
      double minXValue = model.MinXValue, maxXValue = model.MaxXValue;
          
      double barWidth = (gridWidth/(double)(groupCount-1));
      double gridBottom = gridHeight + marginTop + yOffset;
      double dx, dy, gridCx=0, gridCy=0;
      
      string gradientXAML = lf.GetElementGradientXAML();
      
      if(animate)
      {
        _cxs = new double[seriesCount*nValues];
        _cys = new double[seriesCount * nValues];
        gridCx = gridWidth/2 + marginLeft + xOffset;
        gridCy = gridHeight/2 + marginTop;
      }
      
      for (int i = 0; i< seriesCount; ++i)
      {
        for (int j = 0; j < nValues; ++j)
        { 
          dy = gridBottom - gridHeight*(yValues[j,i]-minYValue)/(maxYValue-minYValue);
          dx = marginLeft + gridWidth*(xValues[j,i]-minXValue)/(maxXValue-minXValue);
          
          dotElem = XamlReader.Load(dotXAML) as Path;
          
          EllipseGeometry eg = dotElem.Data as EllipseGeometry;
          
          if(animate)
          {
            dataElems.Add(dotElem);
            eg.Center = new Point(gridCx, gridCy);
            
            // we will use it during animation
            int cIndex = (i*nValues +j);
            _cxs[cIndex] = dx;
            _cys[cIndex] = dy;
          }
          else
          {
            eg.Center = new Point(dx, dy);
          }

          if (gradientXAML != null)
          {
            SetGradientOnElement(dotElem, gradientXAML, seriesColors[i], 0xe5);
          }
          else
          {
            SetFillOnElement(dotElem, seriesColors[i]);
          }
          dotElem.SetValue(Path.StrokeProperty, new SolidColorBrush(seriesColors[i]));

          SetExpandosOnElement(dotElem, j, i, new Point(dx, dy));

          if (DisplayToolTip)
          {
            dotElem.MouseEnter += new MouseEventHandler(ShowToolTip);
            dotElem.MouseLeave += new MouseEventHandler(HideToolTip);
          }

          dotElem.MouseLeftButtonUp += new MouseButtonEventHandler(ChartDataClicked);
          
          //TODO:
          // Silverlight does not support Filter effects so use another do as a shadow for it.
          
          var shadowElem = XamlReader.Load(dotXAML) as Path;
          eg = shadowElem.Data as EllipseGeometry;
          if(animate)
          {
            dataElems.Add(shadowElem);
            eg.Center = new Point(gridCx, gridCy);
          }
          else
          {
            eg.Center = new Point(dx, dy);
          }
          
          shadowElem.Fill = new SolidColorBrush(Color.FromArgb(0x7f, 0x33, 0x33, 0x33));
          shadowElem.SetValue(Path.StrokeThicknessProperty, 0.0);
          
          TranslateTransform tt = new TranslateTransform();
          tt.X = tt.Y = 3;
          shadowElem.SetValue(UIElement.RenderTransformProperty, tt);
          
          ChartCanvas.Children.Add(shadowElem);
          ChartCanvas.Children.Add(dotElem);
        }
      }
    }
    
    protected override ChartEventArgs GetChartEvent(object sender, MouseEventArgs e)
    {
      ElementExpandos ee =  Expandos[sender];
      int i = ee.YValueIndex; 
      int j = ee.SeriesIndex;
      
      int[] seriesIndices = {j};
      int[] yValueIndices = {i};
      double[] yValues = {Model.YValues[i,j]};
      double[] xValues = {Model.XValues[i,j]};
            
      return new ChartEventArgs(seriesIndices, yValueIndices, yValues, xValues);
    }

    protected override Size FillToolTipData(
      object sender,
      MouseEventArgs e,
      Path boundingRectElem,
      Path circleElem)
    {
      ChartEventArgs chartEvent = GetChartEvent(sender, e);
      
      int i = chartEvent.SeriesIndices[0];
      
      double yValue = chartEvent.YValues[0],
             xValue = chartEvent.XValues[0];
      
      ChartModel model = Model;
      string[] seriesLabels = model.SeriesLabels;
          
      TextBlock textElem = ToolTip.Children[2] as TextBlock;
      
      textElem.Text = seriesLabels[i]+ ": ("+ xValue.ToString(Format) + ")    (" + yValue.ToString() +")";
                    
      double labelWidth = textElem.ActualWidth;
      double labelHeight = textElem.ActualHeight;
      
      //We do not need the next label 
      textElem = ToolTip.Children[3] as TextBlock;
      
      textElem.Text = "";
      
      double rectWidth = labelWidth+2*_TEXT_MARGIN;

      RectangleGeometry rg = boundingRectElem.Data as RectangleGeometry;
      Rect rect = rg.Rect;
      double rectHeight = rect.Height;
      
      if (rectHeight > 2 * labelHeight)
        rectHeight -= labelHeight;
      rg.Rect = new Rect(rect.X, rect.Y, rectWidth, rectHeight);
      boundingRectElem.SetValue(Path.StrokeProperty, new SolidColorBrush(Model.SeriesColors[i]));

      EllipseGeometry eg = circleElem.Data as EllipseGeometry;
      eg.RadiusX = eg.RadiusY = 0;
      return new Size(rectWidth, 2 * (labelHeight + _TEXT_MARGIN));      
    }

    protected override Point GetToolTipLocation(object sender, MouseEventArgs e, Size ttBounds)
    {
      Point pt = e.GetPosition(ChartCanvas);
      pt = new Point(pt.X + 16, pt.Y + 16);
      return pt;
    }
    
    double[] _cxs, _cys;
  }
}
