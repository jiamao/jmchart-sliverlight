using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Silverlight.ProcessEditor
{
    public partial class Editor : UserControl
    {
        public Editor()
        {
            InitializeComponent();

            editorCanvas1.GridCanvas.Change += new EventHandler(GridCanvas_Change);
            this.Loaded += new RoutedEventHandler(Editor_Loaded);

            this.MouseMove += editorCanvas1.GridCanvas.Editor_MouseMove;
        }

        void GridCanvas_Change(object sender, EventArgs e)
        {
            GetJson();
        }

        void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            var toolItems = new List<Silverlight.ProcessEditor.Model.Part>();
            toolItems.Add(new Silverlight.ProcessEditor.Model.Part() { Id = "10", Name = "正转", TypeId = "10" });
            toolItems.Add(new Silverlight.ProcessEditor.Model.Part() { Id = "11", Name = "反转", TypeId = "11" });
            toolItems.Add(new Silverlight.ProcessEditor.Model.Part() { Id = "100", Name = "出口", TypeId = "100" });
            LoadItems(toolItems);
        }

        /// <summary>
        /// 加载元件类型
        /// </summary>
        /// <param name="items"></param>
        public void LoadItems(IEnumerable<Model.Part> items)
        {
            toolBar.LoadItems(items);
        }

        /// <summary>
        /// 获取和生成json
        /// </summary>
        /// <returns></returns>
        public string GetJson()
        {
            txtJson.Text = GetCircuit().ToJson();

            return txtJson.Text;
        }

        /// <summary>
        /// 获取当前线路实体
        /// </summary>
        /// <returns></returns>
        public Model.Circuit GetCircuit()
        {
            return editorCanvas1.GridCanvas.Model;
        }
    }
}
