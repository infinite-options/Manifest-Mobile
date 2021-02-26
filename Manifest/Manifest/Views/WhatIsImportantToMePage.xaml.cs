using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using Manifest.Models;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Manifest.Views
{
    public class Options
    {
        public string color { get; set; }
        public int height { get; set; }
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
            //barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
            title.Text = "My Story";


            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = GetCurrentTime();
            startDate = "2021-02-19";
            endDate = "2021-02-24";
            GetHistoryData(startDate, endDate);

        }

        public async void GetHistoryData(string startDate = null, string endDate = null)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("start-date", startDate);
            client.DefaultRequestHeaders.Add("end-date", endDate);

            var userId = (string)Application.Current.Properties["userId"];
            var response = await client.GetAsync("https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev/api/v2/progress/" + userId);
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
            foreach (string key in happy.Keys)
            {
                happyItems.Add(new Options { name = key, height = happy[key], color = (string)Application.Current.Properties["event"] });
            }
            motivationItems.Clear();
            foreach (string key in motivation.Keys)
            {
                motivationItems.Add(new Options { name = key, height = motivation[key], color = (string)Application.Current.Properties["routine"] });
            }
            importantItems.Clear();
            foreach (string key in important.Keys)
            {
                importantItems.Add(new Options { name = key, height = important[key], color = (string)Application.Current.Properties["goal"]});
            }
            ImportantList.ItemsSource = importantItems;
            HappyList.ItemsSource = happyItems;
            MotivationList.ItemsSource = motivationItems;
        }

        void StartDate_Completed(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine("COMPLETED");
            if(StartDate.Text.Length != 10 && StartDate.Text != null)
            {
                startDate = StartDate.Text.Replace(".","-");
                GetHistoryData(startDate, endDate);
            }
        }

        void EndDate_Completed(System.Object sender, System.EventArgs e)
        {
            if (EndDate.Text.Length != 10 && EndDate.Text != null)
            {
                endDate = EndDate.Text.Replace(".", "-");
                GetHistoryData(startDate, endDate);
            }
        }

        public string GetCurrentTime()
        {
            var currentTime = DateTime.Now;
            time = currentTime.ToString("MMMM d, yyyy");
            return time;
        }

        void StartDate_Unfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            Debug.WriteLine("START - UNFOCUSED");
            if (StartDate.Text.Length == 10 && StartDate.Text != null)
            {
                startDate = StartDate.Text.Replace(".", "-");
                GetHistoryData(startDate, endDate);
            }
        }

        void EndDate_Unfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            Debug.WriteLine("END - UNFOCUSED");
            if (EndDate.Text.Length == 10 && EndDate.Text != null)
            {
                endDate = EndDate.Text.Replace(".", "-");
                GetHistoryData(startDate, endDate);
            }
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new SettingsPage("WhatIsImportantToMePage");
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new TodaysListPage(null,null)); ;
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
