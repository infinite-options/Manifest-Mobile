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
    public partial class LoginPage : ContentPage
    {
        LoginViewModel viewModel;
        public LoginPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new LoginViewModel();
            if(Device.RuntimePlatform == Device.iOS)
            {
                OnAppleDevice();
            }
        }

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