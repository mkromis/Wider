using System;
using Wider.Core.Services;
using WiderClipboard.Models;
using WiderClipboard.Views;

namespace WiderClipboard.ViewModels
{
    internal class StringOutputViewModel : ContentViewModel
    {
        public String Content { get; set; }

        public StringOutputViewModel(IWorkspace workspace, ICommandManager commandManager, ILoggerService logger, IMenuService menuService) :
            base(workspace, commandManager, logger, menuService)
        {
            Model = new EmptyModel();
            View = new StringOutputView();
        }
    }
}
