using Prism.Events;
using Prism.Ioc;
using Wider.Core.Services;

namespace WiderRibbonDemo.Models
{
    internal class Workspace : AbstractWorkspace
    {
        private readonly IContainerExtension container;
        private readonly IEventAggregator eventAggregator;

        public Workspace(IContainerExtension container, IEventAggregator eventAggregator) : base(container, eventAggregator)
        {
            this.container = container;
            this.eventAggregator = eventAggregator;
        }
    }
}
