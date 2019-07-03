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
  public class GaugeChart : Chart
  {
    internal GaugeChart(ChartType type, ChartModel model) : base(type, model)
    {
    }

    /// <summary>
    /// Overridden to do nothing for gauge charts
    /// </summary>
    protected override void ComputeMinMaxValues()
    {

    }

    /// <summary>
    /// Overridden to do nothing for gauge charts
    /// </summary>
    protected override void DrawLegend()
    {
    }
    
    /// <summary>
    /// Overridden to do nothing for gauge charts
    /// </summary>
    protected override void DrawGroupLabels()
    {

    }

    /// <summary>
    /// Overridden to do nothing for gauge charts
    /// </summary>
    protected override void LayoutGroupLabels()
    {

    }

    /// <summary>
    /// Overridden to do nothing for gauge charts
    /// </summary>
    protected override void DrawGrid()
    {

    }

    /// <summary>
    /// Overridden to do nothing for gauge charts
    /// </summary>
    protected override void DrawYValueLabels()
    {

    }

    /// <summary>
    /// Overridden to do nothing for gauge charts
    /// </summary>
    protected override void LayoutYValueLabels()
    {

    }
    
    
    public override void SetDataAnimStep(double ratio)
    {
      List<UIElement> animElems = DataElements;
      int animCount = animElems.Count;
      
      ChartModel model = Model;
      double[,] yValues = Model.YValues;
      
      for(int i = 0; i < animCount; ++i)
      {
        // For Dial Chart only one value is applicable
        double yValue = yValues[i,0];
        
        SetIndicatorPosition(yValue, animElems[i], ratio);
      }
    }

    protected virtual void SetIndicatorPosition(double yValue, UIElement indicator, double ratio)
    {
      double theta, cx = _animCenter.X, cy = _animCenter.Y;
      
      ChartModel model = Model;
      double minValue = model.MinYValue,
             maxValue = model.MaxYValue;
             
      double valueRatio = ratio*(yValue - minValue)/(maxValue-minValue);
        
      theta = Math.PI/6 + valueRatio*(5*Math.PI/3);
      
      if(theta < Math.PI/2)
        theta += 3*Math.PI/2;
      else
        theta -= Math.PI/2;
      theta *= 180/Math.PI;
      
      RotateTransform rt = new RotateTransform();
      
      rt.CenterX = cx;
      rt.CenterY = cy;
      rt.Angle = theta;
      
      indicator.RenderTransform =  rt;
    }

    public override void DrawChartData()    
    {
      if(YMinorGridCount<0)
        YMinorGridCount = 4;
      
      var rootCanvas = ChartCanvas;
        
      // calculate the number of rows and columns
      ChartModel model = Model;
      
      double[,] yValues = model.YValues;
      int yValueCount = yValues.GetUpperBound(0) +1;
      
      string [] groupLabels = model.GroupLabels; 
      int groupCount = (groupLabels!= null)?groupLabels.Length:1;
          
      int nCols = (int)Math.Ceiling(Math.Sqrt(yValueCount)), nRows = (int)Math.Round(Math.Sqrt(yValueCount));
      
      double dx=MarginLeft, dy=MarginTop;
      
      double quadWidth = (rootCanvas.Width - MarginLeft - MarginRight)/nCols;
      double vGap = 2*_TEXT_MARGIN;
      
      double quadHeight = (rootCanvas.Height - MarginTop - MarginBottom - (nRows-1)*vGap)/nRows;
      string labelXAML = CurrentLookAndFeel.GetGroupLabelXAML();
      TextBlock labelElem = null;

      for(int i = 0; i<nRows; ++i)
      {
        for(int j = 0; j<nCols; ++j)
        {  
          int iGroup = groupLabels!=null?(i*nCols + j):(-1);
          if(iGroup >= yValueCount)
            break;
          
          string groupLabel = (iGroup == -1)?null:groupLabels[iGroup];
            
          Canvas gaugeContainer = new Canvas();

          rootCanvas.Children.Add(gaugeContainer);
          
          if(groupLabel != null)
            labelElem = XamlReader.Load(labelXAML) as TextBlock;

          double newHeight = DrawGroupLabelTitle(groupLabel, rootCanvas, labelXAML, ref labelElem, 
                                                 dx, dy, quadWidth, quadHeight);
                                          
          double newWidth = quadWidth - 2*_TEXT_MARGIN;
          
          Size gaugeSize = DrawDial(gaugeContainer, newWidth, newHeight, iGroup);
          
          TranslateTransform tt = new TranslateTransform();
          
          tt.X = (dx+_TEXT_MARGIN);
          tt.Y = dy;
          gaugeContainer.RenderTransform = tt;
                    
          if(groupLabel != null)
          {
            if (gaugeSize.Height < newHeight - vGap)
            {
              var newY = (double)labelElem.GetValue(Canvas.TopProperty);
              newY -= (newHeight-gaugeSize.Height)/2 - vGap;
              labelElem.SetValue(Canvas.TopProperty, newY);
            }
          }
          dx +=quadWidth;
        }
        
        dx=MarginLeft;
        dy +=quadHeight+vGap;
      }
    }

    /// <summary>
    /// Method for base classes to provide XAML for the gauge template
    /// </summary>
    /// <returns></returns>
    protected virtual string GetGaugeXAML()    
    {
      return CurrentLookAndFeel.GetCircularGauageXAML();
    }
    
    /// <summary>
    /// Draws the dial for the gauge and returns the actual size of the gauge
    /// </summary>
    /// <param name="gaugeContainer"></param>
    /// <param name="quadWidth"></param>
    /// <param name="quadHeight"></param>
    /// <param name="iGroup"></param>
    /// <returns></returns>
    protected virtual Size DrawDial(
      Canvas gaugeContainer, 
      double quadWidth, 
      double quadHeight, 
      int iGroup)
    {
      ChartModel model = Model;
      
      if(iGroup == -1)
        iGroup = 0;
      
      List<UIElement> dataElems = DataElements;      
      bool animate = (AnimationDuration>0);
      Canvas gauge = XamlReader.Load(GetGaugeXAML()) as Canvas;
      
      gaugeContainer.Children.Add(gauge);

      double gaugeWidth = gauge.Width, gaugeHeight = gauge.Height;
      double gaugeR = gaugeWidth/2;
                  
      Canvas indicator = gauge.FindName("indicator") as Canvas;

      SetExpandosOnElement(indicator, iGroup, 0, new Point());

      if (DisplayToolTip)
      {
        indicator.MouseEnter += new MouseEventHandler(ShowToolTip);
        indicator.MouseLeave += new MouseEventHandler(HideToolTip);
      }

      indicator.MouseLeftButtonUp += new MouseButtonEventHandler(ChartDataClicked);

      if (_animCenter.X == 0)
      {
        _animCenter.X = _animCenter.Y = indicator.Width/2;
      }
          
      if(animate)
        dataElems.Add(indicator);
      
      else
      {
        // If there is no animation lets move the indicators to the last position
        SetIndicatorPosition(model.YValues[iGroup,0], indicator, 1);
      }
      
      CreateTextMarkerGroup(gauge, gaugeR);
      
      return ScaleGauge(gauge, quadWidth, quadHeight, gaugeWidth, gaugeHeight);
    }


    protected virtual void CreateTextMarkerGroup(Canvas gauge, double gaugeR)
    {
      ChartModel model = Model;
      Canvas gElem = new Canvas();

      gauge.Children.Add(gElem);
      
      LookAndFeel lf = CurrentLookAndFeel;
      
      int majorMarkerCount = YMajorGridCount;
      int minorMarkerCount = YMinorGridCount;
      string majorMarkerXAML = lf.GetGauageMarkerMajorXAML(),
             minorMarkerXAML = lf.GetGauageMarkerMinorXAML();
      string textXAML = lf.GetGauageTextXAML();
      TextBlock textElem;

      Canvas markerContainerCanvas = gauge.FindName("markerContainer") as Canvas;
      
      double markerContainerR = markerContainerCanvas.Width/2;
      double minValue = model.MinYValue, maxValue = model.MaxYValue;
      
      double x, y, angle, textMargin = 0.0;
      
      double theta = Math.PI/6;
      for(int i=0; 
          i<=majorMarkerCount; ++i, theta += (5.0*Math.PI/3.0)/majorMarkerCount)
      {
        double adjustedTheta;
        
        if(theta < Math.PI/2)
          adjustedTheta = theta + 3.0*Math.PI/2.0;
        else
          adjustedTheta = theta - Math.PI/2.0;
          
        x = gaugeR-markerContainerR*(Math.Cos(adjustedTheta));
        y = gaugeR-markerContainerR*(Math.Sin(adjustedTheta));
        
        angle = adjustedTheta*180/Math.PI;
        
        Path marker = XamlReader.Load(majorMarkerXAML) as Path;
        
        TransformGroup tg = new TransformGroup();
        TranslateTransform tt = new TranslateTransform();
        RotateTransform rt = new RotateTransform();
        
        tt.X = x;
        tt.Y = y;
        rt.Angle = angle;
        tg.Children.Add(rt);
        tg.Children.Add(tt);
        marker.RenderTransform = tg;
        gElem.Children.Add(marker);
                
        double value = minValue + i*(maxValue-minValue)/(majorMarkerCount);
        textElem = XamlReader.Load(textXAML) as TextBlock;
        textElem.Text = value.ToString(Format);
        
        if(i == 0)
        {
          textMargin = textElem.ActualHeight/2;
        }
        
        x = gaugeR-(markerContainerR-textMargin)*(Math.Cos(adjustedTheta));
        y = gaugeR-(markerContainerR-textMargin)*(Math.Sin(adjustedTheta));

        if(theta >= 5*Math.PI/6 && theta <= 7*Math.PI/6)
        {
          x -= textElem.ActualWidth/2;
        }
        else
        {
          y -= textElem.ActualHeight/2;
          
          if(theta < Math.PI)
            x += 2*_TEXT_MARGIN;
          else
            x -= textElem.ActualWidth + 2*_TEXT_MARGIN;
        }

        tt = new TranslateTransform();
        tt.X = x;
        tt.Y = y;
        
        textElem.RenderTransform = tt;
        gElem.Children.Add(textElem);
      }

      theta = Math.PI/6+(5*Math.PI/3)/(majorMarkerCount*minorMarkerCount);
      for(int i=(minorMarkerCount+1); 
          i<=(majorMarkerCount+1)*minorMarkerCount; 
          ++i, theta += (5*Math.PI/3)/(majorMarkerCount*minorMarkerCount))
      {
        if(i%minorMarkerCount == 0)
          continue;
        
        double adjustedTheta;
        if(theta < Math.PI/2)
          adjustedTheta = theta +3*Math.PI/2;
        else
          adjustedTheta = theta - Math.PI/2;
        
        x = gaugeR-markerContainerR*(Math.Cos(adjustedTheta));
        y = gaugeR-markerContainerR*(Math.Sin(adjustedTheta));
        angle = adjustedTheta*180/Math.PI;
        
        Path marker = XamlReader.Load(minorMarkerXAML) as Path;
        TransformGroup tg = new TransformGroup();
        TranslateTransform tt = new TranslateTransform();
        RotateTransform rt = new RotateTransform();

        tt.X = x;
        tt.Y = y;
        rt.Angle = angle;
        tg.Children.Add(rt);
        tg.Children.Add(tt);
        marker.RenderTransform = tg;
        gElem.Children.Add(marker);
      }
    }

    protected virtual Size ScaleGauge(
      Canvas gauge, 
      double quadWidth, 
      double quadHeight, 
      double gaugeWidth, 
      double gaugeHeight)
    {
      double minSide = Math.Min(quadWidth, quadHeight);
      double tx = (minSide == quadWidth)?0: (quadWidth-minSide)/2,
              ty = (minSide == quadHeight)?0: (quadHeight-minSide)/2;
      double scale = minSide/gaugeWidth;
      
      MatrixTransform mt = new MatrixTransform();
      mt.Matrix = new Matrix(scale, 0, 0, scale, tx, ty);
      gauge.RenderTransform = mt;
      return new Size(gaugeWidth*scale, gaugeHeight*scale);
    }
    
    protected override ChartEventArgs GetChartEvent(object sender, MouseEventArgs e)
    {
      ElementExpandos ee = Expandos[sender];
      int i = ee.YValueIndex;
      
      int[] yValueIndices = {i};
      double[] values = {Model.YValues[i, 0]};

      return new ChartEventArgs(null, yValueIndices, values, null);
    }

    protected override Size FillToolTipData(
      object sender,
      MouseEventArgs e,
      Path boundingRectElem,
      Path circleElem)
    {
      ChartEventArgs chartEvent = GetChartEvent(sender, e);

      double value = chartEvent.YValues[0];
      int i = chartEvent.YValueIndices[0];
      
      TextBlock textElem = ToolTip.Children[2] as TextBlock;
      textElem.Text = value.ToString(Format);

      double labelWidth = textElem.ActualWidth;
      double labelHeight = textElem.ActualHeight;
      
      //We do not need the next label 
      textElem = ToolTip.Children[3] as TextBlock;
      textElem.Text = "";

      double rectWidth = labelWidth + 2 * _TEXT_MARGIN;
      RectangleGeometry rg = boundingRectElem.Data as RectangleGeometry;
      Rect rect = rg.Rect;
      double rectHeight = rect.Height;
      if (rectHeight > 2 * labelHeight)
        rectHeight -= labelHeight;
      rg.Rect = new Rect(rect.X, rect.Y, rectWidth, rectHeight);
      boundingRectElem.SetValue(Path.StrokeProperty, Model.SeriesColors[i]);

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

    internal Point _animCenter = new Point(0,0);
  }
}
