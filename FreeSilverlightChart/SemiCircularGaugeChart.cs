using System;
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
  public class SemiCircularGaugeChart : GaugeChart
  {
    internal SemiCircularGaugeChart(ChartType type, ChartModel model) : base(type, model)
    {
    }

    /// <summary>
    /// Method for base classes to provide XAML for the gauge template
    /// </summary>
    /// <returns></returns>
    protected override string GetGaugeXAML()
    {
      return CurrentLookAndFeel.GetSemiCircularGauageXAML();
    }

    protected override void SetIndicatorPosition(double yValue, UIElement indicator, double ratio)
    {
      double cx = _animCenter.X, cy = _animCenter.Y;
      ChartModel model = Model;
      double minValue = model.MinYValue,
             maxValue = model.MaxYValue;
      double valueRatio = ratio*(yValue - minValue)/(maxValue-minValue);

      double theta = valueRatio * Math.PI;
      theta *= 180 / Math.PI;
      
      RotateTransform rt = new RotateTransform();

      rt.CenterX = cx;
      rt.CenterY = cy;
      rt.Angle = theta;
      
      indicator.RenderTransform =  rt;
    }

    protected override void CreateTextMarkerGroup(Canvas gauge, double gaugeR)
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
      
      for(int i=0; i<=majorMarkerCount; ++i)
      {
        double theta = i*Math.PI/majorMarkerCount;
        x = gaugeR-markerContainerR*(Math.Cos(theta));
        y = gaugeR-markerContainerR*(Math.Sin(theta));
        angle = theta*180/Math.PI;
        
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
        x = gaugeR-(markerContainerR-textMargin)*(Math.Cos(theta));
        y = gaugeR-(markerContainerR-textMargin)*(Math.Sin(theta));

        if(theta >= Math.PI/3 && theta <= 2*Math.PI/3)
        {
          x -= textElem.ActualWidth/2;
        }
        else
        {
          y -= textElem.ActualHeight/2;
          if(theta < Math.PI/2)
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

      for(int i=1; i<=(majorMarkerCount)*minorMarkerCount; ++i)
      {
        if(i%minorMarkerCount == 0)
          continue;
          
        double theta = i*Math.PI/(majorMarkerCount*minorMarkerCount);
        x = gaugeR-markerContainerR*(Math.Cos(theta));
        y = gaugeR-markerContainerR*(Math.Sin(theta));
        
        angle = theta*180/Math.PI;
        
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

    protected override Size ScaleGauge(
      Canvas gauge, 
      double quadWidth, 
      double quadHeight, 
      double gaugeWidth, 
      double gaugeHeight)
    {
      double sx = quadWidth/gaugeWidth, sy = quadHeight/gaugeHeight;
      double scale = Math.Min(sx, sy);
      double tx = (quadWidth<=gaugeWidth)?0:(quadWidth-gaugeWidth)/2,
          ty = (quadHeight<=gaugeHeight)?0:(quadHeight-gaugeHeight)/2;
     
      MatrixTransform mt = new MatrixTransform();
      mt.Matrix = new Matrix(scale, 0, 0, scale, tx, ty);
      gauge.RenderTransform = mt;
      return new Size(gaugeWidth * scale, gaugeHeight * scale);
    }    
  }
}
