using Wider.Shell.Views;
using Prism.Modularity;
using Prism.Regions;
using System;
using Autofac;
using Prism.Autofac;

namespace Wider.Shell
{
    public class ShellModule : IModule
    {
        private IRegionManager _regionManager;
        private ContainerBuilder _builder;

        public ShellModule(ContainerBuilder builder, IRegionManager regionManager)
        {
            _builder = builder;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _builder.RegisterTypeForNavigation<ViewA>();
        }
    }
}