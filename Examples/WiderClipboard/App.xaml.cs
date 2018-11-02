using Prism.Ioc;
using Wider.Core;
using Wider.Core.Services;
using WiderClipboard.Models;

namespace WiderClipboard
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
    }
}
