using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manifest.Config;
using Manifest.LogIn.Classes;
using Xamarin.Auth;
using Xamarin.Forms;
using System.Net.Http;
using Newtonsoft.Json;

namespace Manifest.Views
{
    public class History
    {
        public string message { get; set; }
        public object result { get; set; }
    }

    public partial class ProgressPage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        public ProgressPage()
        {
            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
            
            title.Text = "Progress";
            subTitle.Text = "Your progress";
            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();

            GetUserProgress();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        async void GetUserProgress()
        {
            var client = new HttpClient();
            var startDate = DateTime.Now;
            var endDate = DateTime.Now;
            var userId = (string)Application.Current.Properties["userId"];
            for(int i = 7; i > 0; i--)
            {
                startDate = startDate.AddDays(-1);
            }
            Debug.WriteLine("START: " + startDate);
            Debug.WriteLine("END: " + endDate);

            client.DefaultRequestHeaders.Add("start-date", startDate.ToString("yyyy-MM-dd"));
            client.DefaultRequestHeaders.Add("end-date", endDate.ToString("yyyy-MM-dd"));

            var response = await client.GetAsync("https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev/api/v2/goalRoutineHistory/" + userId);

            if (response.IsSuccessStatusCode)
            {
                
                var data = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<History>(data);
                Debug.WriteLine("HISTORY DATA: " + user.result);
                var dates = JsonConvert.DeserializeObject<IDictionary<string, object>>(user.result.ToString());

            }
            else
            {
                await DisplayAlert("Oops","We weren't able to fulfill your request. Please try again later","OK");
            }

        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Navigation.PopAsync(false);
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SettingsPage(), false);
        }
    }
}
