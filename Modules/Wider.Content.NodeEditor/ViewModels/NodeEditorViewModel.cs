using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Wider.Content.VirtualCanvas.Gestures;
using Wider.Core.Services;

namespace Wider.Content.NodeEditor.ViewModels
{
    public class NodeEditorViewModel : ContentViewModel, INodeEditor
    {
        public MapZoom Zoom { get; private set; }
        public Pan Pan { get; private set; }
        public RectangleSelectionGesture RectZoom { get; private set; }
        public AutoScroll AutoScroll { get; private set; }

        public VirtualCanvas.Controls.VirtualCanvas Graph { get; set; }
        public IEnumerable<INode> Nodes { get; }

        public NodeEditorViewModel(IContainerExtension container) : base(container)
        {
            Views.NodeEditor node = new Views.NodeEditor();
            Graph = node.GetGraph();
            View = node;

            Canvas target = Graph.ContentCanvas;
            Zoom = new MapZoom(target);
            Pan = new Pan(target, Zoom, false);
            AutoScroll = new AutoScroll(target, Zoom);
            RectZoom = new RectangleSelectionGesture(target, Zoom, ModifierKeys.Control)
            {
                ZoomSelection = true
            };
        }
    }
}
