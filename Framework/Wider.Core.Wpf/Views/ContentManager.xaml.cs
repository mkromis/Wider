using Prism.Events;
using Prism.Ioc;
using Prism.Logging;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Wider.Core.Converters;
using Wider.Core.Events;
using Wider.Core.Services;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Wider.Core.Views
{
    /// <summary>
    /// Interaction logic for ContentManager.xaml
    /// </summary>
    public partial class ContentManager : UserControl
    {
        private readonly ContextMenu _docContextMenu;
        private readonly IContainerExtension _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerService _loggerService;
        private readonly MultiBinding _itemSourceBinding;


        public ContentManager(IContainerExtension container)
        {
            InitializeComponent();

            _container = container;
            _loggerService = container.Resolve<ILoggerService>();

            _eventAggregator = container.Resolve<IEventAggregator>();
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

        public void LoadLayout()
        {
            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(dockManager);
            layoutSerializer.LayoutSerializationCallback += (s, e) =>
            {
                if (e.Model is LayoutAnchorable anchorable)
                {
                    IWorkspace workspace = _container.Resolve<IWorkspace>();

                    ToolViewModel model =
                        workspace.Tools.FirstOrDefault(
                            f => f.ContentId == e.Model.ContentId);
                    if (model != null)
                    {
                        e.Content = model;
                        model.IsVisible = anchorable.IsVisible;
                        model.IsActive = anchorable.IsActive;
                        model.IsSelected = anchorable.IsSelected;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
                if (e.Model is LayoutDocument document)
                {
                    IOpenDocumentService fileService =
                        _container.Resolve<IOpenDocumentService>();
                    ContentViewModel<ContentModel> model =
                        fileService.OpenFromID(e.Model.ContentId);
                    if (model != null)
                    {
                        e.Content = model;
                        model.IsActive = document.IsActive;
                        model.IsSelected = document.IsSelected;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            };
            try
            {
                layoutSerializer.Deserialize(@".\AvalonDock.Layout.config");
            }
            catch (Exception)
            {
            }
        }

        public void SaveLayout()
        {
            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(dockManager);
            layoutSerializer.Serialize(@".\AvalonDock.Layout.config");
        }

        private void ThemeChanged(ITheme theme)
        {
            //HACK: Reset the context menu or else old menu status is retained and does not theme correctly
            dockManager.DocumentContextMenu = null;
            dockManager.DocumentContextMenu = _docContextMenu;

            try
            {
                Window window = Application.Current.MainWindow;
                //_docContextMenu.Style = window.FindResource("DocumentContextMenu") as Style;
                //_docContextMenu.ItemContainerStyle = window.FindResource("DocumentMenuStyle") as Style;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Exception in ThemeChanged()");
            }
        }
    }
}
