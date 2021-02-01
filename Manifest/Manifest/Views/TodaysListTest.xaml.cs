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
using Manifest.RDS;
using Xamarin.Essentials;

namespace Manifest.Views
{
    public partial class TodaysListTest : ContentPage
    {
        HttpClient client = new HttpClient();
        public List<Occurance> todaysOccurances;
        public List<Occurance> todaysOccurancesAfternoon;
        public List<Occurance> todaysOccurancesEvening;

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
        public ObservableCollection<Occurance> datagridAfternoon = new ObservableCollection<Occurance>();
        public ObservableCollection<Occurance> datagridEvening = new ObservableCollection<Occurance>();
        DateTime today;
        string todayDate;
        string morningStart;
        string afternoonStart;
        string eveningStart;
        double deviceHeight = DeviceDisplay.MainDisplayInfo.Height;
        double deviceWidth = DeviceDisplay.MainDisplayInfo.Width;

        public TodaysListTest(String userInfo)
        {
            today = DateTime.Today;
            todayDate = today.ToString("d");
            morningStart = todayDate + ", 12:00:00 AM";
            afternoonStart = todayDate + ", 11:00:00 AM";
            eveningStart = todayDate + ", 6:00:00 PM";
            Debug.WriteLine("dotw: " + today.ToString("dddd"));

            InitializeComponent();
            todaysOccurances = new List<Occurance>();
            todaysOccurancesAfternoon = new List<Occurance>();
            todaysOccurancesEvening = new List<Occurance>();
            RdsConnect.storeGUID(GlobalVars.user_guid, userInfo);
            Debug.WriteLine(userInfo);
            //string userID = userInfo.result[0].user_unique_id;
            string userID = userInfo;
            taskList.ItemsSource = datagrid;
            taskListAfternoon.ItemsSource = datagridAfternoon;
            taskListEvening.ItemsSource = datagridEvening;
            initialiseTodaysOccurances(userID);
            Debug.WriteLine(todaysOccurances);

            Debug.WriteLine("device height: " + deviceHeight.ToString());
            Debug.WriteLine("device width: " + deviceWidth.ToString());
            dotw.Text = today.ToString("dddd");
        }

        private async void initialiseTodaysOccurances(string userID)
        {
            //Need to add userID
            string url = RdsConfig.BaseUrl + RdsConfig.goalsAndRoutinesUrl + "/" + userID;
            var response = await client.GetStringAsync(url);
            Debug.WriteLine("Getting user. User info below:");
            //Debug.WriteLine(response);
            OccuranceResponse occuranceResponse = JsonConvert.DeserializeObject<OccuranceResponse>(response);
            Debug.WriteLine("occuranceResponse: ");
            Debug.WriteLine(occuranceResponse);
            ToOccurances(occuranceResponse);
            CreateList();
        }

        //This function takes the response from the endpoint, and formats it into Occurances
        private void ToOccurances(OccuranceResponse occuranceResponse)
        {
            //Clear the occurances, as we are going to get new one now
            todaysOccurances.Clear();
            todaysOccurancesAfternoon.Clear();
            todaysOccurancesEvening.Clear();
            if (occuranceResponse.result == null || occuranceResponse.result.Count == 0)
            {
                DisplayAlert("No tasks today", "OK", "Cancel");
            }
            foreach (OccuranceDto dto in occuranceResponse.result)
            {
                if (dto.is_displayed_today == "True" && (ToDateTime(dto.start_day_and_time) >= ToDateTime(morningStart) || ToDateTime(dto.end_day_and_time) > ToDateTime(morningStart)))
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
                    toAdd.CompletionTime = "Time to complete: " + dto.expected_completion_time;
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
                    //todaysOccurances.Add(toAdd);
                    Debug.WriteLine("title: " + toAdd.Title);
                    Debug.WriteLine("expected completion time: " + dto.expected_completion_time);
                    Debug.WriteLine("imageTriggers: " + dto.is_sublist_available + " persistent: " + dto.is_persistent);

                    Debug.WriteLine("start day and time: " + dto.start_day_and_time);
                    if (toAdd.StartDayAndTime < ToDateTime(afternoonStart))
                        todaysOccurances.Add(toAdd);
                    else if (toAdd.StartDayAndTime < ToDateTime(eveningStart))
                        todaysOccurancesAfternoon.Add(toAdd);
                    else todaysOccurancesEvening.Add(toAdd);
                }
            }
            return;
        }

