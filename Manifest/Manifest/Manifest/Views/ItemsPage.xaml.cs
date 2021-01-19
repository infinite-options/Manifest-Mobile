using Xamarin.Forms;

using Manifest.ViewModels;

namespace Manifest.Views
{
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel _viewModel;

        public ItemsPage()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("IN ITEMS PAGE INITIALIZER");
            BindingContext = _viewModel = new ItemsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}