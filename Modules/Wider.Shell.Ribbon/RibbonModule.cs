using Prism.Ioc;
using Prism.Modularity;
using Wider.Core.Services;
using Wider.Shell.Ribbon.Themes;
using Wider.Shell.Ribbon.Views;

namespace Wider.Shell.Ribbon
{
    public class RibbonModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            // move to module loader
            IThemeManager themneManager = containerProvider.Resolve<IThemeManager>();
            themneManager.Clear();
            themneManager.Add(new DefaultTheme());
            themneManager.Add(new BlueTheme());
            themneManager.Add(new DarkTheme());
            themneManager.Add(new LightTheme());
            themneManager.SetCurrent("Default");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IShell, ShellView>();
        }
    }
}