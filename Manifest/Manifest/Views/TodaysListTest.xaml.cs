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
        public List<Occurance> todaysOccurancesMorning;
        public List<Occurance> todaysOccurancesAfternoon;
        public List<Occurance> todaysOccurancesEvening;
        public List<Occurance> todaysEvents;

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
        public ObservableCollection<Occurance> datagridMorning = new ObservableCollection<Occurance>();
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
            todaysEvents = new List<Occurance>();
            todaysOccurancesAfternoon = new List<Occurance>();
            todaysOccurancesEvening = new List<Occurance>();

            RdsConnect.storeGUID(GlobalVars.user_guid, userInfo);
            Debug.WriteLine(userInfo);
            //string userID = userInfo.result[0].user_unique_id;
            string userID = userInfo;
            taskListMorning.ItemsSource = datagridMorning;
            taskListAfternoon.ItemsSource = datagridAfternoon;
            taskListEvening.ItemsSource = datagridEvening;
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
            await GetEvents();
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
                    toAdd.IsEvent = false;
                    todaysOccurances.Add(toAdd);
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
                Debug.WriteLine("Error in ToDateTime function in TodaysList class. " + e.ToString());
                Debug.WriteLine(dateString);
            }
            return new DateTime();
        }

        //In this function, we merge our events and goals/routines
        private void CreateList()
        {
            int i = 0;
            int j = 0;
            //First sort todaysOccurances
            todaysOccurances.Sort(delegate (Occurance a, Occurance b)
            {
                if (a.StartDayAndTime.TimeOfDay < b.StartDayAndTime.TimeOfDay) return -1;
                else return 1;
            });
            List<Occurance> merged = new List<Occurance>();
            //Debug.WriteLine("Num occurances = " + todaysOccurances.Count);
            //Debug.WriteLine("Num Event = " + todaysEvents.Count);
            while (i < todaysOccurances.Count || j < todaysEvents.Count)
            {
                //xDebug.WriteLine(i.ToString() + j.ToString());
                if (i >= todaysOccurances.Count && j < todaysEvents.Count)
                {
                    merged.Add(todaysEvents[j]);
                    Debug.WriteLine(todaysEvents[j].Title + " start time: " + todaysEvents[j].StartDayAndTime);
                    j++;
                    continue;
                }
                else if (i < todaysOccurances.Count && j >= todaysEvents.Count)
                {
                    merged.Add(todaysOccurances[i]);
                    Debug.WriteLine(todaysOccurances[i].Title + " start time: " + todaysOccurances[i].StartDayAndTime);
                    i++;
                    continue;
                }
                else if (todaysOccurances[i].StartDayAndTime.TimeOfDay < todaysEvents[j].StartDayAndTime.TimeOfDay)
                {
                    merged.Add(todaysOccurances[i]);
                    Debug.WriteLine(todaysOccurances[i].Title + " start time: " + todaysOccurances[i].StartDayAndTime);
                    i++;
                }
                else
                {
                    merged.Add(todaysEvents[j]);
                    Debug.WriteLine(todaysEvents[j].Title + " start time: " + todaysEvents[j].StartDayAndTime);
                    j++;
                }
            }
            initialiseDataGrids(merged);
        }

        //This function is going to initialise our three data grids
        private void initialiseDataGrids(List<Occurance> todaysTasks)
        {
            datagridMorning.Clear();
            datagridAfternoon.Clear();
            datagridEvening.Clear();
            int i = 0;
            while (i < todaysTasks.Count && todaysTasks[i].StartDayAndTime < ToDateTime(afternoonStart) )
            {
                datagridMorning.Add(todaysTasks[i]);
                i++;
            }
            while (i < todaysTasks.Count && todaysTasks[i].StartDayAndTime < ToDateTime(eveningStart))
            {
                datagridAfternoon.Add(todaysTasks[i]);
                i++;
            }
            while (i < todaysTasks.Count)
            {
                datagridEvening.Add(todaysTasks[i]);
                i++;
            }
        }

        private async Task GetEvents()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
            string url = Constant.GoogleCalendarUrl + "?orderBy=startTime&singleEvents=true&";
            Session currSession = (Session)Application.Current.Properties["session"];
            string authToken = currSession.result[0].mobile_auth_token;

            int publicYear = dateTimeOffset.Year;
            int publicMonth = dateTimeOffset.Month;
            int publicDay = dateTimeOffset.Day;

            string timeZoneOffset = dateTimeOffset.ToString();
            string[] timeZoneOffsetParsed = timeZoneOffset.Split('-');
            int timeZoneNum = Int32.Parse(timeZoneOffsetParsed[1].Substring(0, 2));
            string monthString;
            string dayString;
            string paddedTimeZoneNum;
            if (timeZoneNum < 10)
            {
                paddedTimeZoneNum = timeZoneNum.ToString().PadLeft(2, '0');

            }
            else
            {
                paddedTimeZoneNum = timeZoneNum.ToString();
            }

            if (publicMonth < 10)
            {
                monthString = publicMonth.ToString().PadLeft(2, '0');

            }
            else
            {
                monthString = publicMonth.ToString();
            }

            if (publicDay < 10)
            {
                dayString = publicDay.ToString().PadLeft(2, '0');

            }
            else
            {
                dayString = publicDay.ToString();
            }

            string timeMaxMin = String.Format("timeMax={0}-{1}-{2}T23%3A59%3A59-{3}%3A00&timeMin={0}-{1}-{2}T00%3A00%3A01-{3}%3A00", publicYear, monthString, dayString, paddedTimeZoneNum);
            string fullURI = url + timeMaxMin;

            //Set up the request
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(fullURI);
            request.Method = HttpMethod.Get;

            //Format Headers of Request with included Token
            string bearerString = string.Format("Bearer {0}", authToken);
            request.Headers.Add("Authorization", bearerString);
            request.Headers.Add("Accept", "application/json");

            //Debug.WriteLine("Manifest.Services.Google.Calendar: Making request to " + fullURI);

            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            //var json = response.Content;
            //Debug.WriteLine("Calendars response:\n" + json);
            //var serializer = new JavaScriptSerializer(); //using System.Web.Script.Serialization;

            EventResponse eventResponse = JsonConvert.DeserializeObject<EventResponse>(json);
            List<Event> events = eventResponse.ToEvents();
            //Debug.WriteLine("Converted to Events");
            EventsToOccurances(events);
        }

        private void EventsToOccurances(List<Event> events)
        {
            todaysEvents.Clear();
            foreach (Event dto in events)
            {
                Occurance toAdd = new Occurance();
                toAdd.Title = dto.Title;
                toAdd.Description = dto.Description;
                toAdd.StartDayAndTime = dto.StartTime.LocalDateTime;
                toAdd.EndDayAndTime = dto.EndTime.LocalDateTime;
                toAdd.Id = dto.Id;
                toAdd.IsEvent = true;
                todaysEvents.Add(toAdd);
            }
        }

        void navigateToAboutMe(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new AboutMePage();
        }

        void navigatetoTodaysList(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new TodaysListTest((String)Application.Current.Properties["userID"]);
        }

        //This function is called whenever a tile is tapped. It checks for suboccurances, and navigates to a new page if there are any
        async void checkSubOccurance(object sender, EventArgs args)
        {
            Debug.WriteLine("Tapped");
            Debug.WriteLine(sender);
            Debug.WriteLine(args);
            Grid myvar = (Grid)sender;
            Occurance currOccurance = myvar.BindingContext as Occurance;
            if (currOccurance.IsEvent)
            {
                return;
            }
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
