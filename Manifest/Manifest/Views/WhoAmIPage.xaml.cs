using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manifest.Config;
using Manifest.LogIn.Classes;
using Xamarin.Auth;
using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class WhoAmIPage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        string city;
        string time;
        public WhoAmIPage()
        {
            InitializeComponent();

            setting = false;
            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            scheduleFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            lobbyFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            supportFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            title.Text = "My Story";

            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();
            
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new SettingsPage("WhoAmIPage");
        }

        void Button_Clicked_1(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new TodaysListPage());
        }

        void Button_Clicked_2(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void Button_Clicked_3(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new AboutMePage();
        }
    }
}
