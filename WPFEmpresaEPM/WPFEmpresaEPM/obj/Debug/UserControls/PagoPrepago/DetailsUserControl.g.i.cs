﻿#pragma checksum "..\..\..\..\UserControls\PagoPrepago\DetailsUserControl.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "7411B9CAA1D8C37AA9BADFCA931802D9894339888877AAD06A5EE9807301E666"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using WPFEmpresaEPM.UserControls.PagoPrepago;


namespace WPFEmpresaEPM.UserControls.PagoPrepago {
    
    
    /// <summary>
    /// DetailsUserControl
    /// </summary>
    public partial class DetailsUserControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 20 "..\..\..\..\UserControls\PagoPrepago\DetailsUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image BtnExit;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\..\UserControls\PagoPrepago\DetailsUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbTimer;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\..\UserControls\PagoPrepago\DetailsUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image BtnBack;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WPFEmpresaEPM;component/usercontrols/pagoprepago/detailsusercontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\UserControls\PagoPrepago\DetailsUserControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.BtnExit = ((System.Windows.Controls.Image)(target));
            
            #line 27 "..\..\..\..\UserControls\PagoPrepago\DetailsUserControl.xaml"
            this.BtnExit.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnExit_TouchDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.tbTimer = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.BtnBack = ((System.Windows.Controls.Image)(target));
            
            #line 42 "..\..\..\..\UserControls\PagoPrepago\DetailsUserControl.xaml"
            this.BtnBack.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnBack_TouchDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

