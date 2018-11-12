using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wider.Core.Controls;
using Wider.Core.Services;
using Xceed.Wpf.AvalonDock.Converters;

namespace Wider.Core.Views
{
    /// <summary>
    /// Interaction logic for Toolbar.xaml
    /// </summary>
    public partial class Toolbar : UserControl
    {
        // Most of this code was in the ToolbarService, however that ties it to wpf.
        // So trying to use toolbar service in more of a data context way, and generate
        // here instead of service.

        private ToolbarService _toolbarService;
        private static BoolToVisibilityConverter btv = new BoolToVisibilityConverter();

        public Toolbar(IToolbarService toolbarService)
        {
            InitializeComponent();

            _toolbarService = toolbarService as ToolbarService;

            // Update toolbar if service changed.
            _toolbarService.PropertyChanged += (s, e) =>
            {
                RefreshToolbar();
            };

            // Inital refresh
            RefreshToolbar();
        }

        /// <summary>
        /// The toolbar tray which will be used in the application
        /// </summary>
        private void RefreshToolbar()
        {
            ToolBarTray tray = new ToolBarTray();
            IAddChild child = tray;

            foreach (AbstractCommandable node in _toolbarService.Children)
            {
                if (node is AbstractToolbar value)
                {
                    ToolBar tb = new ToolBar();

                    DataTemplateSelector t = FindResource("toolBarItemTemplateSelector") as DataTemplateSelector;
                    tb.SetValue(ItemsControl.ItemTemplateSelectorProperty, t);

                    //Set the necessary bindings
                    Binding bandBinding = new Binding("Band");
                    Binding bandIndexBinding = new Binding("BandIndex");
                    Binding visibilityBinding = new Binding("IsChecked")
                    {
                        Converter = btv
                    };

                    bandBinding.Source = value;
                    bandIndexBinding.Source = value;
                    visibilityBinding.Source = value;

                    bandBinding.Mode = BindingMode.TwoWay;
                    bandIndexBinding.Mode = BindingMode.TwoWay;

                    tb.SetBinding(ToolBar.BandProperty, bandBinding);
                    tb.SetBinding(ToolBar.BandIndexProperty, bandIndexBinding);
                    tb.SetBinding(ToolBar.VisibilityProperty, visibilityBinding);

                    tb.ItemsSource = value.Children;
                    child.AddChild(tb);
                }
            }

            // update context menu if available
            tray.ContextMenu = null;
            if (_toolbarService.ContextMenuItems != null && _toolbarService.ContextMenuItems.Children.Count() > 0)
            {
                // Update Context Menu
                tray.ContextMenu = new ContextMenu
                {
                    ItemsSource = _toolbarService.ContextMenuItems.Children,
                    ItemContainerStyle = FindResource("ToolbarContextMenu") as Style,
                };
            }

            content.Content = tray;
        }
    }
}
