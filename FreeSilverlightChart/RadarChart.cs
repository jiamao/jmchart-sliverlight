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
  public class RadarChart : Chart
  {
    public RadarChart(ChartType type, ChartModel model) : base(type, model)
    {
    }


    public override void DrawChartData()
    {
      DrawChartData(1, 0);
    }

    public override void DrawChartData(int mod, int rem)
    {
      _drawRadar();      
    }

    protected override void SetGridAnimStep(double ratio)
    {
      List<UIElement> animElems = GridElements;
      int animCount = animElems.Count;

      for (int i = 0; i < animCount; ++i)
      {
        animElems[i].Opacity = ratio;
      }
    }

    public override void SetDataAnimStep(double ratio)
    {
      double marginLeft = MarginLeft, marginTop = MarginTop;
      
      bool isRadarArea =(Type == Chart.ChartType.RADAR_AREA);
      
      ChartModel model = Model;
       
      string [] seriesLabels = model.SeriesLabels;
      int  seriesCount = seriesLabels.Length;
      
      double gridWidth = (ChartCanvas.Width - marginLeft - MarginRight);
      double gridHeight = (ChartCanvas.Height - marginTop - MarginBottom);
      double cx = marginLeft+gridWidth/2.0, cy = marginTop+gridHeight/2.0;
      double newRatio = ratio*(double)seriesCount;
      int animSeriesIndex = 0;

      if(newRatio > 1)
      {
        animSeriesIndex = (int)Math.Floor(newRatio);
        if(animSeriesIndex >= seriesCount)
          animSeriesIndex = seriesCount - 1;
        newRatio = newRatio - (int)Math.Floor(newRatio);
      }

      double tx = (1-newRatio)*cx, ty = (1-newRatio)*cy;
      Matrix m = new Matrix(newRatio, 0, 0, newRatio, tx, ty);

      // We will make each series appear separately
      int i = animSeriesIndex;
      
      this._setRadarSeriesAnimStep(i, isRadarArea, m); 
      
      if(i>0)
      {
        _setRadarSeriesAnimStep(i - 1, isRadarArea, Matrix.Identity);
      }
      
      // make sure that everything is scaled properly at the end
      if(ratio == 1)
      {
        for(int j = 0; j < seriesCount; ++j)
        {
          _setRadarSeriesAnimStep(j, isRadarArea, Matrix.Identity);
        }
        _dotElements = null;
      }
    }

    private void _setRadarSeriesAnimStep(int i, bool isRadarArea, Matrix m)
    {
      MatrixTransform transform =  DataElements[i].RenderTransform as MatrixTransform;
      transform.Matrix = m;
      
      if(!isRadarArea)
      {
        for (int j = _dotElements.GetUpperBound(1); j >= 0; --j)
        {
          transform = _dotElements[i,j].RenderTransform as MatrixTransform;
          transform.Matrix = m;
        }
      }
    }


    /**
     * Adjusts the legend location when it is at the top
     * @param ty(int) the original y location of the legend
     */
    protected override double SetLegendTopAdjustment(double ty)
    {
      ty -= HLabelBounds.Height +_TEXT_MARGIN;
      return ty;
    }

    /**
     * Adjusts the legend location when it is at the bottom
     * @param ty(int) the original y location of the legend
     */
    protected override double SetLegendBottomAdjustment(double ty)
    {
      if(HLabelContainer.Children.Count > 0)
      {
        ty += HLabelBounds.Height +_TEXT_MARGIN;
      }
      return ty;
    }

    /**
     * Adjusts the legend location when it is at the Left
     * @param tx(int) the original x location of the legend
     */
    protected override double SetLegendLeftAdjustment(double tx)
    {
      if (HLabelContainer.Children.Count > 0)
      {
        tx -= HLabelBounds.Width+_TEXT_MARGIN;
      }
      return tx;
    }

    /**
     * Adjusts the legend location when it is at the Right
     * @param tx(int) the original x location of the legend
     */
    protected override double SetLegendRightAdjustment(double tx)
    {
      if (HLabelContainer.Children.Count > 0)
        tx += HLabelBounds.Width + _TEXT_MARGIN;
      return tx;
    }

    protected override void DrawGroupLabels()
    {
      ChartModel model = Model;
      Canvas container = new Canvas();
      HLabelContainer = container;
      
      List<UIElement> labelElems = LabelElements;
      bool animate = (AnimationDuration>0);
      
      string[] groupLabels = model.GroupLabels;
      int vLineCount = groupLabels.Length;

      TextBlock labelElem;
      string labelText;
      string labelXAML = CurrentLookAndFeel.GetGroupLabelXAML();
      double maxWidth = 0, maxHeight = 0;
      TextBlock [] gLabelElems = new TextBlock[vLineCount];
      
      GroupLabelElems = gLabelElems;
      
      for(int i = 0; i<vLineCount; ++i)
      {
        labelText = groupLabels[i];
        
        if(labelText == null || labelText.Length == 0)
          continue;
          
        labelElem = XamlReader.Load(labelXAML) as TextBlock;
        
        if(animate)
		    {
		      labelElems.Add(labelElem);
		      labelElem.Opacity = 0;
		    }
		    
        labelElem.Text = labelText;
        container.Children.Add(labelElem);
        gLabelElems[i] = labelElem;
        maxWidth = Math.Max(maxWidth, labelElem.ActualWidth);
        maxHeight = Math.Max(maxHeight, labelElem.ActualHeight);
      }
      ChartCanvas.Children.Add(container);
      HLabelBounds = new Size(maxWidth, maxHeight);
    }

    protected override void AdjustMarginsForGroupLabels()
    {
      if (HLabelContainer.Children.Count > 0)
      {
        Size bBox = HLabelBounds;
        double dxVertical = bBox.Width + _TEXT_MARGIN,
            dyVertical = bBox.Height + _TEXT_MARGIN;
        
        MarginTop += dyVertical;
        MarginBottom += dyVertical;
        MarginLeft += dxVertical;
        MarginRight += dxVertical;
      }
    }

    protected override void LayoutGroupLabels()
    {
      ChartModel model = Model;
      double marginLeft = MarginLeft, marginTop = MarginTop; 
      double gridWidth = (ChartCanvas.Width - marginLeft - MarginRight);
      double gridHeight, cy = 0;
      double cx = marginLeft+gridWidth/2, radius=0;
      
      string [] groupLabels = model.GroupLabels;
      int vLineCount = groupLabels.Length;
      
      TextBlock labelElem;
      
      double groupWidth = gridWidth/(double)vLineCount;
      Canvas container = HLabelContainer;
      
      UIElementCollection childNodes = container.Children;

      bool firstLabelDrawn = false;
      TextBlock[] gLabelElems = GroupLabelElems;
      
      if(childNodes.Count == 0)
        return;
        
      for(int i = 0; i<vLineCount; ++i)
      {
        labelElem = gLabelElems[i];
        
        if(labelElem == null)
          continue;

        if (!firstLabelDrawn)
        {
          gridHeight = (ChartCanvas.Height - marginTop - MarginBottom);
          cy = marginTop+gridHeight/(double)2;
          radius = Math.Min(gridWidth, gridHeight)/(double)2 + 
                   labelElem.ActualHeight - _TEXT_MARGIN /(double)2;
          firstLabelDrawn = true;
        }
        
        double theta = (i)*2*Math.PI/(double)vLineCount;
        
        double dx= cx + radius*Math.Sin(theta),
               dy = cy - radius*Math.Cos(theta);

        labelElem.SetValue(Canvas.TopProperty, dy - labelElem.ActualHeight/(double)2);
        
        if(theta > Math.PI)
          dx -= labelElem.ActualWidth;

        labelElem.SetValue(Canvas.LeftProperty, dx);
      }
    }

    protected override void DrawGrid()
    {
      // No perspective support for radar
      this.Draw2DGrid();
    }

    protected override void Draw2DGrid()
    {
      List<UIElement> gridElems = GridElements;
      bool animate = (AnimationDuration>0);
      
      double marginLeft = MarginLeft, marginTop = MarginTop; 
      double gridWidth = (ChartCanvas.Width - marginLeft - MarginRight);
      double gridHeight = (ChartCanvas.Height - marginTop - MarginBottom);
      double cx = marginLeft+gridWidth/(double)2, cy = marginTop+gridHeight/(double)2;
      double radius = Math.Min(gridWidth, gridHeight)/(double)2;
      String gridGradientXAML = CurrentLookAndFeel.GetGridGradientXAML();
      
      string circleXAML = CurrentLookAndFeel.GetRadarCircleXAML();
      
      Path circle = XamlReader.Load(circleXAML) as Path;
      EllipseGeometry eg = circle.Data as EllipseGeometry;
      
      eg.Center = new Point(cx, cy);
      eg.RadiusY = eg.RadiusX = radius;
      
      if (gridGradientXAML != null)
      {
        circle.Fill = XamlReader.Load(gridGradientXAML) as Brush;
      }
      
      if(animate)
      {
        gridElems.Add(circle);
        circle.Opacity = 0.0; 
      }
      
      ChartCanvas.Children.Add(circle);
      
      int vLineCount = this.GetVLineCount();
      int circleCount = this.GetHLineCount();
      
      // inner circles
      CurrentLookAndFeel.GetRadarInnerCircleXAML();
      
      for (var i = 0; i< circleCount-1; ++i)
      {
        circle = XamlReader.Load(circleXAML) as Path;
        eg = circle.Data as EllipseGeometry;
        eg.Center = new Point(cx, cy);
        
        if (animate)
        {
          gridElems.Add(circle);
          circle.Opacity = 0;
        }
        double newRadius = radius - (i+1)*radius/circleCount;
        eg.RadiusY = eg.RadiusX = newRadius;
        
        ChartCanvas.Children.Add(circle);
      }

      StringBuilder sb = new StringBuilder();
           
      sb.Append(" M").Append(cx).Append(",").Append(cy);
      sb.Append(" l").Append(0).Append(",").Append(-radius);

      for(int i = 0; i<vLineCount-1; ++i)
      {
        double dx= cx + radius*Math.Sin((i+1)*2*Math.PI/(double)vLineCount),
            dy = cy - radius*Math.Cos((i+1)*2*Math.PI/(double)vLineCount);
        sb.Append(" M").Append(cx).Append(",").Append(cy);
        sb.Append(" L").Append(dx).Append(",").Append(dy);    
      }

      Path pathElem = CreatePathFromXAMLAndData(CurrentLookAndFeel.GetRadarGridPathXAML(), sb);
      
      if(animate)
      {
        gridElems.Add(pathElem);
        pathElem.Opacity = 0;
      }
      
      ChartCanvas.Children.Add(pathElem);
    }

    protected override void DrawYValueLabels()
    {
      // For radar the y values do not effect margins 
      // so we do our drawing and layout in one go.
    }

    protected override void LayoutYValueLabels()
    {
      ChartModel model = Model;
      double marginLeft = MarginLeft, marginTop = MarginTop; 
      double gridWidth = (ChartCanvas.Width - marginLeft - MarginRight);
      double gridHeight = (ChartCanvas.Height - marginTop - MarginBottom);
      
      double cx = marginLeft+gridWidth/2, cy = marginTop+gridHeight/2.0;
      double radius = Math.Min(gridWidth, gridHeight)/2.0;  

      int vLineCount = this.GetVLineCount(), circleCount = this.GetHLineCount();          
      double minValue = model.MinYValue, maxValue = model.MaxYValue;      
      string labelXAML = CurrentLookAndFeel.GetYValueLabelXAML();

      double textHeight = double.NaN;

      _addRadarYLabelAt(labelXAML, cx, (double)marginTop, ref textHeight, maxValue);
      _addRadarYLabelAt(labelXAML, cx, cy, ref textHeight, minValue);

      // horizontal lines
      for (int i = 0; i< circleCount-1; ++i)
      {
        double newRadius = (i+1)*radius/(double)circleCount;
        double value = ((maxValue-minValue)*(i+1)/(double)circleCount) + minValue;
        this._addRadarYLabelAt(labelXAML, cx,
                               radius-newRadius+marginTop, ref textHeight, value);
      }
    }

    private void _addRadarYLabelAt
    (
      string labelXAML,
      double x, 
      double y, 
      ref double textHeight, 
      double value)
    {
      TextBlock labelElem = XamlReader.Load(labelXAML) as TextBlock;
      
      labelElem.Text = value.ToString(Format);
      ChartCanvas.Children.Add(labelElem);
      
      if(double.IsNaN(textHeight))
      {
        textHeight = labelElem.ActualHeight;
      }
      
      // readjust to right align
      double labelMargin = _TEXT_MARGIN, 
             textLength = labelElem.ActualWidth;
             
      double dx = x-textLength-labelMargin;
      
      labelElem.SetValue(Canvas.LeftProperty, dx);
      labelElem.SetValue(Canvas.TopProperty, y - textHeight/2.0);
      labelElem.SetValue(Canvas.ZIndexProperty, 100);
      
      if (AnimationDuration > 0)
      {
        LabelElements.Add(labelElem);
        labelElem.Opacity = 0;
      }
    }

    private void _drawRadar()
    {
      ChartModel model = Model;
      
      List<UIElement> dataElems = DataElements;
      bool animate = (AnimationDuration>0);
      double marginLeft = MarginLeft, marginTop = MarginTop; 
      double gridWidth = (ChartCanvas.Width - marginLeft - MarginRight);
      double gridHeight = (ChartCanvas.Height - marginTop - MarginBottom);
      
      double cx = marginLeft+gridWidth/2.0, cy = marginTop+gridHeight/2.0;
      double radius = Math.Min(gridWidth, gridHeight)/2.0;
      
      bool isRadarArea =(Type == Chart.ChartType.RADAR_AREA);
      LookAndFeel lf = CurrentLookAndFeel;
      string pathXAML = isRadarArea ? lf.GetAreaPathXAML() : lf.GetLinePathXAML();
      
      string lineDotXAML = null;
      
      string[] seriesLabels = model.SeriesLabels;
      int seriesCount = seriesLabels.Length;
      Color [] seriesColors = model.SeriesColors;
      double[,] yValues = model.YValues;
      double minValue = model.MinYValue, maxValue = model.MaxYValue;
      
      string gradientXAML = lf.GetElementGradientXAML();
      MatrixTransform defaultTransform = null;
      
      int yValueCount = yValues.GetUpperBound(0)+1;
      double dx, dy;
      
      if(!isRadarArea)
        lineDotXAML = CurrentLookAndFeel.GetLineDotXAML();

      if (animate && _dotElements == null)
        _dotElements = new UIElement[seriesCount, yValueCount];

      for (int i = 0; i< seriesCount; ++i)
      {
        StringBuilder sb = new StringBuilder();

        for (int j = 0; j < yValueCount; ++j)
        {
          double yPoint = radius*(yValues[j, i]-minValue)/(maxValue-minValue);
          
          dx= cx + yPoint*Math.Sin((j)*2*Math.PI/yValueCount);
          dy = cy - yPoint*Math.Cos((j)*2*Math.PI/yValueCount);
          
          if(j == 0)
          {
            sb.Append("M").Append(dx).Append(",").Append(dy);
          }
          else
          {
            sb.Append(" L").Append(dx).Append(",").Append(dy);    
          }
          
          if(!isRadarArea)
          {
            Path dotElem = XamlReader.Load(lineDotXAML) as Path;
            EllipseGeometry eg = dotElem.Data as EllipseGeometry;
            eg.Center = new Point(dx, dy);

            if (gradientXAML != null)
            {
              SetGradientOnElement(dotElem, gradientXAML, seriesColors[i], 0xe5);
            }
            else
            {
              SetFillOnElement(dotElem, seriesColors[i]);
            }
            dotElem.SetValue(Path.StrokeProperty, new SolidColorBrush(seriesColors[i]));
            
            if(animate)
            {
              _dotElements[i, j] = dotElem;
              defaultTransform = new MatrixTransform();
              Matrix m = new Matrix();
              m.M11 = m.M22 = 0.0;
              defaultTransform.Matrix = m;
              dotElem.SetValue(UIElement.RenderTransformProperty, defaultTransform);
            }
            ChartCanvas.Children.Add(dotElem);
          }
        }
        sb.Append("Z");
        
        Path pathElem = CreatePathFromXAMLAndData(pathXAML, sb);

        if (animate)
        {
          dataElems.Add(pathElem);
          defaultTransform = new MatrixTransform();
          Matrix m = new Matrix();
          m.M11 = m.M22 = 0.0;
          defaultTransform.Matrix = m;
          pathElem.SetValue(UIElement.RenderTransformProperty, defaultTransform);
        }
        
        if(isRadarArea)
        {
          if (gradientXAML != null)
          {
            SetGradientOnElement(pathElem, gradientXAML, seriesColors[i], 0x72);
          }
          else
          {
            SetFillOnElement(pathElem, seriesColors[i]);
          }
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
    
    private bool _isPointInPolygon(double [] xs, double []ys, double x, double y)
    {
      int i, j, npol = ys.Length;
      bool inside = false;
      
      for (i = 0, j = npol-1; i < npol; j = i++) 
      {
        if ((((ys[i]<=y) && (y<ys[j])) ||
             ((ys[j]<=y) && (y<ys[i]))) &&
            (x < (xs[j] - xs[i]) * (y - ys[i]) / (ys[j] - ys[i]) + xs[i]))
        {
          inside = !inside;
        }
      }
      return inside;
    }

    protected override ChartEventArgs GetChartEvent(object sender, MouseEventArgs e)
    {
      Point pt = e.GetPosition(ChartCanvas);

      bool isRadarArea = (Type == ChartType.RADAR_AREA);
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
      
      double cx = marginLeft+gridWidth/2, cy = marginTop+gridHeight/2;
      double radius = Math.Min(gridWidth, gridHeight)/2;
      
      int seriesCount = model.SeriesLabels.Length;
      int seriesIndex = -1;
      ElementExpandos ee = Expandos[sender];
      
      if(pt.X < marginLeft || 
         pt.X>(marginLeft + gridWidth) ||
         pt.Y < marginTop || 
         pt.Y>(marginTop + gridHeight))
      {
        return null;
      }
      
      if(!isRadarArea)
        seriesIndex = ee.SeriesIndex;

      _seriesXs = new List<double>(seriesCount);        
      _seriesYs = new List<double>(seriesCount);
        
      double dx1, dy1, dx2, dy2, value;
      List<int> seriesIndices = new List<int>(seriesCount);
      List<double> seriesValues = new List<double>(seriesCount);

      for (int i = 0; i< seriesCount; ++i)
      {
        if(!isRadarArea && (seriesIndex != i))
          continue;
        
        for (int j = 0; j < yValueCount; ++j)
        {
          double nextYVal = (j != yValueCount -1)? yValues[j+1,i]: yValues[0,i];
          double yPoint = radius*(yValues[j,i]-minValue)/(maxValue-minValue);
          double yPoint2 = radius*(nextYVal-minValue)/(maxValue-minValue);
          double angle = j*2.0*Math.PI/yValueCount, 
                 nextAngle = (j+1)*2.0*Math.PI/yValueCount;
          dx1 = cx + yPoint*Math.Sin(angle);
          dy1 = cy - yPoint*Math.Cos(angle);
          
          dx2 = cx + yPoint2*Math.Sin(nextAngle);
          dy2 = cy - yPoint2*Math.Cos(nextAngle);

          if (_isPointInPolygon(new double [] { cx, dx1, dx2 }, new double [] { cy, dy1, dy2 }, pt.X, pt.Y))
          {
            // find the point on the radar that matches the current mouse 
            // using the angle of the current mouse point
            double mousePtAngle = Math.Atan2(cy - pt.Y, pt.X - cx);
            
            if(mousePtAngle <= Math.PI/2)
              mousePtAngle = Math.PI/2 - mousePtAngle;
            else
              mousePtAngle = 3*Math.PI/2 + (Math.PI - mousePtAngle);
            
            double ratio = (mousePtAngle - angle)/(nextAngle - angle);
            
            value = yValues[j,i] + (nextYVal-yValues[j,i])*ratio;
            
            seriesValues.Add(value);
            seriesIndices.Add(i);
            _seriesYs.Add(dy1+(dy2-dy1)*ratio);
            _seriesXs.Add(dx1+(dx2-dx1)*ratio);
            break;
          }
        }
      }
      
      return new ChartEventArgs(seriesIndices.ToArray(),null, seriesValues.ToArray(),null);
    }

    protected override void ShowToolTip(object sender, MouseEventArgs e)
    {    
      // Hide any existing tooltips
      HideToolTip(sender, e);
      
      var chartEvent = this.GetChartEvent(sender, e);
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

      int tooltipCount = seriesIndices.Length; ;
      Point pt = e.GetPosition(ChartCanvas);
      double dx, dy;

      if (_tooltips == null)
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

        textElem.Text = seriesLabels[seriesIndex] + ":  " + seriesValues[i].ToString(Format);
        var textElemCount = seriesValues.Length;
        var rectWidth = textElem.ActualWidth;
        
        
        RectangleGeometry rg = boundingRectElem.Data as RectangleGeometry;
        Rect rect = rg.Rect;
        // Initially the template tooltip has an extra text node
        if(resizeOnInit)
        {
          dy = textElem.ActualHeight;
          rg.Rect = new Rect(rect.X, rect.Y, rectWidth, rect.Height - dy);
          textElem = toolTip.Children[3] as TextBlock;
          textElem.Text = "";
        }

        rect = rg.Rect;
        rectWidth += 2*_TEXT_MARGIN;
        double rectHeight = rect.Height;
        rg.Rect = new Rect(rect.X, rect.Y, rectWidth, rectHeight);
        
        dx = _seriesXs[i];
        dy = _seriesYs[i] - rectHeight;

        EllipseGeometry eg = circleElem.Data as EllipseGeometry;
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
        
        if(dy - rectHeight < 0)
        {
          dy += rectHeight;
          cy = 0;
        }
        else
        {
          cy = rectHeight;
        }

        eg.Center = new Point(cx, cy);
        SolidColorBrush scb = new SolidColorBrush(seriesColors[seriesIndex]);
        boundingRectElem.SetValue(Path.StrokeProperty, scb);
        circleElem.SetValue(Path.StrokeProperty, scb);

        TranslateTransform tTrans = new TranslateTransform();
        tTrans.X = dx;
        tTrans.Y = dy;
        toolTip.RenderTransform = tTrans;
      }
    }

    protected virtual void HideToolTip(object sender, MouseEventArgs e)
    {
      int tooltipCount = _tooltips != null ? _tooltips.Length : 0;
      for(var i = 0; i<tooltipCount; ++i)
      { 
        Canvas toolTip = _tooltips[i];
        if(toolTip != null)
          toolTip.Visibility = Visibility.Collapsed;
      }
    }

    private List<double> _seriesXs, _seriesYs;
    UIElement[,] _dotElements;
    private Canvas[] _tooltips;       
  }
}
