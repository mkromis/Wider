#region License

// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Windows;
using Prism.Ioc;
using Prism.Modularity;
using Wider.Core;
using Wider.Core.Services;
using Wider.Splash;
using WiderMD.Views;

namespace WiderMD
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : WiderApplication
    {
        //private MDBootstrapper b;

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterTypes(containerRegistry);

            //Register your splash view or else the default splash will load
            containerRegistry.RegisterSingleton<ISplashView, AppSplash>();

            //Register your workspace here - if you have any
            containerRegistry.RegisterSingleton<IWorkspace, MDWorkspace>();

        }

        protected override void InitializeModules()
        {
            // You can also override the logger service. Currently, NLog is used.
            // Since the config file is there in the output location, text files should be available in the Logs folder.

            //Initialize the original bootstrapper which will load modules from the probing path. Check app.config for probing path.
            base.InitializeModules();

            IShell shell = Container.Resolve<IShell>();
            (shell as Window).Loaded += App_Loaded;
            (shell as Window).Unloaded += App_Unloaded;
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            MultipleDirectoryModuleCatalog catalog =
                new MultipleDirectoryModuleCatalog(new String[] { @".", @".\External", @".\Internal" });
            return catalog;
        }

        void App_Unloaded(Object sender, System.EventArgs e)
        {
            IShell shell = Container.Resolve<IShell>();
            shell.SaveLayout();
        }

        void App_Loaded(Object sender, RoutedEventArgs e)
        {
            IShell shell = Container.Resolve<IShell>();
            shell.LoadLayout();
        }
    }
}