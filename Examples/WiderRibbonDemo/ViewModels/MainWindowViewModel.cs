using Prism.Mvvm;

namespace WiderRibbonDemo.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private System.String _title = "Prism Application";
        public System.String Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public MainWindowViewModel()
        {

        }
    }
}
