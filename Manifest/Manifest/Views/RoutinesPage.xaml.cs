using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class RoutinesPage : ContentPage
    {
        public RoutinesPage()
        {
            InitializeComponent();
        }
        private void goToTodaysList(object sender, EventArgs args)
        {
            Application.Current.MainPage = new TodaysListTest((String)Application.Current.Properties["userID"]);
        }
        void navigateToAboutMe(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new AboutMePage();
        }
    }
}
