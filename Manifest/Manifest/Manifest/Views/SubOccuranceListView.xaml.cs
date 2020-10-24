using Manifest.ViewModels;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Manifest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubOccuranceListView : ContentPage
    {
        private readonly SubOccuranceListViewModel viewModel;
        public SubOccuranceListView(string id, InformStatus informStatus)
        {
            InitializeComponent();
            BindingContext = viewModel = new SubOccuranceListViewModel(new WeakReference<SubOccuranceListView>(this, false), informStatus);
            viewModel.OccuranceId = id;
            viewModel.Navigation = Navigation;
        }

        public async void LoadUI(int index)
        {
            SubOccuranceCollectionView.ItemsSource = viewModel.Tiles;
            SubOccuranceCollectionView.ScrollTo(index);
        }

        public async void ChangeButtonToDone()
        {
            DoneButton.BackgroundColor = Color.Black;
            DoneButton.TextColor = Color.White;
            DoneButton.Text = "Done";
        }

        private async void GoToPrevScreen(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void GoToRootScreen(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private void TileTapped(object sender, EventArgs e)
        {
            Debug.WriteLine("Tile Tapped");
        }
    }
}