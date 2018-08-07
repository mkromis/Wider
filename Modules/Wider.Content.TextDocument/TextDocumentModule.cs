using DryIoc;
using Prism.Events;
using Prism.Modularity;
using Prism.Regions;
using Wider.Content.Services;
using Wider.Content.TextDocument.ViewModels;
using Wider.Content.TextDocument.Views;
using Wider.Interfaces.Events;
using Wider.Interfaces.Services;

namespace Wider.Content.TextDocument
{
    public class TextDocumentModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IContainer _container;

        private IEventAggregator EventAggregator => _container.Resolve<IEventAggregator>();

        public TextDocumentModule(IContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            // Note splash page
            EventAggregator.GetEvent<SplashMessageUpdateEvent>()
                .Publish(new SplashMessageUpdateEvent { Message = "Loading TextDocument Module" });

            // Register container types
            _container.Register<TextViewModel>();
            _container.Register<TextModel>();
            _container.Register<TextView>();
            _container.Register<AllFileHandler>();

            //Register a default file opener
            IContentHandlerRegistry registry = _container.Resolve<IContentHandlerRegistry>();
            registry.Register(_container.Resolve<AllFileHandler>());
        }
    }
}