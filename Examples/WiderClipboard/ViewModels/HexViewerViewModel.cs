using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wider.Core.Services;
using WiderClipboard.Models;
using WiderClipboard.Views;

namespace WiderClipboard.ViewModels
{
    public class HexViewerViewModel : ContentViewModel
    {
        private MemoryStream _memoryStream;

        public HexViewerViewModel(IContainerExtension container) : base(container)
        {
            Model = new EmptyModel();
            View = new HexViewer();
        }

        public MemoryStream MemoryStreamItem {
            get => _memoryStream;
            set => SetProperty(ref _memoryStream, value);
        }
    }
}
