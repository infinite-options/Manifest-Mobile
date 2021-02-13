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
using System.Text;
using System.Collections;

namespace Manifest.Views
{
    public partial class TodaysListPage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;

        HttpClient client = new HttpClient();
        HttpClient client2 = new HttpClient();
        public List<Occurance> todaysOccurances;
        public List<Occurance> todaysOccurancesMorning;
        public List<Occurance> todaysOccurancesAfternoon;
        public List<Occurance> todaysOccurancesEvening;
        public List<Occurance> todaysEvents;
        public List<Event> eventsToday;

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
        List<Occurance> commonOccur;

        public TodaysListPage()
        {
            today = DateTime.Today;
            todayDate = today.ToString("d");
            morningStart = todayDate + ", 12:00:00 AM";
            afternoonStart = todayDate + ", 11:00:00 AM";
            eveningStart = todayDate + ", 6:00:00 PM";
            Debug.WriteLine("dotw: " + today.ToString("dddd"));

            InitializeComponent();

            setting = false;
            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;

            frameColor.BackgroundColor = Color.FromHex("#9DB2CB");
            title.Text = today.ToString("dddd");

            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();
            //barStackLayoutProperties.BackgroundColor = Color.FromHex("#FF7555");

            NavigationPage.SetHasNavigationBar(this, false);
            string userInfo = (string)Application.Current.Properties["userId"];
            if (Application.Current.Properties["userId"] == null || (String)Application.Current.Properties["userId"] == "" || Application.Current.Properties["accessToken"] == null || (String)Application.Current.Properties["accessToken"] == "")
            {
                Application.Current.MainPage = new LogInPage();
            }
            try
            {
                //today = DateTime.Today;
                //todayDate = today.ToString("d");
                //morningStart = todayDate + ", 12:00:00 AM";
                //afternoonStart = todayDate + ", 11:00:00 AM";
                //eveningStart = todayDate + ", 6:00:00 PM";
                //Debug.WriteLine("dotw: " + today.ToString("dddd"));
                todaysOccurances = new List<Occurance>();
                todaysEvents = new List<Occurance>();
                todaysOccurancesAfternoon = new List<Occurance>();
                todaysOccurancesEvening = new List<Occurance>();
                commonOccur = new List<Occurance>();

                RdsConnect.storeGUID(GlobalVars.user_guid, userInfo);
                Debug.WriteLine(userInfo);
                //string userID = userInfo.result[0].user_unique_id;
                string userID = userInfo;
                taskListMorning.ItemsSource = datagridMorning;
                taskListAfternoon.ItemsSource = datagridAfternoon;
                taskListEvening.ItemsSource = datagridEvening;
                initialiseTodaysOccurances(userID);
                Debug.WriteLine(todaysOccurances);

                Debug.WriteLine("device height: " + deviceHeight.ToString());
                Debug.WriteLine("device width: " + deviceWidth.ToString());
                dotw.Text = today.ToString("dddd");
            }
            catch (Exception e)
            {
                DisplayAlert("Alert", "Error in TodaysListTest initializer. Error: " + e.ToString(), "OK");
            }


        }

