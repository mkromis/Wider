#region License

// Copyright (c) 2018 Mark Kromis
// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to 
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
// sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Wider.Core.Controls;
using Wider.Core.Events;
using Wider.Core.Services;
using Wider.Core.Settings;
using Wider.Interfaces.Controls;
using Wider.Shell.Themes;
using WiderMD.Core.Settings;

namespace WiderMD.Core
{
    [Module(ModuleName = "WiderMD.Core")]
    [ModuleDependency("Wider.Tools.Logger")]
    public class CoreModule : IModule
    {
        IContainerProvider containerProvider;
        IEventAggregator eventAggregator;

        public void OnInitialized(IContainerProvider newContainerProvider)
        {
            containerProvider = newContainerProvider;
            eventAggregator = containerProvider.Resolve<IEventAggregator>();

            eventAggregator.GetEvent<SplashMessageUpdateEvent>()
                .Publish(new SplashMessageUpdateEvent { Message = "Loading Core Module" });

            IContentHandler handler = containerProvider.Resolve<MDHandler>();
            containerProvider.Resolve<IContentHandlerRegistry>().Register(handler);

            LoadTheme();
            LoadCommands();
            LoadMenus();
            LoadToolbar();
            LoadSettings();
        }

        private void LoadToolbar()
        {
            eventAggregator.GetEvent<SplashMessageUpdateEvent>().Publish(new SplashMessageUpdateEvent { Message = "Toolbar.." });
            IToolbarService toolbarService = containerProvider.Resolve<IToolbarService>();
            IMenuService menuService = containerProvider.Resolve<IMenuService>();
            ICommandManager manager = containerProvider.Resolve<ICommandManager>();

            toolbarService.Add(new ToolbarViewModel("Standard", 1) { Band = 1, BandIndex = 1 });
            toolbarService.Get("Standard").Add(menuService.Get("_File").Get("_New"));
            toolbarService.Get("Standard").Add(menuService.Get("_File").Get("_Open"));

            toolbarService.Add(new ToolbarViewModel("Edit", 1) { Band = 1, BandIndex = 2 });
            toolbarService.Get("Edit").Add(menuService.Get("_Edit").Get("_Undo"));
            toolbarService.Get("Edit").Add(menuService.Get("_Edit").Get("_Redo"));
            toolbarService.Get("Edit").Add(menuService.Get("_Edit").Get("Cut"));
            toolbarService.Get("Edit").Add(menuService.Get("_Edit").Get("Copy"));
            toolbarService.Get("Edit").Add(menuService.Get("_Edit").Get("_Paste"));

            toolbarService.Add(new ToolbarViewModel("Debug", 1) { Band = 1, BandIndex = 3 });
            toolbarService.Get("Debug").Add(new MenuItemViewModel("Debug", 1, new BitmapImage(new Uri(@"pack://application:,,,/WiderMD.Core;component/Icons/Play.png"))));
            toolbarService.Get("Debug").Get("Debug").Add(new MenuItemViewModel("Debug with Chrome", 1, new BitmapImage(new Uri(@"pack://application:,,,/WiderMD.Core;component/Icons/Play.png")), manager.GetCommand("OPEN")));
            toolbarService.Get("Debug").Get("Debug").Add(new MenuItemViewModel("Debug with FireFox", 2, new BitmapImage(new Uri(@"pack://application:,,,/WiderMD.Core;component/Icons/Play.png")), manager.GetCommand("OPEN")));
            toolbarService.Get("Debug").Get("Debug").Add(new MenuItemViewModel("Debug with Explorer", 3, new BitmapImage(new Uri(@"pack://application:,,,/WiderMD.Core;component/Icons/Play.png")), manager.GetCommand("OPEN")));

            if (toolbarService.ContextMenuItems != null)
            {
                menuService.Get("_Tools").Add(toolbarService.ContextMenuItems);
            }

            //Initiate the position settings changes for toolbar
            containerProvider.Resolve<IToolbarPositionSettings>();
        }

        private void LoadSettings()
        {
            ISettingsManager manager = containerProvider.Resolve<ISettingsManager>();
            manager.Add(new MDSettingsItem("Text Editor", 1, null));
            manager.Get("Text Editor").Add(new MDSettingsItem("General", 1, EditorOptions.Default));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<MDHandler>();
            containerRegistry.Register<MDViewModel>();
            containerRegistry.Register<MDView>();
        }

        private void LoadTheme()
        {
            eventAggregator.GetEvent<SplashMessageUpdateEvent>()
                .Publish(new SplashMessageUpdateEvent { Message = "Themes.." });
            IThemeManager manager = containerProvider.Resolve<IThemeManager>();
            IThemeSettings themeSettings = containerProvider.Resolve<IThemeSettings>();
            Window win = containerProvider.Resolve<IShell>() as Window;
            manager.AddTheme(new DefaultTheme());
            manager.AddTheme(new CleanTheme());
            manager.AddTheme(new VS2010Theme());
            manager.AddTheme(new BlueTheme());
            manager.AddTheme(new LightTheme());
            manager.AddTheme(new DarkTheme());
            win.Dispatcher.InvokeAsync(() => manager.SetCurrent(themeSettings.SelectedTheme));
        }

