using System;
using System.Collections.Generic;
using Manifest.Models;
using Xamarin.Forms;
using Manifest.Config;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Manifest.Views
{
    public partial class SubListPage : ContentPage
    {
        public List<SubOccurance> subTasks;
        HttpClient client = new HttpClient();

        class SubOccuranceDto
        {
            public string at_unique_id { get; set; }
            public string at_title { get; set; }
            public string goal_routine_id { get; set; }
            public int at_sequence { get; set; }
            public string is_available { get; set; }
            public string is_complete { get; set; }
            public string is_in_progress { get; set; }
            public string is_sublist_available { get; set; }
            public string is_must_do { get; set; }
            public string photo { get; set; }
            public string is_timed { get; set; }
            public string datetime_completed { get; set; }
            public string datetime_started { get; set; }
            public string expected_completion_time { get; set; }
            public string available_start_time { get; set; }
            public string available_end_time { get; set; }
        }

        class SubOccuranceResponse
        {
            public string message { get; set; }
            public List<SubOccuranceDto> result { get; set; }
        }

        public ObservableCollection<SubOccurance> datagrid = new ObservableCollection<SubOccurance>();

        public SubListPage(string occuranceID)
        {
            InitializeComponent();
            subTasks = new List<SubOccurance>();
            subTaskList.ItemsSource = datagrid;
            initializeSubTasks(occuranceID);
        }

        //This function makes a call to the database to get all the sub tasks for the given occurance, and displays it on the device
        private async void initializeSubTasks(string occuranceID)
        {
            string url = RdsConfig.BaseUrl + RdsConfig.actionAndTaskUrl + '/' + occuranceID;
            var response = await client.GetStringAsync(url);
            SubOccuranceResponse subOccuranceResponse = JsonConvert.DeserializeObject<SubOccuranceResponse>(response);
            ToSubOccurances(subOccuranceResponse);
            CreateList();
        }

        //This function converts the response we got from the endpoint to a list of SubOccurance's
        private void ToSubOccurances(SubOccuranceResponse subOccuranceResponse)
        {
            //Clear the occurances, as we are going to get new one now
            subTasks.Clear();
            if (subOccuranceResponse.result == null || subOccuranceResponse.result.Count == 0)
            {
                DisplayAlert("No tasks today", "OK", "Cancel");
            }
            foreach (SubOccuranceDto dto in subOccuranceResponse.result)
            {
                SubOccurance toAdd= new SubOccurance();
                toAdd.Id = dto.at_unique_id;
                toAdd.Title = dto.at_title;
                toAdd.GoalRoutineID = dto.goal_routine_id;
                toAdd.AtSequence = dto.at_sequence;
                toAdd.IsAvailable = ToBool(dto.is_available);
                toAdd.IsComplete = ToBool(dto.is_complete);
                toAdd.IsInProgress = ToBool(dto.is_in_progress);
                toAdd.IsSublistAvailable = ToBool(dto.is_sublist_available);
                toAdd.IsMustDo = ToBool(dto.is_must_do);
                toAdd.PicUrl = dto.photo;
                toAdd.IsTimed = ToBool(dto.is_timed);
                toAdd.DateTimeCompleted = ToDateTime(dto.datetime_completed);
                toAdd.DateTimeStarted = ToDateTime(dto.datetime_started);
                toAdd.ExpectedCompletionTime = ToTimeSpan(dto.expected_completion_time);
                toAdd.AvailableStartTime = ToDateTime(dto.available_start_time);
                toAdd.AvailableEndTime = ToDateTime(dto.available_end_time);
                subTasks.Add(toAdd);
                Debug.WriteLine(toAdd.Id);
            }
        }

        //This function converts a string to a bool
        private bool ToBool(string boolString)
        {
            if (String.IsNullOrEmpty(boolString))
            {
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
            catch (Exception e)
            {
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
            for (int i = 0; i < subTasks.Count; i++)
            {
                this.datagrid.Add(subTasks[i]);
            }
        }

        private void goToTodaysList(object sender, EventArgs args)
        {
            Application.Current.MainPage = new TodaysListTest((Session)Application.Current.Properties["session"]);
        }
    }
}
