using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Wider.Content.Services;
using Wider.Content.TextDocument.ViewModels;
using Wider.Content.TextDocument.Views;
using Wider.Core.Events;
using Wider.Core.Services;

namespace Wider.Content.TextDocument
{
    [Module(ModuleName = "Wider.Content.TextDocument")]
    public class TextDocumentModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register container types
            containerRegistry.Register<TextViewModel>();
            containerRegistry.Register<TextModel>();
            containerRegistry.Register<TextView>();
            containerRegistry.Register<AllFileHandler>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            IEventAggregator eventAggregator = containerProvider.Resolve<IEventAggregator>();
            // Note splash page
            eventAggregator.GetEvent<SplashMessageUpdateEvent>()
                .Publish(new SplashMessageUpdateEvent { Message = "Loading TextDocument Module" });

            //Register a default file opener
            IContentHandlerRegistry registry = containerProvider.Resolve<IContentHandlerRegistry>();
            registry.Register(containerProvider.Resolve<AllFileHandler>());
        }
    }
}