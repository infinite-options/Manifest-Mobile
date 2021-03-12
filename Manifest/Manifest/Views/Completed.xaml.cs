using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class Completed : ContentPage
    {
        double deviceHeight = DeviceDisplay.MainDisplayInfo.Height;
        double deviceWidth = DeviceDisplay.MainDisplayInfo.Width;

        public Completed(string goalTitle, string photoLink, string subtaskColor)
        {
            InitializeComponent();
            frameColor.BackgroundColor = Color.FromHex("#9DB2CB");
            title.Text = "Goals";
            subTitle.Text = goalTitle;
            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();

            mainFrame.BackgroundColor = Color.FromHex(subtaskColor);
            image.Source = photoLink;

            NavigationPage.SetHasNavigationBar(this, false);
            checkPlatform();
        }

        void checkPlatform()
        {
            mainFrame.HeightRequest = deviceHeight;
            mainFrame.Margin = new Thickness(15, deviceHeight / 30, 15, -deviceHeight / 10);

            congratsLabel.FontSize = deviceWidth / 23;
            congratsLabel.Margin = new Thickness(0, deviceHeight / 45, 0, 0);
            spacer1.HeightRequest = deviceHeight / 30;

            image.WidthRequest = deviceWidth / 3;
            image.HeightRequest = deviceWidth / 3;
            spacer2.HeightRequest = deviceHeight / 45;
            completedLabel.FontSize = deviceWidth / 25;
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Navigation.PopAsync(false);
        }

        void TodaysListPageClicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new TodaysListPage());
        }

        void AboutMeClicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void HelpClicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new AboutMePage();
        }
    }
}
