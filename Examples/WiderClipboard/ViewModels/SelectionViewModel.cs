using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Wider.Core.Services;
using WiderClipboard.Models;
using WiderClipboard.Views;

namespace WiderClipboard.ViewModels
{
    class SelectionViewModel : ToolViewModel
    {
        private String _selectedItem;
        private IEnumerable<String> _items;
        private readonly SelectionModel _model;

        public override PaneLocation PreferredLocation => PaneLocation.Right;

        public ICommand RefreshCommand { get; private set; }
        public IEnumerable<String> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }
        public String SelectedItem {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    if (value != null)
                    {
                        _model.LoadDocument(value);
                    }
                }
            }
        }

        public SelectionViewModel(IContainerExtension container)
        {
            ICommandManager commandManager = container.Resolve<ICommandManager>();

            Model = _model = container.Resolve<SelectionModel>();
            View = new SelectionView();

            Title = "Selection";


            RefreshCommand = commandManager.GetCommand("RefreshCommand");

        }

        public void Refresh() => Items = _model.Refresh();
    }
}
