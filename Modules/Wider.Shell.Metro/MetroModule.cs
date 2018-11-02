using Prism.Ioc;
using Prism.Modularity;
using Wider.Core.Services;
using Wider.Shell.Metro.Views;

namespace Wider.Shell.Metro
{
    [Module(ModuleName = "Wider.Shell.Metro")]
    public class MetroModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry) => containerRegistry.RegisterSingleton<IShell, ShellView>();

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}