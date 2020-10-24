using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Manifest.Models;
using Manifest.Services;
using Manifest.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Manifest.Views
{
    public partial class AboutPage : ContentPage
    {

        //public Person person;
        AboutViewModel viewModel;
        private Repository repository = Repository.Instance;
        public ICommand TouchCommand { get; set; }

        public AboutPage()
        {
            InitializeComponent();
            viewModel = new AboutViewModel();
        }

        protected override void OnAppearing()
        {
            User userData = repository.LoadUserData();
            user.BindingContext = userData;
            importantPeople.ItemsSource = userData.ImportantPeople;
        }

        public async void dialPhone(string phoneNumber)
        {
            viewModel.DialPhone(phoneNumber);
        }

        async void LogoutButton_Clicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.Properties.Remove("access_token");
            //Application.Current.Properties.Remove("refreshToken");
            // MADE A CORRECTION FROM refreshToken to refresh_token
            //Application.Current.Properties.Remove("refresh_token");
            //await SecureStorage.SetAsync(SplashScreenViewModel.AppleUserIdKey, "");

            repository.ClearSession();
            Debug.WriteLine("Manifest.Views.AboutPage.LogoutButton_Clicked: Preferences cleared, new count: "+ Application.Current.Properties.Count);
            //Application.Current.Properties.Remove("access_token");
            //Application.Current.Properties.Remove("refresh_token");
            //Application.Current.Properties.Remove("user_id");

            //await Navigation.PushAsync(new LoginPage());
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        void TapGestureRecognizer_Tapped(System.Object sender, EventArgs e)
        {
            Image lblClicked = (Image)sender;
            var item = (TapGestureRecognizer)lblClicked.GestureRecognizers[0];
            var PhoneNumber = item.CommandParameter;
            dialPhone(PhoneNumber.ToString());
        }
    }
}