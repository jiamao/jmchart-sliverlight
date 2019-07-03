using System;
using System.Net;
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
    /// 基础对象
    /// </summary>
    public interface IBaseControl
    {
        void Draw();

        void Show();

        void Hide();
    }
}
