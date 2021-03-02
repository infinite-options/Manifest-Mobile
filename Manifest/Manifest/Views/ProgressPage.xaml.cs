using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manifest.Config;
using Manifest.LogIn.Classes;
using Xamarin.Auth;
using Xamarin.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Manifest.Views
{
    public class History
    {
        public string message { get; set; }
        public object result { get; set; }
    }

    public class HeaderInfo
    {
        public string name { get; set; }
        public string date { get; set; }
    }

    public class ProgressRow
    {
        public string name { get; set; }
        public string statusA { get; set; }
        public string statusB { get; set; }
        public string statusC { get; set; }
        public string statusD { get; set; }
        public string statusE { get; set; }
        public string statusF { get; set; }
        public string statusG { get; set; }
    }

    public partial class ProgressPage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        ObservableCollection<HeaderInfo> dates;
        ObservableCollection<ProgressRow> rows;

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
            dates = new ObservableCollection<HeaderInfo>();
            rows = new ObservableCollection<ProgressRow>();
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

            dates.Add(new HeaderInfo() { name = "Goals", date = "" });
            for(int i = 0; i < 7; i++)
            {
                var temp = startDate.AddDays(i);

                dates.Add(new HeaderInfo() { name = temp.ToString("ddd"), date = temp.ToString("MM/dd") });
            }

            DatesList.ItemsSource = dates;

            client.DefaultRequestHeaders.Add("start-date", startDate.ToString("yyyy-MM-dd"));
            client.DefaultRequestHeaders.Add("end-date", endDate.ToString("yyyy-MM-dd"));

            var response = await client.GetAsync("https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev/api/v2/goalRoutineHistory/" + userId);

            if (response.IsSuccessStatusCode)
            {
                
                var data = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<History>(data);
                Debug.WriteLine("HISTORY DATA: " + user.result);
                var dates = JsonConvert.DeserializeObject<IDictionary<string, object>>(user.result.ToString());



                var dateItems = new List<string>();
                var goalsItems = new List<string>();

                
                foreach (string key in dates.Keys)
                {
                    dateItems.Add(key);
                }

                if(dates.Keys.Count != 0)
                {
                    var subSet = JsonConvert.DeserializeObject<IDictionary<string, object>>(dates[dateItems[0]].ToString());
                    foreach (string key in subSet.Keys)
                    {
                        goalsItems.Add(key);
                    }
                }

                var grid = new int[dateItems.Count,goalsItems.Count];
                foreach(string key in dates.Keys)
                {

                }

                foreach(string key in goalsItems)
                {

                    rows.Add(new ProgressRow() { name = key });
                }


                GoalsStatusList.ItemsSource = rows;

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
