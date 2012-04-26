﻿#pragma checksum "..\..\DataWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "BF5DBDD6ADD956D1F3220A74AFFCCD8B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3625
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


namespace DataMiner {
    
    
    /// <summary>
    /// DataWindow
    /// </summary>
    public partial class DataWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\DataWindow.xaml"
        internal System.Windows.Controls.TabControl TimeTabs;
        
        #line default
        #line hidden
        
        
        #line 7 "..\..\DataWindow.xaml"
        internal System.Windows.Controls.TabItem OneYear;
        
        #line default
        #line hidden
        
        
        #line 8 "..\..\DataWindow.xaml"
        internal System.Windows.Controls.TabItem SixtyDays;
        
        #line default
        #line hidden
        
        
        #line 9 "..\..\DataWindow.xaml"
        internal System.Windows.Controls.TabItem ThirtyDays;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\DataWindow.xaml"
        internal System.Windows.Controls.TabControl StockSymbolTabs;
        
        #line default
        #line hidden
        
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
            System.Uri resourceLocater = new System.Uri("/DataMiner;component/datawindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\DataWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.TimeTabs = ((System.Windows.Controls.TabControl)(target));
            return;
            case 2:
            this.OneYear = ((System.Windows.Controls.TabItem)(target));
            
            #line 7 "..\..\DataWindow.xaml"
            this.OneYear.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.TimePeriodClick);
            
            #line default
            #line hidden
            return;
            case 3:
            this.SixtyDays = ((System.Windows.Controls.TabItem)(target));
            
            #line 8 "..\..\DataWindow.xaml"
            this.SixtyDays.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.TimePeriodClick);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ThirtyDays = ((System.Windows.Controls.TabItem)(target));
            
            #line 9 "..\..\DataWindow.xaml"
            this.ThirtyDays.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.TimePeriodClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.StockSymbolTabs = ((System.Windows.Controls.TabControl)(target));
            return;
            case 6:
            
            #line 29 "..\..\DataWindow.xaml"
            ((System.Windows.Controls.TabItem)(target)).MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.newSearch);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
