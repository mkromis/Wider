using Autofac;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows;
using System.Windows.Input;
using Wider.Core;
using Wider.Core.Controls;
using Wider.Core.Services;

namespace WiderClipboard.Models
{
    internal class Workspace : AbstractWorkspace
    {
        public Workspace(IContainer container, IEventAggregator eventAggregator) : base(container, eventAggregator)
        {
            Title = "Wider Clipboard Viewer";

            LoadCommand();
            LoadMenu();
        }


        private void LoadCommand()
        {
            ICommandManager commandManager = _container.Resolve<ICommandManager>();

            ICommand refreshCommand = new DelegateCommand(RefreshCommand);
            ICommand updateClipboard = new DelegateCommand<String>(x =>
            {
                MessageBox.Show(App.Current.MainWindow, x);
            });
            ICommand exitCommand = new DelegateCommand(() => App.Current.MainWindow.Close());
            

            commandManager.RegisterCommand("RefreshCommand", refreshCommand);
            commandManager.RegisterCommand("UpdateCommand", updateClipboard);
            commandManager.RegisterCommand("ExitCommand", exitCommand);
        }

        private void RefreshCommand()
        {
            // Refresh Menu items
            ICommandManager commandManager = _container.Resolve<ICommandManager>();
            IMenuService menuService = _container.Resolve<IMenuService>();

            MenuItemViewModel item = menuService.Get("_Clipboard") as MenuItemViewModel;
            foreach (AbstractCommandable child in item.Children)
            {
                item.Remove(child.GuidString);
            }

            Int32 index = 300;
            foreach (String name in Clipboard.GetDataObject().GetFormats(false))
            {
                item.Add(new MenuItemViewModel(name, ++index, null,
                    commandManager.GetCommand("UpdateCommand"))
                {
                    CommandParameter = name
                });
            }

            menuService.Refresh();
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

            // Clipboard items
            menuService.Add(new MenuItemViewModel("_Clipboard", 3));

        }
    }
}
