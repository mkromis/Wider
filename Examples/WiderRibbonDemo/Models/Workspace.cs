using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Wider.Content.TextDocument;
using Wider.Content.TextDocument.ViewModels;
using Wider.Content.TextDocument.Views;
using Wider.Core.Controls;
using Wider.Core.Services;

namespace WiderRibbonDemo.Models
{
    // Workspace will control anything not controlled by the controler's view model
    internal class Workspace : AbstractWorkspace
    {
        public Workspace(IContainerExtension container, IEventAggregator eventAggregator) : base(container, eventAggregator)
        {
            Title = "Wider Ribbon Control Demo";

            LoadCommands();
            LoadMenus();
        }

        public void LoadCommands() { }

        public void LoadMenus()
        {
            IMenuService menuService = _container.Resolve<IMenuService>();
            menuService.Add(new MenuItemViewModel("_File", 100));
            menuService.Get("_File").Add(new MenuItemViewModel("_Save", 110));
            //menuService.Get("_File").Get("_Save").Add(new MenuItemViewModel("Nope", 120));
        }
        
        public ICommand OpenTextSample => new DelegateCommand(() =>
        {
            // Copied from clipboard sample
            TextViewModel vm = _container.Resolve<TextViewModel>();
            TextModel model = _container.Resolve<TextModel>();
            TextView view = _container.Resolve<TextView>();

            // set viewmodel to model?, copied from Wider.Content.TextDocument
            view.DataContext = model;

            //Clear the undo stack
            model.Document.UndoStack.ClearAll();
            model.Document.Text = "This is a simple text document to show how to load from an external module";
            model.IsDirty = false;

            //Set the model and view
            vm.Model = model;
            vm.View = view;
            vm.Title = "Sample Text Document";

            Documents.Add(vm);
            ActiveDocument = vm;
        });
    }
}
