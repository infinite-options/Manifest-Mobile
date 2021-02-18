using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class FirstPulsePage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        string city;
        string time;

        public FirstPulsePage()
        {
            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
            title.Text = "How do I feel?";
            subTitle.Text = "Choose 2";

            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();
            barStackLayoutProperties.BackgroundColor = Color.Salmon;
            barStackLayoutRow.Height = 0;
            buttonStackLayoutRow.Height = lastRowHeight;
            //calendarSwitch.IsToggled = (bool)Application.Current.Properties["showCalendar"];
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

         void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {

        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new SettingsPage("FirstPulsePage");
        }
    }
}
