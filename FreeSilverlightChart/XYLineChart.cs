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
  public class XYLineChart : ScatterPlotChart
  {
    internal XYLineChart(ChartType type, ChartModel model) : base(type, model)
    {
    }

    public override void DrawChartData()
    {
      DrawChartData(1, 0);
    }

    public override void DrawChartData(int mod, int rem)
    {
      if (IsPerspective)
        _drawPerspectiveXYValues();
      else
        _drawXYValues();
    }
    
    protected override bool AnimateAlongXAxis()
    {
      // always around x axis
      return true;  
    }
    
    private void _drawXYValues()
    {
      bool animate = (this.AnimationDuration > 0);

      double marginLeft = MarginLeft, marginTop = MarginTop;
      double marginRight = MarginRight, marginBottom = MarginBottom;

      double gridWidth = (ChartCanvas.Width - marginLeft - marginRight);
      double gridHeight = (ChartCanvas.Height - marginTop - marginBottom);
      LookAndFeel lf = CurrentLookAndFeel;
      string pathXAML = lf.GetLinePathXAML();
      Path pathElem;
      List<UIElement> dataElems = null;

      if (animate)
      {
        dataElems = DataElements;
      }

      ChartModel model = Model;

      string[] seriesLabels = model.SeriesLabels;
      int seriesCount = seriesLabels.Length;

      Color[] seriesColors = model.SeriesColors;

      double[,] yValues = model.YValues;
      double minYValue = model.MinYValue, maxYValue = model.MaxYValue;
      int nValues = yValues.GetUpperBound(0) + 1;
      double[,] xValues = model.XValues;
      double minXValue = model.MinXValue, maxXValue = model.MaxXValue;

      double dx, dy;
      MatrixTransform defaultTransform;
      
      for (int i = 0; i< seriesCount; ++i)
      {
        StringBuilder sb = new StringBuilder();
        
        dx = marginLeft;
        dy = gridHeight + marginTop;
        
        
        
        for (int j = 0; j < nValues; ++j)
        {
          dy = gridHeight + marginTop - gridHeight*(yValues[j,i]-minYValue)/(maxYValue-minYValue);
          dx = marginLeft + gridWidth*(xValues[j,i]-minXValue)/(maxXValue-minXValue);
          
          if(j==0)
            sb.Append("M").Append(dx).Append(",").Append(dy);
          else
            sb.Append(" L").Append(dx).Append(",").Append(dy);
        }

        pathElem = CreatePathFromXAMLAndData(pathXAML, sb);

        if (animate)
        {
          dataElems.Add(pathElem);
          defaultTransform = new MatrixTransform();
          Matrix m = new Matrix();
          m.M11 = 0.0;
          defaultTransform.Matrix = m;
          pathElem.SetValue(UIElement.RenderTransformProperty, defaultTransform);
        }

        SetExpandosOnElement(pathElem, -1, i, new Point());

        if (DisplayToolTip)
        {
          pathElem.MouseMove += new MouseEventHandler(ShowToolTip);
          pathElem.MouseLeave += new MouseEventHandler(HideToolTip);
        }

        pathElem.MouseLeftButtonUp += new MouseButtonEventHandler(ChartDataClicked);    

        pathElem.SetValue(Path.StrokeProperty, new SolidColorBrush(seriesColors[i]));
        ChartCanvas.Children.Add(pathElem);
      }
    }

    private void _drawPerspectiveXYValues()
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

      if (animate)
      {
        dataElems = DataElements;
      }

      ChartModel model = Model;

      string[] seriesLabels = model.SeriesLabels;
      int seriesCount = seriesLabels.Length;

      Color[] seriesColors = model.SeriesColors;

      double[,] yValues = model.YValues;
      double minYValue = model.MinYValue, maxYValue = model.MaxYValue;
      int nValues = yValues.GetUpperBound(0) + 1;
      double[,] xValues = model.XValues;
      double minXValue = model.MinXValue, maxXValue = model.MaxXValue;

      double gridBottom = gridHeight + marginTop + yOffset;
      double dx, dy;
      string gradientXAML = lf.GetElementGradientXAML();
      MatrixTransform defaultTransform;
      
      for (int i = 0; i< seriesCount; ++i)
      {
        StringBuilder sb = new StringBuilder();

        for (var j = 0; j < nValues; ++j)
        { 
          dy = gridBottom - gridHeight*(yValues[j,i]-minYValue)/(maxYValue-minYValue);
          dx = marginLeft + gridWidth*(xValues[j,i]-minXValue)/(maxXValue-minXValue);
          if(j != nValues - 1)
          {
            sb.Append(" M").Append(dx).Append(",").Append(dy);
            sb.Append(" l").Append(xOffset).Append(",").Append(-yOffset);
            double nextdy, nextdx;
            nextdx = marginLeft + gridWidth*(xValues[j+1,i]-minXValue)/(maxXValue-minXValue);
            nextdy = gridBottom - 
                      gridHeight*(yValues[j+1,i]-minYValue)/(maxYValue-minYValue);
            sb.Append(" L").Append(nextdx+xOffset).Append(",").Append(nextdy-yOffset);
            sb.Append(" l").Append(-xOffset).Append(",").Append(yOffset);
            sb.Append(" L").Append(dx).Append(",").Append(dy);
          }
        }

        pathElem = CreatePathFromXAMLAndData(pathXAML, sb);

        if (animate)
        {
          dataElems.Add(pathElem);
          defaultTransform = new MatrixTransform();
          Matrix m = new Matrix();
          m.M11 = 0.0;
          defaultTransform.Matrix = m;
          pathElem.SetValue(UIElement.RenderTransformProperty, defaultTransform);
        }

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

        ChartCanvas.Children.Add(pathElem);
      }
    }
    
    protected override void ShowToolTip(object sender, MouseEventArgs e)
    {
      // first hide any existing tooltip
      HideToolTip(sender, e);
      base.ShowToolTip(sender, e);
    }

    protected override ChartEventArgs GetChartEvent(object sender, MouseEventArgs e)
    {
      Point pt = e.GetPosition(ChartCanvas);
      ElementExpandos ee = Expandos[sender];
      int i = ee.SeriesIndex;

      bool isPerspective = IsPerspective;
      double yOffset = YOffsetPerspective;
      ChartModel model = Model;
      double[,] xValues = model.XValues; 
      double[,] yValues = model.YValues; 
      int nValues = yValues.GetUpperBound(0)+1;
      
      double minYValue = model.MinYValue, maxYValue = model.MaxYValue;
      double minXValue = model.MinXValue, maxXValue = model.MaxXValue;
      
      double marginLeft = MarginLeft, marginTop = MarginTop; 
      double gridWidth = (ChartCanvas.Width - marginLeft - MarginRight);
      double gridHeight = (ChartCanvas.Height - marginTop - MarginBottom);
          
      double gridBottom = gridHeight + marginTop +(isPerspective?yOffset:0);
      double dx, dy, xValue = 0.0, yValue = 0.0;
      double nextdy, nextdx;

      for (int j = 0; j < nValues; ++j)
      { 
        if(j != nValues - 1)
        {
          dx = marginLeft + gridWidth*(xValues[j,i]-minXValue)/(maxXValue-minXValue);
          nextdx = marginLeft + gridWidth*(xValues[j+1,i]-minXValue)/(maxXValue-minXValue);
          
          if(pt.X > dx && pt.X < (dx+nextdx))
          {
            dy = gridBottom - gridHeight*(yValues[j,i]-minYValue)/(maxYValue-minYValue);
            nextdy = gridBottom - 
                    gridHeight*(yValues[j+1,i]-minYValue)/(maxYValue-minYValue);

            yValue = yValues[j,i] + (yValues[j+1,i]-yValues[j,i])*(pt.Y - dy)/(nextdy-dy);
            xValue = xValues[j,i] + (xValues[j+1,i]-xValues[j,i])*(pt.X - dx)/(nextdx-dx);
            break;
          }
        }
      }
      
      int[] seriesIndices = {i};
      //double[] yEventValues = {yValue};
      //double[] xEventValues = {xValue};

      return new ChartEventArgs(seriesIndices, null, new double[] { yValue }, new double[] { xValue });
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
      string [] seriesLabels = model.SeriesLabels;
        
      TextBlock textElem = ToolTip.Children[2] as TextBlock;
      textElem.Text = seriesLabels[i]+ ": ("+ xValue.ToString(Format) + ")    (" + yValue.ToString(Format) +")";
                    
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
      boundingRectElem.SetValue(Path.StrokeProperty, new SolidColorBrush(Model.SeriesColors[i]));

      EllipseGeometry eg = circleElem.Data as EllipseGeometry;
      eg.RadiusX = eg.RadiusY = 0;
      return new Size(rectWidth, 2 * (labelHeight + _TEXT_MARGIN));      
    }

  }
}
