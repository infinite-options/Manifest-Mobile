using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using Manifest.Config;
using Manifest.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Manifest.Views
{
    public partial class TodaysListTest : ContentPage
    {
        HttpClient client = new HttpClient();
        public List<Occurance> todaysOccurances;

        class OccuranceResponse
        {
            public string message { get; set; }
            public List<OccuranceDto> result { get; set; }
        }

            private class OccuranceDto
        {
            public string gr_unique_id { get; set; }
            public string gr_title { get; set; }
            public string user_id { get; set; }
            public string is_available { get; set; }
            public string is_complete { get; set; }
            public string is_in_progress { get; set; }
            public string is_displayed_today { get; set; }
            public string is_persistent { get; set; }
            public string is_sublist_available { get; set; }
            public string is_timed { get; set; }
            public string photo { get; set; }
            public string start_day_and_time { get; set; }
            public string end_day_and_time { get; set; }
            public string repeat { get; set; }
            public string repeat_type { get; set; }
            public string repeat_ends_on { get; set; }
            public int repeat_occurences { get; set; }
            public int repeat_every { get; set; }
            public string repeat_frequency { get; set; }
            public string repeat_week_days { get; set; }
            public string datetime_started { get; set; }
            public string datetime_completed { get; set; }
            public string expected_completion_time { get; set; }
            public object completed { get; set; }
        }
        public ObservableCollection<Occurance> datagrid = new ObservableCollection<Occurance>();

        public TodaysListTest(Session userInfo)
        {
            InitializeComponent();
            todaysOccurances = new List<Occurance>();
            Debug.WriteLine(userInfo);
            string userID = userInfo.result[0].user_unique_id;
            initialiseTodaysOccurances(userID);
            Debug.WriteLine(todaysOccurances);
        }

        private async void initialiseTodaysOccurances(string userID)
        {
            //Need to add userID
            string url = RdsConfig.BaseUrl + RdsConfig.goalsAndRoutinesUrl + "/" + userID;
            var response = await client.GetStringAsync(url);
            Debug.WriteLine("Getting user. User info below:");
            //Debug.WriteLine(response);
            OccuranceResponse occuranceResponse = JsonConvert.DeserializeObject<OccuranceResponse>(response);
            //Debug.WriteLine(occuranceResponse);
            ToOccurances(occuranceResponse);
            taskList.ItemsSource = datagrid;
            CreateList();
        }

        //This function takes the response from the endpoint, and formats it into Occurances
        private void ToOccurances(OccuranceResponse occuranceResponse)
        {
            //Clear the occurances, as we are going to get new one now
            todaysOccurances.Clear();
            if (occuranceResponse.result == null || occuranceResponse.result.Count == 0)
            {
                DisplayAlert("No tasks today", "OK", "Cancel");
            }
            foreach (OccuranceDto dto in occuranceResponse.result)
            {
                if (dto.is_displayed_today == "True")
                {
                    Occurance toAdd = new Occurance();
                    toAdd.Id = dto.gr_unique_id;
                    toAdd.Title = dto.gr_title;
                    toAdd.PicUrl = dto.photo;
                    toAdd.IsPersistent = ToBool(dto.is_persistent);
                    toAdd.IsInProgress = ToBool(dto.is_in_progress);
                    toAdd.IsComplete = ToBool(dto.is_complete);
                    toAdd.IsSublistAvailable = ToBool(dto.is_sublist_available);
                    toAdd.ExpectedCompletionTime = ToTimeSpan(dto.expected_completion_time);
                    toAdd.CompletionTime = dto.expected_completion_time;
                    toAdd.DateTimeCompleted = ToDateTime(dto.datetime_completed);
                    toAdd.DateTimeStarted = ToDateTime(dto.datetime_started);
                    toAdd.StartDayAndTime = ToDateTime(dto.start_day_and_time);
                    toAdd.EndDayAndTime = ToDateTime(dto.end_day_and_time);
                    toAdd.Repeat = ToBool(dto.repeat);
                    toAdd.RepeatEvery = dto.repeat_every;
                    toAdd.RepeatFrequency = dto.repeat_frequency;
                    toAdd.RepeatType = dto.repeat_type;
                    toAdd.RepeatOccurences = dto.repeat_occurences;
                    toAdd.RepeatEndsOn = ToDateTime(dto.repeat_ends_on);
                    //toAdd.RepeatWeekDays = ParseRepeatWeekDays(repeat_week_days);
                    toAdd.UserId = dto.user_id;
                    todaysOccurances.Add(toAdd);
                }
            }
            return;
        }

        //This function converts a string to a bool
        private bool ToBool(string boolString)
        {
            if (String.IsNullOrEmpty(boolString)){
                return false;
            }
            try
            {
                return Boolean.Parse(boolString);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in ToBool function in TodaysList class");
                return false;
            }
        }

        //This function converts a string to a TimeSpan
        private TimeSpan ToTimeSpan(string timeString)
        {
            if (String.IsNullOrEmpty(timeString))
            {
                return new TimeSpan();
            }
            try
            {
                return TimeSpan.Parse(timeString);
            }
            catch (Exception e){
                Debug.WriteLine("Error in ToTimeSpan function in TodaysList class");
            }
            return new TimeSpan();
        }

        //This function convert a string to a DateTime
        private DateTime ToDateTime(string dateString)
        {
            try
            {
                return DateTime.Parse(dateString);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in ToDateTime function in TodaysList class");
            }
            return new DateTime();
        }

        private void CreateList()
        {
            for (int i = 0; i < todaysOccurances.Count; i++)
            {
                this.datagrid.Add(todaysOccurances[i]);
            }
        }

        void Button_Clicked_2(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine("Button 2 pressed");
        }

        void Button_Clicked_1(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine("Button 1 pressed");
        }

        //This function is called whenever a tile is tapped. It checks for suboccurances, and navigates to a new page if there are any
        void checkSubOccurance(object sender, EventArgs args)
        {
            Debug.WriteLine("Tapped");
            Debug.WriteLine(sender);
            Debug.WriteLine(args);
            Grid myvar = (Grid)sender;
            //Button b = (Button)sender;
            Occurance currOccurance = myvar.BindingContext as Occurance;
            //ms.MealQuantity++;
            //var currOccurance = (Occurance)sender;
            Debug.WriteLine(currOccurance.Id);
            //If there is a sublist available, navigate to the sublist page
            if (currOccurance.IsSublistAvailable)
            {
                Application.Current.MainPage = new SubListPage(currOccurance.Id);
            }
        }
    }
}
