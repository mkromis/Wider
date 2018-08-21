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
        String _content;

        public StringOutputViewModel(IWorkspace workspace, ICommandManager commandManager, ILoggerService logger, IMenuService menuService) : 
            base(workspace, commandManager, logger, menuService)
        { }

        internal static ContentViewModel Create(IContainer container, String format, String data)
        {
            StringOutputViewModel viewModel = container.Resolve<StringOutputViewModel>();
            StringOutputView view = container.Resolve<StringOutputView>();
            StringOutputModel model = container.Resolve<StringOutputModel>();

            model.Content = data;
            view.DataContext = model;

            viewModel.Model = model;
            viewModel.View = view;
            viewModel.Title = format;

            return viewModel;
        }
    }
}
