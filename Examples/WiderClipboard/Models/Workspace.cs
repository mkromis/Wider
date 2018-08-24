using Autofac;
using Prism.Commands;
using Prism.Events;
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
        public Workspace(IContainer container, IEventAggregator eventAggregator) : base(container, eventAggregator)
        {
            Title = "Wider Clipboard Viewer";

            LoadCommand();
            LoadMenu();
            //LoadToolbar();
            LoadTools();
        }

        private void LoadCommand()
        {
            ICommandManager commandManager = _container.Resolve<ICommandManager>();

            ICommand selectionView = new DelegateCommand(() => Tools.Where(x => x.Title == "Selection").First().IsVisible = true);
            ICommand refreshCommand = new DelegateCommand(() => ((SelectionViewModel)Tools.Where(x => x.Title == "Selection").First()).Refresh());
            ICommand exitCommand = new DelegateCommand(() => App.Current.MainWindow.Close());

            commandManager.RegisterCommand("SelectionView", selectionView);
            commandManager.RegisterCommand("RefreshCommand", refreshCommand);
            commandManager.RegisterCommand("ExitCommand", exitCommand);
        }

        private void LoadMenu()
        {
            ICommandManager commandManager = _container.Resolve<ICommandManager>();
            IMenuService menuService = _container.Resolve<IMenuService>();

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
        }

        private void LoadToolbar()
        {
            ICommandManager commandManager = _container.Resolve<ICommandManager>();
            IMenuService menuService = _container.Resolve<IMenuService>();
            IToolbarService toolbarService = _container.Resolve<IToolbarService>();

            toolbarService.Add(new ToolbarViewModel("Standard", 1) { Band = 1, BandIndex = 1 });
            toolbarService.Get("Standard").Add(menuService.Get("_Refersh"));
            toolbarService.Get("Standard").Add(new MenuItemViewModel("Clipboard", 201));
        }

        private void LoadTools()
        {
            SelectionViewModel selectionViewModel = _container.Resolve<SelectionViewModel>();
            Tools.Add(selectionViewModel);
        }
    }
}
