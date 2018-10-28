using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Reflection;
using System.Windows;
using Wider.Core.Services;

namespace Wider.Core
{
    public class WiderApplication : PrismApplication
    {
        private CoreModule coreModule;

        protected override Window CreateShell()
        {
            // now we should have a shell, load settings and show if we can.
            IShell shell = Container.Resolve<IShell>();
            coreModule.LoadSettings();

            // Assign main window object and show window.
            Window window = shell as Window;
            window.DataContext = Container.Resolve<IWorkspace>();
            return window;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Load core module, this is internal so call manually
            coreModule = Container.Resolve<CoreModule>();
            coreModule.RegisterTypes(containerRegistry);
            coreModule.OnInitialized(Container);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.Initialize();

            // Check for existance, make sure we init first.
            // load from disk in case of it not showing up in module catalog.
            // resolving seems to init properly.
            if (System.IO.File.Exists("Wider.Splash.dll"))
            {
                // Assume exits
                Assembly assembly = Assembly.LoadFrom("Wider.Splash.dll");
                Type type = assembly.GetType("Wider.Splash.SplashModule");
                IModule splash = (IModule)Container.Resolve(type);
                splash.OnInitialized(Container);
            }
            base.ConfigureModuleCatalog(moduleCatalog);
        }
    }
}
