﻿#pragma checksum "C:\Users\Administrator\Desktop\Twitter\Chicken\Chicken\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "95C369D2BB378E4E4E268670B090FB91"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.225
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using Chicken.ViewModel;
using Microsoft.Phone.Controls;
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


namespace Chicken {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal Chicken.ViewModel.ConvertVisibility VisibilityConverter;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.Pivot MainPivot;
        
        internal Microsoft.Phone.Controls.PivotItem HomePivot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.ListBox HomeListBoxControl;
        
        internal Microsoft.Phone.Controls.PivotItem MentionsPivot;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Chicken;component/MainPage.xaml", System.UriKind.Relative));
            this.VisibilityConverter = ((Chicken.ViewModel.ConvertVisibility)(this.FindName("VisibilityConverter")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.MainPivot = ((Microsoft.Phone.Controls.Pivot)(this.FindName("MainPivot")));
            this.HomePivot = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("HomePivot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.HomeListBoxControl = ((System.Windows.Controls.ListBox)(this.FindName("HomeListBoxControl")));
            this.MentionsPivot = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("MentionsPivot")));
        }
    }
}

