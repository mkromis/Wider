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
using Prism.Modularity;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using Wider.Core;
using Wider.Interfaces;

namespace Wider.Shell
{
    public class WiderBootstrapper : AutofacBootstrapper
    {
        public static Boolean IsMetro { get; protected set; }

        public WiderBootstrapper(Boolean isMetro = true) => IsMetro = isMetro;

        //If you want your own splash window - inherit from the bootstrapper and register type ISplashView
        protected override void InitializeModules()
        {
            ModuleCatalog.Initialize();

            // Check for existance, make sure we init first.
            // load from disk in case of it not showing up in module catalog.
            // resolving seems to init properly.
            if (System.IO.File.Exists("Wider.Splash.dll"))
            {
                // Assume exits
                Assembly assembly = Assembly.LoadFrom("Wider.Splash.dll");
                Type type = assembly.GetType("Wider.Splash.SplashModule");
                IModule splash = (IModule)Container.Resolve(type);
                splash.Initialize();
            }

            // Load core module 
            IModule coreModule = Container.Resolve<CoreModule>();
            coreModule.Initialize();

            base.InitializeModules();
            Application.Current.MainWindow.DataContext = Container.Resolve<AbstractWorkspace>();

            (Shell as Window).Show();
        }

        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            //Create an instance of the workspace
            if (IsMetro)
            {
                //Use MahApps Metro window
                builder.RegisterType<ShellViewMetro>().As<IShell>().SingleInstance();
            }
            else
            {
                //Use regular window
                builder.RegisterType<ShellView>().As<IShell>().SingleInstance();
            }
            base.ConfigureContainerBuilder(builder);
        }

        protected override DependencyObject CreateShell() => (DependencyObject)Container.Resolve<IShell>();

        protected override void InitializeShell()
        {
            base.InitializeShell();
            Application.Current.MainWindow = (Window) Shell;
        }
    }
}