        private void LoadCommands()
        {
            eventAggregator.GetEvent<SplashMessageUpdateEvent>().Publish(new SplashMessageUpdateEvent
            { Message = "Commands.." });
            ICommandManager manager = containerProvider.Resolve<ICommandManager>();

            DelegateCommand openCommand = new DelegateCommand(OpenModule);
            DelegateCommand exitCommand = new DelegateCommand(CloseCommandExecute);
            DelegateCommand saveCommand = new DelegateCommand(SaveDocument, CanExecuteSaveDocument);
            DelegateCommand saveAsCommand = new DelegateCommand(SaveAsDocument, CanExecuteSaveAsDocument);
            DelegateCommand<String> themeCommand = new DelegateCommand<String>(ThemeChangeCommand);
            DelegateCommand loggerCommand = new DelegateCommand(ToggleLogger);


            manager.RegisterCommand("OPEN", openCommand);
            manager.RegisterCommand("SAVE", saveCommand);
            manager.RegisterCommand("SAVEAS", saveAsCommand);
            manager.RegisterCommand("EXIT", exitCommand);
            manager.RegisterCommand("LOGSHOW", loggerCommand);
            manager.RegisterCommand("THEMECHANGE", themeCommand);
        }

        private void CloseCommandExecute()
        {
            IShell shell = containerProvider.Resolve<IShell>();
            shell.Close();
        }

        private void LoadMenus()
        {
            eventAggregator.GetEvent<SplashMessageUpdateEvent>()
                .Publish(new SplashMessageUpdateEvent { Message = "Menus.." });
            ICommandManager manager = containerProvider.Resolve<ICommandManager>();
            IMenuService menuService = containerProvider.Resolve<IMenuService>();
            ISettingsManager settingsManager = containerProvider.Resolve<ISettingsManager>();
            IThemeSettings themeSettings = containerProvider.Resolve<IThemeSettings>();
            IRecentViewSettings recentFiles = containerProvider.Resolve<IRecentViewSettings>();
            IWorkspace workspace = containerProvider.Resolve<IWorkspace>();
            ToolViewModel logger = workspace.Tools.FirstOrDefault(f => f.ContentId == "Logger");

            menuService.Add(new MenuItemViewModel("_File", 1));

            menuService.Get("_File")
                .Add((new MenuItemViewModel("_New", 3,
                    new BitmapImage(new Uri(@"pack://application:,,,/WiderMD.Core;component/Icons/NewRequest_8796.png")),
                    manager.GetCommand("NEW"),
                    new KeyGesture(Key.N, ModifierKeys.Control, "Ctrl + N"))));
            menuService.Get("_File")
                .Add((new MenuItemViewModel("_Open", 4,
                    new BitmapImage(new Uri(@"pack://application:,,,/WiderMD.Core;component/Icons/OpenFileDialog_692.png")),
                    manager.GetCommand("OPEN"),
                    new KeyGesture(Key.O, ModifierKeys.Control, "Ctrl + O"))));
            menuService.Get("_File")
                .Add(new MenuItemViewModel("_Save", 5,
                    new BitmapImage(new Uri(@"pack://application:,,,/WiderMD.Core;component/Icons/Save_6530.png")),
                    manager.GetCommand("SAVE"),
                    new KeyGesture(Key.S, ModifierKeys.Control, "Ctrl + S")));
            menuService.Get("_File")
                .Add(new SaveAsMenuItemViewModel(
                    containerProvider, "Save As..", 6,
                    new BitmapImage(new Uri(@"pack://application:,,,/WiderMD.Core;component/Icons/Save_6530.png")),
                    manager.GetCommand("SAVEAS")));
            menuService.Get("_File")
                .Add(new MenuItemViewModel("Close", 8, null,
                    manager.GetCommand("CLOSE"),
                    new KeyGesture(Key.F4, ModifierKeys.Control, "Ctrl + F4")));

            menuService.Get("_File").Add(recentFiles.RecentMenu);

            menuService.Get("_File")
                .Add(new MenuItemViewModel("E_xit", 101, null,
                    manager.GetCommand("EXIT"),
                    new KeyGesture(Key.F4, ModifierKeys.Alt, "Alt + F4")));


            menuService.Add(new MenuItemViewModel("_Edit", 2));
            menuService.Get("_Edit")
                .Add(new MenuItemViewModel("_Undo", 1,
                    new BitmapImage(new Uri(@"pack://application:,,,/WiderMD.Core;component/Icons/Undo_16x.png")),
                    ApplicationCommands.Undo));
            menuService.Get("_Edit")
                .Add(new MenuItemViewModel("_Redo", 2,
                    new BitmapImage(new Uri(@"pack://application:,,,/WiderMD.Core;component/Icons/Redo_16x.png")),
                    ApplicationCommands.Redo));
            menuService.Get("_Edit").Add(MenuItemViewModel.Separator(15));
            menuService.Get("_Edit")
                .Add(new MenuItemViewModel("Cut", 20,
                    new BitmapImage(new Uri(@"pack://application:,,,/WiderMD.Core;component/Icons/Cut_6523.png")),
                    ApplicationCommands.Cut));
            menuService.Get("_Edit")
                .Add(new MenuItemViewModel("Copy", 21,
                    new BitmapImage(new Uri(@"pack://application:,,,/WiderMD.Core;component/Icons/Copy_6524.png")),
                    ApplicationCommands.Copy));
            menuService.Get("_Edit")
                .Add(new MenuItemViewModel("_Paste", 22,
                    new BitmapImage(new Uri(@"pack://application:,,,/WiderMD.Core;component/Icons/Paste_6520.png")),
                    ApplicationCommands.Paste));

            menuService.Add(new MenuItemViewModel("_View", 3));

            if (logger != null)
            {
                menuService.Get("_View")
                    .Add(new MenuItemViewModel("_Logger", 1,
                        new BitmapImage(new Uri(@"pack://application:,,,/WiderMD.Core;component/Icons/Undo_16x.png")),
                        manager.GetCommand("LOGSHOW"))
                    { IsCheckable = true, IsChecked = logger.IsVisible });
            }

            menuService.Get("_View").Add(new MenuItemViewModel("Themes", 1));

            // Get the listed themses and add to menu
            IThemeManager themeManager = containerProvider.Resolve<IThemeManager>();
            MenuItemViewModel themeMenu = menuService.Get("_View").Get("Themes") as MenuItemViewModel;

            // add items based on theme programmed
            foreach (ITheme theme in themeManager.Themes)
            {
                themeMenu
                .Add(new MenuItemViewModel(theme.Name, 1, null,
                    manager.GetCommand("THEMECHANGE"))
                {
                    IsCheckable = true,
                    IsChecked = (themeSettings.SelectedTheme == theme.Name),
                    CommandParameter = theme.Name
                });
            }

            menuService.Add(new MenuItemViewModel("_Tools", 4));
            menuService.Get("_Tools").Add(new MenuItemViewModel("Settings", 1, null, settingsManager.SettingsCommand));

            menuService.Add(new MenuItemViewModel("_Help", 4));
        }

