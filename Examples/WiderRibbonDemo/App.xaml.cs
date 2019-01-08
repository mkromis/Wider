using System.Windows;
using Prism.Ioc;
using Prism.Regions;
using Wider.Core;
using Wider.Core.Services;
using WiderRibbonDemo.Models;
using WiderRibbonDemo.Views;

namespace WiderRibbonDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : WiderApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterTypes(containerRegistry);
            containerRegistry.RegisterSingleton<IWorkspace, Workspace>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Container.Resolve<IRegionManager>().RegisterViewWithRegion("MainRibbon", typeof(MainRibbon));
        }
    }
}
