using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Wider.Core.Services;
using WiderClipboard.Models;
using WiderClipboard.Views;

namespace WiderClipboard.ViewModels
{
    class StringOutputViewModel : ContentViewModel
    {
        public String Content { get; set; }

        public StringOutputViewModel(IWorkspace workspace, ICommandManager commandManager, ILoggerService logger, IMenuService menuService) : 
            base(workspace, commandManager, logger, menuService)
        {
            Model = new StringOutputModel();
            View = new StringOutputView();
        }
    }
}
