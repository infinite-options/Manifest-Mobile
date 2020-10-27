using Manifest.Models;
using Manifest.Services;
using Manifest.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Manifest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubOccuranceCarousalView : ContentPage
    {
        SubOccuranceCarousalViewModel _viewModel;
        
        public SubOccuranceCarousalView(string id, InformStatus informStatus)
        {
            InitializeComponent();
            BindingContext = _viewModel = new SubOccuranceCarousalViewModel(new WeakReference<SubOccuranceCarousalView>(this, false), informStatus);
            _viewModel.OccuranceId = id;
        }

        public void LoadUI(int index)
        {
            CarousalSubOccurance.ItemsSource = _viewModel.Tiles;
            CarousalSubOccurance.Position = index;
        }

        private async void NextClicked(object sender, EventArgs e)
        {
            if (NextButton.Text=="Done")
            {
                _viewModel.InformParent();
                GoToPrevScreen(null, null);
                return;
            }
            //CarousalSubOccurance.CurrentItem = await _viewModel.GetUpdatedItem(CarousalSubOccurance.Position);
            await _viewModel.GetUpdatedItem(CarousalSubOccurance.Position);
            ScrollNext();
            
        }

        private async void ScrollNext()
        {
            if (CarousalSubOccurance.Position + 1 < _viewModel.SubOccurances.Count)
                CarousalSubOccurance.Position += 1;
        }

        public async void ChangeButtonToDone()
        {
            NextButton.BackgroundColor = Color.Black;
            NextButton.TextColor = Color.White;
            NextButton.Text = "Done";
        }

        private async void GoToPrevScreen(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void GoToRootScreen(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//TodaysList");
        }
    }
}