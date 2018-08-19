using Wider.Shell.Metro.Views;
using Prism.Modularity;
using Prism.Regions;
using System;
using Autofac;
using Prism.Autofac;
using Wider.Core.Services;

namespace Wider.Shell.Metro
{
    [Module(ModuleName = "Wider.Shell.Metro")]
    //[ModuleDependency("Wider.Shell")]
    public class MetroModule : IModule
    {
        private IRegionManager _regionManager;
        private ContainerBuilder _builder;
        private IContainer _container;

        public MetroModule(ContainerBuilder builder, IContainer container, IRegionManager regionManager)
        {
            _builder = builder;
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            //Use MahApps Metro window
            _builder.RegisterType<ShellView>().As<IShell>().SingleInstance();
            _builder.Update(_container);
        }
    }
}