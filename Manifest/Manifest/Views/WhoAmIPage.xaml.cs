using System;
using System.Collections.Generic;

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

            frameColor.BackgroundColor = Color.FromHex("#9DB2CB");
            title.Text = "Who am I?";

            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();
            barStackLayoutProperties.BackgroundColor = Color.FromHex("#FF7555");
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }
    }
}
