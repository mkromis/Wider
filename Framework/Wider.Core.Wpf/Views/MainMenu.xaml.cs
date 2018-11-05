using System.Windows.Controls;
using Wider.Core.Controls;
using Wider.Core.Services;

namespace Wider.Core.Views
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {

        /// <summary>
        /// The menu service
        /// </summary>
        protected MenuItemViewModel _menus;


        public MainMenu(IMenuService menuService)
        {
            InitializeComponent();
            DataContext = menuService;
        }
    }
}
