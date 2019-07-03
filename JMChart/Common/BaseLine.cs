using System;
using System.Net;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace JMChart.Common
{
    /// <summary>
    /// 基线
    /// </summary>
    public class BaseLine : Path, IBaseControl
    {
        /// <summary>
        /// 线的形状
        /// </summary>
        protected PathGeometry lineGeometry = new PathGeometry();

        /// <summary>
        /// 当前线的点集合
        /// </summary>
        public List<Point> Points = new List<Point>();

        /// <summary>
        /// 画当前基线
        /// </summary>
        public void Draw()
        {
            this.Data = lineGeometry;
            var linePoints = new PathFigure();
            if (Points.Count > 0)
            {
                linePoints.StartPoint = Points[0];
                for (var i = 1; i < Points.Count; i++)
                {
                    linePoints.Segments.Add(new LineSegment() { Point = Points[i] });
                }
            }

            this.lineGeometry.Figures.Clear();
            this.lineGeometry.Figures.Add(linePoints);
        }


        public void Show()
        {
            this.Visibility = System.Windows.Visibility.Visible;
        }

        public void Hide()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
