using System;
using Prism.Events;
using Prism.Ioc;
using Wider.Core.Events;
using Wider.Core.Services;

namespace WiderRibbonDemo.Models
{
    internal class Workspace : AbstractWorkspace
    {

        public Workspace(IContainerExtension container) : base(container)
        {
            _eventAggregator.GetEvent<ActiveContentChangedEvent>().Subscribe(ActiveDocumentChanged);
        }

        private void ActiveDocumentChanged(ContentViewModel obj)
        {
            switch (obj)
            {
                case Object obj2:
                    // something;
                    break;
                default:
                    break;
            }
            if (obj is Object)
            {
                // i
            }
        }
    }
}
