using System;
using System.Windows;
using System.Windows.Controls;
using Wider.Core.Services;

namespace WiderClipboard.Views
{
    /// <summary>
    /// Interaction logic for HexViewer
    /// </summary>
    public partial class HexViewer : IContentView, IDisposable
    {
        public HexViewer() => InitializeComponent();

        public void Dispose()
        {
            editor.Dispose();
            editor = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
