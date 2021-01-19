using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manifest.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.Xaml;

namespace Manifest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    //Creates LoginPage class, that inherits ContentPage
    public partial class LoginPage : ContentPage
    {
        LoginViewModel viewModel;

        //Constructor
        public LoginPage()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("IN LOGIN PAGE INITALIZER");
            //This line binds the properties of the LoginPage class to the LoginViewModel class
            this.BindingContext = viewModel = new LoginViewModel();

            //If the device is iOS, offers apple login
            if(Device.RuntimePlatform == Device.iOS)
            {
                OnAppleDevice();
            }
            System.Diagnostics.Debug.WriteLine("IN LOGIN PAGE INITIALIZER");
        }

        //Private function that displays the apple login button
        private void OnAppleDevice()
        {
            IconGrid.ColumnDefinitions.Add(new ColumnDefinition());
            var button = new ImageButton()
            {
                Source = "apple_login.png",
                BackgroundColor = Color.Transparent,
                Command = viewModel.AppleLoginCommand
            };
            Grid.SetColumn(button, 2);
            IconGrid.Children.Add(button);
        }
    }
}