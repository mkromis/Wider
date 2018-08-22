using Autofac;
using Prism.Commands;
using Prism.Events;
using System;
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
            LoadToolbar();
        }

        private void LoadCommand()
        {
            ICommandManager commandManager = _container.Resolve<ICommandManager>();

            ICommand refreshCommand = new DelegateCommand(RefreshCommand);
            ICommand updateClipboard = new DelegateCommand<String>(UpdateDocument);
            ICommand exitCommand = new DelegateCommand(() => App.Current.MainWindow.Close());

            commandManager.RegisterCommand("RefreshCommand", refreshCommand);
            commandManager.RegisterCommand("UpdateCommand", updateClipboard);
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

            // Clipboard items
            menuService.Add(new MenuItemViewModel("_Clipboard", 3));
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

        // Menu command to refersh the cliboard command
        private void RefreshCommand()
        {
            // Refresh Menu items
            ICommandManager commandManager = _container.Resolve<ICommandManager>();
            IToolbarService toolbarService = _container.Resolve<IToolbarService>();
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

                toolbarService.Get("Standard").Get("Clipboard").Add(
                    new MenuItemViewModel(name, ++index, null, commandManager.GetCommand("UpdateCommand"))
                    {
                        CommandParameter = name
                    });
            }
            menuService.Refresh();
            Documents.Clear();
        }

        // Show the document for clipboard strings.
        private void UpdateDocument(String format)
        {
            try
            {
                Object data = Clipboard.GetData(format);

                switch (data)
                {
                    case String stringData:
                        {
                            StringOutputViewModel viewModel = _container.Resolve<StringOutputViewModel>();
                            viewModel.Title = format;
                            viewModel.Content = stringData;
                            Documents.Add(viewModel);
                            return;
                        }
                    case String[] stringArray:
                        {
                            StringOutputViewModel viewModel = _container.Resolve<StringOutputViewModel>();
                            viewModel.Title = format;
                            viewModel.Content = String.Join("\n", stringArray);
                            Documents.Add(viewModel);
                            return;
                        }
                }
            }
            catch (COMException e)
            {
                StringOutputViewModel viewModel = _container.Resolve<StringOutputViewModel>();
                viewModel.Title = format;
                viewModel.Content = e.Message;
                Documents.Add(viewModel);
            }
        }
    }
}
