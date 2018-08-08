using Autofac;
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
        private readonly ContainerBuilder _builder;
        private readonly IContainer _container;

        private IEventAggregator EventAggregator => _container.Resolve<IEventAggregator>();

        public TextDocumentModule(ContainerBuilder builder, IContainer container, IRegionManager regionManager)
        {
            _builder = builder;
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            // Note splash page
            EventAggregator.GetEvent<SplashMessageUpdateEvent>()
                .Publish(new SplashMessageUpdateEvent { Message = "Loading TextDocument Module" });

            // Register container types
            _builder.RegisterType<TextViewModel>();
            _builder.RegisterType<TextModel>();
            _builder.RegisterType<TextView>();
            _builder.RegisterType<AllFileHandler>();

            //Register a default file opener
            IContentHandlerRegistry registry = _container.Resolve<IContentHandlerRegistry>();
            registry.Register(_container.Resolve<AllFileHandler>());
        }
    }
}