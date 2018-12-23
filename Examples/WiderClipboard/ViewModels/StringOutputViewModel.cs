using Prism.Ioc;
using System;
using Wider.Core.Services;
using WiderClipboard.Models;
using WiderClipboard.Views;

namespace WiderClipboard.ViewModels
{
    internal class StringOutputViewModel : ContentViewModel
    {
        public String Content { get; set; }

        public StringOutputViewModel(IContainerExtension container) : base (container)
        {
            Model = new EmptyModel();
            View = new StringOutputView();
        }
    }
}
