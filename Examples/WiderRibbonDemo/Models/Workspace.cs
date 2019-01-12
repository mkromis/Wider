using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Wider.Core.Events;
using Wider.Core.Services;
using Wider.Core.Settings;
using Wider.Shell.Ribbon.Themes;
using WiderRibbonDemo.ViewModels;

namespace WiderRibbonDemo.Models
{
    public class Workspace : AbstractWorkspace
    {
        // Handles data context for ribbon.
        private VirtualCanvasViewModel _canvasViewModel;
        private readonly IThemeSettings _themeSettings;
        private readonly IThemeManager _themneManager;

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

        public Workspace(IContainerExtension container) : base(container)
        {
            _themeSettings = Container.Resolve<IThemeSettings>();
            _themneManager = Container.Resolve<IThemeManager>();


            String themeName = _themeSettings.SelectedTheme;
            if (themeName == "Default")
            {
                themeName = _themeSettings.GetSystemTheme();
            }
            _themneManager.SetCurrent(themeName);
        }

        public IEnumerable<String> ThemeList => _themneManager.Themes.Select(x => x.Name);

        public String SelectedTheme
        {
            set => _themneManager.SetCurrent(value);
            get => _themneManager.Current.Name;
        }
    }
}
