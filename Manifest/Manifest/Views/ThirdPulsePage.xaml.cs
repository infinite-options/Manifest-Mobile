using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class ThirdPulsePage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        string city;
        string time;
        string option;
        public ThirdPulsePage()
        {
            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;


            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
            title.Text = "What is important to me?";
            subTitle.Text = "Choose 2";
            NavigationPage.SetHasNavigationBar(this, false);
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            if (option != null && option != "")
            { 
                var helper = new FirstPulsePage();
                var response = await helper.SendRequest("important", option);

                if (!response)
                {
                    await DisplayAlert("Oops", "We were not able to fulfill this request. Please check 'changeAboutMeHistory' endpoint.", "OK");
                }
                _ = Navigation.PushAsync(new FourthPulsePage(), false);
            }
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            var selection = (StackLayout)sender;
            option = selection.ClassId;
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new SettingsPage("FirstPulsePage");
            Navigation.PushAsync(new SettingsPage(), false);
        }
    }
}
