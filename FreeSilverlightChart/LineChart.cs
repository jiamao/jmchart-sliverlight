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
  public class LineChart : Chart
  {
    public LineChart(ChartType type, ChartModel model) : base(type, model)
    {
    }

    public override void DrawChartData()
    {
      DrawChartData(1, 0);
    }

    public override void DrawChartData(int mod, int rem)
    {
      if(IsPerspective)
        _drawPerspectiveLines(mod, rem);
      else
        _drawLines(mod, rem);
    }

    protected override bool AnimateAlongXAxis()
    {
      // always around x axis
      return true;  
    }

    private void _drawLines(int mod, int rem)
    {
      bool animate = (this.AnimationDuration > 0);

      double marginLeft = MarginLeft, marginTop = MarginTop;
      double marginRight = MarginRight, marginBottom = MarginBottom;

      double gridWidth = (ChartCanvas.Width - marginLeft - marginRight);
      double gridHeight = (ChartCanvas.Height - marginTop - marginBottom);
      LookAndFeel lf = CurrentLookAndFeel;
      string lineDotXAML = lf.GetLineDotXAML(),
          pathXAML = lf.GetLinePathXAML();
      Path pathElem, dotElement;
      List<UIElement> dataElems = null;
      MatrixTransform defaultTransform = null;

      if (animate)
      {
        dataElems = DataElements;
      }

      bool isStacked = (Type == ChartType.AREA_STACKED);
      ChartModel model = Model;

      string[] groupLabels = model.GroupLabels;
      int groupCount = groupLabels.Length;

      string[] seriesLabels = model.SeriesLabels;
      int seriesCount = seriesLabels.Length;

      Color[] seriesColors = model.SeriesColors;
      double[,] yValues = model.YValues;

      double minValue = model.MinYValue, maxValue = model.MaxYValue;

      int yValueCount = yValues.GetUpperBound(0) + 1;

      string gradientXAML = lf.GetElementGradientXAML();
      double barWidth = gridWidth/Math.Max(yValueCount,groupCount);
      double dx, dy;

      for (int i = 0; i< seriesCount; ++i)
      {
        // for combo charts we draw a bar every once in a mod.
        if ((mod > 1) && (i % mod) != rem)
          continue;
          
        dx = marginLeft+barWidth/2;
        
        StringBuilder sb = new StringBuilder();
        
        for (int j = 0; j < yValueCount; ++j)
        {
          dy = gridHeight + marginTop - gridHeight*(yValues[j,i]-minValue)/(maxValue-minValue);
          
          if(j == 0)
            sb.Append("M").Append(dx).Append(",").Append(dy);
          else
            sb.Append("L").Append(dx).Append(",").Append(dy);

          dotElement = XamlReader.Load(lineDotXAML) as Path;
          EllipseGeometry rectG = dotElement.Data as EllipseGeometry;
          rectG.Center = new Point(dx, dy);

          if (gradientXAML != null)
          {
            SetGradientOnElement(dotElement, gradientXAML, seriesColors[i], 0xe5);
          }
          else
          {
            SetFillOnElement(dotElement, seriesColors[i]);
          }

          dotElement.SetValue(Path.StrokeProperty, new SolidColorBrush(seriesColors[i]));

          if (animate)
          {
            dataElems.Add(dotElement);
            defaultTransform = new MatrixTransform();
            Matrix m = new Matrix();
            m.M11 = 0.0;
            defaultTransform.Matrix = m;
            dotElement.SetValue(UIElement.RenderTransformProperty, defaultTransform);
          }
          ChartCanvas.Children.Add(dotElement);
          
          dx += barWidth;
        }
        
        pathElem = CreatePathFromXAMLAndData(pathXAML, sb);
        // There is no fill for lines
        pathElem.SetValue(Path.StrokeProperty, new SolidColorBrush(seriesColors[i]));

        SetExpandosOnElement(pathElem, -1, i, new Point());
        if (DisplayToolTip)
        {
          pathElem.MouseMove += new MouseEventHandler(ShowToolTip);
          pathElem.MouseLeave += new MouseEventHandler(HideToolTip);
        }
        pathElem.MouseLeftButtonUp += new MouseButtonEventHandler(ChartDataClicked); 

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

    private void _drawPerspectiveLines(int mod, int rem)
    {
      bool animate = (this.AnimationDuration > 0);
      double xOffset = XOffsetPerspective, yOffset = YOffsetPerspective;

      double marginLeft = MarginLeft, marginTop = MarginTop;
      double marginRight = MarginRight, marginBottom = MarginBottom;

      double gridWidth = (ChartCanvas.Width - marginLeft - marginRight - xOffset);
      double gridHeight = (ChartCanvas.Height - marginTop - marginBottom - yOffset);
      LookAndFeel lf = CurrentLookAndFeel;
      string pathXAML = lf.GetLinePath3DXAML();
      Path pathElem;
      List<UIElement> dataElems = null;
      MatrixTransform defaultTransform = null;

      if (animate)
      {
        dataElems = DataElements;
      }

      ChartModel model = Model;

      string[] groupLabels = model.GroupLabels;
      int groupCount = groupLabels.Length;

      string[] seriesLabels = model.SeriesLabels;
      int seriesCount = seriesLabels.Length;

      Color[] seriesColors = model.SeriesColors;
      double[,] yValues = model.YValues;

      double minValue = model.MinYValue, maxValue = model.MaxYValue;

      // For combo graphs we display every once in mod
      int seriesBars = (mod > 1) ? (int)Math.Ceiling(1 / (double)mod) : seriesCount;

      int yValueCount = yValues.GetUpperBound(0) + 1;

      string gradientXAML = lf.GetElementGradientXAML();

      double barWidth = (gridWidth/Math.Max(yValueCount,groupCount));
      double gridBottom = gridHeight + marginTop + yOffset, dx, dy;
      
      for (int i = 0; i< seriesCount; ++i)
      {
        // for combo charts we draw a bar every once in a mod.
        if ((mod > 1) && (i % mod) != rem)
          continue;
          
        dx = marginLeft+barWidth/2;
        
        StringBuilder sb = new StringBuilder();
        
        for (int j = 0; j < yValueCount; ++j)
        {      
          dy = gridBottom - gridHeight*(yValues[j,i]-minValue)/(maxValue-minValue);
                
          if(j != yValueCount - 1)
          {
            sb.Append("M").Append(dx).Append(",").Append(dy);
            sb.Append(" l").Append(xOffset).Append(",").Append(-yOffset);
            
            var nextdy = gridBottom - gridHeight*(yValues[j+1,i]-minValue)/(maxValue-minValue);
            var nextdx = dx+barWidth;
            
            sb.Append(" L").Append(nextdx+xOffset).Append(",").Append(nextdy-yOffset);
            sb.Append(" L").Append(nextdx).Append(",").Append(nextdy);
            sb.Append(" L").Append(dx).Append(",").Append(dy);            
            dx += barWidth;
          }
        }
        pathElem = CreatePathFromXAMLAndData(pathXAML, sb);

        if (gradientXAML != null)
        {
          SetGradientOnElement(pathElem, gradientXAML, seriesColors[i], 0xC0);
        }
        else
        {
          SetFillOnElement(pathElem, seriesColors[i]);
        }
        pathElem.SetValue(Path.StrokeProperty, new SolidColorBrush(seriesColors[i]));
        (pathElem.Data as PathGeometry).FillRule = FillRule.Nonzero;

        SetExpandosOnElement(pathElem, -1, i, new Point());

        if (DisplayToolTip)
        {
          pathElem.MouseMove += new MouseEventHandler(ShowToolTip);
          pathElem.MouseLeave += new MouseEventHandler(HideToolTip);
        }

        pathElem.MouseLeftButtonUp += new MouseButtonEventHandler(ChartDataClicked); 

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

    protected override void ShowToolTip(object sender, MouseEventArgs e)
    {
      // first hide any existing tooltip
      HideToolTip(sender, e);
      base.ShowToolTip(sender, e);
    }

    protected override Point GetToolTipLocation(object sender, MouseEventArgs e, Size ttBounds)
    {
      Point pt = e.GetPosition(ChartCanvas);
      pt = new Point(pt.X + 16, pt.Y + 16);
      return pt;
    }

    protected override ChartEventArgs GetChartEvent(object sender, MouseEventArgs e)
    {
      Point pt = e.GetPosition(ChartCanvas);
      ElementExpandos ee = Expandos[sender];
      int i = ee.SeriesIndex;
      
      bool isPerspective = IsPerspective;
      double yOffset = YOffsetPerspective;
      
      ChartModel model = Model;
      double [,] yValues = model.YValues;
      
      string [] groupLabels = model.GroupLabels;
      int groupCount = groupLabels.Length;
      
      double marginLeft = MarginLeft, marginTop = MarginTop; 
      double gridWidth = (ChartCanvas.Width - marginLeft - MarginRight);
      double gridHeight = (ChartCanvas.Height - marginTop - MarginBottom);
      int yValueCount = yValues.GetUpperBound(0)+1;  
      
      double barWidth = (gridWidth/Math.Max(yValueCount,groupCount));
      
      double gridBottom = gridHeight + marginTop +(isPerspective?yOffset:0);
      double dx = marginLeft+barWidth/2, value = 0.0;

      for (int j = 0; j < yValueCount; ++j)
      {
        if(j == yValueCount - 1)
          continue;

        if(pt.X > dx && pt.X < (dx+barWidth))
        {
          value = yValues[j,i] + (yValues[j+1,i]-yValues[j,i])*(pt.X - dx)/barWidth;
          break;
        }
        dx += barWidth; 
      }
      
      int[] seriesIndices = {i};
      double[] values = {value};

      return new ChartEventArgs(seriesIndices, null, values,  null);
    }

    protected override Size FillToolTipData(
      object sender,
      MouseEventArgs e,
      Path boundingRectElem,
      Path circleElem)
    {
      ChartEventArgs chartEvent = GetChartEvent(sender, e);
      
      int i = chartEvent.SeriesIndices[0];
      double value = chartEvent.YValues[0];      
      string []seriesLabels = Model.SeriesLabels;
      
      TextBlock textElem = ToolTip.Children[2] as TextBlock;
      textElem.Text = seriesLabels[i]+ ": "+value.ToString(Format);
                    
      double labelWidth = textElem.ActualWidth;
      double labelHeight = textElem.ActualHeight;
      //We do not need the next label 
      textElem = ToolTip.Children[3] as TextBlock;
      textElem.Text = "";

      double rectWidth = labelWidth + 2 * _TEXT_MARGIN;
      RectangleGeometry rg = boundingRectElem.Data as RectangleGeometry;
      Rect rect = rg.Rect;
      double rectHeight = rect.Height;
      if(rectHeight > 2*labelHeight)
        rectHeight -= labelHeight;
      rg.Rect = new Rect(rect.X, rect.Y, rectWidth, rectHeight);  
      boundingRectElem.SetValue(Path.StrokeProperty, Model.SeriesColors[i]);
      
      EllipseGeometry eg = circleElem.Data as EllipseGeometry;
      eg.RadiusX = eg.RadiusY =  0;
      return new Size(rectWidth, 2 * (labelHeight + _TEXT_MARGIN));
    }
    
  }
}
