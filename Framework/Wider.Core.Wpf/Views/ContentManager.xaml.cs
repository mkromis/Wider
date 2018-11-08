using Prism.Events;
using Prism.Logging;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Wider.Core.Converters;
using Wider.Core.Events;
using Wider.Core.Services;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Controls;

namespace Wider.Core.Views
{
    /// <summary>
    /// Interaction logic for ContentManager.xaml
    /// </summary>
    public partial class ContentManager : UserControl
    {
        private ContextMenu _docContextMenu;
        private IEventAggregator _eventAggregator = null;
        private ILoggerService _loggerService = null;
        private MultiBinding _itemSourceBinding;


        public ContentManager(IEventAggregator eventAggregator, ILoggerService loggerService)
        {
            InitializeComponent();

            _loggerService = loggerService;

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ThemeChangeEvent>().Subscribe(ThemeChanged);

            _docContextMenu = new ContextMenu();
            dockManager.DocumentContextMenu = _docContextMenu;
            _docContextMenu.ContextMenuOpening += _docContextMenu_ContextMenuOpening;
            _docContextMenu.Opened += _docContextMenu_Opened;

            _itemSourceBinding = new MultiBinding
            {
                Converter = new DocumentContextMenuMixingConverter()
            };
            Binding origModel = new Binding(".");
            Binding docMenus = new Binding("Model.Menus");
            _itemSourceBinding.Bindings.Add(origModel);
            _itemSourceBinding.Bindings.Add(docMenus);
            origModel.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            docMenus.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            _itemSourceBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            _docContextMenu.SetBinding(ContextMenu.ItemsSourceProperty, _itemSourceBinding);

        }

        private void DockManager_ActiveContentChanged(Object sender, EventArgs e)
        {
            DockingManager manager = sender as DockingManager;
            ContentViewModel cvm = manager.ActiveContent as ContentViewModel;
            _eventAggregator.GetEvent<ActiveContentChangedEvent>().Publish(cvm);
            if (cvm != null)
            {
                _loggerService.Log("Active document changed to " + cvm.Title, Category.Info, Priority.None);
            }
        }

        private void _docContextMenu_ContextMenuOpening(Object sender, ContextMenuEventArgs e)
        {
            /* When you right click a document - move the focus to that document, so that commands on the context menu
             * which are based on the ActiveDocument work correctly. Example: Save.
             */
            if (_docContextMenu.DataContext is LayoutDocumentItem doc)
            {
                if (doc.Model is ContentViewModel model && model != dockManager.ActiveContent)
                {
                    dockManager.ActiveContent = model;
                }
            }
            e.Handled = false;
        }

        private void _docContextMenu_Opened(Object sender, RoutedEventArgs e) => RefreshMenuBinding();

        private void RefreshMenuBinding()
        {
            MultiBindingExpression b =
                BindingOperations.GetMultiBindingExpression(
                    _docContextMenu,
                    ContextMenu.ItemsSourceProperty);
            b.UpdateTarget();
        }

        private void ThemeChanged(ITheme theme)
        {
            //HACK: Reset the context menu or else old menu status is retained and does not theme correctly
            dockManager.DocumentContextMenu = null;
            dockManager.DocumentContextMenu = _docContextMenu;
            _docContextMenu.Style = FindResource("MetroContextMenu") as Style;
            _docContextMenu.ItemContainerStyle = FindResource("MetroMenuStyle") as Style;
        }
    }
}
