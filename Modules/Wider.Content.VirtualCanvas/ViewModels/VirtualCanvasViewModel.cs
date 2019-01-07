//-----------------------------------------------------------------------
// <copyright file="Window1.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Win32;
using Wider.Content.VirtualCanvas.Gestures;
using Prism.Mvvm;
using Prism.Ioc;
using Wider.Core.Services;

namespace Wider.Content.VirtualCanvas.ViewModels
{
    /// <summary>
    /// This demo shows the VirtualCanvas managing up to 50,000 random WPF shapes providing smooth scrolling and
    /// zooming while creating those shapes on the fly.  This helps make a WPF canvas that is a lot more
    /// scalable.
    /// </summary>
    public class VirtualCanvasViewModel : ContentViewModel

    {
        public Controls.VirtualCanvas Graph { get; set; }
        public MapZoom Zoom { get; }

        public VirtualCanvasViewModel(IContainerExtension containerExtension) : base(containerExtension)
        {
            Views.VirtualCanvas canvas = new Views.VirtualCanvas();
            View = canvas;

            Graph = canvas.Graph;
            Zoom = Graph.Zoom;
        }
    }
}
