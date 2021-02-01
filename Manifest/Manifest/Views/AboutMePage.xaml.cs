using System;
using System.Collections.Generic;
using System.Diagnostics;

using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class AboutMePage : ContentPage
    {
        public AboutMePage()
        {
            InitializeComponent();
            Debug.WriteLine("In aboutME page");
            Debug.WriteLine(Application.Current.Properties["userID"]);
            Debug.WriteLine(Application.Current.Properties["time_stamp"]);
        }

        public void logUserOut(object sender, EventArgs args)
        {
            Application.Current.Properties.Remove("session");
            Application.Current.Properties.Remove("userID");
            Application.Current.Properties.Remove("platform");
            Application.Current.Properties.Remove("time_stamp");
            Application.Current.MainPage = new LogInPage();
        }

        void navigateToAboutMe(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new AboutMePage();
        }

        void navigatetoTodaysList(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new TodaysListTest((String)Application.Current.Properties["userID"]);
        }

    }
}
