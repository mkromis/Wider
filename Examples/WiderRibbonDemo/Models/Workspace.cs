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
        private readonly ISettingsManager _settingsManager;
        private readonly IThemeSettings _themeSettings;
        private readonly IThemeManager _themeManager;

        public VirtualCanvasViewModel CanvasViewModel
        {
            get => _canvasViewModel;
            set => SetProperty(ref _canvasViewModel, value);
        }

        public ICommand OpenCanvasCommand => new DelegateCommand(() =>
        {
            CanvasViewModel = OpenAndFocus<VirtualCanvasViewModel>();
            CanvasViewModel.IsClosing += (s, e) => CanvasViewModel = null;
        });

        public ICommand TaskRunCommand => new DelegateCommand(() =>
        {
            ContentViewModel<ContentModel> vm = (ContentViewModel<ContentModel>)Documents.Where(x => x.GetType() == typeof(TaskRunTestsViewModel)).FirstOrDefault();
            if (vm == null)
            {
                TaskRunTestsViewModel model = Container.Resolve<TaskRunTestsViewModel>();
                Documents.Add(model);
            }

            // if not exist
            ActiveDocument = vm;
        });

        public ICommand SettingsCommand => _settingsManager.SettingsCommand;

        private T OpenAndFocus<T>() where T : ContentViewModel<ContentModel>
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
            _settingsManager = Container.Resolve<ISettingsManager>();
            _themeSettings = Container.Resolve<IThemeSettings>();
            _themeManager = Container.Resolve<IThemeManager>();

            // Setup theme
            String themeName = _themeSettings.SelectedTheme;
            if (themeName == "Default")
            {
                themeName = _themeSettings.GetSystemTheme();
            }
            _themeManager.SetCurrent(themeName);

            // Setup settings
            _settingsManager.Add(new SettingsItem("General", Container.Resolve<GeneralSettings>()));
        }

        public IEnumerable<String> ThemeList => _themeManager.Themes.Select(x => x.Name);

        public String SelectedTheme
        {
            set => _themeManager.SetCurrent(value);
            get => _themeManager.Current.Name;
        }
    }
}
