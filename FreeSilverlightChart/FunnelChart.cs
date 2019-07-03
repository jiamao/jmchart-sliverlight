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
  public class FunnelChart : Chart
  {
    public FunnelChart(ChartType type, ChartModel model) : base(type, model)
    {
    }
    
    public override void SetDataAnimStep(double ratio)
    {
      List<UIElement> animElems = DataElements;
      int animCount = animElems.Count;
      
      Matrix m = new Matrix(ratio, 0, 0, ratio, 0, 0);
      
      for(int i = 0; i < animCount; ++i)
      {
        MatrixTransform transform = animElems[i].RenderTransform as MatrixTransform;
        transform.Matrix = m;
      }
    }

    public override void DrawChartData()
    {          
      // calculate the number of rows and columns
      ChartModel model = Model;
      double [,] yValues = model.YValues;
      int yValueCount = yValues.GetUpperBound(0)+1;
      string[] groupLabels = model.GroupLabels;
      int groupCount = (groupLabels!=null)?groupLabels.Length:1;
          
      int nCols = (int)Math.Ceiling(Math.Sqrt(yValueCount)), 
          nRows = (int)Math.Round(Math.Sqrt(yValueCount));
      
      double dx=MarginLeft, dy=MarginTop;
      double quadWidth = (ChartCanvas.Width - MarginLeft - MarginRight)/(double)nCols;
      
      // We do not need any gap for 3D pie chart because of the vertical scaling
      double vGap = IsPerspective?0:2.0*_TEXT_MARGIN;
      double quadHeight = (ChartCanvas.Height - MarginTop - MarginBottom - (nRows-1)*vGap)/(double)nRows;
      LookAndFeel lf = CurrentLookAndFeel;
      string labelXAML = lf.GetGroupLabelXAML();
      TextBlock labelElem = null;
      
      for(int i = 0; i<nRows; ++i)
      {
        for(int j = 0; j<nCols; ++j)
        {  
          int iGroup = (groupLabels != null)?(i*nCols + j):(-1);
          if(iGroup >= yValueCount)
            break;
          
          string groupLabel = (iGroup == -1)?null:groupLabels[iGroup];
            
          Canvas fnlContainer = new Canvas();
          
          ChartCanvas.Children.Add(fnlContainer);

          double newHeight = DrawGroupLabelTitle(groupLabel, ChartCanvas, labelXAML, ref labelElem, 
                                                 dx, dy, quadWidth, quadHeight);
          double newWidth = quadWidth - 2*_TEXT_MARGIN;

          TranslateTransform transform = new TranslateTransform();
          if(IsPerspective)
          {
            // Top ring has a height of 1/6 that of the width. So we need to compensate for half of it.
            newHeight -= newWidth/6;
            _drawPerspectiveFunnel(fnlContainer, newWidth, newHeight, iGroup);
            transform.X = (dx+_TEXT_MARGIN);
            transform.Y = (dy+newWidth/12);
          }
          else
          {
            _drawFunnel(fnlContainer, newWidth, newHeight, iGroup);
            transform.X = dx + _TEXT_MARGIN;
            transform.Y = dy;
          }
          
          fnlContainer.RenderTransform = transform;
          dx +=quadWidth;
        }
        dx = MarginLeft;
        dy +=quadHeight+vGap;
      }  
    }

   /// <summary>
    /// Overridden to do nothing for funnel charts
    /// </summary>
    protected override void ComputeMinMaxValues()
    {

    }

    /// <summary>
    /// Overridden to do nothing for funnel charts
    /// </summary>
    protected override void DrawGroupLabels()
    {

    }

    /// <summary>
    /// Overridden to do nothing for funnel charts
    /// </summary>
    protected override void LayoutGroupLabels()
    {

    }

    /// <summary>
    /// Overridden to do nothing for funnel charts
    /// </summary>
    protected override void DrawGrid()
    {

    }

    /// <summary>
    /// Overridden to do nothing for funnel charts
    /// </summary>
    protected override void DrawYValueLabels()
    {

    }

    /// <summary>
    /// Overridden to do nothing for funnel charts
    /// </summary>
    protected override void LayoutYValueLabels()
    {

    }

    private void _drawFunnel(
      Canvas fnlContainer, 
      double quadWidth, 
      double quadHeight, 
      int iGroup)
    {
      ChartModel model = Model;
      
      double[,] yValues = model.YValues;
      string [] groupLabels = model.GroupLabels;
      Color []seriesColors = model.SeriesColors;

      if(iGroup == -1)
        iGroup = 0;
      
      // Number of segments
      int nSeg = yValues.GetUpperBound(1) +1;
      double total = 0;
      
      for (int i = 0; i < nSeg; ++i)
      {
        total += yValues[iGroup,i];
      }
      
      List<UIElement> dataElems = DataElements;
      bool animate = (AnimationDuration>0);
      
      string funnelXAML = CurrentLookAndFeel.GetFunnelPathXAML();
      Path pathElem;
      string gradientXAML = CurrentLookAndFeel.GetElementGradientXAML();
      MatrixTransform defaultTransform;
      
      double x = 0, y = 0, slope = (quadWidth/2)/quadHeight,
             dx = quadWidth, dy, nextX, nextY;

      for (int i = nSeg-1; i >= 0; --i)
      {
        
        
        double valueRatio = (yValues[iGroup,i])/(total);
        StringBuilder sb = new StringBuilder();
        sb.Append("M").Append(x).Append(",").Append(y);
        sb.Append(" L").Append(dx).Append(",").Append(y);
        
        dy = (quadHeight)*valueRatio;
        nextY   = y + dy;
        nextX = quadWidth/2 - slope*(quadHeight-(nextY) );
        dx = quadWidth - nextX;
        
        if(i != 0)
        {
          sb.Append(" L").Append(dx).Append(",").Append(nextY);
          sb.Append(" L").Append(nextX).Append(",").Append(nextY);
          sb.Append(" Z");
        }
        else
        {
          double startTipY = (dy/3.0<=_MAX_FUNNEL_TIP)?y+(dy - dy/3.0):
                              quadHeight-_MAX_FUNNEL_TIP;
          
          nextX = quadWidth/2 - slope*(quadHeight-(startTipY) );
          dx = quadWidth - nextX;
          sb.Append("L").Append(dx).Append(",").Append(startTipY);
          sb.Append("L").Append(dx).Append(",").Append(quadHeight);
          sb.Append("L").Append(nextX).Append(",").Append(quadHeight);
          sb.Append("L").Append(nextX).Append(",").Append(startTipY);
          sb.Append("Z");
        }

        pathElem = CreatePathFromXAMLAndData(funnelXAML, sb);

        if (animate)
        {
          dataElems.Add(pathElem);
          defaultTransform = new MatrixTransform();
          Matrix m = new Matrix(0, 0, 0, 0, 0, 0);
          defaultTransform.Matrix = m;
          pathElem.RenderTransform = defaultTransform;
        }

        if (gradientXAML != null)
        {
          SetGradientOnElement(pathElem, gradientXAML, seriesColors[i], 0xE5);
        }
        else
        {
          SetFillOnElement(pathElem, seriesColors[i]);
        }
        
        pathElem.SetValue(Path.StrokeProperty, new SolidColorBrush(seriesColors[i]));

        SetExpandosOnElement(pathElem, iGroup, i, new Point(nextX+(dx-nextX)/2.0, y + (nextY-y)/2.0));
        if (DisplayToolTip)
        {
          pathElem.MouseEnter += new MouseEventHandler(ShowToolTip);
          pathElem.MouseLeave += new MouseEventHandler(HideToolTip);
        }
        
        pathElem.MouseLeftButtonUp += new MouseButtonEventHandler(ChartDataClicked);
        
        fnlContainer.Children.Add(pathElem);
        y = nextY;
        x = nextX;
      }  
    }

    private void _drawPerspectiveFunnel(
      Canvas fnlContainer, 
      double quadWidth, 
      double quadHeight, 
      int iGroup)
    {
      ChartModel model = Model;
      
      double[,] yValues = model.YValues;
      string [] groupLabels = model.GroupLabels;
      Color []seriesColors = model.SeriesColors;

      if(iGroup == -1)
        iGroup = 0;
      
      // Number of segments
      int nSeg = yValues.GetUpperBound(1) +1;
      double total = 0;
      
      for (int i = 0; i < nSeg; ++i)
      {
        total += yValues[iGroup,i];
      }
      
      List<UIElement> dataElems = DataElements;
      bool animate = (AnimationDuration>0);

      string funnelXAML = CurrentLookAndFeel.GetFunnelPathXAML();
      Path pathElem;
      string gradientXAML = CurrentLookAndFeel.GetElementGradientXAML();
      MatrixTransform defaultTransform;
            
      double x = 0, y = 0, slope = (quadWidth/2.0)/quadHeight,
             dx = quadWidth, dy, nextX, oldDx, nextY;

      // the ring height is 1/12 of the width
      double rx = dx/2.0, ry = dx/24.0, oldRx, oldRy;
      
      for (int i = nSeg-1; i >= 0; --i)
      {        
        double valueRatio = (yValues[iGroup,i])/(total);
        StringBuilder sb = new StringBuilder();
        
        sb.Append("M").Append(x).Append(",").Append(y);
        sb.Append(" A").Append(rx).Append(",").Append(ry);
        sb.Append(" 0 1,0 ").Append(dx).Append(",").Append(y);
        sb.Append(" A").Append(rx).Append(",").Append(ry);
        sb.Append(" 0 1,0 ").Append(x).Append(",").Append(y);

        oldDx = dx;
        oldRx = rx;
        oldRy = ry;    
        dy = (quadHeight)*valueRatio;
        nextY  = y + dy;
        nextX = quadWidth/2.0 - slope*(quadHeight-(nextY) );
        dx = quadWidth - nextX;
        rx = (dx-nextX)/2.0;
        ry = rx/12.0;
        
        if(i != 0)
        {
          sb.Append("L").Append(nextX).Append(",").Append(nextY);
          sb.Append("A").Append(rx).Append(",").Append(ry);
          sb.Append(" 0 1,0 ").Append(dx).Append(",").Append(nextY);
          sb.Append("L").Append(oldDx).Append(",").Append(y);
        }
        else
        {
          double startTipY = (dy/3.0<=_MAX_FUNNEL_TIP)?y+(dy - dy/3.0):
                           quadHeight-_MAX_FUNNEL_TIP;
                           
          nextX = quadWidth/2.0 - slope*(quadHeight-(startTipY));
          dx = quadWidth - nextX;
          
          rx = (dx-nextX)/2.0;
          ry = rx/12.0;
          
          sb.Append(" L").Append(nextX).Append(",").Append(startTipY);
          sb.Append(" L").Append(nextX).Append(",").Append(quadHeight);
          sb.Append(" A").Append(rx).Append(",").Append(ry);
          sb.Append(" 0 1,0 ").Append(dx).Append(",").Append(quadHeight);
          sb.Append(" A").Append(rx).Append(",").Append(ry);
          sb.Append(" 0 1,0 ").Append(nextX).Append(",").Append(quadHeight);
          sb.Append(" A").Append(rx).Append(",").Append(ry);
          sb.Append(" 0 1,0 ").Append(dx).Append(",").Append(quadHeight);
          sb.Append(" L").Append(dx).Append(",").Append(startTipY);
          sb.Append(" L").Append(oldDx).Append(",").Append(y);
        }

        pathElem = CreatePathFromXAMLAndData(funnelXAML, sb);

        if (animate)
        {
          dataElems.Add(pathElem);
          defaultTransform = new MatrixTransform();
          Matrix m = new Matrix(0, 0, 0, 0, 0, 0);
          defaultTransform.Matrix = m;
          pathElem.RenderTransform = defaultTransform;
        }

        if (gradientXAML != null)
        {
          SetGradientOnElement(pathElem, gradientXAML, seriesColors[i], 0xE5);
        }
        else
        {
          SetFillOnElement(pathElem, seriesColors[i]);
        }
        
        (pathElem.Data as PathGeometry).FillRule = FillRule.Nonzero;
        pathElem.SetValue(Path.StrokeProperty, new SolidColorBrush(seriesColors[i]));
        pathElem.SetValue(Canvas.ZIndexProperty, i+1);

        SetExpandosOnElement(pathElem, iGroup, i, new Point(nextX+(dx-nextX)/2.0, y + (nextY-y)/2.0));
        if (DisplayToolTip)
        {
          pathElem.MouseEnter += new MouseEventHandler(ShowToolTip);
          pathElem.MouseLeave += new MouseEventHandler(HideToolTip);
        }

        pathElem.MouseLeftButtonUp += new MouseButtonEventHandler(ChartDataClicked);
        
        fnlContainer.Children.Add(pathElem);
        y = nextY;
        x = nextX;
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

      TranslateTransform tt = (parent.RenderTransform as TranslateTransform);
      pt = new Point(pt.X + tt.X, pt.Y + tt.Y);        
      return pt;
    }
    
    private const int _MAX_FUNNEL_TIP = 16;

  }
}
