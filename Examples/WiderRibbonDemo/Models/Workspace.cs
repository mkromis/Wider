using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Wider.Core.Events;
using Wider.Core.Services;
using WiderRibbonDemo.ViewModels;

namespace WiderRibbonDemo.Models
{
    public class Workspace : AbstractWorkspace
    {
        // Handles data context for ribbon.
        private VirtualCanvasViewModel _canvasViewModel;
        private NodeEditorViewModel _nodeEditorViewModel;

        /// <summary>
        /// This is needed for ribbon control
        /// </summary>
        public VirtualCanvasViewModel CanvasViewModel
        {
            get => _canvasViewModel;
            set => SetProperty(ref _canvasViewModel, value);
        }

        public NodeEditorViewModel NodeEditorViewModel {
            get => _nodeEditorViewModel;
            private set => SetProperty(ref _nodeEditorViewModel, value);
        }

        public ICommand OpenCanvasCommand => new DelegateCommand(() =>
        {
            if (CanvasViewModel != null)
            {
                ActiveDocument = CanvasViewModel;
                return;
            }

            // if not exist
            CanvasViewModel = _container.Resolve<VirtualCanvasViewModel>();
            Documents.Add(CanvasViewModel);
            ActiveDocument = CanvasViewModel;
        });

        public ICommand OpenNodeEditor => new DelegateCommand(() =>
        {
            if (NodeEditorViewModel != null)
            {
                ActiveDocument = NodeEditorViewModel;
                return;
            }

            NodeEditorViewModel = _container.Resolve<NodeEditorViewModel>();
            Documents.Add(NodeEditorViewModel);
            ActiveDocument = NodeEditorViewModel;
        });

        public Workspace(IContainerExtension container) : base(container) =>
            _eventAggregator.GetEvent<ActiveContentChangedEvent>().Subscribe(ActiveDocumentChanged);

        private void ActiveDocumentChanged(ContentViewModel obj)
        {
            // reset defaults
            if (CanvasViewModel != null) { CanvasViewModel.ShowContextRibbon = false; }

            // enable on required items
            switch (obj)
            {
                case VirtualCanvasViewModel canvas:
                    canvas.ShowContextRibbon = true;
                    break;
                default:
                    break;
            }
        }
    }
}
