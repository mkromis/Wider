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
        private Visibility _canvasVisibility = Visibility.Collapsed;


        public Visibility CanvasVisibility
        {
            get => _canvasVisibility;
            set => SetProperty(ref _canvasVisibility, value);
        }

        public ICommand OpenCanvasCommand => new DelegateCommand(() =>
        {
            IEnumerable<ContentViewModel> docs = Documents.Where(x => x is CanvasSampleViewModel);
            if (docs.Count() > 0)
            {
                ActiveDocument = docs.First();
                return;
            }

            // if not exist
            CanvasSampleViewModel newItem = _container.Resolve<CanvasSampleViewModel>();
            Documents.Add(newItem);
            ActiveDocument = newItem;
        });

        public Workspace(IContainerExtension container) : base(container)
        {
            _eventAggregator.GetEvent<ActiveContentChangedEvent>().Subscribe(ActiveDocumentChanged);
        }

        private void ActiveDocumentChanged(ContentViewModel obj)
        {
            CanvasVisibility = Visibility.Collapsed;
            switch (obj)
            {
                case CanvasSampleViewModel canvas:
                    CanvasVisibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }
    }
}
