using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using Manifest.Config;
using Manifest.Models;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Manifest.Views
{
    public class Options
    {
        public string color { get; set; }
        public double height { get; set; }
        public string name { get; set; }


    }

    public partial class WhatIsImportantToMePage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        string city;
        string time;
        string startDate;
        string endDate;
        ObservableCollection<Options> importantItems;
        ObservableCollection<Options> happyItems;
        ObservableCollection<Options> motivationItems;

        DateTime start;

        public WhatIsImportantToMePage()
        {
            InitializeComponent();
            setting = false;
            importantItems = new ObservableCollection<Options>();
            happyItems = new ObservableCollection<Options>();
            motivationItems = new ObservableCollection<Options>();
            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            scheduleFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            lobbyFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            supportFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            //barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
            title.Text = "My Values";
            var date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            start = firstDayOfMonth;
            rangeSliderAge.MinimumValue = (float)Double.Parse(firstDayOfMonth.ToString("dd"));
            rangeSliderAge.LowerValue = (float)Double.Parse(firstDayOfMonth.ToString("dd"));
            rangeSliderAge.MaximumValue = (float)Double.Parse(lastDayOfMonth.ToString("dd"));
            rangeSliderAge.UpperValue = (float)Double.Parse(lastDayOfMonth.ToString("dd"));

            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = GetCurrentTime();
           
            GetHistoryData(firstDayOfMonth.ToString("yyyy-MM-dd"), lastDayOfMonth.ToString("yyyy-MM-dd"));

        }

        void rangeSliderAge_DragCompleted(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine("DRAG IS COMPLETED");

            var startDate = start.AddDays(rangeSliderAge.LowerValue - 1);
            var endDate = start.AddDays(rangeSliderAge.UpperValue - 1);
            Debug.WriteLine("START:" + startDate.ToString("yyyy-MM-dd"));
            Debug.WriteLine("END:" + endDate.ToString("yyyy-MM-dd"));

            GetHistoryData(startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
        }

        public async void GetHistoryData(string startDate = null, string endDate = null)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("start-date", startDate);
                client.DefaultRequestHeaders.Add("end-date", endDate);

                var userId = (string)Application.Current.Properties["userId"];
                var response = await client.GetAsync(AppConstants.BaseUrl + AppConstants.history + userId);
                var data = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("IS CALL TO ENDPOINT SUCCESSFUL: " + response.IsSuccessStatusCode);
                Debug.WriteLine("RETURNED DATA: " + data);

                var parser = JsonConvert.DeserializeObject<UserHistory>(data);
                //Debug.WriteLine("FELLINGS: " + parser.result.feelings);
                Debug.WriteLine("HAPPY: " + parser.result.happy);
                Debug.WriteLine("MOTIVATION: " + parser.result.motivation);
                Debug.WriteLine("IMPORTANT: " + parser.result.important);

                //var feelings = JsonConvert.DeserializeObject<IDictionary<string, int>>(parser.result.feelings.ToString());
                var happy = JsonConvert.DeserializeObject<IDictionary<string, int>>(parser.result.happy.ToString());
                var motivation = JsonConvert.DeserializeObject<IDictionary<string, int>>(parser.result.motivation.ToString());
                var important = JsonConvert.DeserializeObject<IDictionary<string, int>>(parser.result.important.ToString());

                happyItems.Clear();

                var factor = 0.0;
                foreach (string key in happy.Keys)
                {
                    factor = Math.Max(factor, happy[key]);
                }

                var temp = factor;
                factor = 100 / factor;
                foreach (string key in happy.Keys)
                {
                    happyItems.Add(new Options { name = key, height = happy[key] * factor, color = (string)Application.Current.Properties["event"] });
                }

                factor = 0.0;
                foreach (string key in motivation.Keys)
                {
                    factor = Math.Max(factor, motivation[key]);
                }

                temp = factor;
                factor = 100 / factor;
                motivationItems.Clear();
                foreach (string key in motivation.Keys)
                {
                    motivationItems.Add(new Options { name = key, height = motivation[key] * factor, color = (string)Application.Current.Properties["routine"] });
                }
                factor = 0.0;
                foreach (string key in important.Keys)
                {
                    factor = Math.Max(factor, important[key]);
                }

                temp = factor;
                factor = 100 / factor;
                importantItems.Clear();
                foreach (string key in important.Keys)
                {
                    importantItems.Add(new Options { name = key, height = important[key] * factor, color = (string)Application.Current.Properties["goal"] });
                }
                ImportantList.ItemsSource = importantItems;
                HappyList.ItemsSource = happyItems;
                MotivationList.ItemsSource = motivationItems;
            }
            catch (Exception history)
            {
                await DisplayAlert("Oops",history.Message,"OK");
            }
        }

        public string GetCurrentTime()
        {
            var currentTime = DateTime.Now;
            time = currentTime.ToString("MMMM d, yyyy");
            return time;
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new SettingsPage("WhatIsImportantToMePage");
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new TodaysListPage()); ;
        }

        void TapGestureRecognizer_Tapped_1(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void TapGestureRecognizer_Tapped_2(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new AboutMePage();
        }

    }
}
