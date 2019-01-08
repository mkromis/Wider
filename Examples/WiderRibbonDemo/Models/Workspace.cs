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

        public ICommand OpenCanvasCommand => new DelegateCommand(() => OpenAndFocus<VirtualCanvasViewModel>());

        public ICommand TaskRunCommand => new DelegateCommand(() => OpenAndFocus<TaskRunTestsViewModel>());

        private T OpenAndFocus<T>() where T : ContentViewModel
        {
            T vm = (T)Documents.Where(x => x is T).FirstOrDefault();
            if (vm == null)
            {
                vm = Container.Resolve<T>();
                Documents.Add(vm);
            }

            // if not exist
            ActiveDocument = vm;
            return vm;
        }

        public Workspace(IContainerExtension container) : base(container) => 
            _eventAggregator.GetEvent<ActiveContentChangedEvent>().Subscribe(ActiveDocumentChanged);

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
