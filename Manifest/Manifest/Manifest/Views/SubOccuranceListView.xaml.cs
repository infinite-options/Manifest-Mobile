using Manifest.ViewModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
            System.Diagnostics.Debug.WriteLine("IN SUBOCCURANCE LIST VIEW INITIALIZER");
            BindingContext = viewModel = new SubOccuranceListViewModel(new WeakReference<SubOccuranceListView>(this, false), informStatus);
            viewModel.OccuranceId = id;
            viewModel.Navigation = Navigation;
        }

        public async void LoadUI(int index)
        {
            SubOccuranceCollectionView.ItemsSource = viewModel.Tiles;
            SubOccuranceCollectionView.ScrollTo(index);
            //MainTitle.Text = viewModel.Occurance.Title;
            MainHeading.Text = viewModel.Occurance.Title;
            //MainPicture.Source = viewModel.Occurance.PicUrl;
            Timing.Text = "This task takes: " + (viewModel.Occurance.EndDayAndTime - viewModel.Occurance.StartDayAndTime).ToString(@"hh\:mm") + "hours";
        }

        public async void ChangeButtonToDone()
        {
            DoneButton.BackgroundColor = Color.Black;
            DoneButton.TextColor = Color.White;
            DoneButton.Text = "Done";
        }

        private async void GoToPrevScreen(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void GoToRootScreen(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//TodaysList");
        }

        private void TileTapped(object sender, EventArgs e)
        {
            Debug.WriteLine("Tile Tapped");
        }

        private void DoneClicked(object sender, EventArgs e)
        {
            if(DoneButton.Text == "Done")
            {
                viewModel.InformParent();
            }
            GoToPrevScreen(null, null);
        }
    }
}