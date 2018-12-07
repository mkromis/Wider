using System.Windows;
using Prism.Ioc;
using Prism.Regions;
using Wider.Core;
using WiderRibbonDemo.Views;

namespace WiderRibbonDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : WiderApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry) => base.RegisterTypes(containerRegistry);

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Container.Resolve<IRegionManager>().RegisterViewWithRegion("MainRibbon", typeof(MainRibbon));
        }
    }
}
