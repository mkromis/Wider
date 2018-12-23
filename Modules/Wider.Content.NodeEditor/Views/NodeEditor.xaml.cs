using System;
using System.Windows.Controls;
using Wider.Content.VirtualCanvas.Controls;
using Wider.Core.Services;

namespace Wider.Content.NodeEditor.Views
{
    /// <summary>
    /// Interaction logic for NodeEditor
    /// </summary>
    public partial class NodeEditor : UserControl, IContentView
    {
        public NodeEditor()
        {
            InitializeComponent();
        }

        internal VirtualCanvas.Controls.VirtualCanvas GetGraph()
        {
            return this.Graph;
        }
    }
}
