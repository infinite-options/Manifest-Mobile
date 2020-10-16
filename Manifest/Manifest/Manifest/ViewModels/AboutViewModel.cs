using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Manifest.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";
            //OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamain-quickstart"));
        }

        public async void DialPhone(string phoneNumber) {
            if (phoneNumber == "") await Application.Current.MainPage.DisplayAlert("Sorry!", $"Hmmm... We don't have a phone number on file", "OK");
            else
            {
                //Phone number of TA
                //Console.WriteLine("ZZZZZZZZZZZZZZZ");
                //Console.WriteLine("Phone number of TA:" + phoneNumber);
                //Console.WriteLine("ZZZZZZZZZZZZZZZ");
                await Launcher.OpenAsync(new Uri("tel:" + phoneNumber));
            }
        }
    }
}