using System;
using System.ComponentModel;
using System.Windows.Input;
using Manifest.Models;
using Manifest.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Manifest.Views
{
    public partial class AboutPage : ContentPage
    {

        //public Person person;
        AboutViewModel viewModel;

        public ICommand TouchCommand { get; set; }

        public AboutPage()
        {
            InitializeComponent();
            viewModel = new AboutViewModel();
            User userData = Manifest.Services.Repository.Instance.GetUser("100-000028");
            user.BindingContext = userData;
            importantPeople.ItemsSource = userData.ImportantPeople;
            //TouchCommand = new Command(TapGestureRecognizer_Tapped);

        }

        public async void dialPhone(string phoneNumber)
        {
            viewModel.DialPhone(phoneNumber);
            //Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXX");
            //Console.WriteLine("Phone number of TA:" + phoneNumber);
        }

        void LogoutButton_Clicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.Properties.Remove("access_token");
            //Application.Current.Properties.Remove("refreshToken");
            //Application.Current.Properties.Remove("user_id");
            //await Navigation.PushAsync(new LoginPage());
        }



        //void TapGestureRecognizer_Tapped(object phoneNumber)
        //{
        //    dialPhone(phoneNumber.ToString());
        //    Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXX");
        //    Console.WriteLine("Phone number:" + phoneNumber.ToString());
        //}

        void TapGestureRecognizer_Tapped(System.Object sender, EventArgs e)
        {
            //TapGestureRecognizer tap = new TapGestureRecognizer();
            //tap.Tapped += (sender, e) =>
            //{
            //    Image lblClicked = (Image)sender;
            //    string phoneNumber = lblClicked.ToString();
            //    Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXX");
            //    Console.WriteLine("Phone number:" + phoneNumber);
            //};

            //  dialPhone(phoneNumber.ToString());
            //string phNumber = (Image)sender;
            Image lblClicked = (Image)sender;
            var item = (TapGestureRecognizer)lblClicked.GestureRecognizers[0];
            var PhoneNumber = item.CommandParameter;

            dialPhone(PhoneNumber.ToString());
            //Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXX");
            //Console.WriteLine("Phone number:" + PhoneNumber.ToString());
        }
    }
}