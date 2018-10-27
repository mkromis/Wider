using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wider.Core.Services;
using Autofac;
using Prism.Events;

namespace WiderRibbonDemo.Models
{
    class Workspace : AbstractWorkspace
    {
        IContainer container;
        IEventAggregator eventAggregator;

        public Workspace(IContainer container, IEventAggregator eventAggregator) : base(container, eventAggregator)
        {
            this.container = container;
            this.eventAggregator = eventAggregator;
        }
    }
}
