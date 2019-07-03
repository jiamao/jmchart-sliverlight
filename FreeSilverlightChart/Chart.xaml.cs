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
  public class Chart : Canvas
  {
  
    static Chart()
    {
      // Register the basic look and feel
      LookAndFeel lf = new GradientLookAndFeel();
      _LookAndFeelMap.Add(lf.GetId(), lf);

      _currentLookAndFeelId = lf.GetId();
      _currentLookAndFeel = lf;
      
      // register another sample lood and feel
      //lf = new TransparentLookAndFeel();
      //_LookAndFeelMap.Add(lf.GetId(), lf); 
    }
    
    /// <summary>
    /// Default constructor. Plese Do not use this. This is only provided 
    /// so that the assembly can be loaded from the XAML file
    /// </summary>
    public Chart()
    {    
    }
    
    public Chart(ChartType type, ChartModel model)
    {
      _model = model;
      _type = type;

      // Initialize the default values
      _initDefaultValues();
      
      System.IO.Stream s = this.GetType().Assembly.GetManifestResourceStream("FreeSilverlightChart.Chart.xaml");
      _chartCanvas = XamlReader.Load(new System.IO.StreamReader(s).ReadToEnd()) as Canvas;
      _timer = _chartCanvas.FindName("timer") as Storyboard;
      this.Children.Add(_chartCanvas);
    }


    public static Chart CreateChart(ChartType type, ChartModel model)
    {
      Chart chart = null;
      if (type == ChartType.VBAR || 
          type == ChartType.VBAR_STACKED || 
          type == ChartType.BAR_LINE_COMBO ||
          type == ChartType.BAR_AREA_COMBO ||
          type == ChartType.BAR_LINE_AREA_COMBO)
      {
        chart = new BarChart(type, model);
      }
      else if (type == ChartType.HBAR || type == ChartType.HBAR_STACKED)
      {
        chart = new HorizontalBarChart(type, model);
      }
      else if( type == ChartType.CYLINDERBAR)
      {
        chart = new CylinderBarChart(type, model);
      }
      
      else if (type == ChartType.PIE)
      {
        chart = new PieChart(type, model);
      }
      else if (type == ChartType.AREA || type == ChartType.AREA_STACKED)
      {
        chart = new AreaChart(type, model);
      }
      else if (type == ChartType.LINE)
      {
        chart = new LineChart(type, model);
      }
      else if (type == ChartType.SCATTER_PLOT)
      {
        chart = new ScatterPlotChart(type, model);
      }
      else if (type == ChartType.XYLINE)
      {
        chart = new XYLineChart(type, model);
      }
      else if (type == ChartType.RADAR || type == ChartType.RADAR_AREA)
      {
        chart = new RadarChart(type, model);
      }
      else if (type == ChartType.FUNNEL)
      {
        chart = new FunnelChart(type, model);
      }
      else if (type == ChartType.SEMI_CIRCULAR_GAUGE)
      {
        chart = new SemiCircularGaugeChart(type, model);
      }
      else if (type == ChartType.CIRCULAR_GAUGE)
      {
        chart = new GaugeChart(type, model);
      }
      else if (type == ChartType.CANDLE_STICK)
      {
        chart = new CandleStickChart(type, model);
      }
      return chart;
    }

    public static void RegisterLookAndFeel(LookAndFeel lAndF)
    {
      _LookAndFeelMap.Add(lAndF.GetId(), lAndF);
      // set the current look and feel to the newly registered ones
      _currentLookAndFeelId = lAndF.GetId();
      _currentLookAndFeel = lAndF;
    }

    public static string[] GetLookAndFeelIds()
    {
      Dictionary<string, LookAndFeel>.KeyCollection keys = _LookAndFeelMap.Keys;

      string [] ids = new string[keys.Count];
      keys.CopyTo(ids, 0);

      return ids;
    }

    private void _initDefaultValues()
    {
      _legendPosition = LegendLocation.BOTTOM;
      _isPerspective = true;
      _displayToolTip = true;
      _animationDuration = 1.5;
      _yMajorGridCount = 7;
      _yMinorGridCount = -1;
      _xMajorGridCount = -1;
      _format = "$#,##0.00;($#,##0.00);0";
      
      _expandos = new Dictionary<object, ElementExpandos>();
    }
    
    protected virtual void ComputeMinMaxValues()
    {
      double [,] yValues = _model.YValues;
      double [,] xValues = _model.XValues;
      
      double maxYValue = _model.MaxYValue;
      double maxXValue = _model.MaxXValue;
      
      double minYValue = _model.MinYValue;
      double minXValue = _model.MinXValue;
      
      string [] seriesLabels = _model.SeriesLabels;

      if (yValues != null && (double.IsNegativeInfinity(maxYValue) || double.IsPositiveInfinity(minYValue)))
      {
        _computeAxisMinMaxValues(yValues, seriesLabels.Length, ref maxYValue, ref minYValue);
        _model.MaxYValue = maxYValue;
        _model.MinYValue = minYValue;
      }

      if (xValues != null && (double.IsNegativeInfinity(maxXValue) || double.IsPositiveInfinity(minXValue)))
      {
        _computeAxisMinMaxValues(xValues, seriesLabels.Length, ref maxXValue, ref minXValue);
        _model.MaxXValue = maxXValue;
        _model.MinXValue = minXValue;
      }
    }

    private void _computeAxisMinMaxValues(double[,] values, int seriesSize, ref double maxValue, ref double minValue)
    {
      double stackedTotal, value;
      double curMaxValue = Double.NegativeInfinity;
      double curMinValue = Double.PositiveInfinity;
       
      bool isStacked = false;
      int groupsCount = values.GetUpperBound(0)+1;
      
      if(_type == ChartType.VBAR_STACKED || _type == ChartType.HBAR_STACKED || _type == ChartType.AREA_STACKED)
      {
        isStacked = true;
      }
      
      for (int i = 0; i < groupsCount; ++i)
      {
        stackedTotal = 0.0;
        
        for (int j = 0; j < seriesSize; ++j)
        {
          value = values[i,j];
          if (isStacked)
            stackedTotal += value;
          else
          {
            curMaxValue = Math.Max(curMaxValue, value);
            curMinValue = Math.Min(curMinValue, value);
          }
        }
        
        if (isStacked)
        {
          curMaxValue = Math.Max(curMaxValue, stackedTotal);
          curMinValue = Math.Min(curMinValue, stackedTotal);
        }
      }

      double maxMult = curMaxValue > 0 ? _MAX_MULTIPLIER : _MIN_MULTIPLIER;
      double minMult = curMinValue > 0 ? _MIN_MULTIPLIER : _MAX_MULTIPLIER;

      if (double.IsNegativeInfinity(maxValue))
        maxValue = curMaxValue * maxMult;
      if (double.IsPositiveInfinity(minValue))
        minValue = curMinValue * minMult;
    }
    
    /// <summary>
    /// Draws the chart control. 
    /// </summary>
    public void Draw()
    {
      // make sure to clear the chart first
      clear();

      ComputeMinMaxValues();
      
      
      // margins around our drawing area. The margins increase as we put labels,
      // legend etc.
      _marginLeft = _marginTop = _marginRight = _marginBottom = 2;

      // Initialize any gradients that we need
      InitializeGradients();
            
      // Note the ordering is important. The grid takes the space after the title etc.
      DrawBorder();
      DrawTitles();
      
      bool isHorizontal = IsHorizontalChart();
      
      // First just draw the label elements so that we can estimate the space requirements
      if(!isHorizontal)
      {
        DrawGroupLabels();
        DrawYValueLabels();
      }
      else
      {
        DrawHChartGroupLabels();
        DrawHChartYValueLabels();      
      }
            
      // Now adjust margins based on the labels
      AdjustMarginsForGroupLabels();
      AdjustMarginsForYLabels();
      
      // Now start drawing the graph so that it gobbles the left over space
      DrawLegend();
      if(!isHorizontal)
        LayoutGroupLabels();
      else
        LayoutHChartYValueLabels();
      
      if (!isHorizontal)
        LayoutYValueLabels();
      else
        LayoutHChartGroupLabels();
      
      DrawGrid();
      DrawChartData();
      
      if(_animationDuration > 0)
        Animate();
    }
    
    
    /// <summary>
    /// Setups the attributes in the delegate chart so that it can be used to draw combo data
    /// </summary>
    /// <param name="source"></param>
    /// <param name="?"></param>
    protected virtual void SetupDelegateChart(Chart source, Chart destination)
    {
      destination._chartCanvas = source._chartCanvas;
      destination._marginLeft = source._marginLeft;
      destination._marginRight = source._marginRight;
      destination._marginTop = source._marginTop;
      destination._marginBottom = source._marginBottom;
      destination._isPerspective = source._isPerspective;
      destination._displayToolTip = source._displayToolTip;
    }

    /// <summary>
    /// Sets the bounds of the chart control
    /// </summary>
    /// <param name="bounds"> A rect object contains the bounds for the chart</param>
    public void SetBounds(Rect bounds)
    {
      _chartCanvas.SetValue(Canvas.LeftProperty, bounds.Left);
      _chartCanvas.SetValue(Canvas.TopProperty, bounds.Top);
      _chartCanvas.SetValue(Canvas.WidthProperty, bounds.Width);
      _chartCanvas.SetValue(Canvas.HeightProperty, bounds.Height);
    }
    
    /// <summary>
    /// Draws the border for the chart component
    /// </summary>
    protected virtual void DrawBorder()
    {
      double borderSize = _currentLookAndFeel.GetBorderSize();
      if(borderSize>0)
      {
        double stroke = borderSize/2;
        Rectangle rectElem = XamlReader.Load(_currentLookAndFeel.GetBorderXAML()) as Rectangle;
        
        rectElem.SetValue(Canvas.LeftProperty, 0.0);
        rectElem.SetValue(Canvas.TopProperty, 0.0);
        
        rectElem.SetValue(Rectangle.StrokeThicknessProperty, stroke);

        rectElem.SetValue(Rectangle.RadiusXProperty, stroke);
        rectElem.SetValue(Rectangle.RadiusYProperty, stroke);
        rectElem.SetValue(Rectangle.WidthProperty, _chartCanvas.Width - stroke);
        rectElem.SetValue(Rectangle.HeightProperty, _chartCanvas.Height - stroke);

        _chartCanvas.Children.Add(rectElem);

        _marginLeft += borderSize;
        _marginRight += borderSize;
        _marginTop += borderSize;
        _marginBottom += borderSize;
      }
    }

    /// <summary>
    /// Draws the titles for the chart component
    /// </summary>
    protected virtual void DrawTitles()
    {
      string title = _model.Title, 
             subTitle = _model.SubTitle, 
             footNote = _model.FootNote;
      
      if (title != null)
        _drawTitleElem(_currentLookAndFeel.GetTitleXAML(), title, false);

      if (subTitle != null)
        _drawTitleElem(_currentLookAndFeel.GetSubTitleXAML(), subTitle, false);

      if (footNote != null)
        _drawTitleElem(_currentLookAndFeel.GetFootNoteXAML(), footNote, true);
    }

    private void _drawTitleElem(string xaml, string title, bool isFooter)
    {
      double width = _chartCanvas.Width, height = _chartCanvas.ActualHeight;
      double gridWidth = (width - _marginLeft - _marginRight);

      TextBlock textElem = XamlReader.Load(xaml) as TextBlock;
      
      if(_animationDuration > 0)
      {
        LabelElements.Add(textElem);
        textElem.Opacity = 0.0;
      }

      textElem.Text = title;

      _chartCanvas.Children.Add(textElem);
      
      double dx = _marginLeft;
      double textWidth = textElem.ActualWidth;
      double textHeight = textElem.ActualHeight;
      
      if(isFooter && width > textWidth + _marginRight)
        dx = (width-textWidth)-_marginRight;
      
      if(!isFooter && gridWidth > textWidth)
		    dx = (gridWidth-textWidth)/2+ _marginLeft;
        
      textElem.SetValue(Canvas.LeftProperty,dx);
      if(isFooter)
      {
        textElem.SetValue(Canvas.TopProperty, height - textHeight - _marginBottom);
        _marginBottom += textHeight + _TEXT_MARGIN;
      }
      else
      {
        textElem.SetValue(Canvas.TopProperty, _marginTop);
        _marginTop += textHeight + _TEXT_MARGIN;
      }
    }

    /// <summary>
    /// Indicates if the chart is a horizontal chart. By default charts are vertical
    /// </summary>
    /// <returns>true for horizontal charts otherwise false</returns>
    protected virtual bool IsHorizontalChart()
    {
      // By default charts are vertical charts
      return false;
    }
        
    /// <summary>
    /// Draws the group labels for the chart component
    /// </summary>
    protected virtual void DrawGroupLabels()
    {
      _hLabelContainer = new Canvas();
      
      int vLineCount = GetVLineCount();

      _groupLabelElems = new TextBlock[vLineCount];
      
      TextBlock labelElem;      
      string labelText;
      string labelXAML = _currentLookAndFeel.GetGroupLabelXAML();
      
      bool animate = (_animationDuration > 0);
      List<UIElement> labelElems = LabelElements;
      
      double maxWidth = 0, maxHeight = 0;
      UIElementCollection children = _hLabelContainer.Children;
      for (var i = 0; i < vLineCount; ++i)
      {
        // draw the horizontal label
        labelText = GetGroupLabelAtIndex(i);
        if ((labelText == null) || (labelText.Length == 0))
          continue;

        labelElem = XamlReader.Load(labelXAML) as TextBlock;
        if (animate)
        {
          labelElems.Add(labelElem);
          labelElem.Opacity = 0;
        }
        labelElem.Text = labelText;
        children.Add(labelElem);
        _groupLabelElems[i] = labelElem;

        if (_rotateHorizontalLabels)
        {
          RotateTransform rt = new RotateTransform();
          rt.Angle = -45;
          labelElem.RenderTransform = rt;
          var sideValue = labelElem.ActualWidth / Math.Sqrt(2);

          maxWidth = Math.Max(maxWidth, sideValue + labelElem.ActualHeight / Math.Sqrt(2));
          maxHeight = Math.Max(maxHeight, sideValue + labelElem.ActualHeight*Math.Sqrt(2));
        }
        else
        {
          maxWidth = Math.Max(maxWidth, labelElem.ActualWidth);
          maxHeight = Math.Max(maxHeight, labelElem.ActualHeight);
        }
      }

      _chartCanvas.Children.Add(_hLabelContainer);
      _hLabelBounds = new Size(maxWidth, maxHeight);
    }

    /// <summary>
    /// Returns the group label at a particular index.
    /// This method can be overriden by XY charts to return the X value instead
    /// </summary>
    /// <param name="index"></param>
    /// <returns>string representing the label text</returns>
    protected virtual string GetGroupLabelAtIndex(int index)
    {
      return _model.GroupLabels[index];
    }
    
    /// <summary>
    /// Draws the group labels for the horizontal chart components
    /// </summary>
    protected virtual void DrawHChartGroupLabels()
    {
      // Since the horizontal bar chart is flipped Y labels are horizontal
      _vLabelContainer = new Canvas();

      string[] groupLabels = Model.GroupLabels;
      int hLineCount = groupLabels.Length;

      _groupLabelElems = new TextBlock[hLineCount];
      
      TextBlock labelElem;
      string labelText;
      string labelXAML = _currentLookAndFeel.GetGroupLabelXAML();

      bool animate = (_animationDuration > 0);
      List<UIElement> labelElems = LabelElements;

      UIElementCollection children = _vLabelContainer.Children;
      double maxWidth = 0, maxHeight = 0;

      // horizontal lines
      for (int i = 0; i < hLineCount; ++i)
      {
        labelText = groupLabels[i];
        if (labelText == null || labelText.Length == 0)
          continue;

        labelElem = XamlReader.Load(labelXAML) as TextBlock;
        if (animate)
        {
          labelElems.Add(labelElem);
          labelElem.Opacity = 0;
        }

        labelElem.Text = labelText;
        children.Add(labelElem);
        _groupLabelElems[i] = labelElem;
        maxWidth = Math.Max(maxWidth, labelElem.ActualWidth);
        maxHeight = Math.Max(maxHeight, labelElem.ActualHeight);
      }

      _chartCanvas.Children.Add(_vLabelContainer);
      _vLabelBounds = new Size(maxWidth, maxHeight);
    }
    
    /// <summary>
    /// Draws the yValue labels for the chart component
    /// </summary>
    protected virtual void DrawYValueLabels()
    {
      _vLabelContainer = new Canvas();
      double maxWidth = 0, maxHeight = 0;

      double minValue = _model.MinYValue, maxValue = _model.MaxYValue;
      string labelXAML = _currentLookAndFeel.GetYValueLabelXAML();

      _addVLabelIntoContainer(minValue, labelXAML, _vLabelContainer, ref maxWidth, ref maxHeight);
      _addVLabelIntoContainer(maxValue, labelXAML, _vLabelContainer, ref maxWidth, ref maxHeight);
      
      var hLineCount = _yMajorGridCount;
      // horizontal lines
      for (var i = 0; i < hLineCount - 1; ++i)
      {
        double value = ((maxValue - minValue) * (i + 1) / hLineCount) + minValue;
        _addVLabelIntoContainer(value, labelXAML, _vLabelContainer, ref maxWidth, ref maxHeight);
      }

      _chartCanvas.Children.Add(_vLabelContainer);
      _vLabelBounds = new Size(maxWidth, maxHeight);
    }

    /// <summary>
    /// Draws the yValue labels for the horizontal chart component
    /// </summary>
    protected virtual void DrawHChartYValueLabels()
    {
      var vLineCount = _yMajorGridCount;

      // Since the horizontal bar chart is flipped group labels are vertical
      _hLabelContainer = new Canvas();
      
      double minValue = _model.MinYValue, maxValue = _model.MaxYValue;
      string labelXAML = _currentLookAndFeel.GetYValueLabelXAML();

      List<UIElement> labelElems = LabelElements;
      bool animate = (_animationDuration>0);
      double value;
      double maxWidth = 0, maxHeight = 0;
      
      for (int i = 0; i< vLineCount+1; ++i)
      {
        // draw the horizontal label
        TextBlock labelElem = XamlReader.Load(labelXAML) as TextBlock;
        
        if(animate)
        {
          labelElems.Add(labelElem);
          labelElem.Opacity = 0;
        }
        
        if(i==0)
          value = minValue;
        else if(i==vLineCount)
          value = maxValue;
        else
          value = (((maxValue-minValue)*(i)/vLineCount) + minValue);
          
        labelElem.Text = value.ToString(_format);
        _hLabelContainer.Children.Add(labelElem);
        maxWidth = Math.Max(maxWidth, labelElem.ActualWidth);
        maxHeight = Math.Max(maxHeight, labelElem.ActualHeight);
      }

      _chartCanvas.Children.Add(_hLabelContainer);
      _hLabelBounds = new Size(maxWidth, maxHeight);
    }

    private void _addVLabelIntoContainer(
      double value, 
      string labelXAML, 
      Canvas container,
      ref double maxWidth,
      ref double maxHeight)
    {
      TextBlock labelElem = XamlReader.Load(labelXAML) as TextBlock;
      if (_animationDuration > 0)
      {
        LabelElements.Add(labelElem);
        labelElem.Opacity = 0.0;
      }
      labelElem.Text = value.ToString(_format);
      container.Children.Add(labelElem);
      maxWidth = Math.Max(maxWidth, labelElem.ActualWidth);
      maxHeight = Math.Max(maxHeight, labelElem.ActualHeight);
    }
    
    /// <summary>
    /// Indicates if the group label should be center aligned or edge aligned
    /// </summary>
    /// <returns>
    /// true(String) indicates center aligned, false indicates it is edge aligned
    /// </returns>
    protected virtual bool IsGroupLabelCentered()
    {
      return true;
    }

    /// <summary>
    /// Adjust the margins based on the dimension of the group labels
    /// </summary>
    protected virtual void AdjustMarginsForGroupLabels()
    {
      if (_hLabelContainer != null && (_hLabelContainer.Children.Count > 0))
      {
        _marginBottom += _hLabelBounds.Height + _TEXT_MARGIN;
        bool isCentered = this.IsGroupLabelCentered();
        if (!isCentered)
        {
          var textWidth = (_hLabelContainer.Children[_hLabelContainer.Children.Count-1] as TextBlock).ActualWidth;
          if ((textWidth / 2) > _marginRight)
            _marginRight += (textWidth/2);
        }
      }
    }

    /// <summary>
    /// Adjusts the margins for the yLabels to be layed out properly
    /// </summary>
    protected virtual void AdjustMarginsForYLabels()
    {
      if (_vLabelContainer != null && _vLabelContainer.Children.Count > 0)
        _marginLeft += _vLabelBounds.Width + _TEXT_MARGIN;
    }

    /// <summary>
    /// Draws the legend for the chart component
    /// </summary>
    protected virtual void DrawLegend()
    {
      if(_legendPosition == LegendLocation.NONE)
      {
        return;
      }
      
      string [] seriesLabels = _model.SeriesLabels;
      int seriesCount = seriesLabels.Length;
      
      Color[] seriesColors = _model.SeriesColors;

      UIElement rectElem;
      TextBlock labelElem;
      double iconHeight = 0.0, iconWidth = 0.0;
      double marginLeft =  _marginLeft;
      Canvas legendGroup = new Canvas();
      bool animate = (_animationDuration>0);
      List<UIElement> labelElems = LabelElements;
      
      _chartCanvas.Children.Add(legendGroup);

      if(_isPerspective)
      {
        marginLeft += _XOFFSET_PERSPECTIVE;
      }

      double width = _chartCanvas.Width, height = _chartCanvas.Height;
      double gridWidth = (width - marginLeft - _marginRight),
             gridHeight = (height - _marginTop - _marginBottom);
      
      if(animate)
      {
        labelElems.Add(legendGroup);
        legendGroup.Opacity = 0;
      }

      double dx = 0, dy = 0, tx = marginLeft, ty = height - _marginBottom;
      bool drawSideWays = (_legendPosition == LegendLocation.START || 
                          _legendPosition == LegendLocation.END);
      
      string legendTextXAML = _currentLookAndFeel.GetLegendTextXAML();
      string legendIconXAML = _currentLookAndFeel.GetLegendIconXAML();
      
      double maxWidth = 0.0, maxHeight = 0.0, textHeight;
      
      for (int i = 0; i < seriesCount; ++i)
      {
        if(drawSideWays)
          dx = 0;
          
        rectElem = XamlReader.Load(legendIconXAML) as UIElement;
        
        if (i == 0)
        {
          iconHeight = (double)rectElem.GetValue(Canvas.HeightProperty);
          iconWidth = (double)rectElem.GetValue(Canvas.WidthProperty);
        }
        
        rectElem.SetValue(Canvas.LeftProperty, dx);
        
        SetFillOnElement(rectElem, seriesColors[i]);



        legendGroup.Children.Add(rectElem);

        dx += 1.5 * iconWidth;

        labelElem = XamlReader.Load(legendTextXAML) as TextBlock;
        labelElem.Text = seriesLabels[i];
        textHeight = labelElem.ActualHeight;
        
        labelElem.SetValue(Canvas.LeftProperty, dx);

        labelElem.SetValue(Canvas.TopProperty,
                     (dy - textHeight) - ((iconHeight > textHeight) ? ((iconHeight - textHeight)/2) : 0));
        rectElem.SetValue(Canvas.TopProperty, 
                     (dy - iconHeight) - ((textHeight > iconHeight) ? ((textHeight - iconHeight)/2) : 0));
        
        legendGroup.Children.Add(labelElem);
        
        
        if(!drawSideWays)
          dx += labelElem.ActualWidth + iconWidth;
        else
          dy += 1.5 * iconHeight;
        
        if(i == 0 && !drawSideWays)
        {
          if (_legendPosition == LegendLocation.TOP)
          {
            ty = SetLegendTopAdjustment(_marginTop+labelElem.ActualHeight);
            _marginTop += labelElem.ActualHeight + _TEXT_MARGIN;
          }
          else
          {
            ty = SetLegendBottomAdjustment(ty);
            _marginBottom += labelElem.ActualHeight + _TEXT_MARGIN;
          }
        }
        
        if(drawSideWays)
        {
          maxWidth = Math.Max(maxWidth, labelElem.ActualWidth);
          maxHeight = Math.Max(maxHeight, labelElem.ActualHeight);
        }
      }
      
      if(!drawSideWays && gridWidth > dx)
        tx = (gridWidth - dx) / 2 + marginLeft;
      
      if(drawSideWays)
      {
        maxWidth += 1.5 * iconWidth;

        if (_legendPosition == LegendLocation.START)
        {
          tx = this.SetLegendLeftAdjustment(_marginLeft);
          _marginLeft += maxWidth + _TEXT_MARGIN;
        }
        else
        {
          _marginRight += maxWidth + _TEXT_MARGIN;
          tx = width - _marginRight + _TEXT_MARGIN;
          tx = this.SetLegendRightAdjustment(tx);
        }
        
        if(gridHeight > dy)
          ty = (gridHeight - maxHeight) / 2 + _marginTop;
        else
          ty = gridHeight + _marginTop - maxHeight;
      }
      
      TranslateTransform transform = new TranslateTransform();
      
      transform.SetValue(TranslateTransform.XProperty, tx);
      transform.SetValue(TranslateTransform.YProperty, ty);
      
      legendGroup.RenderTransform = transform; 
    }

    /// <summary>
    /// Adjusts the legend location when it is at the top
    /// </summary>
    /// <param name="ty">the original y location of the legend</param>
    /// <returns>adjusted value</returns>
    protected virtual double SetLegendTopAdjustment(double ty)
    {
      // By default we need not adjust anything
      return ty;
    }

    /// <summary>
    /// Adjusts the legend location when it is at the bottom
    /// </summary>
    /// <param name="ty">the original y location of the legend</param>
    /// <returns>adjusted value</returns>
    protected virtual double SetLegendBottomAdjustment(double ty)
    {
      if(_hLabelContainer!= null && _hLabelContainer.Children.Count > 0)
      {
        ty += _hLabelBounds.Height + _TEXT_MARGIN;
      }     
      return ty;
    }

    /// <summary>
    /// Adjusts the legend location when it is at the left
    /// </summary>
    /// <param name="tx">the original x location of the legend</param>
    /// <returns>adjusted value</returns>
    protected virtual double SetLegendLeftAdjustment(double tx)
    {
      var container = this._vLabelContainer;
      if (_vLabelContainer != null)
      {
        tx -= _vLabelBounds.Width + _TEXT_MARGIN;
      }
      return tx;
    }

    /// <summary>
    /// Adjusts the legend location when it is at the Right
    /// </summary>
    /// <param name="tx">the original x location of the legend</param>
    /// <returns>adjusted value</returns>
    protected virtual double SetLegendRightAdjustment(double tx)
    {
      // By default we need not adjust anything
      return tx;
    }

    /// <summary>
    /// Draws the group for the chart component
    /// </summary>
    protected virtual void LayoutGroupLabels()
    {
      if (_hLabelContainer.Children.Count == 0)
        return;
      
      double marginLeft = _marginLeft;
      if (_isPerspective)
        marginLeft += _XOFFSET_PERSPECTIVE;

      double width = _chartCanvas.Width, height = _chartCanvas.ActualHeight;
      double gridWidth = (width - marginLeft - _marginRight);
      bool isCenterAligned = this.IsGroupLabelCentered();
      
      int vLineCount = GetVLineCount();
      
      TextBlock labelElem;
      
      double groupWidth = gridWidth / (isCenterAligned ? vLineCount : vLineCount - 1);
      
      double dx = 0, dy = height - _marginBottom + _hLabelBounds.Height + _TEXT_MARGIN;

      for (var i = 0; i < vLineCount; ++i)
      {
        labelElem = _groupLabelElems[i];
        
        if (labelElem == null)
          continue;

        if (_rotateHorizontalLabels)
          labelElem.SetValue(Canvas.TopProperty, dy - labelElem.ActualHeight*Math.Sqrt(2));
        else
          labelElem.SetValue(Canvas.TopProperty, dy - _hLabelBounds.Height);
        
        double textWidth = labelElem.ActualWidth;

        if (!_rotateHorizontalLabels)
        {
          if (isCenterAligned)
          {
            if (groupWidth > textWidth)
              dx = (groupWidth - textWidth) / 2;
            else
              dx = 2;
          }
          else
          {
            dx = (-textWidth) / 2;
            if (_isPerspective)
              dx -= _XOFFSET_PERSPECTIVE;
          }
        }
        else
        {
          if (isCenterAligned)
            dx = groupWidth/2 - (labelElem.ActualWidth/Math.Sqrt(2) + labelElem.ActualHeight*Math.Sqrt(2) / 2);
          else
          {
            dx = -(labelElem.ActualWidth / Math.Sqrt(2) + labelElem.ActualHeight * Math.Sqrt(2) / 2);
            if (_isPerspective)
              dx -= _XOFFSET_PERSPECTIVE;
          }
        }
        labelElem.SetValue(Canvas.LeftProperty, marginLeft + dx + i * groupWidth);

      }
    }

    /// <summary>
    /// Overriden to swap the labels
    /// </summary>
    protected virtual void LayoutHChartGroupLabels()
    {
      double marginLeft = MarginLeft, marginTop = MarginTop;
      var gridHeight = (ChartCanvas.Height - marginTop - MarginBottom);

      if (IsPerspective)
        gridHeight -= YOffsetPerspective;

      Canvas container = VLabelContainer;
      UIElementCollection children = VLabelContainer.Children;

      if (children.Count == 0)
        return;

      TextBlock labelElem;
      string[] groupLabels = Model.GroupLabels;
      int hLineCount = groupLabels.Length;

      double textHeight = VLabelBounds.Height;
      TextBlock[] gLabelElems = this.GroupLabelElems;

      // horizontal lines
      for (int i = 0; i < hLineCount; ++i)
      {
        labelElem = gLabelElems[i];

        if (labelElem == null)
          continue;

        this.SetVerticalLabelAt(labelElem,
              (hLineCount - i) * gridHeight / hLineCount + marginTop - (gridHeight / (2 * hLineCount)),
              marginLeft, textHeight);
      }
    }
    
    /// <summary>
    /// Laysout the yValue labels for the chart component
    /// </summary>
    protected virtual void LayoutYValueLabels()
    {
      var gridHeight = (_chartCanvas.ActualHeight - _marginTop - _marginBottom);

      if (_isPerspective)
        gridHeight -= _YOFFSET_PERSPECTIVE;

      var textHeight = _vLabelBounds.Height;

      SetVerticalLabelAt(_vLabelContainer.Children[0] as TextBlock, gridHeight + _marginTop,
                               _marginLeft, textHeight);

      SetVerticalLabelAt(_vLabelContainer.Children[1] as TextBlock, _marginTop,
                               _marginLeft, textHeight);

      var hLineCount = _yMajorGridCount;
      
      // horizontal lines
      for (int i = 0; i < hLineCount - 1; ++i)
      {
        this.SetVerticalLabelAt(_vLabelContainer.Children[i + 2] as TextBlock,
              ((hLineCount - i - 1) * gridHeight )/ (hLineCount) + _marginTop,
              _marginLeft, textHeight);

      }
    }

    /// <summary>
    /// Draws a vertical label at a particular location
    /// </summary>
    /// <param name="labelElem"></param>
    /// <param name="y"></param>
    /// <param name="marginLeft"></param>
    /// <param name="textHeight"></param>
    protected virtual void SetVerticalLabelAt(
        TextBlock labelElem, 
        double y, 
        double marginLeft, 
        double textHeight)
    {
      if(_isPerspective)
        y += _YOFFSET_PERSPECTIVE;
      
      // readjust to right align
      double labelMargin = _TEXT_MARGIN, 
             textLength = labelElem.ActualWidth, 
             dx = labelMargin;
      
      if(marginLeft>textLength+labelMargin)
        dx = marginLeft-textLength-labelMargin;
      
      labelElem.SetValue(Canvas.LeftProperty, dx);
      labelElem.SetValue(Canvas.TopProperty, y - (textHeight/2));
    }

    /// <summary>
    /// Laysout the yValue labels for the horizontal chart component
    /// </summary>
    protected virtual void LayoutHChartYValueLabels()
    {
      var gridWidth = (_chartCanvas.Width - _marginLeft - _marginRight);
      Canvas container = _hLabelContainer;
      UIElementCollection childNodes = container.Children;
        
      if(_isPerspective)
        gridWidth -= _XOFFSET_PERSPECTIVE;

      int vLineCount = _yMajorGridCount;
      TextBlock labelElem;
      
      List<UIElement> labelElems = LabelElements;
      bool animate = (_animationDuration>0);

      double yValWidth = gridWidth / vLineCount;
      double dy = _chartCanvas.Height - _marginBottom + _hLabelBounds.Height + _TEXT_MARGIN;
      
      for (int i = 0; i< vLineCount+1; ++i)
      {
        // draw the horizontal label
        labelElem = childNodes[i] as TextBlock;

        labelElem.SetValue(Canvas.TopProperty, dy - labelElem.ActualHeight);
        double textWidth = labelElem.ActualWidth;
        labelElem.SetValue(Canvas.LeftProperty, _marginLeft-textWidth/2+i*yValWidth);	    
      }
    }

    protected virtual double DrawGroupLabelTitle(
      string label, 
      Canvas container, 
      string labelXAML,
      ref TextBlock labelElem,
      double dx, 
      double dy,
      double quadWidth, 
      double quadHeight)
    {
      if(label == null || label.Length == 0)
        return quadHeight;

      labelElem = XamlReader.Load(labelXAML) as TextBlock;
      labelElem.Text = label;
      labelElem.SetValue(Canvas.TopProperty, dy + quadHeight-labelElem.ActualHeight);
      container.Children.Add(labelElem);

      double textWidth = labelElem.ActualWidth;
      
      if(quadWidth > textWidth)
        dx += (quadWidth-textWidth)/2;
      else
        dx += 2;

      labelElem.SetValue(Canvas.LeftProperty, dx);
    	
      if(_animationDuration>0)
        LabelElements.Add(labelElem);
    	
      quadHeight -= labelElem.ActualHeight+_TEXT_MARGIN;
      return quadHeight;
    }
    
    /// <summary>
    /// Draws the grid for the chart component
    /// </summary>
    protected virtual void DrawGrid()
    {
      if (_isPerspective)
        DrawPerspectiveGrid();
      else
        Draw2DGrid();
    }

    /// <summary>
    /// Draws the 2D grid for the chart component
    /// </summary>
    protected virtual void Draw2DGrid()
    {

      bool animate = (_animationDuration > 0);
      double width = _chartCanvas.Width, height = _chartCanvas.ActualHeight;
      double gridWidth = (width - _marginLeft - _marginRight);
      double gridHeight = (height - _marginTop - _marginBottom);

      Path rectElem = XamlReader.Load(_currentLookAndFeel.GetGridRectXAML()) as Path;
      String gridGradientXAML = _currentLookAndFeel.GetGridGradientXAML();
      
      RectangleGeometry rGeometry = new RectangleGeometry();
      rGeometry.Rect = new Rect(_marginLeft, _marginTop, gridWidth, gridHeight);
      rectElem.Data = rGeometry;

      if (gridGradientXAML != null)
      {
        rectElem.Fill = XamlReader.Load(gridGradientXAML) as Brush;
      }

      _chartCanvas.Children.Add(rectElem);


      // Now draw the grid lines
      int vLineCount = this.GetVLineCount(), hLineCount = this.GetHLineCount();
      StringBuilder sb = new StringBuilder();

      // horizontal lines
      for (int i = 0; i < hLineCount - 1; ++i)
      {
        sb.Append("M").Append(_marginLeft).Append(",")
          .Append((i + 1) * gridHeight / hLineCount + _marginTop);
        sb.Append(" h").Append(gridWidth);
      }

      // vertical lines
      for (int i = 0; i < vLineCount - 1; ++i)
      {
        sb.Append("M")
          .Append(_marginLeft + ((i + 1) * gridWidth / vLineCount))
          .Append(",").Append(_marginTop);
        sb.Append(" v").Append(gridHeight);
      }

      Path pathElem = CreatePathFromXAMLAndData(_currentLookAndFeel.GetGridPathXAML(), sb);
      
      if (animate)
      {
        GridElements.Add(pathElem);
        MatrixTransform transform = new MatrixTransform();
        Matrix m = new Matrix();
        if (AnimateAlongXAxis())
          m.M22 = 0.0;
        else
          m.M11 = 0.0;
        transform.Matrix = m;
        pathElem.SetValue(UIElement.RenderTransformProperty, transform);
      }      

      _chartCanvas.Children.Add(pathElem);
    }

    /// <summary>
    /// Retuns the number of vertical lines to draw for the grid
    /// </summary>
    /// <returns></returns>
    protected virtual int GetVLineCount()
    {
      if (_xMajorGridCount >= 0)
        return _xMajorGridCount;
      else
        return _model.GroupLabels.Length;
    }

    /// <summary>
    /// Retuns the number of horizontal lines to draw for the grid
    /// </summary>
    /// <returns></returns>
    protected virtual int GetHLineCount()
    {
      return _yMajorGridCount;  
    }

    /// <summary>
    /// Draws the 2D grid for the chart component
    /// </summary>
    protected virtual void DrawPerspectiveGrid()
    {
      bool animate = (_animationDuration > 0);
      double xOffset = _XOFFSET_PERSPECTIVE, yOffset = _YOFFSET_PERSPECTIVE;
      double width = _chartCanvas.Width, height = _chartCanvas.Height;
      double gridWidth = (width - _marginLeft - _marginRight - xOffset);
      double gridHeight = (height - _marginTop - _marginBottom - yOffset);
      
      Path rectElem = XamlReader.Load(_currentLookAndFeel.GetGridRectXAML()) as Path;
      String gridGradientXAML = _currentLookAndFeel.GetGridGradientXAML();

      RectangleGeometry rGeometry = new RectangleGeometry();
      rGeometry.Rect = new Rect(_marginLeft + xOffset, _marginTop, gridWidth, gridHeight);
      rectElem.Data = rGeometry;
            
      if (gridGradientXAML != null)
      {
        rectElem.Fill =  XamlReader.Load(gridGradientXAML) as Brush;
      }

      _chartCanvas.Children.Add(rectElem);

      // Draw the perspective rects
      StringBuilder sb = new StringBuilder();
      sb.Append("M").Append(_marginLeft + xOffset).Append(",").Append(_marginTop);
      sb.Append(" l").Append(-xOffset).Append(",").Append(yOffset);
      sb.Append(" v").Append(gridHeight);
      sb.Append(" l").Append(xOffset).Append(",").Append(-yOffset);
      sb.Append(" m").Append(gridWidth).Append(",").Append(0);
      sb.Append(" l").Append(-xOffset).Append(",").Append(yOffset);
      sb.Append(" h").Append(-gridWidth);

      Path pathElem = CreatePathFromXAMLAndData(_currentLookAndFeel.GetGridPath3DXAML(), sb);

      if (gridGradientXAML != null)
      {
        pathElem.Fill = XamlReader.Load(gridGradientXAML) as Brush;
      }

      _chartCanvas.Children.Add(pathElem);
      
      // Now draw the grid lines      
      int vLineCount = this.GetVLineCount(), hLineCount = this.GetHLineCount();
      sb = new StringBuilder();
      
      // horizontal lines
      for (int i = 0; i < hLineCount - 1; ++i)
      {
        sb.Append("M").Append(_marginLeft).Append(",")
                     .Append((((i + 1) * gridHeight) / hLineCount) + _marginTop + yOffset);
        sb.Append(" l").Append(xOffset).Append(",").Append(-yOffset);
        sb.Append(" h").Append(gridWidth);
      }

      // vertical lines
      for (int i = 0; i < vLineCount - 1; ++i)
      {
        sb.Append("M")
          .Append(_marginLeft + xOffset + (((i + 1) * gridWidth) / vLineCount))
          .Append(",").Append(_marginTop);
        sb.Append(" v").Append(gridHeight);
        sb.Append(" l").Append(-xOffset).Append(",").Append(yOffset);
      }

      pathElem = CreatePathFromXAMLAndData(_currentLookAndFeel.GetGridPathXAML(), sb);

      if (animate)
      {
        GridElements.Add(pathElem);
        MatrixTransform transform = new MatrixTransform();
        Matrix m = new Matrix();
        if (AnimateAlongXAxis())
          m.M22 = 0.0;
        else
          m.M11 = 0.0;

        transform.Matrix = m;
        pathElem.SetValue(UIElement.RenderTransformProperty, transform);
      }
      
      _chartCanvas.Children.Add(pathElem);
    }
            
    /// <summary>
    /// Draws the data elements for the chart component
    /// </summary>
    public virtual void DrawChartData(){}
    
    /// <summary>
    /// Draws the data elements for chart component. 
    /// This method is used to draw combo charts
    /// </summary>
    /// <param name="mod">The modulus operators determining the total of the combos</param>
    /// <param name="rem">The remainder used to determine the series within the combo</param>
     
    public virtual void DrawChartData(int mod, int rem)
    {
      //No default implementation. Certain charts like bar, area, line, can participate in the combo charts 
    }

    protected void SetExpandosOnElement(object element, int yValueIndex, int seriesIndex, Point ttPreferedPoint)
    {
      ElementExpandos ee = new ElementExpandos();
      ee.YValueIndex = yValueIndex;
      ee.SeriesIndex = seriesIndex;
      ee.TooltipPreferedPoint = ttPreferedPoint;
      _expandos.Add(element, ee);
    }

    protected virtual void ShowToolTip(object sender, MouseEventArgs e)
    {
      if (this._toolTipVisible)
        return;
      
      if (_toolTip == null)
      {
        _toolTip = XamlReader.Load(CurrentLookAndFeel.GetTooltipXAML()) as Canvas;
        _chartCanvas.Children.Add(_toolTip);
      }
      _toolTip.Visibility = Visibility.Visible;
      
      
      Path circleElem = _toolTip.Children[0] as Path;
      Path boundingRectElem = _toolTip.Children[1] as Path;
      _toolTipBounds = this.FillToolTipData(sender, e, boundingRectElem, circleElem);

      Point pt = this.GetToolTipLocation(sender, e, _toolTipBounds);
      double dx = pt.X, dy = pt.Y, cx, cy;
      EllipseGeometry eg = circleElem.Data as EllipseGeometry;
      
      if (dx + _toolTipBounds.Width > _chartCanvas.Width)
      {
        dx -= _toolTipBounds.Width;
        
        cx = _toolTipBounds.Width;
      }
      else
      {
        cx = 0;
      }

      if (dy - _toolTipBounds.Height < 0)
      {
        dy += _toolTipBounds.Height;
        cy =  0;
      }
      else
      {
        cy = _toolTipBounds.Height;
      }
      
      eg.Center = new Point(cx, cy);
            
      TranslateTransform tTrans = new TranslateTransform();
      tTrans.X = dx;
      tTrans.Y = dy;
      _toolTip.RenderTransform = tTrans;
      this._toolTipVisible = true;
    }

    protected virtual Point GetToolTipLocation(object sender, MouseEventArgs e, Size ttBounds)
    {
      ElementExpandos ee =  _expandos[sender];
      Point pt = ee.TooltipPreferedPoint;
      return new Point(pt.X, pt.Y - ttBounds.Height);
    }

    protected virtual Size FillToolTipData(
      object sender,
      MouseEventArgs e,
      Path boundingRectElem,
      Path circleElem)
    {
      ChartEventArgs chartEvent = GetChartEvent(sender, e);
      
      var j = chartEvent.SeriesIndices[0];

      string [] groupLabels = _model.GroupLabels, 
                seriesLabels = _model.SeriesLabels;
                
      double [] yValues = chartEvent.YValues;

      Color[] seriesColors = _model.SeriesColors;

      //top label
      TextBlock textElem = _toolTip.Children[2] as TextBlock;
      textElem.Text = seriesLabels[j];
                    
      var labelWidth = textElem.ActualWidth;      
      
      //actual value
      textElem = _toolTip.Children[3] as TextBlock;
      textElem.Text = yValues[0].ToString(_format);
      
      var dataWidth = textElem.ActualWidth;
      
      // leave a  clearance on either end of the text
      double xMargin = _TEXT_MARGIN, dx = xMargin;
      
      if (labelWidth > dataWidth)
        dx = (labelWidth-dataWidth)/2+xMargin;
      
      textElem.SetValue(Canvas.LeftProperty,dx);
      
      var rectWidth = Math.Max(labelWidth,dataWidth)+2*xMargin;
      
      RectangleGeometry rg = boundingRectElem.Data as RectangleGeometry;
      Rect rect = rg.Rect;
      rg.Rect = new Rect(0,0, rectWidth, rect.Height);

      circleElem.Stroke = boundingRectElem.Stroke = new SolidColorBrush(seriesColors[j]);
      
      return new Size(rectWidth, 2*(textElem.ActualHeight + _TEXT_MARGIN));
    }

    protected virtual void HideToolTip(object sender, EventArgs e)
    {
      if(_toolTip != null)
        _toolTip.Visibility = Visibility.Collapsed;
      
      _toolTipVisible = false;
    }

    protected virtual ChartEventArgs GetChartEvent(object sender, MouseEventArgs e)
    {      
      ElementExpandos ee =  _expandos[sender];
      int i = ee.YValueIndex; 
      int j = ee.SeriesIndex;
      
      int[] seriesIndices = {j};
      int[] yValueIndices = {i};
      double[] yValues = {_model.YValues[i,j]};

      return new ChartEventArgs(seriesIndices, yValueIndices, yValues, null);
    }

    /// <summary>
    /// Fires the ChartClicked event
    /// </summary>
    protected virtual void ChartDataClicked(object sender, MouseEventArgs e)
    {
      if(ChartClicked != null)
      {
        ChartClicked(sender, GetChartEvent(sender, e));
      }
    }
    
    protected virtual void InitializeGradients()
    {
      // FIXTHIS: Currently GraidentBrushes cannot be re-used across elements
      // So we cannot initialize them and use them later
      
      /*
       string gradientXAML = _currentLookAndFeel.GetElementGradientXAML();
      
      if(gradientXAML != null)
      {
        Color[] colors = _model.SeriesColors;
        int seriesCount = _model.SeriesLabels.Length;        
        _gradientBrushs = new GradientBrush[seriesCount];
        
        for(int i = 0; i<seriesCount; i++)
        {
          GradientBrush brush = XamlReader.Load(gradientXAML) as GradientBrush;
          GradientStopCollection gStops = brush.GradientStops;
          int stopCount = gStops.Count;
          Color color = colors[i];
          
          for (int j = 0; j < stopCount; ++j)
          {
            if (j == 0)
              gStops[0].Color = color;
            else
            {
              byte r = (byte)Math.Min(((j + 1) * 1.8) / stopCount * color.R, 255);
              byte g = (byte)Math.Min(((j + 1) * 1.8) / stopCount * color.G, 255);
              byte b = (byte)Math.Min(((j + 1) * 1.8) / stopCount * color.B, 255);
              gStops[j].Color = Color.FromRgb(r, g, b);
            }
          }
          _gradientBrushs[i] = brush;
        }
      }*/
    }

    protected virtual void Animate()
    {
      _startTime = DateTime.Now;
      _timer.Completed += new EventHandler(DoAnimation);
      _timer.Begin();
    }

    protected virtual void DoAnimation(object sender, EventArgs e)
    {
      TimeSpan diffTime = DateTime.Now.Subtract(_startTime);
      if(diffTime.TotalSeconds >= _animationDuration)
      {
        SetDataAnimStep(1);
        SetLabelsAnimStep(1);
        SetGridAnimStep(1);
        // we do not need the elements any more.
        _dataElements.Clear();
        _labelElements.Clear();
        
        if(_gridElements != null)
          _gridElements.Clear();
      }
      else
      {
        double ratio = (diffTime.TotalSeconds) / _animationDuration;
        SetDataAnimStep(ratio);
        SetLabelsAnimStep(ratio);
        SetGridAnimStep(ratio);
        
        // restart the animation
        _timer.Begin();
      }
    }

    public virtual void SetDataAnimStep(double ratio)
    {
      int animCount = _dataElements.Count;
      bool animHorizontal = AnimateAlongXAxis();

      // Default implementation is to make the elements appear from x axis or y axis 
      if(animHorizontal)
      {
        for(int i = 0; i < animCount; ++i)
        {
          double tx = (1-ratio)*_marginLeft;
          MatrixTransform mt = _dataElements[i].GetValue(UIElement.RenderTransformProperty) as MatrixTransform;
          Matrix m = mt.Matrix;
          m.OffsetX = tx;
          m.M11 = ratio;
          mt.Matrix = m;
        }    
      }
      else
      {
        double cy = (_chartCanvas.Height - _marginBottom);
        
        for(int i = 0; i < animCount; ++i)
        {
          double ty = (1-ratio)*cy;
          MatrixTransform mt = _dataElements[i].GetValue(UIElement.RenderTransformProperty) as MatrixTransform;
          Matrix m = mt.Matrix;
          m.OffsetY = ty;
          m.M22 = ratio;
          mt.Matrix = m;
        }
      }
    }
    
    /// <summary>
    /// Indicates if the animation happens along X axis. Horizontal charts like 
    /// horizontal bar charts can override this method to indicate animation along X axis
    /// </summary>
    /// <returns></returns>
    protected virtual bool AnimateAlongXAxis()
    {
      return false;  
    }

    protected virtual void SetLabelsAnimStep(double ratio)
    {
      int animCount = _labelElements!= null ? _labelElements.Count: 0;
      
      // Default implementation is to make the elements fade in
      for(int i = 0; i < animCount; ++i)
      {
        _labelElements[i].Opacity = ratio;
      }
    }

    protected virtual void SetGridAnimStep(double ratio)
    {
      int animCount = _gridElements != null ?_gridElements.Count:0;
      bool animHorizontal = AnimateAlongXAxis();
      
      // Default implementation is to make the grid appear along the x axis or y axis
      if(animHorizontal)
      {
        double cy = (_chartCanvas.Height - _marginBottom);
        
        // reverse the animation for horizontal chart
        for(int i = 0; i < animCount; ++i)
        {
          double ty = (1-ratio)*cy;
          MatrixTransform mt = _gridElements[i].GetValue(UIElement.RenderTransformProperty) as MatrixTransform;
          Matrix m = mt.Matrix;
          m.OffsetY = ty;
          m.M22 = ratio;
          mt.Matrix = m;
        }
      }
      else 
      {
        for(int i = 0; i < animCount; ++i)
        {
          double tx = (1-ratio)*_marginLeft;
          MatrixTransform mt = _gridElements[i].GetValue(UIElement.RenderTransformProperty) as MatrixTransform;
          Matrix m = mt.Matrix;
          m.OffsetX = tx;
          m.M11 = ratio;
          mt.Matrix = m;
        }
      }
    }
        
    protected virtual void SetGradientOnElement(UIElement element, string xaml, Color color, byte alpha)
    {
      // FIXTHIS: this is highly inefficient, however looks like currently
      // GraidentBrushes cannot be re-used across elements
      
      GradientBrush brush = XamlReader.Load(xaml) as GradientBrush;
      
      GradientStopCollection gStops = brush.GradientStops;
      int stopCount = gStops.Count;
      
      for(int i = 0; i<stopCount; ++i)
      {
        if(i ==0)
        {
            gStops[0].Color = Color.FromArgb(alpha, color.R, color.G, color.B);
        }
        else
        {
          // create a lighter color
          
          byte r = (byte)Math.Min(((i+1)*1.8)/stopCount*color.R, 255);
          byte g = (byte)Math.Min(((i + 1) * 1.8) / stopCount * color.G, 255);
          byte b = (byte)Math.Min(((i + 1) * 1.8) / stopCount * color.B, 255);
          gStops[1].Color = Color.FromArgb(alpha, r, g, b);
        }
      }
      
      element.SetValue(Shape.FillProperty, brush);
    }

    protected virtual void SetFillOnElement(UIElement element, Color color)
    {
      SolidColorBrush brush = element.GetValue(Shape.FillProperty) as SolidColorBrush;
      
      if(brush == null)
      {
        brush = new SolidColorBrush();
        element.SetValue(Shape.FillProperty, brush);
      }
      
      brush.Color = color;
    }

    protected Path CreatePathFromXAMLAndData(String pathXAML, StringBuilder pathData)
    {
      // HACK!!! in silverlight 2.0 beta 2 we can no longer set path data using string 
      // So until there is some kind of parse function for PathGeometry, create the path using
      // XAML and pathData

      String pathAfterStr = pathXAML.Substring(5); // After "<Path"

      Path path = (Path)XamlReader.Load("<Path Data=\""
                                        + pathData.Append("\" ").Append(pathAfterStr));

      return path;

    }
    public virtual void clear()
    {
      _chartCanvas.Children.Clear();
    }

    public enum ChartType
    {
      VBAR = 1,
      HBAR,
      CYLINDERBAR,
      VBAR_STACKED,
      HBAR_STACKED,
      PIE,
      AREA,
      AREA_STACKED,
      LINE,
      BAR_LINE_COMBO,
      BAR_AREA_COMBO,
      BAR_LINE_AREA_COMBO,
      XYLINE,
      SCATTER_PLOT,
      RADAR,
      RADAR_AREA,
      FUNNEL,
      CIRCULAR_GAUGE,
      SEMI_CIRCULAR_GAUGE,
      CANDLE_STICK
    }

    public enum LegendLocation
    {
      NONE = 1,
      TOP = 2,
      END = 3,
      BOTTOM = 4,
      START = 5
    }
    
    private Canvas _chartCanvas;

    /// <summary>
    /// Accessor so that derived classes can get hold of the canvas.
    /// </summary>
    public Canvas ChartCanvas
    {
      get{return _chartCanvas;}
    }
    
    private double _marginLeft, _marginTop, _marginRight, _marginBottom;
    private Canvas _hLabelContainer, _vLabelContainer;
    private Size _hLabelBounds, _vLabelBounds;
    private TextBlock[] _groupLabelElems;
    
    private static Dictionary<string, LookAndFeel> _LookAndFeelMap = new Dictionary<string, LookAndFeel>();
    private static string _currentLookAndFeelId;
    private static LookAndFeel _currentLookAndFeel;
    
    public static string CurrentLookAndFeelId
    {
      get{return _currentLookAndFeelId;}
      set
      {
        _currentLookAndFeelId = value;
        _currentLookAndFeel = _LookAndFeelMap[value];
      }
    }

    public static LookAndFeel CurrentLookAndFeel
    {
      get { return _currentLookAndFeel; }
    }
    
    private ChartModel _model;
    private ChartType _type;
    
    public event ChartEventHandler ChartClicked;
    
    private bool _isPerspective;
    private LegendLocation _legendPosition;
    private bool _displayToolTip;
    private double _animationDuration;
    private int _yMajorGridCount;
    private int _yMinorGridCount;
    private int _xMajorGridCount;
    private string _format;
    private bool _rotateHorizontalLabels;

    private Storyboard _timer;
    private DateTime _startTime;
    
    private List<UIElement> _dataElements, _labelElements, _gridElements;
    
    private Canvas _toolTip = null;
    private bool _toolTipVisible;
    private Size _toolTipBounds;
    
    private Dictionary<object, ElementExpandos> _expandos;
     
    public ChartModel Model
    {
      get{return _model;}
      set{_model = value;}
    }
    
    
    public  ChartType Type 
    {
      get { return _type; }
      set { _type = value; }
    }
        
    /// <summary>
    /// indicates if the chart is drawn with a perspective (2.5D)
    /// </summary>
    public bool IsPerspective
    {
      get { return _isPerspective; }
      set { _isPerspective = value; }
    }

    /// <summary>
    /// indicates if the chart will rotate the group labels to save space 
    /// </summary>
    public bool RotateHorizontalLabels
    {
      get { return _rotateHorizontalLabels; }
      set { _rotateHorizontalLabels = value; }
    }
    /// <summary>
    /// Location of the legend
    /// </summary>
    public LegendLocation LegendPosition
    {
      get { return _legendPosition; }
      set { _legendPosition = value; }
    }
    
    /// <summary>
    /// Indicates if the tooltips are displayed for the chart
    /// </summary>
    public bool DisplayToolTip
    {
      get { return _displayToolTip; }
      set { _displayToolTip = value; }
    }
  
    /// <summary>
    /// The duration of the animation of the graph in seconds
    /// </summary>
    public double AnimationDuration
    {
      get { return _animationDuration; }
      set { _animationDuration = value; }
    }

    /// <summary>
    /// The number of Major Lines to draw on y axis
    /// </summary>
    public int YMajorGridCount
    {
      get { return _yMajorGridCount; }
      // number of sections is 1 greater than the lines
      set { _yMajorGridCount = value>0?value+1:value; }
    }

    /// <summary>
    /// The number of Minor Line section to draw on y axis
    /// </summary>
    public int YMinorGridCount
    {
      get { return _yMinorGridCount; }
      // number of sections is 1 greater than the lines
      set { _yMinorGridCount = value>0?value+1:value; }      
    }

    /// <summary>
    /// The number of Major Line sections to draw on x axis
    /// </summary>
    public int XMajorGridCount
    {
      get { return _xMajorGridCount; }
      // number of sections is 1 greater than the lines
      set { _xMajorGridCount = value>0?value+1:value; }
    }

    /// <summary>
    /// The format string used to format the values
    /// </summary>
    public string Format
    {
      get { return _format; }
      set { _format = value; }
    }

    /// <summary>
    /// List of label elements that will be animated during animation
    /// </summary>
    protected List<UIElement> LabelElements
    {
      get
      {
        if (_labelElements == null)
          _labelElements = new List<UIElement>();
        return _labelElements;
      }
    }

    /// <summary>
    /// List of label elements that will be animated during animation
    /// </summary>
    protected List<UIElement> GridElements
    {
      get
      {
        if (_gridElements == null)
          _gridElements = new List<UIElement>();
        return _gridElements;
      }
    }
        
    /// <summary>
    /// List of data elements that will be animated during animation
    /// </summary>
    protected List<UIElement> DataElements
    { 
      get
      {
        if(_dataElements == null)
          _dataElements = new List<UIElement>();
        return _dataElements;
      }
    }

    protected Canvas HLabelContainer
    {
      get{return _hLabelContainer;}
      set{_hLabelContainer = value;}
    }
    
    protected Canvas VLabelContainer
    {
      get { return _vLabelContainer; }
      set { _vLabelContainer = value; }
    }

    protected Size HLabelBounds
    {
      get { return _hLabelBounds; }
      set { _hLabelBounds = value; }
    }

    protected Size VLabelBounds
    {
      get { return _vLabelBounds; }
      set { _vLabelBounds = value; }
    }
    
    protected TextBlock[] GroupLabelElems
    {
      get { return _groupLabelElems; }
      set { _groupLabelElems = value; }
    }
    
    protected int XOffsetPerspective
    {
      get {return _XOFFSET_PERSPECTIVE;}
    }
    
    protected int YOffsetPerspective
    {
      get { return _YOFFSET_PERSPECTIVE; }
    }

    protected double MarginLeft
    {
      get {return _marginLeft;}
      set { _marginLeft = value; }
    }
    
    protected double MarginTop
    {
      get {return _marginTop;}
      set { _marginTop = value; }
    }
    
    protected double MarginRight
    {
      get {return _marginRight;}
      set { _marginRight = value; }
    }   
     
    protected double MarginBottom
    {
      get {return _marginBottom;}
      set { _marginBottom = value; }
    }

    protected Canvas ToolTip
    {
      get{return _toolTip;}
    }
    
    protected bool ToolTipVisible
    {
      get{return _toolTipVisible;}
      set{_toolTipVisible = value;}
    }
    
    protected Size ToolTipBounds
    {
      get{return _toolTipBounds;}
    }
    
    protected Dictionary<object, ElementExpandos> Expandos
    {
      get { return _expandos; }
    }
    
    // margin generally used around text
    internal const int _TEXT_MARGIN = 2;

    private const double _MAX_MULTIPLIER = 1.2;
    private const double _MIN_MULTIPLIER = .8;

    private const int _XOFFSET_PERSPECTIVE = 12;
    private const int _YOFFSET_PERSPECTIVE = 9;
    
  }
}
