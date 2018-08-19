#region License
// Copyright (c) 2018 Mark Kromis
// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using Autofac;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Wider.Core.Controls;
using Wider.Core.Events;
using Wider.Core.Services;
using Wider.Core.Settings;
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
        private IContainer _container;

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
            _builder.RegisterType<ThemeSettings>().As<IThemeSettings>().SingleInstance();
            _builder.RegisterType<RecentViewSettings>().As<IRecentViewSettings>().SingleInstance();
            _builder.RegisterType<WindowPositionSettings>().As<IWindowPositionSettings>().SingleInstance();
            _builder.RegisterType<ToolbarPositionSettings>().As<IToolbarPositionSettings>().SingleInstance();
            _builder.RegisterType<CommandManager>().As<ICommandManager>().SingleInstance();
            _builder.RegisterType<ContentHandlerRegistry>().As<IContentHandlerRegistry>().SingleInstance();
            _builder.RegisterType<WiderStatusbar>().As<IStatusbarService>().SingleInstance();
            _builder.RegisterType<ThemeManager>().As<IThemeManager>().SingleInstance();

            // https://bitbucket.org/dadhi/dryioc/wiki/Wrappers#markdown-header-func-of-a-with-parameters
            _builder.RegisterType<MenuItemViewModel>()
                .As<IMenuService>()
                .WithParameter(TypedParameter.From<String>("$MAIN$"))
                .WithParameter(TypedParameter.From<Int32>(1))
                .WithParameter(TypedParameter.From<ImageSource>(null))
                .WithParameter(TypedParameter.From<ICommand>(null))
                .WithParameter(TypedParameter.From<KeyGesture>(null))
                .WithParameter(TypedParameter.From<Boolean>(false))
                .WithParameter(TypedParameter.From<Boolean>(false))
                .WithParameter(TypedParameter.From<IContainer>(_container))
                .SingleInstance();

            _builder.RegisterType<ToolbarViewModel>()
                .As<IToolbar>()
                .WithParameter(TypedParameter.From<String>("$MAIN$"))
                .WithParameter(TypedParameter.From<Int32>(1))
                .WithParameter(TypedParameter.From<ImageSource>(null))
                .WithParameter(TypedParameter.From<ICommand>(null))
                .WithParameter(TypedParameter.From<Boolean>(false))
                .WithParameter(TypedParameter.From<IContainer>(_container))
                .SingleInstance();

            _builder.RegisterType<OpenDocumentService>().As<IOpenDocumentService>().SingleInstance();

            _builder.RegisterType<Workspace>().As<AbstractWorkspace>().SingleInstance().PreserveExistingDefaults();
            _builder.RegisterType<DebugLogService>().As<ILoggerService>().SingleInstance().PreserveExistingDefaults();
            _builder.Update(_container);

            AppCommands();

            // This is done in bootstrapper to handle delayed loaded IShell
            //LoadSettings();
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

        public void LoadSettings()
        {
            //Resolve to get the last used theme from the settings
            _container.Resolve<ThemeSettings>();
            WindowPositionSettings position = _container.Resolve<IWindowPositionSettings>() as WindowPositionSettings;

            //Set the position of the window based on previous session values, delayed loaded in WiderBootstrpper
            if (_container.Resolve<IShell>() is IShell view)
            {
                view.Top = position.Top;
                view.Left = position.Left;
                view.Width = position.Width;
                view.Height = position.Height;
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
                MessageBoxResult res = 
                    MessageBox.Show($"Save changes for document '{activeDocument.Title}'?",
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