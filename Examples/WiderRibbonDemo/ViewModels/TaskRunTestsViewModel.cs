using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Wider.Core.Services;
using WiderRibbonDemo.Models;
using WiderRibbonDemo.Views;

namespace WiderRibbonDemo.ViewModels
{
    public class TaskRunTestsViewModel : ContentViewModel<EmptyModel>
    {
        private Double _progress;

        public TaskRunTestsViewModel(IContainerExtension container) : base(container)
        {
            Title = "Task Run Tests";
            Model = new EmptyModel();
            View = new TaskRunTests();
        }

        public Double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public ICommand StartRun => new DelegateCommand(() =>
        {
            Task.Run(async () =>
            {
                for (Int32 x = 0; x < 11; ++x)
                {
                    await Task.Delay(250);
                    Progress = x / 10.0d;
                }
            });
        });
    }
}
