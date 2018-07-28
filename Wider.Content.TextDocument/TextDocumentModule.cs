using Wider.Content.TextDocument.Views;
using Prism.Modularity;
using Prism.Regions;
using System;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Wider.Content.TextDocument.ViewModels;
using Wider.Content.Services;
using Wider.Interfaces.Services;

namespace Wider.Content.TextDocument
{
    public class TextDocumentModule : IModule
    {
        private IRegionManager _regionManager;
        private IUnityContainer _container;

        public TextDocumentModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _container.RegisterType<TextViewModel>();
            _container.RegisterType<TextModel>();
            _container.RegisterType<TextView>();
            _container.RegisterType<AllFileHandler>();

            //Register a default file opener
            IContentHandlerRegistry registry = _container.Resolve<IContentHandlerRegistry>();
            registry.Register(_container.Resolve<AllFileHandler>());
        }
    }
}