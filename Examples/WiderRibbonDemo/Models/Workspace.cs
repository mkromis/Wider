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


        public VirtualCanvasViewModel CanvasViewModel
        {
            get => _canvasViewModel;
            set => SetProperty(ref _canvasViewModel, value);
        }

        public ICommand OpenCanvasCommand => new DelegateCommand(() =>
        {
            IEnumerable<ContentViewModel> docs = Documents.Where(x => x is VirtualCanvasViewModel);
            if (docs.Count() > 0)
            {
                ActiveDocument = docs.First();
                return;
            }

            // if not exist
            CanvasViewModel = _container.Resolve<VirtualCanvasViewModel>();
            Documents.Add(CanvasViewModel);
            ActiveDocument = CanvasViewModel;
        });

        public Workspace(IContainerExtension container) : base(container)
        {
            _eventAggregator.GetEvent<ActiveContentChangedEvent>().Subscribe(ActiveDocumentChanged);
        }

        private void ActiveDocumentChanged(ContentViewModel obj)
        {
            //switch (obj)
            //{
            //    case VirtualCanvasViewModel canvas:
            //        CanvasVisibility = Visibility.Visible;
            //        break;
            //    default:
            //        break;
            //}
        }
    }
}
