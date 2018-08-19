using Autofac;
using Wider.Core;
using Wider.Core.Services;
using WiderClipboard.Models;

namespace WiderClipboard
{
    internal class Bootstrapper : WiderBootstrapper
    {
        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            builder.RegisterType<Workspace>().As<IWorkspace>().SingleInstance();
            base.ConfigureContainerBuilder(builder);
        }
    }
}
