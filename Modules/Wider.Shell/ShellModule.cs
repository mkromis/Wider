using Wider.Shell.Views;
using Prism.Modularity;
using Prism.Regions;
using System;
using Autofac;
using Prism.Autofac;
using Wider.Core.Services;
using Wider.Shell.Services;
using Wider.Shell.Settings;

namespace Wider.Shell
{
    [Module(ModuleName = "Wider.Shell")]
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
            _builder.RegisterType<SettingsManager>().As<ISettingsManager>().SingleInstance();
            _builder.RegisterType<ToolbarService>().As<IToolbarService>().SingleInstance();
        }
    }
}