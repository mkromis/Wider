#region License

// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using Autofac;
using Prism.Autofac;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Wider.Core.Services;
using Wider.Core.Settings;
using Wider.Interfaces;
using Wider.Interfaces.Controls;
using Wider.Interfaces.Events;
using Wider.Interfaces.Services;
using Wider.Interfaces.Settings;
using Wider.Shell;
using CommandManager = Wider.Core.Services.CommandManager;

namespace Wider.Core
{
    /// <summary>
    /// The Wider Core module which does the following things:
    /// 1. Registers <see cref="IOpenDocumentService" /> - The file service can be used to open a file/object from a location or from a content ID
    /// 2. Registers <see cref="ICommandManager" /> - The command manager can be used to register commands and reuse the commands in different locations
    /// 3. Registers <see cref="IContentHandlerRegistry" /> - A registry to maintain different content handlers. Each content handler should be able to open a different kind of file/object.
    /// 4. Registers <see cref="IThemeManager" /> - A registry for themes
    /// 5. Registers <see cref="ILoggerService" /> - If not registered already, registers the NLogService which can be used anywhere in the application
    /// 6. Registers <see cref="IMenuService"/> - The menu service used in the Wider application
    /// 7. Registers <see cref="IStatusbarService"/> - The status bar service used in the Wider application
    /// 8. Registers <see cref="IToolbarService" /> - The toolbar service used to register multiple toolbar's
    /// 9. Registers <see cref="AbstractMenuItem" /> - This acts as the menu service for the application - menus can be added/removed.
    /// 10. Adds an AllFileHandler which can open any file from the system - to override this handler, participating modules can add more handlers to the <see cref="IContentHandlerRegistry" />
    /// </summary>
    [Module(ModuleName = "Wider.Core")]
    public sealed class CoreModule : IModule
    {
        /// <summary>
        /// The container used in the application
        /// </summary>
        private readonly ContainerBuilder _builder;
        private readonly IContainer _container;

        /// <summary>
        /// The constructor of the CoreModule
        /// </summary>
        /// <param name="container">The injected container used in the application</param>
        /// <param name="eventAggregator">The injected event aggregator</param>
        public CoreModule(ContainerBuilder builder, IContainer container, IEventAggregator eventAggregator)
        {
            _builder = builder;
            _container = container;
            EventAggregator = eventAggregator;
        }

        /// <summary>
        /// The event aggregator pattern
        /// </summary>
        private IEventAggregator EventAggregator { get; }

        #region IModule Members

        /// <summary>
        /// The initialize call of the module - this gets called when the container is trying to load the modules.
        /// Register your <see cref="Type"/>s and Commands here
        /// </summary>
        public void Initialize()
        {
            EventAggregator.GetEvent<SplashMessageUpdateEvent>().Publish(
                new SplashMessageUpdateEvent {Message = "Loading Core Module"});

            // Register types
            _builder.RegisterType<ThemeSettings>().As<IThemeSettings>();
            _builder.RegisterType<RecentViewSettings>().As<IRecentViewSettings>();
            _builder.RegisterType<WindowPositionSettings>().As<IWindowPositionSettings>();
            _builder.RegisterType<ToolbarPositionSettings>().As<IToolbarPositionSettings>();
            _builder.RegisterType<CommandManager>().As<ICommandManager>();
            _builder.RegisterType<ContentHandlerRegistry>().As<IContentHandlerRegistry>();
            _builder.RegisterType<WiderStatusbar>().As<IStatusbarService>();
            _builder.RegisterType<ThemeManager>().As<IThemeManager>();
            _builder.RegisterType<ToolbarService>().As<IToolbarService>();

            // https://bitbucket.org/dadhi/dryioc/wiki/Wrappers#markdown-header-func-of-a-with-parameters
            //_container.Register<IMenuService, MenuItemViewModel>(
            //    Made.Of(() => new MenuItemViewModel(
            //        Arg.Of<String>("$MAIN$"),
            //        Arg.Of<Int32>(-1),
            //        Arg.Of<ImageSource>(null),
            //        Arg.Of<ICommand>(null),
            //        Arg.Of<KeyGesture>(null),
            //        Arg.Of<Boolean>(false),
            //        Arg.Of<Boolean>(false),
            //        Arg.Of<DryIoc.IContainer>(_container))));

            //new InjectionConstructor(
            //    new InjectionParameter(typeof (String),"$MAIN$"),
            //    new InjectionParameter(typeof (Int32), 1),
            //    new InjectionParameter(typeof (ImageSource), null),
            //    new InjectionParameter(typeof (ICommand),null),
            //    new InjectionParameter(typeof (KeyGesture), null),
            //    new InjectionParameter(typeof (Boolean), false),
            //    new InjectionParameter(typeof (Boolean), false),
            //    new InjectionParameter(typeof (DryIoc.IContainer), _container)));

            //_container.Register<ToolbarViewModel>(
            //    Made.Of(() => new ToolbarViewModel(
            //        Arg.Of<String>("$MAIN$"),
            //        Arg.Of<Int32>(1),
            //        Arg.Of<ImageSource>(null),
            //        Arg.Of<ICommand>(null),
            //        Arg.Of<Boolean>(false),
            //        Arg.Of<DryIoc.IContainer>(_container))));
            //new InjectionConstructor(
            //    new InjectionParameter(typeof(String), "$MAIN$"),
            //    new InjectionParameter(typeof(Int32), 1),
            //    new InjectionParameter(typeof(ImageSource), null),
            //    new InjectionParameter(typeof(ICommand), null),
            //    new InjectionParameter(typeof(Boolean), false),
            //    new InjectionParameter(typeof(DryIoc.IContainer), _container)));

            _builder.RegisterType<SettingsManager>().As<ISettingsManager>();
            _builder.RegisterType<OpenDocumentService>().As<IOpenDocumentService>();

            AppCommands();
            LoadSettings();

            //Try resolving a workspace
            if (_container.TryResolve<AbstractWorkspace>(out AbstractWorkspace temp1) == false) 
            {
                _builder.RegisterType<Workspace>().As<AbstractWorkspace>();
            }

            // Try resolving a logger service - if not found, then register the NLog service
            if (_container.TryResolve<ILoggerService>(out ILoggerService temp2) == false)
            {
                _builder.RegisterType<DebugLogService>().As<ILoggerService>();
            }
        }

