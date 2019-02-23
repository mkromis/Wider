using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Wider.Core;
using Wider.Core.Controls;
using Wider.Core.Services;
using WiderClipboard.ViewModels;

namespace WiderClipboard.Models
{
    internal class Workspace : AbstractWorkspace
    {
        readonly IThemeManager _themeManager;

        public Workspace(IContainerExtension container, IThemeManager themeManager) : base(container)
        {
            _themeManager = themeManager;
            Title = "Wider Clipboard Viewer";

            LoadCommand();
            LoadMenu();
            LoadToolbar();
            LoadTools();
        }

        private void LoadCommand()
        {
            ICommandManager commandManager = Container.Resolve<ICommandManager>();

            ICommand selectionView = new DelegateCommand(() => Tools.Where(x => x.Title == "Selection").First().IsVisible = true);
            ICommand refreshCommand = new DelegateCommand(() => ((SelectionViewModel)Tools.Where(x => x.Title == "Selection").First()).Refresh());
            ICommand exitCommand = new DelegateCommand(() => App.Current.MainWindow.Close());
            ICommand themeCommand = new DelegateCommand<String>((x) => _themeManager.SetCurrent(x));

            commandManager.RegisterCommand("SelectionView", selectionView);
            commandManager.RegisterCommand("RefreshCommand", refreshCommand);
            commandManager.RegisterCommand("ThemeCommand", themeCommand);
            commandManager.RegisterCommand("ExitCommand", exitCommand);
        }

        private void LoadMenu()
        {
            ICommandManager commandManager = Container.Resolve<ICommandManager>();
            IMenuService menuService = Container.Resolve<IMenuService>();

            // Add file command
            menuService.Add(new MenuItemViewModel("_File", 1));

            menuService.Get("_File")
                .Add(new MenuItemViewModel("E_xit", 101, null,
                    commandManager.GetCommand("ExitCommand"),
                    new KeyGesture(Key.F4, ModifierKeys.Alt, "Alt + F4")));

            // Add refresh command
            menuService.Add(new MenuItemViewModel("_Refersh", 2, null,
                commandManager.GetCommand("RefreshCommand"),
                new KeyGesture(Key.F5)));

            // Tools Menu
            menuService.Add(new MenuItemViewModel("_Tools", 3));
            menuService.Get("_Tools").Add(new MenuItemViewModel("_Selection", 401, null,
                commandManager.GetCommand("SelectionView"), new KeyGesture(Key.F9)));

            menuService.Add(new MenuItemViewModel("T_hemes", 4));
            Int32 themeCount = 500;
            foreach (ITheme theme in _themeManager.Themes)
            {
                menuService.Get("T_hemes")
                    .Add(new MenuItemViewModel(theme.Name, ++themeCount, null, commandManager.GetCommand("ThemeCommand"))
                    {
                        CommandParameter = theme.Name,
                    });
            }
        }

        private void LoadToolbar()
        {
            IMenuService menuService = Container.Resolve<IMenuService>();
            IToolbarService toolbarService = Container.Resolve<IToolbarService>();

            toolbarService.Add(new ToolbarViewModel("Standard", 1) { Band = 1, BandIndex = 1 });
            toolbarService.Get("Standard").Add(menuService.Get("_Refersh"));
            toolbarService.Get("Standard").Add(new MenuItemViewModel("Clipboard", 201));
        }

        private void LoadTools()
        {
            SelectionViewModel selectionViewModel = Container.Resolve<SelectionViewModel>();
            Tools.Add(selectionViewModel);
        }
    }
}