        #region Commands

        #region Open

        private void OpenModule()
        {
            IOpenDocumentService service = containerProvider.Resolve<IOpenDocumentService>();
            service.Open();
        }

        #endregion

        #region Save

        private Boolean CanExecuteSaveDocument()
        {
            IWorkspace workspace = containerProvider.Resolve<IWorkspace>();
            if (workspace.ActiveDocument != null)
            {
                return workspace.ActiveDocument.Model.IsDirty;
            }
            return false;
        }

        private Boolean CanExecuteSaveAsDocument()
        {
            IWorkspace workspace = containerProvider.Resolve<IWorkspace>();
            return (workspace.ActiveDocument != null);
        }

        private void SaveDocument()
        {
            IWorkspace workspace = containerProvider.Resolve<IWorkspace>();
            ICommandManager manager = containerProvider.Resolve<ICommandManager>();
            workspace.ActiveDocument.Handler.SaveContent(workspace.ActiveDocument);
            manager.Refresh();
        }

        private void SaveAsDocument()
        {
            IWorkspace workspace = containerProvider.Resolve<IWorkspace>();
            ICommandManager manager = containerProvider.Resolve<ICommandManager>();
            if (workspace.ActiveDocument != null)
            {
                workspace.ActiveDocument.Handler.SaveContent(workspace.ActiveDocument, true);
                manager.Refresh();
            }
        }
        #endregion

        #region Theme

        private void ThemeChangeCommand(String s)
        {
            IThemeManager manager = containerProvider.Resolve<IThemeManager>();
            IMenuService menuService = containerProvider.Resolve<IMenuService>();
            Window win = containerProvider.Resolve<IShell>() as Window;

            MenuItemViewModel themeMenu = menuService.Get("_View").Get("Themes") as MenuItemViewModel;

            // Clear all checkboxes
            foreach (MenuItemViewModel menuitem in themeMenu.Children)
            {
                menuitem.IsChecked = false;
            }


            if (manager.CurrentTheme?.Name != s)
            {
                if (themeMenu.Get(s) is MenuItemViewModel mvm)
                {
                    mvm.IsChecked = true;
                }

                win.Dispatcher.InvokeAsync(() => manager.SetCurrent(s));
            }

        }

        #endregion

        #region Logger click

        private void ToggleLogger()
        {
            IWorkspace workspace = containerProvider.Resolve<IWorkspace>();
            IMenuService menuService = containerProvider.Resolve<IMenuService>();
            ToolViewModel logger = workspace.Tools.First(f => f.ContentId == "Logger");
            if (logger != null)
            {
                logger.IsVisible = !logger.IsVisible;
                MenuItemViewModel mi = menuService.Get("_View").Get("_Logger") as MenuItemViewModel;
                mi.IsChecked = logger.IsVisible;
            }
        }
        #endregion

        #endregion
    }
}