        #endregion

        /// <summary>
        /// The AppCommands registered by the Core Module
        /// </summary>
        private void AppCommands()
        {
            ICommandManager manager = _container.Resolve<ICommandManager>();
            IContentHandlerRegistry registry = _container.Resolve<IContentHandlerRegistry>();

            //TODO: Check if you can hook up to the Workspace.ActiveDocument.CloseCommand
            DelegateCommand<Object> closeCommand = new DelegateCommand<Object>(CloseDocument, CanExecuteCloseDocument);
            manager.RegisterCommand("CLOSE", closeCommand);

            
            manager.RegisterCommand("NEW", registry.NewCommand);
        }

        private void LoadSettings()
        {
            ShellView view;
            ShellViewMetro metroView;

            //Resolve to get the last used theme from the settings
            _container.Resolve<ThemeSettings>();
            IShell shell = _container.Resolve<IShell>();
            WindowPositionSettings position = _container.Resolve<IWindowPositionSettings>() as WindowPositionSettings;

            //Set the position of the window based on previous session values based on metro or regular
            if (WiderBootstrapper.IsMetro == true)
            {
                metroView = shell as ShellViewMetro;
                if (metroView != null)
                {
                    metroView.Top = position.Top;
                    metroView.Left = position.Left;
                    metroView.Width = position.Width;
                    metroView.Height = position.Height;
                    metroView.WindowState = position.State;
                }
            }
            else
            {
                view = shell as ShellView;
                if (view != null)
                {
                    view.Top = position.Top;
                    view.Left = position.Left;
                    view.Width = position.Width;
                    view.Height = position.Height;
                    view.WindowState = position.State;
                }
            }
        }

        #region Commands

        /// <summary>
        /// Can the close command execute? Checks if there is an ActiveDocument - if present, returns true.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns><c>true</c> if this instance can execute close document; otherwise, <c>false</c>.</returns>
        private Boolean CanExecuteCloseDocument(Object obj)
        {
            if (obj is ContentViewModel vm)
            {
                return true;
            }

            IWorkspace workspace = _container.Resolve<AbstractWorkspace>();
            return workspace.ActiveDocument != null;
        }

        /// <summary>
        /// CloseDocument method that gets called when the Close command gets executed.
        /// </summary>
        private void CloseDocument(Object obj)
        {
            IWorkspace workspace = _container.Resolve<AbstractWorkspace>();
            ILoggerService logger = _container.Resolve<ILoggerService>();
            IMenuService menuService = _container.Resolve<IMenuService>();

            System.ComponentModel.CancelEventArgs e = obj as System.ComponentModel.CancelEventArgs;

            if (!(obj is ContentViewModel activeDocument))
            {
                activeDocument = workspace.ActiveDocument;
            }

            if (activeDocument.Handler != null && activeDocument.Model.IsDirty)
            {
                //means the document is dirty - show a message box and then handle based on the user's selection
                MessageBoxResult res = MessageBox.Show(String.Format("Save changes for document '{0}'?", activeDocument.Title),
                                          "Are you sure?", MessageBoxButton.YesNoCancel);

                //Pressed Yes
                if (res == MessageBoxResult.Yes)
                {
                    if (!workspace.ActiveDocument.Handler.SaveContent(workspace.ActiveDocument))
                    {
                        //Failed to save - return cancel
                        res = MessageBoxResult.Cancel;

                        //Cancel was pressed - so, we cant close
                        if (e != null)
                        {
                            e.Cancel = true;
                        }
                        return;
                    }
                }

                //Pressed Cancel
                if (res == MessageBoxResult.Cancel)
                {
                    //Cancel was pressed - so, we cant close
                    if (e != null)
                    {
                        e.Cancel = true;
                    }
                    return;
                }
            }

            if (e == null)
            {
                logger.Log("Closing document " + activeDocument.Model.Location, Category.Info, Priority.None);
                workspace.Documents.Remove(activeDocument);
                EventAggregator.GetEvent<ClosedContentEvent>().Publish(activeDocument);
                menuService.Refresh();
            }
            else
            {
                // If the location is not there - then we can remove it.
                // This can happen when on clicking "No" in the popup and we still want to quit
                if (activeDocument.Model.Location == null)
                {
                    workspace.Documents.Remove(activeDocument);
                    EventAggregator.GetEvent<ClosedContentEvent>().Publish(activeDocument);
                }
            }
        }
        #endregion
    }
}