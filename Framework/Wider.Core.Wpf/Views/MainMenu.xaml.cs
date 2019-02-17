using Prism.Events;
using System;
using System.Windows;
using System.Windows.Controls;
using Wider.Core.Controls;
using Wider.Core.Events;
using Wider.Core.Services;

namespace Wider.Core.Views
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    { 
        public MainMenu(IMenuService menuService)
        {
            InitializeComponent();
            DataContext = menuService;
        }
    }
}
