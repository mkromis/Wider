using Autofac;
using Prism.Modularity;
using Prism.Regions;
using Wider.Core.Services;
using Wider.Shell.Metro.Views;

namespace Wider.Shell.Metro
{
    [Module(ModuleName = "Wider.Shell.Metro")]
    public class MetroModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private ContainerBuilder _builder;
        private readonly IContainer _container;

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