using WiderClipboard.Views;
using System.Windows;
using Prism.Modularity;
using Autofac;
using Prism.Autofac;
using Wider.Core;

namespace WiderClipboard
{
    class Bootstrapper : WiderBootstrapper
    {
        //protected override DependencyObject CreateShell()
        //{
        //    return Container.Resolve<MainWindow>();
        //}

        //protected override void InitializeShell()
        //{
        //    Application.Current.MainWindow.Show();
        //}

        //protected override void ConfigureModuleCatalog()
        //{
        //    var moduleCatalog = (ModuleCatalog)ModuleCatalog;
        //    //moduleCatalog.AddModule(typeof(YOUR_MODULE));
        //}
    }
}
