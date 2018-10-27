using Autofac;
using Prism.Autofac;
using Prism.Modularity;
using System.Windows;
using Wider.Core;
using WiderRibbonDemo.Views;

namespace WiderRibbonDemo
{
    internal class Bootstrapper : WiderBootstrapper
    {
        protected override void ConfigureModuleCatalog()
        {
            ModuleCatalog moduleCatalog = (ModuleCatalog)ModuleCatalog;
            //moduleCatalog.AddModule(typeof(YOUR_MODULE));
        }
    }
}
