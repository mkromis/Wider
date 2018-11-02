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

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Load core module, this is internal so call manually
            coreModule = Container.Resolve<CoreModule>();
            coreModule.RegisterTypes(containerRegistry);
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog { ModulePath = "." };
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

        protected override Window CreateShell()
        {
            coreModule.OnInitialized(Container);
            base.InitializeModules();

            // now we should have a shell, load settings and show if we can.
            IShell shell = Container.Resolve<IShell>();
            coreModule.LoadSettings();

            // Assign main window object and show window.
            Window main = shell as Window;
            main.DataContext = Container.Resolve<IWorkspace>();
            return main;
        }
    }
}
