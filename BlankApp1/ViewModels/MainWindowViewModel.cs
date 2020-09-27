using BlankApp1.Views;
using Prism.Mvvm;
using Prism.Regions;

namespace BlankApp1.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IRegionManager _regionManger;
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManger = regionManager;

            _regionManger.RegisterViewWithRegion("ContentRegion", typeof(SampleView));
        }
    }
}
