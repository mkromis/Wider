using Prism.Ioc;
using Prism.Modularity;
using Wider.Core.Services;
using Wider.Shell.Metro.Views;
using Wider.Shell.Themes;

namespace Wider.Shell.Metro
{
    [Module(ModuleName = "Wider.Shell.Metro")]
    public class MetroModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            IThemeManager themeManager = containerProvider.Resolve<IThemeManager>();
            themeManager.Clear();
#warning Theme issue with Xceed 3.5.0 Use only Blue Light and Dark.
            //themeManager.Add(new DefaultTheme());
            //themeManager.Add(new CleanTheme());
            //themeManager.Add(new VS2010Theme());
            themeManager.Add(new BlueTheme());
            themeManager.Add(new LightTheme());
            themeManager.Add(new DarkTheme());

            themeManager.SetCurrent("Blue");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) => 
            containerRegistry.RegisterSingleton<IShell, ShellView>();

    }
}