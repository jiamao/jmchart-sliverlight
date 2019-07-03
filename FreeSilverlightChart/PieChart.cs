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
  public class PieChart : Chart
  {
    public PieChart(ChartType type, ChartModel model) : base(type, model)
    {
    }

    public override void DrawChartData()
    {  
      var rootElem = ChartCanvas;
        
      // calculate the number of rows and columns
      ChartModel model = Model;
      double[,] yValues = model.YValues;
      int yValueCount = yValues.GetUpperBound(0)+1;
      string[] groupLabels = model.GroupLabels;
      int groupCount = groupLabels!= null?groupLabels.Length:1;
          
      int nCols = (int)Math.Ceiling(Math.Sqrt(yValueCount)), nRows = (int)Math.Round(Math.Sqrt(yValueCount));
      string labelXAML = CurrentLookAndFeel.GetGroupLabelXAML();
      TextBlock labelElem = null;
      
      double dx=MarginLeft, dy=MarginTop;
      double quadWidth = (rootElem.Width - dx - MarginRight)/nCols;
      bool animate = (AnimationDuration>0);
      
      double vGap = 2*_TEXT_MARGIN;
      var quadHeight = (rootElem.Height - MarginTop - MarginBottom - (nRows - 1) * vGap) / nRows;

      if(animate)
      {
        _pieAnimAngles = new List<double>();
        _pieAnimRadii = new List<double>();
      }
      
      for(int i = 0; i<nRows; ++i)
      {
        for(int j = 0; j<nCols; ++j)
        {  
          int iGroup = (groupLabels != null)?(i*nCols + j):(-1);
          if(iGroup >= yValueCount)
            break;
          
          string groupLabel = (iGroup == -1)?null:groupLabels[iGroup];
          Canvas pieContainer = new Canvas();
          rootElem.Children.Add(pieContainer);

          double newHeight = DrawGroupLabelTitle(groupLabel, rootElem, labelXAML, ref labelElem, 
                                                 dx, dy, quadWidth, quadHeight);
                                                
          double newWidth = quadWidth - 2*_TEXT_MARGIN;
          double cx= dx+quadWidth/2 + _TEXT_MARGIN, cy = dy+newHeight/2;
          
          if(animate)
          {
            _pieAnimRadii.Add(Math.Max(cx, cy));
          }
          
          if(IsPerspective)
          {
            this._draw3DPies(pieContainer, newWidth, newHeight, iGroup);
            MatrixTransform mt = new MatrixTransform();
            mt.Matrix = new Matrix(1,0,0,.707, cx, cy);
            // The chart is draw with the center at 0 so we need to compensate for it.
            pieContainer.RenderTransform = mt;
          }
          else
          {
            this._drawPies(pieContainer, newWidth, newHeight, iGroup);
            TranslateTransform tt = new TranslateTransform();
            tt.X = cx;
            tt.Y = cy;
            pieContainer.RenderTransform = tt;
          }
          dx +=quadWidth;
        }
        
        dx = MarginLeft;
        dy += quadHeight+vGap;
      }  
    }

    /// <summary>
    /// Overridden to do nothing for pie charts
    /// </summary>
    protected override void ComputeMinMaxValues()
    {

    }

    /// <summary>
    /// Overridden to do nothing for pie charts
    /// </summary>
    protected override void DrawGroupLabels()
    {

    }

    /// <summary>
    /// Overridden to do nothing for pie charts
    /// </summary>
    protected override void LayoutGroupLabels()
    {

    }

    /// <summary>
    /// Overridden to do nothing for pie charts
    /// </summary>
    protected override void DrawGrid()
    {

    }

    /// <summary>
    /// Overridden to do nothing for pie charts
    /// </summary>
    protected override void DrawYValueLabels()
    {

    }

    /// <summary>
    /// Overridden to do nothing for pie charts
    /// </summary>
    protected override void LayoutYValueLabels()
    {

    }

    public override void SetDataAnimStep(double ratio)
    {
      bool isPerspective = IsPerspective;
      int angleIndex = 0, elemIndex = 0;
      List<UIElement> animElems = DataElements;
      int chartCount = _pieAnimRadii.Count;
      double[,] yValues = Model.YValues;
      int nPies = yValues.GetUpperBound(1) + 1;
      double perPieRatio = 1/(double)nPies;
      int pieStartIndex = _currentPieIndex;
      double radius;
      
      // calculate the last pie that we are going to animate
      while (ratio > (_currentPieIndex + 1) * perPieRatio)
      {
        _currentPieIndex++;
      }
      
      // We are animating parependicular to the tangent to the middle of the pie
      for (int i = 0; i < chartCount; ++i)
      {
        for (int j = pieStartIndex; j <= _currentPieIndex; ++j)
        {
          angleIndex = i * nPies + j;
          elemIndex = i * nPies *(isPerspective?3:1) + j*(isPerspective?3:1);
          if(j == _currentPieIndex)
          {
            double curRatio = (ratio - _currentPieIndex* perPieRatio) / perPieRatio;
            radius = _pieAnimRadii[i] * (1 - curRatio);
          }
          else
          {
            // This will ensure that all the previous pies will move to the correct location
            radius = 0;
          }
          
          double angle = _pieAnimAngles[angleIndex++] * 2 * Math.PI;
          double tx = radius * Math.Sin(angle), ty = radius * Math.Cos(angle);

          if (angle <= Math.PI / 2)
          {
            ty = -ty;
          }
          else if (angle <= Math.PI)
          {
            ;
          }
          else if (angle <= 3 * Math.PI / 2)
          {
            tx = -tx;
          }
          else
          {
            ty = -ty;
            tx = -tx;
          }

          TranslateTransform transform = new TranslateTransform();
          transform.X = tx;
          transform.Y = ty;

          animElems[elemIndex++].SetValue(Canvas.RenderTransformProperty, transform);

          if (isPerspective)
          {
            transform = new TranslateTransform();
            transform.X = tx;
            transform.Y = ty;
            animElems[elemIndex++].SetValue(Canvas.RenderTransformProperty, transform);

            transform = new TranslateTransform();
            transform.X = tx;
            transform.Y = ty;
            animElems[elemIndex++].SetValue(Canvas.RenderTransformProperty, transform);
          }
        }
      }
    }
    
    private void _drawPies(
      Canvas pieContainer, 
      double quadWidth, 
      double quadHeight, 
      int iGroup)
    {
      ChartModel model = Model;
      double[,] yValues = model.YValues;
      string [] groupLabels = model.GroupLabels;
      Color[] seriesColors = model.SeriesColors;
      
      var pieSize = Math.Min(quadWidth/2, quadHeight/2);
      
      if(iGroup == -1)
        iGroup = 0;

      double nPies = yValues.GetUpperBound(1)+1;
      double pieTotal = 0;
        
      for (int i = 0; i < nPies; ++i)
      {
        pieTotal += yValues[iGroup,i];
      }
      
      string pathXAML = CurrentLookAndFeel.GetPiePathXAML();
      Path pathElem;
      double pieStart = 0, animAngleStart = 0;
      List<UIElement> dataElems = null;
      bool animate = (AnimationDuration > 0);
      
      if(animate)
        dataElems = DataElements;

      string gradientXAML = CurrentLookAndFeel.GetElementGradientXAML();
      TranslateTransform defaultTransform;
      
      for (int i = 0; i<nPies; ++i)
      {
        double valueRatio = 1 - (yValues[iGroup,i])/(pieTotal);
        
        double x1 = pieSize*Math.Cos(pieStart*Math.PI*2), 
               y1 = pieSize*Math.Sin(pieStart*Math.PI*2);

        StringBuilder sb = new StringBuilder();
        sb.Append("M0,0 L");
        sb.Append(x1);
        sb.Append(",").Append(y1);
        
        double x2 = pieSize* Math.Cos((pieStart+valueRatio)*Math.PI*2), 
               y2 = pieSize*Math.Sin((pieStart+valueRatio)*Math.PI*2);
        
        if (valueRatio >= .5) // major arc
        {
          sb.Append(" A").Append(pieSize).Append(" ").Append(pieSize).Append(" 1 0 0 ");
        }
        else
        {
          sb.Append(" A").Append(pieSize).Append(" ").Append(pieSize).Append(" 1 1 0 ");
        }
        sb.Append(x2);
        sb.Append(",").Append(y2);    
        sb.Append(" z");

        pathElem = CreatePathFromXAMLAndData(pathXAML, sb);
        if (animate)
        {
          dataElems.Add(pathElem);

          defaultTransform = new TranslateTransform();
          defaultTransform.X = defaultTransform.Y = -10000;
          pathElem.SetValue(Canvas.RenderTransformProperty, defaultTransform);

          double curAnimRatio = (yValues[iGroup, i]) / (pieTotal);
          _pieAnimAngles.Add(animAngleStart + curAnimRatio / 2);
          animAngleStart += curAnimRatio;
        }

        if (gradientXAML != null)
          SetGradientOnElement(pathElem, gradientXAML, seriesColors[i], 0xe5);
        else
          SetFillOnElement(pathElem, seriesColors[i]);

        pathElem.SetValue(Rectangle.StrokeProperty, new SolidColorBrush(seriesColors[i]));

        // centroid of the triangle used for tooltips
        Point centroid = new Point((x1 + x2)/3, (y1 + y2)/3);
        SetExpandosOnElement(pathElem, iGroup, i, centroid);

        if (DisplayToolTip)
        {
          pathElem.MouseEnter += new MouseEventHandler(ShowToolTip);
          pathElem.MouseLeave += new MouseEventHandler(HideToolTip);
        }

        pathElem.MouseLeftButtonUp += new MouseButtonEventHandler(ChartDataClicked);
        
        pieStart += valueRatio;
        pieContainer.Children.Add(pathElem);
      }
      
      
      /*for (var i = 0; i< nPies; ++i)
      {
        // calculate the pie gradient:
        pieContainer.appendChild(pieElems[i]);
      }*/  
    }

    private void _draw3DPies(
      Canvas pieContainer, 
      double quadWidth, 
      double quadHeight, 
      int iGroup)
    {
      ChartModel model = Model;
      double[,] yValues = model.YValues;
      string[] groupLabels = model.GroupLabels;
      Color[] seriesColors = model.SeriesColors;

      var pieSize = Math.Min(quadWidth / 2, quadHeight / 2);

      if (iGroup == -1)
        iGroup = 0;

      int nPies = yValues.GetUpperBound(1) + 1;
      double pieTotal = 0;

      for (int i = 0; i < nPies; ++i)
      {
        pieTotal += yValues[iGroup, i];
      }

      string pathXAML = CurrentLookAndFeel.GetPiePathXAML();
      Path pathElem;
      double pieStart = 0;
      List<UIElement> dataElems = null;
      bool animate = (AnimationDuration > 0);

      if (animate)
        dataElems = DataElements;

      double perspectiveHeight = pieSize / 4;
      Path[] pieElems = new Path[nPies],
             ringElems = new Path[nPies],
             edgeElems = new Path[nPies];
             
      if (perspectiveHeight > _MAX_PERSPECTIVE_HEIGHT)
        perspectiveHeight = _MAX_PERSPECTIVE_HEIGHT;

      string gradientXAML = CurrentLookAndFeel.GetElementGradientXAML();
      TranslateTransform defaultTransform;

      for (int i = 0; i < nPies; ++i)
      {
        double valueRatio = 1 - (yValues[iGroup, i]) / (pieTotal);

        double arcBeginX, arcBeginY, arcEndX, arcEndY;    
        arcBeginX = pieSize*Math.Cos(pieStart*Math.PI*2);
        arcBeginY = pieSize*Math.Sin(pieStart*Math.PI*2);

        StringBuilder sb = new StringBuilder();
        sb.Append("M0,0L").Append(arcBeginX).Append(",").Append(arcBeginY);

        arcEndX = pieSize*Math.Cos((pieStart+valueRatio)*Math.PI*2);
        arcEndY = pieSize*Math.Sin((pieStart+valueRatio)*Math.PI*2);
        
        if (valueRatio >= .5) 
        {
          sb.Append(" A").Append(pieSize).Append(",").Append(pieSize).Append(" 1 0 0 ");
        }
        else
        {
          sb.Append(" A").Append(pieSize).Append(",").Append(pieSize).Append(" 1 1 0 ");
        }
        
        sb.Append(arcEndX).Append(",").Append(arcEndY);
        sb.Append(" z");

        pathElem = CreatePathFromXAMLAndData(pathXAML, sb);
        if (animate)
        {
          dataElems.Add(pathElem);

          defaultTransform = new TranslateTransform();
          defaultTransform.X = defaultTransform.Y = -10000;
          pathElem.SetValue(Canvas.RenderTransformProperty, defaultTransform);

          _pieAnimAngles.Add(pieStart + valueRatio / 2);
        }

        if (gradientXAML != null)
          SetGradientOnElement(pathElem, gradientXAML, seriesColors[i], 0xe5);
        else
          SetFillOnElement(pathElem, seriesColors[i]);

        pathElem.SetValue(Rectangle.StrokeProperty, new SolidColorBrush(seriesColors[i]));

        // centroid of the pie triangle
        Point centroid = new Point((arcBeginX + arcEndX) / 3, (arcBeginY + arcEndY) / 3);
        SetExpandosOnElement(pathElem, iGroup, i, centroid);
        if (DisplayToolTip)
        {
          pathElem.MouseEnter += new MouseEventHandler(ShowToolTip);
          pathElem.MouseLeave += new MouseEventHandler(HideToolTip);      
        }

        pathElem.MouseLeftButtonUp += new MouseButtonEventHandler(ChartDataClicked);

               
        
        sb = new StringBuilder();
        sb.Append("M").Append(arcBeginX).Append(",").Append(arcBeginY);
        if (valueRatio >= .5) // major arc
        {
          sb.Append(" A").Append(pieSize).Append(",").Append(pieSize).Append(" 1 0 0 ");
        }
        else
        {
          sb.Append(" A").Append(pieSize).Append(" ").Append(pieSize).Append(" 1 1 0 ");
        }

        sb.Append(arcEndX).Append(",").Append(arcEndY);        
        
        sb.Append(" v").Append(perspectiveHeight);
        sb.Append(" M").Append(arcEndX).Append(",").Append(arcEndY+perspectiveHeight);
                
        if (valueRatio >= .5) // major arc
        {
          sb.Append(" A").Append(pieSize).Append(",").Append(pieSize).Append(" 1 0 1 ");
        }
        else
        {
          sb.Append(" A").Append(pieSize).Append(",").Append(pieSize).Append(" 1 1 1 ");
        }
        
        sb.Append(arcBeginX).Append(",").Append(arcBeginY+perspectiveHeight);
        sb.Append(" v").Append(-perspectiveHeight);

        Path pathRingElem = CreatePathFromXAMLAndData(pathXAML, sb);
         
        if (gradientXAML != null)
          SetGradientOnElement(pathRingElem, gradientXAML, seriesColors[i], 0xe5);
        else
          SetFillOnElement(pathRingElem, seriesColors[i]);

        pathRingElem.SetValue(Rectangle.StrokeProperty, new SolidColorBrush(seriesColors[i]));

        sb = new StringBuilder();
        sb.Append("M0,0L");
        sb.Append(arcBeginX).Append(",").Append(arcBeginY);
        sb.Append("v").Append(perspectiveHeight);
        sb.Append("L").Append(0).Append(",").Append(perspectiveHeight);
        sb.Append("z");
        sb.Append("M0,0L");
        sb.Append(arcEndX).Append(",").Append(arcEndY);
        sb.Append("v").Append(perspectiveHeight);
        sb.Append("L").Append(0).Append(",").Append(perspectiveHeight);
        sb.Append("z");


        Path pathEdgeElem = CreatePathFromXAMLAndData(pathXAML, sb);

        if (animate)
        {
          dataElems.Add(pathRingElem);

          defaultTransform = new TranslateTransform();
          defaultTransform.X = defaultTransform.Y = -10000;
          pathRingElem.SetValue(Canvas.RenderTransformProperty, defaultTransform);
          dataElems.Add(pathEdgeElem);

          defaultTransform = new TranslateTransform();
          defaultTransform.X = defaultTransform.Y = -10000;
          pathEdgeElem.SetValue(Canvas.RenderTransformProperty, defaultTransform);
        }

        (pathEdgeElem.Data as PathGeometry).FillRule = FillRule.Nonzero;

        if (gradientXAML != null)
          SetGradientOnElement(pathEdgeElem, gradientXAML, seriesColors[i], 0xe5);
        else
          SetFillOnElement(pathEdgeElem, seriesColors[i]);

        pathEdgeElem.SetValue(Rectangle.StrokeProperty, new SolidColorBrush(seriesColors[i]));

        if (DisplayToolTip)
        {
          pathRingElem.MouseEnter += new MouseEventHandler(ShowToolTip);
          pathRingElem.MouseLeave += new MouseEventHandler(HideToolTip);
          pathEdgeElem.MouseEnter += new MouseEventHandler(ShowToolTip);
          pathEdgeElem.MouseLeave += new MouseEventHandler(HideToolTip);
        }
        SetExpandosOnElement(pathRingElem, iGroup, i, centroid);
        SetExpandosOnElement(pathEdgeElem, iGroup, i, centroid);
        
        pieStart += valueRatio;
        pieElems[i] = pathElem;
        ringElems[i] = pathRingElem;
        edgeElems[i] = pathEdgeElem;
      }
      
      UIElementCollection children = pieContainer.Children;
      
      // For the top half, edges have preference over rings
      double totalRatio = 0;
      for (int i = 0; i< nPies; ++i)
      {
        if(totalRatio <= .5)
          children.Add(ringElems[i]);
        totalRatio += (yValues[iGroup,i])/(pieTotal);
      }
      
      totalRatio = 0;
      for (int i = 0; i< nPies; ++i)
      {
        if(totalRatio <= .5)
          children.Add(edgeElems[i]);
        totalRatio += (yValues[iGroup,i])/(pieTotal);
      }
      
      // For the bottom half, rings have preference over edges
      totalRatio = 0;
      for (int i = 0; i< nPies; ++i)
      {
        if(totalRatio > .5)
          children.Add(edgeElems[i]);
        totalRatio += (yValues[iGroup,i])/(pieTotal);
      }

      totalRatio = 0;
      for (int i = 0; i< nPies; ++i)
      {
        if(totalRatio > .5)
          children.Add(ringElems[i]);
        totalRatio += (yValues[iGroup,i])/(pieTotal);
      }  
      
      for (int i = 0; i< nPies; ++i)
      {
        children.Add(pieElems[i]);
      }
    }

    /// <summary>
    /// Overrides the default location for the tooltip location
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="ttBounds"></param>
    /// <returns></returns>
    protected override Point GetToolTipLocation(object sender, MouseEventArgs e, Size ttBounds)
    {
      Point pt = base.GetToolTipLocation(sender, e, ttBounds);
      Canvas parent = (Canvas)((sender as FrameworkElement).Parent);
      
      if(IsPerspective)
      {
        Matrix m = (parent.RenderTransform as MatrixTransform).Matrix;
        pt = new Point(pt.X + m.OffsetX, pt.Y*m.M22 + m.OffsetY);        
      }
      else
      {
        TranslateTransform tt = (parent.RenderTransform as TranslateTransform);
        pt = new Point(pt.X + tt.X, pt.Y + tt.Y);
      }
      return pt;
    }
        
    private List<double> _pieAnimRadii, _pieAnimAngles;
    private int _currentPieIndex = 0;
    private const int _MAX_PERSPECTIVE_HEIGHT = 30;
  }
}