        //void Button_Clicked(System.Object sender, System.EventArgs e)
        //{
        //    Navigation.PushAsync(new GoalsPage(),false);
        //}

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void TodaysListPageClicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new TodaysListPage();
        }


        private async void initialiseTodaysOccurances(string userID)
        {
            try
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
            catch (Exception e)
            {
                await DisplayAlert("Alert", "Error in TodaysListTest initialiseTodaysOccurances. Error: " + e.ToString(), "OK");
            }
        }

        bool firstRunPassed = false;
        //This function takes the response from the endpoint, and formats it into Occurances
        private void ToOccurances(OccuranceResponse occuranceResponse)
        {
            try
            {
                //Clear the occurances, as we are going to get new one now
                todaysOccurances.Clear();
                if (occuranceResponse.result == null || occuranceResponse.result.Count == 0)
                {
                    DisplayAlert("No tasks today", "OK", "Cancel");
                }

                //sort by end time
                List<OccuranceDto> todayOccurs = new List<OccuranceDto>();
                foreach (OccuranceDto dto in occuranceResponse.result)
                {
                    if (dto.is_displayed_today == "True")
                    {
                        if (todayOccurs.Count == 0)
                            todayOccurs.Add(dto);
                        else
                        {
                            for (int i = 0; i < todayOccurs.Count; i++)
                            {
                                if (i == todayOccurs.Count - 1 && DateTime.Parse(dto.end_day_and_time).TimeOfDay > DateTime.Parse(todayOccurs[i].end_day_and_time).TimeOfDay)
                                {
                                    todayOccurs.Add(dto);
                                    break;
                                }
                                //else if (DateTime.Parse(dto.end_day_and_time).TimeOfDay > DateTime.Parse(todayOccurs[i].end_day_and_time).TimeOfDay)
                                //{
                                //    todayOccurs.Insert(i+1, dto);
                                //    break;
                                //}
                                else if (DateTime.Parse(dto.end_day_and_time).TimeOfDay <= DateTime.Parse(todayOccurs[i].end_day_and_time).TimeOfDay)
                                {
                                    todayOccurs.Insert(i, dto);
                                    break;
                                }
                            }
                        }
                        
                    }
                }

                //sort by start time
                for (int j = 0; j < todayOccurs.Count - 1; j++)
                {
                    for (int i = j + 1; i < todayOccurs.Count; i++)
                    {
                        if (DateTime.Parse(todayOccurs[i].start_day_and_time).TimeOfDay < DateTime.Parse(todayOccurs[j].start_day_and_time).TimeOfDay)
                        {
                            OccuranceDto temp = todayOccurs[i];
                            todayOccurs[i] = todayOccurs[j];
                            todayOccurs[j] = temp;
                        }
                    }
                }

                Debug.WriteLine("passed sorting");
                foreach (OccuranceDto dto in todayOccurs)
                {
                    Debug.WriteLine("starting: " + DateTime.Parse(dto.start_day_and_time).TimeOfDay + " ending: " + DateTime.Parse(dto.end_day_and_time).TimeOfDay);
                }

                foreach (OccuranceDto dto in todayOccurs)
                {
                    if (dto.is_displayed_today == "True")
                    {
                        Debug.WriteLine("occurance tracked");
                        Occurance toAdd = new Occurance();
                        toAdd.Id = dto.gr_unique_id;
                        toAdd.Title = dto.gr_title;
                        toAdd.PicUrl = dto.photo;
                        toAdd.IsPersistent = ToBool(dto.is_persistent);
                        toAdd.IsInProgress = ToBool(dto.is_in_progress);
                        toAdd.IsComplete = ToBool(dto.is_complete);
                        toAdd.IsSublistAvailable = ToBool(dto.is_sublist_available);
                        toAdd.ExpectedCompletionTime = ToTimeSpan(dto.expected_completion_time);
                        toAdd.CompletionTime = "This takes " + dto.expected_completion_time;
                        toAdd.DateTimeCompleted = ToDateTime(dto.datetime_completed);
                        toAdd.DateTimeStarted = ToDateTime(dto.datetime_started);
                        toAdd.StartDayAndTime = ToDateTime(dto.start_day_and_time);
                        toAdd.EndDayAndTime = ToDateTime(dto.end_day_and_time);
                        toAdd.TimeInterval = DateTime.Parse(dto.start_day_and_time).ToString("t") + "-" + DateTime.Parse(dto.end_day_and_time).ToString("t");

                        if (toAdd.IsPersistent == true)
                            toAdd.StatusColor = Color.FromHex("#FF6B4A");
                        else toAdd.StatusColor = Color.FromHex("#FFBD27");

                        //highlighting occurances happening now
                        //if (DateTime.Now.TimeOfDay >= toAdd.StartDayAndTime.TimeOfDay && DateTime.Now.TimeOfDay <= toAdd.EndDayAndTime.TimeOfDay)
                        //    toAdd.StatusColor = Color.FromHex("#FFBD27");
                        //else toAdd.StatusColor = Color.FromHex("#9DB2CB");

                        Debug.WriteLine("start time: " + dto.start_day_and_time);

                        toAdd.Repeat = ToBool(dto.repeat);
                        toAdd.RepeatEvery = dto.repeat_every;
                        toAdd.RepeatFrequency = dto.repeat_frequency;
                        toAdd.RepeatType = dto.repeat_type;
                        toAdd.RepeatOccurences = dto.repeat_occurences;
                        toAdd.RepeatEndsOn = ToDateTime(dto.repeat_ends_on);
                        //toAdd.RepeatWeekDays = ParseRepeatWeekDays(repeat_week_days);
                        toAdd.UserId = dto.user_id;
                        toAdd.IsEvent = false;

                        if ("6:00 AM" == "6:00 AM")
                        {
                            Debug.WriteLine("first if passed");
                        }
                        if (String.Compare("6:00 PM", "6:00 AM") >= 0)
                        {
                            Debug.WriteLine("second if passed");
                        }
                        if (String.Compare("11:00 AM", "6:00 AM") >= 0)
                        {
                            Debug.WriteLine("third if passed");
                        }
                        if (String.Compare("11:00 AM", "2:00 PM") >= 0)
                        {
                            Debug.WriteLine("fourth if passed");
                        }
                        if (ToDateTime("11:00 AM").TimeOfDay <= ToDateTime("2:00 PM").TimeOfDay)
                        {
                            Debug.WriteLine("testing if passed");
                        }

                        if (firstRunPassed == false && toAdd.IsPersistent == false)
                        {
                            commonOccur.Add(toAdd);
                            firstRunPassed = true;
                        }
                        else if (toAdd.IsPersistent == false && commonOccur.Count != 0 && ToDateTime(commonOccur[0].StartDayAndTime.ToString("t")).TimeOfDay <= ToDateTime(toAdd.StartDayAndTime.ToString("t")).TimeOfDay
                            && ToDateTime(commonOccur[0].EndDayAndTime.ToString("t")).TimeOfDay >= ToDateTime(toAdd.EndDayAndTime.ToString("t")).TimeOfDay)
                        {
                            commonOccur.Add(toAdd);
                        }
                        else if (toAdd.IsPersistent == false && commonOccur.Count > 1 && (ToDateTime(commonOccur[0].StartDayAndTime.ToString("t")).TimeOfDay < ToDateTime(toAdd.StartDayAndTime.ToString("t")).TimeOfDay
                            || ToDateTime(commonOccur[0].EndDayAndTime.ToString("t")).TimeOfDay < ToDateTime(toAdd.EndDayAndTime.ToString("t")).TimeOfDay))
                        {
                            Debug.WriteLine("starttimes: " + commonOccur[0].StartDayAndTime.TimeOfDay + " " + toAdd.StartDayAndTime.TimeOfDay);
                            Debug.WriteLine("endtimes: " + commonOccur[0].EndDayAndTime.TimeOfDay + " " + toAdd.EndDayAndTime.TimeOfDay);
                            Occurance toAddPursue = new Occurance();
                            toAddPursue.Title = "Pursue A Goal";
                            toAddPursue.StartDayAndTime = commonOccur[0].StartDayAndTime;
                            toAddPursue.EndDayAndTime = commonOccur[0].EndDayAndTime;
                            toAddPursue.TimeInterval = commonOccur[0].TimeInterval;
                            toAddPursue.IsEvent = false;

                            toAddPursue.StatusColor = Color.FromHex("#FFBD27");
                            //if (DateTime.Now.TimeOfDay >= toAddPursue.StartDayAndTime.TimeOfDay && DateTime.Now.TimeOfDay <= toAddPursue.EndDayAndTime.TimeOfDay)
                            //    toAddPursue.StatusColor = Color.FromHex("#FFBD27");
                            //else toAddPursue.StatusColor = Color.FromHex("#9DB2CB");

                            List<Occurance> holder = new List<Occurance>(commonOccur);
                            Debug.WriteLine("holder count before: " + holder.Count);
                            toAddPursue.commonTimeOccurs = holder;
                            todaysOccurances.Add(toAddPursue);
                            commonOccur.Clear();
                            commonOccur.Add(toAdd);
                            Debug.WriteLine("holder count after: " + holder.Count);
                        }
                        else if (toAdd.IsPersistent == false && commonOccur.Count == 1 && (ToDateTime(commonOccur[0].StartDayAndTime.ToString("t")).TimeOfDay < ToDateTime(toAdd.StartDayAndTime.ToString("t")).TimeOfDay
                            || ToDateTime(commonOccur[0].EndDayAndTime.ToString("t")).TimeOfDay  < ToDateTime(toAdd.EndDayAndTime.ToString("t")).TimeOfDay))
                        {
                            todaysOccurances.Add(commonOccur[0]);
                            commonOccur.Clear();
                            commonOccur.Add(toAdd);
                        }
                        else
                        {
                            todaysOccurances.Add(toAdd);
                        }

                        //todaysOccurances.Add(toAdd);
                        //need a isPursueAGoal variable to know if the thing being clicked needs to be navigated to
                        //send in list of goals for the pursue a goal into goals page instead of calling the endpoint again
                        //
                        //store the incoming occurance in listOccur, if it doesn't have the same time as the last occurance, clear listOccur,
                        //if the incoming occurance is at the same hour and minutes as the one in listOccur, add the time to listTime (that will be passed to goals.xaml.cs)
                    }
                }

                if (commonOccur.Count > 1 && commonOccur[0].IsPersistent == false)
                {
                    Occurance toAddPursue = new Occurance();
                    toAddPursue.Title = "Pursue A Goal";
                    toAddPursue.StartDayAndTime = commonOccur[0].StartDayAndTime;
                    toAddPursue.EndDayAndTime = commonOccur[0].EndDayAndTime;
                    toAddPursue.TimeInterval = commonOccur[0].TimeInterval;
                    toAddPursue.IsEvent = false;

                    toAddPursue.StatusColor = Color.FromHex("#FFBD27");
                    //if (DateTime.Now.TimeOfDay >= toAddPursue.StartDayAndTime.TimeOfDay && DateTime.Now.TimeOfDay <= toAddPursue.EndDayAndTime.TimeOfDay)
                    //    toAddPursue.StatusColor = Color.FromHex("#FFBD27");
                    //else toAddPursue.StatusColor = Color.FromHex("#9DB2CB");

                    List<Occurance> holder = new List<Occurance>(commonOccur);
                    Debug.WriteLine("holder count before: " + holder.Count);
                    toAddPursue.commonTimeOccurs = holder;
                    todaysOccurances.Add(toAddPursue);
                }
                else if (commonOccur.Count == 1 && commonOccur[0].IsPersistent == false)
                {
                    todaysOccurances.Add(commonOccur[0]);
                }

                return;
            }
            catch (Exception e)
            {
                DisplayAlert("Alert", "Error in TodaysListTest ToOccurances(). Error: " + e.ToString(), "OK");
            }
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
                Debug.WriteLine("inToDateTime: " + dateString);
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
            try
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
            catch (Exception e)
            {
                DisplayAlert("Alert", "Error in TodaysListTest CreateList(). Error: " + e.ToString(), "OK");
            }
        }

        //public async Times timeSettings()
        //{

        //}

        //This function is going to initialise our three data grids
        private async void initialiseDataGrids(List<Occurance> todaysTasks)
        {

            try
            {
                string url = RdsConfig.BaseUrl + RdsConfig.timeSettingsUrl + "/" + Application.Current.Properties["userId"];
                var response2 = await client2.GetStringAsync(url);
                Debug.WriteLine("Getting time settings:");
                Debug.WriteLine(response2);
                int subtract = response2.Length - 4;
                response2 = response2.Substring(1, subtract);
                Debug.WriteLine(response2);

                //int subtract2 = response2.Length - 3;
                //response2 = response2.Substring(0, subtract);
                //Debug.WriteLine(response2);
                Times timeResponse = JsonConvert.DeserializeObject<Times>(response2);

                //Debug.WriteLine("daytime: " + timeResponse.morning_time);

                datagridMorning.Clear();
                datagridAfternoon.Clear();
                datagridEvening.Clear();
                int i = 0;
                int morningTaskCount = 0;
                int afternoonTaskCount = 0;
                int eveningTaskCount = 0;
                string todaysTaskTime = (todaysTasks[i].StartDayAndTime.ToString("HH") + ":" + todaysTasks[i].StartDayAndTime.ToString("mm"));
                while (i < todaysTasks.Count && String.Compare(todaysTaskTime,timeResponse.afternoon_time) < 0)
                {
                    Debug.WriteLine("taskStartTime: " + todaysTasks[i].StartDayAndTime.TimeOfDay.ToString() + " afternoonStart: " + ToDateTime(afternoonStart).TimeOfDay.ToString());
                    datagridMorning.Add(todaysTasks[i]);
                    morningTaskCount++;
                    i++;
                    todaysTaskTime = (todaysTasks[i].StartDayAndTime.ToString("HH") + ":" + todaysTasks[i].StartDayAndTime.ToString("mm"));
                }
                while (i < todaysTasks.Count && String.Compare(todaysTaskTime, timeResponse.evening_time) < 0)
                {
                    Debug.WriteLine("afternoonTaskStartTime: " + todaysTasks[i].StartDayAndTime.TimeOfDay.ToString());
                    datagridAfternoon.Add(todaysTasks[i]);
                    afternoonTaskCount++;
                    i++;
                    todaysTaskTime = (todaysTasks[i].StartDayAndTime.ToString("HH") + ":" + todaysTasks[i].StartDayAndTime.ToString("mm"));
                }
                while (i < todaysTasks.Count)
                {
                    Debug.WriteLine("nightTaskStartTime: " + todaysTasks[i].StartDayAndTime.TimeOfDay.ToString());
                    Debug.WriteLine("nightTaskStartHrMin: " + todaysTasks[i].StartDayAndTime.ToString("HH") + ":" + todaysTasks[i].StartDayAndTime.ToString("mm"));
                    datagridEvening.Add(todaysTasks[i]);
                    eveningTaskCount++;
                    i++;
                    //todaysTaskTime = (todaysTasks[i].StartDayAndTime.ToString("HH") + ":" + todaysTasks[i].StartDayAndTime.ToString("mm"));
                }

                Debug.WriteLine("morningCount: " + morningTaskCount.ToString());
                Debug.WriteLine("afternoonCount: " + afternoonTaskCount.ToString());
                Debug.WriteLine("eveningCount: " + eveningTaskCount.ToString());

                taskListMorning.HeightRequest = (morningTaskCount * 115) + 60;
                if (morningTaskCount == 0)
                    taskListEvening.HeightRequest = 60;
                //stack1.IsVisible = false;
                taskListAfternoon.HeightRequest = (afternoonTaskCount * 115) + 60;
                if (afternoonTaskCount == 0)
                    taskListEvening.HeightRequest = 60;
                //stack2.IsVisible = false;
                taskListEvening.HeightRequest = (eveningTaskCount * 115) + 60;
                if (eveningTaskCount == 0)
                    taskListEvening.HeightRequest = 60;
                //stack3.IsVisible = false;
            }
            catch (Exception e)
            {
                DisplayAlert("Alert", "Error in TodaysListTest initialiseDataGrids(). Error: " + e.ToString(), "OK");
            }
        }

        private async Task GetEvents()
        {
            try
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
                string url = Constant.GoogleCalendarUrl + "?orderBy=startTime&singleEvents=true&";
                var authToken = (String)Application.Current.Properties["accessToken"];
                Debug.WriteLine("AuthToken: " + authToken);
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
                //List<Event> events = eventResponse.ToEvents();
                eventsToday = eventResponse.ToEvents();
                //Debug.WriteLine("Converted to Events");
                EventsToOccurances(eventsToday);
            }
            catch (Exception e)
            {
                await DisplayAlert("Alert", "Error in TodaysListTest GetEvents(). Error: " + e.ToString(), "OK");
            }
        }

        private void EventsToOccurances(List<Event> events)
        {
            try
            {
                todaysEvents.Clear();
                foreach (Event dto in events)
                {
                    Occurance toAdd = new Occurance();
                    toAdd.Title = dto.Title;
                    toAdd.Description = dto.Description;
                    toAdd.StartDayAndTime = dto.StartTime.LocalDateTime;
                    toAdd.EndDayAndTime = dto.EndTime.LocalDateTime;
                    toAdd.TimeInterval = dto.StartTime.LocalDateTime.ToString("t") + "-" + dto.EndTime.LocalDateTime.ToString("t");

                    toAdd.StatusColor = Color.FromHex("#67ABFC");
                    //highlighting events happening now
                    //if (DateTime.Now.TimeOfDay >= toAdd.StartDayAndTime.TimeOfDay && DateTime.Now.TimeOfDay <= toAdd.EndDayAndTime.TimeOfDay)
                    //    toAdd.StatusColor = Color.FromHex("#FFBD27");
                    //else toAdd.StatusColor = Color.FromHex("#9DB2CB");

                    toAdd.Id = dto.Id;
                    toAdd.IsEvent = true;
                    toAdd.PicUrl = "calendarFive.png"; //Image must be a png
                    todaysEvents.Add(toAdd);
                }
            }
            catch (Exception e)
            {
                DisplayAlert("Alert", "Error in TodaysListTest EventsToOccurances. Error: " + e.ToString(), "OK");
            }
        }

        // void navigateToAboutMe(System.Object sender, System.EventArgs e)
        // {
        //     Application.Current.MainPage = new AboutMePage();
        // }

        // void navigatetoTodaysList(System.Object sender, System.EventArgs e)
        // {
        //     Debug.WriteLine(Application.Current.Properties["userID"]);
        //     Application.Current.MainPage = new TodaysListTest((String)Application.Current.Properties["userID"]);
        // }

        // void navigatetoRoutines(System.Object sender, System.EventArgs e)
        // {
        //     Debug.WriteLine(Application.Current.Properties["userID"]);
        //     Application.Current.MainPage = new RoutinesPage((String)Application.Current.Properties["userID"]);
        // }

        void navigateToGoals(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new Goals();
        }

        async void goToEventsPage(Occurance eventOccurance)
        {
            //First find the event
            Event currEvent = null;
            foreach (Event dto in eventsToday)
            {
                if (dto.Id == eventOccurance.Id)
                {
                    currEvent = dto;
                    break;
                }
            }

            if (currEvent == null)
            {
                await DisplayAlert("Error!", "There seems to be a problem displaying the current event", "OK");
            }
            else
            {
                Application.Current.MainPage = new EventsPage(currEvent);
            }
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
                goToEventsPage(currOccurance);
                return;
            }
            Debug.WriteLine(currOccurance.Id);
            //var currSession = (Session)Application.Current.Properties["session"];
            string url = RdsConfig.BaseUrl + RdsConfig.updateGoalAndRoutine;
            //If there is a sublist available, go to goals page if its a Pursue A Goal
            //if (currOccurance.Title == "Pursue A Goal")
            if (currOccurance.IsPersistent == false)
            {
                if (currOccurance.Title == "Pursue A Goal")
                    await Navigation.PushAsync(new GoalsPage(currOccurance.commonTimeOccurs[0].StartDayAndTime.ToString("t"), currOccurance.commonTimeOccurs[0].EndDayAndTime.ToString("t")), false);
                await Navigation.PushAsync(new GoalsPage(currOccurance.StartDayAndTime.ToString("t"), currOccurance.EndDayAndTime.ToString("t")), false);
                //old code
                //Application.Current.MainPage = new GoalsPage(currOccurance.commonTimeOccurs);
            }
            else if (currOccurance.IsInProgress == false && currOccurance.IsComplete == false)
            {
                currOccurance.updateIsInProgress(true);
                currOccurance.DateTimeStarted = DateTime.Now;
                Debug.WriteLine("Should be changed to in progress. InProgress = " + currOccurance.IsInProgress);
                //string toSend = updateOccurance(currOccurance);
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
