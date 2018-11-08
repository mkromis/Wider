using Prism.Ioc;
using Prism.Modularity;
using Wider.Core.Services;
using Wider.Shell.Ribbon.Views;

namespace Wider.Shell.Ribbon
{
    public class RibbonModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry) => 
            containerRegistry.Register<IShell, ShellView>();
    }
}