using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class GoalStepsPage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        public GoalStepsPage()
        {
            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;

            frameColor.BackgroundColor = Color.FromHex("#9DB2CB");
            title.Text = "Goals";
            subTitle.Text = "Get crafty";
            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();


            NavigationPage.SetHasNavigationBar(this, false);
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Navigation.PopAsync(false);
        }
    }
}
