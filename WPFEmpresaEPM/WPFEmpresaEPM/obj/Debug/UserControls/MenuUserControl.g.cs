﻿#pragma checksum "..\..\..\UserControls\MenuUserControl.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "F4DEF9E05FDF9C2B8AD5697BB53FCF1FE5D1E9E6D416BBA7B9B14B76F4D40355"
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
using WPFEmpresaEPM.UserControls;


namespace WPFEmpresaEPM.UserControls {
    
    
    /// <summary>
    /// MenuUserControl
    /// </summary>
    public partial class MenuUserControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 22 "..\..\..\UserControls\MenuUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbTimer;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\UserControls\MenuUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image btnPagoFactura;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\UserControls\MenuUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image btnPagoMedida;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\UserControls\MenuUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image btnPagoPrepago;
        
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
            System.Uri resourceLocater = new System.Uri("/WPFEmpresaEPM;component/usercontrols/menuusercontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\UserControls\MenuUserControl.xaml"
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
            this.tbTimer = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.btnPagoFactura = ((System.Windows.Controls.Image)(target));
            
            #line 39 "..\..\..\UserControls\MenuUserControl.xaml"
            this.btnPagoFactura.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.btnOption_TouchDown);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnPagoMedida = ((System.Windows.Controls.Image)(target));
            
            #line 52 "..\..\..\UserControls\MenuUserControl.xaml"
            this.btnPagoMedida.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.btnOption_TouchDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnPagoPrepago = ((System.Windows.Controls.Image)(target));
            
            #line 65 "..\..\..\UserControls\MenuUserControl.xaml"
            this.btnPagoPrepago.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.btnOption_TouchDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

