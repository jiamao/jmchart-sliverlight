﻿#pragma checksum "D:\微云同步盘\273389528\项目\DoNet.Common\Silverlight\Silverlight.ProcessEditor\Editor.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "933F5EDDAB848C9227132723993288F4"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18444
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using Silverlight.ProcessEditor.View;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Silverlight.ProcessEditor {
    
    
    public partial class Editor : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Silverlight.ProcessEditor.View.ToolBar toolBar;
        
        internal Silverlight.ProcessEditor.View.EditorCanvas editorCanvas1;
        
        internal System.Windows.Controls.TextBox txtJson;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Silverlight.ProcessEditor;component/Editor.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.toolBar = ((Silverlight.ProcessEditor.View.ToolBar)(this.FindName("toolBar")));
            this.editorCanvas1 = ((Silverlight.ProcessEditor.View.EditorCanvas)(this.FindName("editorCanvas1")));
            this.txtJson = ((System.Windows.Controls.TextBox)(this.FindName("txtJson")));
        }
    }
}
