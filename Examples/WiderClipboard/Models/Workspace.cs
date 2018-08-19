using Autofac;
using Prism.Events;
using System;
using Wider.Core.Services;

namespace WiderClipboard.Models
{
    internal class Workspace : AbstractWorkspace
    {
        public Workspace(IContainer container, IEventAggregator eventAggregator) : base(container, eventAggregator)
        {
            Title = "Wider Clipboard Viewer";
        }

    }
}