        //This function converts a string to a bool
        private bool ToBool(string boolString)
        {
            if (String.IsNullOrEmpty(boolString)) {
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
            catch (Exception e) {
                Debug.WriteLine("Error in ToTimeSpan function in TodaysList class");
            }
            return new TimeSpan();
        }

        //This function convert a string to a DateTime
        private DateTime ToDateTime(string dateString)
        {
            try
            {
                Debug.WriteLine(dateString);
                return DateTime.Parse(dateString);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in ToDateTime function in TodaysList class");
                Debug.WriteLine(dateString);
            }
            return new DateTime();
        }

        private void CreateList()
        {
            for (int i = 0; i < todaysOccurances.Count; i++)
            {
                this.datagrid.Add(todaysOccurances[i]);
            }

            Debug.WriteLine("todaysOccurances.Count: " + todaysOccurances.Count.ToString());
            //stack1.HeightRequest = 15 + 50 + (todaysOccurances.Count * 180);
            //stack1.HeightRequest = 15 + 50 + taskList.Height;
            taskList.HeightRequest = (todaysOccurances.Count * 160);
            if (todaysOccurances.Count == 0)
                stack1.IsVisible = false;

            for (int i = 0; i < todaysOccurancesAfternoon.Count; i++)
            {
                this.datagridAfternoon.Add(todaysOccurancesAfternoon[i]);
            }

            //stack2.HeightRequest = 15 + 50 + (todaysOccurancesAfternoon.Count * 160);
            taskListAfternoon.HeightRequest = (todaysOccurancesAfternoon.Count * 170);
            if (todaysOccurancesAfternoon.Count == 0)
                stack2.IsVisible = false;

            for (int i = 0; i < todaysOccurancesEvening.Count; i++)
            {
                this.datagridEvening.Add(todaysOccurancesEvening[i]);
            }

            Debug.WriteLine("evening occurances: " + todaysOccurancesEvening.Count.ToString());
            //stack3.HeightRequest = 15 + 50 + (todaysOccurancesEvening.Count * 160);
            taskListEvening.HeightRequest = (todaysOccurancesEvening.Count * 170);
            if (todaysOccurancesEvening.Count == 0)
                stack3.IsVisible = false;
        }

        void navigateToAboutMe(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new AboutMePage();
        }

        void navigatetoTodaysList(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new TodaysListTest((String)Application.Current.Properties["userID"]);
        }


        //public class UpdateOccuranceDataType
        //{
        //    public string id { get; set; }
        //    public DateTime datetime_completed { get; set; }
        //    public DateTime datetime_started { get; set; }
        //    public bool is_in_progress { get; set; }
        //    public bool is_complete { get; set; }
        //}

        ////This function returns a string that we can send to an endpoint to update the status of a goal/routine
        //public string updateOccurance(Occurance toUpdate)
        //{
        //    var toSend = new UpdateOccuranceDataType()
        //    {
        //        id = toUpdate.Id,
        //        datetime_completed = toUpdate.DateTimeCompleted,
        //        datetime_started = toUpdate.DateTimeStarted,
        //        is_in_progress = toUpdate.IsInProgress,
        //        is_complete = toUpdate.IsComplete
        //    };
        //    return JsonConvert.SerializeObject(toSend);
        //}

        //This function is called whenever a tile is tapped. It checks for suboccurances, and navigates to a new page if there are any
        async void checkSubOccurance(object sender, EventArgs args)
        {
            Debug.WriteLine("Tapped");
            Debug.WriteLine(sender);
            Debug.WriteLine(args);
            Grid myvar = (Grid)sender;
            Occurance currOccurance = myvar.BindingContext as Occurance;
            Debug.WriteLine(currOccurance.Id);
            var currSession = (Session)Application.Current.Properties["session"];
            string url = RdsConfig.BaseUrl + RdsConfig.updateGoalAndRoutine;
            //If there is a sublist available, navigate to the sublist page
            if (currOccurance.IsSublistAvailable)
            {
                Application.Current.MainPage = new SubListPage(currOccurance);
            }
            else if (currOccurance.IsInProgress == false && currOccurance.IsComplete == false)
            {
                currOccurance.updateIsInProgress(true);
                currOccurance.DateTimeStarted = DateTime.Now;
                Debug.WriteLine("Should be changed to in progress. InProgress = " + currOccurance.IsInProgress);
                //string toSend = updateOccurance(currOccurance);
                UpdateOccurance updateOccur = new UpdateOccurance(){
                    id = currOccurance.Id,
                    datetime_completed = currOccurance.DateTimeCompleted,
                    datetime_started = currOccurance.DateTimeStarted,
                    is_in_progress = currOccurance.IsInProgress,
                    is_complete = currOccurance.IsComplete
                };
                string toSend = updateOccur.updateOccurance();
                var content = new StringContent(toSend);
                var res = await client.PostAsync(url, content);
                if (res.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Wrote to the datebase");
                }
                else
                {
                    Debug.WriteLine("Some error");
                    Debug.WriteLine(toSend);
                    Debug.WriteLine(res.ToString());
                }
            }
            else if (currOccurance.IsInProgress == true && currOccurance.IsComplete == false)
            {
                Debug.WriteLine("Should be changed to in complete");
                currOccurance.updateIsInProgress(false);
                currOccurance.updateIsComplete(true);
                currOccurance.DateTimeCompleted = DateTime.Now;
                UpdateOccurance updateOccur = new UpdateOccurance()
                {
                    id = currOccurance.Id,
                    datetime_completed = currOccurance.DateTimeCompleted,
                    datetime_started = currOccurance.DateTimeStarted,
                    is_in_progress = currOccurance.IsInProgress,
                    is_complete = currOccurance.IsComplete
                };
                string toSend = updateOccur.updateOccurance();
                var content = new StringContent(toSend);
                _ = await client.PostAsync(url, content);

            }
        }
    }
}
