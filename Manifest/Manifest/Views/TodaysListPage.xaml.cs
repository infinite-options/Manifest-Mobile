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
using Xamarin.Auth;
using Manifest.LogIn.Classes;
using Manifest.RDS;

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
        public List<Occurance> displayedOccurances;
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

        public TodaysListPage(List<Occurance> occurances, List<Event> events)
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

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            title.Text = today.ToString("dddd");

            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();

            NavigationPage.SetHasNavigationBar(this, false);
            string userInfo = (string)Application.Current.Properties["userId"];
            if (Application.Current.Properties["userId"] == null || (String)Application.Current.Properties["userId"] == "" || Application.Current.Properties["accessToken"] == null || (String)Application.Current.Properties["accessToken"] == "")
            {
                Application.Current.MainPage = new LogInPage();
            }
            todaysOccurances = new List<Occurance>();
            displayedOccurances = new List<Occurance>();
            todaysEvents = new List<Occurance>();
            todaysOccurancesAfternoon = new List<Occurance>();
            todaysOccurancesEvening = new List<Occurance>();
            commonOccur = new List<Occurance>();


            Debug.WriteLine(userInfo);
            //string userID = userInfo.result[0].user_unique_id;
            string userID = userInfo;
            taskListMorning.ItemsSource = datagridMorning;
            taskListAfternoon.ItemsSource = datagridAfternoon;
            taskListEvening.ItemsSource = datagridEvening;
            if (occurances == null)
            {
                Debug.WriteLine("Entered if initializer");
                try
                {
                    //today = DateTime.Today;
                    //todayDate = today.ToString("d");
                    //morningStart = todayDate + ", 12:00:00 AM";
                    //afternoonStart = todayDate + ", 11:00:00 AM";
                    //eveningStart = todayDate + ", 6:00:00 PM";
                    //Debug.WriteLine("dotw: " + today.ToString("dddd"));
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
            else
            {
                Debug.WriteLine("Entered else initializer");
                todaysOccurances = occurances;
                eventsToday = events;
                //displayedOccurances = occurances;
                SortOccurancesAndGroupGoals();
                initialiseDataGrids(displayedOccurances);
            }

        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void TodaysListPageClicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new TodaysListPage(null, null));
        }

        void AboutMeClicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void HelpClicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new AboutMePage();
        }


        private async void initialiseTodaysOccurances(string userID)
        {
            try
            {
                string url = AppConstants.BaseUrl + AppConstants.goalsAndRoutinesUrl + "/" + userID;
                todaysOccurances = await RdsConnect.getOccurances(url);
                SortOccurancesAndGroupGoals();
                await CallGetEvents();
                CreateList();
            }
            catch (Exception e)
            {
                await DisplayAlert("Alert", "Error in TodaysListTest initialiseTodaysOccurances. Error: " + e.ToString(), "OK");
            }
        }

        bool hasItems = false;
        bool scrollViewSet = false;
        float scrollViewY = 0;
        //This function takes the response from the endpoint, and formats it into Occurances
        private void SortOccurancesAndGroupGoals()
        {
            try
            {
                SortOccurances();
                for (int i = 0; i < todaysOccurances.Count; i++)
                {
                    Occurance dto = todaysOccurances[i];
                    if (dto.EndDayAndTime.TimeOfDay < DateTime.Now.TimeOfDay) {
                        dto.StatusColor = Color.FromHex("#BBC7D7");
                    }
                    else {
                        if (dto.IsPersistent == true)
                            dto.StatusColor = Color.FromHex("#FF6B4A");
                        else if (dto.IsEvent == false) dto.StatusColor = Color.FromHex("#FFBD27");
                        else dto.StatusColor = Color.FromHex("#67ABFC");
                    }


                    Debug.WriteLine("occurance tracked: " + dto.Title);
                        Occurance toAdd = new Occurance();
                    if (!dto.IsEvent)
                    {
                        dto.CompletionTime = "This takes " + dto.CompletionTime;
                        dto.TimeInterval = dto.StartDayAndTime.ToString("t") + "-" + dto.EndDayAndTime.ToString("t");
                    }

                    if (!dto.IsEvent && !dto.IsPersistent)
                    {
                        if (hasItems == false)
                        {
                            Debug.WriteLine("first if entered");
                            commonOccur.Add(dto);
                            hasItems = true;
                        }
                        else if (commonOccur.Count != 0 && commonOccur[0].StartDayAndTime.TimeOfDay <= dto.StartDayAndTime.TimeOfDay
                            && commonOccur[0].EndDayAndTime.TimeOfDay >= dto.EndDayAndTime.TimeOfDay)
                        {
                            Debug.WriteLine("second if entered");
                            commonOccur.Add(dto);
                        }
                        else if (commonOccur.Count > 1 && (commonOccur[0].StartDayAndTime.TimeOfDay < dto.StartDayAndTime.TimeOfDay
                            || commonOccur[0].EndDayAndTime.TimeOfDay < dto.EndDayAndTime.TimeOfDay))
                        {
                            Debug.WriteLine("third if entered");
                            Debug.WriteLine("starttimes: " + commonOccur[0].StartDayAndTime.TimeOfDay + " " + dto.StartDayAndTime.TimeOfDay);
                            Debug.WriteLine("endtimes: " + commonOccur[0].EndDayAndTime.TimeOfDay + " " + dto.EndDayAndTime.TimeOfDay);
                            Occurance toAddPursue = new Occurance();
                            toAddPursue.Title = "Pursue A Goal";
                            toAddPursue.StartDayAndTime = commonOccur[0].StartDayAndTime;
                            toAddPursue.EndDayAndTime = commonOccur[0].EndDayAndTime;
                            toAddPursue.TimeInterval = commonOccur[0].TimeInterval;
                            toAddPursue.IsEvent = false;

                            toAddPursue.StatusColor = Color.FromHex("#FFBD27");

                            List<Occurance> holder = new List<Occurance>(commonOccur);
                            Debug.WriteLine("holder count before: " + holder.Count);
                            toAddPursue.commonTimeOccurs = holder;
                            displayedOccurances.Add(toAddPursue);
                            commonOccur.Clear();
                            commonOccur.Add(dto);
                            Debug.WriteLine("holder count after: " + holder.Count);
                        }
                        else if (commonOccur.Count == 1 && (commonOccur[0].StartDayAndTime.TimeOfDay < dto.StartDayAndTime.TimeOfDay
                            || commonOccur[0].EndDayAndTime.TimeOfDay > dto.EndDayAndTime.TimeOfDay))
                        {
                            Debug.WriteLine("not lumped together: " + dto.Title);
                            displayedOccurances.Add(commonOccur[0]);
                            commonOccur.Clear();
                            commonOccur.Add(dto);
                        }
                        else
                        {
                            Debug.WriteLine("else entered");
                            displayedOccurances.Add(dto);
                        }
                    }
                    else
                    {
                        addCommonOccur(commonOccur);
                        displayedOccurances.Add(dto);

                    }
                }
                addCommonOccur(commonOccur);
                return;
            }
            catch (Exception e)
            {
                DisplayAlert("Alert", "Error in TodaysListTest ToOccurances(). Error: " + e.ToString(), "OK");
            }
        }

        private void SortOccurances()
        {
            displayedOccurances.Sort(delegate (Occurance a, Occurance b)
            {
                if (a.StartDayAndTime.TimeOfDay < b.StartDayAndTime.TimeOfDay) return -1;
                else if (a.StartDayAndTime.TimeOfDay == b.StartDayAndTime.TimeOfDay)
                {
                    if (a.IsEvent == true && b.IsEvent == false)
                    {
                        return -1;
                    }
                    else if (a.IsEvent == false && b.IsEvent == true)
                    {
                        return 1;
                    }
                    else if (a.IsPersistent == true && b.IsPersistent == false)
                    {
                        return -1;
                    }
                    else if (a.IsPersistent == false && b.IsPersistent == true)
                    {
                        return 1;
                    }
                    else if (a.EndDayAndTime.TimeOfDay < b.EndDayAndTime.TimeOfDay)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else return 1;
            });
        }

        //This function groups goals for us
        private void addCommonOccur(List<Occurance> commonOccur)
        {
            if (commonOccur.Count > 1)
            {
                Occurance toAddPursue = new Occurance();
                toAddPursue.Title = "Pursue A Goal";
                toAddPursue.StartDayAndTime = commonOccur[0].StartDayAndTime;
                toAddPursue.EndDayAndTime = commonOccur[0].EndDayAndTime;
                toAddPursue.TimeInterval = commonOccur[0].TimeInterval;
                toAddPursue.IsEvent = false;

                toAddPursue.StatusColor = Color.FromHex("#FFBD27");

                List<Occurance> holder = new List<Occurance>(commonOccur);
                Debug.WriteLine("holder count before: " + holder.Count);
                toAddPursue.commonTimeOccurs = holder;
                displayedOccurances.Add(toAddPursue);
            }
            else if (commonOccur.Count == 1)
            {
                displayedOccurances.Add(commonOccur[0]);
            }
            commonOccur.Clear();
            hasItems = false;
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
            Debug.WriteLine("Entered CreateList");
            try
            {
                int i = 0;
                int j = 0;
                //First sort todaysOccurances
                List<Occurance> merged = new List<Occurance>();
                while (i < displayedOccurances.Count || j < todaysEvents.Count)
                {
                    //xDebug.WriteLine(i.ToString() + j.ToString());
                    if (i >= displayedOccurances.Count && j < todaysEvents.Count)
                    {
                        merged.Add(todaysEvents[j]);
                        Debug.WriteLine(todaysEvents[j].Title + " start time: " + todaysEvents[j].StartDayAndTime);
                        j++;
                        continue;
                    }
                    else if (i < displayedOccurances.Count && j >= todaysEvents.Count)
                    {
                        merged.Add(displayedOccurances[i]);
                        Debug.WriteLine(displayedOccurances[i].Title + " start time: " + displayedOccurances[i].StartDayAndTime);
                        i++;
                        continue;
                    }
                    else if (displayedOccurances[i].StartDayAndTime.TimeOfDay < todaysEvents[j].StartDayAndTime.TimeOfDay)
                    {
                        merged.Add(displayedOccurances[i]);
                        Debug.WriteLine(displayedOccurances[i].Title + " start time: " + displayedOccurances[i].StartDayAndTime);
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

        //This function is going to initialise our three data grids

        private async void initialiseDataGrids(List<Occurance> todaysTasks)
        {

            try
            {
                string url = AppConstants.BaseUrl + AppConstants.timeSettingsUrl + "/" + Application.Current.Properties["userId"];
                var response2 = await client2.GetStringAsync(url);  
                int subtract = response2.Length - 4;
                response2 = response2.Substring(1, subtract);
                Debug.WriteLine("Getting time settings:");
                Debug.WriteLine(response2);

                Times timeResponse = JsonConvert.DeserializeObject<Times>(response2);


                datagridMorning.Clear();
                datagridAfternoon.Clear();
                datagridEvening.Clear();
                int i = 0;
                int morningTaskCount = 0;
                int afternoonTaskCount = 0;
                int eveningTaskCount = 0;
                string todaysTaskTime = (todaysTasks[i].StartDayAndTime.ToString("HH") + ":" + todaysTasks[i].StartDayAndTime.ToString("mm"));
                scrollViewY += 75;
                while (i < todaysTasks.Count && String.Compare(todaysTaskTime,timeResponse.afternoon_time) < 0)
                {
                    todaysTasks[i].borderWidth = 0;
                    Debug.WriteLine("taskStartTime: " + todaysTasks[i].StartDayAndTime.TimeOfDay.ToString() + " afternoonStart: " + ToDateTime(afternoonStart).TimeOfDay.ToString());
                    datagridMorning.Add(todaysTasks[i]);
                    todaysTaskTime = (todaysTasks[i].StartDayAndTime.ToString("HH") + ":" + todaysTasks[i].StartDayAndTime.ToString("mm"));
                    if (todaysTasks[i].EndDayAndTime.TimeOfDay < DateTime.Now.TimeOfDay && !scrollViewSet)
                    {
                        scrollViewY += 115;
                    }
                    else if (!scrollViewSet)
                    {
                        scrollViewSet = true;
                        todaysTasks[i].updateBorderWidth(5);
                    }
                    morningTaskCount++;
                    i++;
                }
                if (!scrollViewSet)
                {
                    scrollViewY += 135;
                }
                while (i < todaysTasks.Count && String.Compare(todaysTaskTime, timeResponse.evening_time) < 0)
                {
                    todaysTasks[i].borderWidth = 0;
                    Debug.WriteLine("afternoonTaskStartTime: " + todaysTasks[i].StartDayAndTime.TimeOfDay.ToString());
                    datagridAfternoon.Add(todaysTasks[i]);
                    todaysTaskTime = (todaysTasks[i].StartDayAndTime.ToString("HH") + ":" + todaysTasks[i].StartDayAndTime.ToString("mm"));
                    if (todaysTasks[i].EndDayAndTime.TimeOfDay < DateTime.Now.TimeOfDay && !scrollViewSet)
                    {
                        scrollViewY += 115;
                    }
                    else if (!scrollViewSet)
                    {
                        scrollViewSet = true;
                        todaysTasks[i].updateBorderWidth(5);
                    }
                    afternoonTaskCount++;
                    i++;
                }
                if (!scrollViewSet)
                {
                    scrollViewY += 135;
                }
                while (i < todaysTasks.Count)
                {
                    todaysTasks[i].borderWidth = 0;
                    Debug.WriteLine("nightTaskStartTime: " + todaysTasks[i].StartDayAndTime.TimeOfDay.ToString());
                    Debug.WriteLine("nightTaskStartHrMin: " + todaysTasks[i].StartDayAndTime.ToString("HH") + ":" + todaysTasks[i].StartDayAndTime.ToString("mm"));
                    datagridEvening.Add(todaysTasks[i]);
                    if (todaysTasks[i].EndDayAndTime.TimeOfDay < DateTime.Now.TimeOfDay && !scrollViewSet)
                    {
                        scrollViewY += 115;
                    }
                    else if (!scrollViewSet)
                    {
                        scrollViewSet = true;
                        todaysTasks[i].updateBorderWidth(5);
                    }
                    eveningTaskCount++;
                    i++;
                }
                Debug.WriteLine("morningCount: " + morningTaskCount.ToString());
                Debug.WriteLine("afternoonCount: " + afternoonTaskCount.ToString());
                Debug.WriteLine("eveningCount: " + eveningTaskCount.ToString());

                taskListMorning.HeightRequest = (morningTaskCount * 115) + 60;
                if (morningTaskCount == 0)
                    taskListEvening.HeightRequest = 60;
                taskListAfternoon.HeightRequest = (afternoonTaskCount * 115) + 60;
                if (afternoonTaskCount == 0)
                    taskListEvening.HeightRequest = 60;
                taskListEvening.HeightRequest = (eveningTaskCount * 115) + 60;
                if (eveningTaskCount == 0)
                    taskListEvening.HeightRequest = 60;
                await todaysSchedule.ScrollToAsync(0, scrollViewY, true);
            }
            catch (Exception e)
            {
                await DisplayAlert("Alert", "Error in TodaysListTest initialiseDataGrids(). Error: " + e.ToString(), "OK");
            }
        }

        private async Task CallGetEvents()
        {
            string valid = await GetEvents();
            if (valid == "FAILURE")
            {
                Debug.WriteLine("Need to refresh token");
                //We want to get a new token in this case
                string refresh = (string)Application.Current.Properties["refreshToken"];
                bool new_tokens = await GoogleAPI.RefreshToken(refresh);
                if (new_tokens == false)
                {
                    //Navigate back to login page
                    Application.Current.MainPage = new LogInPage();
                }
                //Otherwise, we simply call getEvents again
                valid = await GetEvents();
                if (valid == "SUCCESS")
                {
                    Debug.WriteLine("Successfully got events!");
                }
                else
                {
                    Debug.WriteLine("Error getting refresh tokens");
                }
            }
        }

        private async Task<string> GetEvents()
        {
            try
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
                string url = AppConstants.GoogleCalendarUrl + "?orderBy=startTime&singleEvents=true&";
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
                Debug.WriteLine("EVEN URL: " + fullURI);
                request.RequestUri = new Uri(fullURI);
                request.Method = HttpMethod.Get;

                //Format Headers of Request with included Token
                string bearerString = string.Format("Bearer {0}", authToken);
                request.Headers.Add("Authorization", bearerString);
                request.Headers.Add("Accept", "application/json");

                //Debug.WriteLine("Manifest.Services.Google.Calendar: Making request to " + fullURI);

                var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return "FAILURE";
                }
                var json = await response.Content.ReadAsStringAsync();
                //var json = response.Content;
                Debug.WriteLine("Calendars response:\n" + json);
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
                return "FAILURE";
            }
            return "SUCCESS";
        }

        private void EventsToOccurances(List<Event> events)
        {
            Debug.WriteLine("Entered EventsToOccurances");
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
                await Navigation.PushAsync(new EventsPage(currEvent), false);
            }
        }

        //This function is called whenever a tile is tapped. It checks for suboccurances, and navigates to a new page if there are any
        async void checkSubOccurance(object sender, EventArgs args)
        {
            try
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
                string url = AppConstants.BaseUrl + AppConstants.updateGoalAndRoutine;
                //If there is a sublist available, go to goals page if its a Pursue A Goal
                //if (currOccurance.Title == "Pursue A Goal")
                if (currOccurance.IsPersistent == false)
                {
                    if (currOccurance.Title == "Pursue A Goal")
                        await Navigation.PushAsync(new GoalsPage(currOccurance.commonTimeOccurs[0].StartDayAndTime.ToString("t"), currOccurance.commonTimeOccurs[0].EndDayAndTime.ToString("t")), false);
                    else await Navigation.PushAsync(new GoalsPage(currOccurance.StartDayAndTime.ToString("t"), currOccurance.EndDayAndTime.ToString("t")), false);
                    //old code
                    //Application.Current.MainPage = new GoalsPage(currOccurance.commonTimeOccurs);
                }
                else if (currOccurance.IsPersistent == true && currOccurance.IsSublistAvailable == true)
                {
                    await Navigation.PushAsync(new RoutinePage());
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
            catch (Exception e)
            {
               await DisplayAlert("Alert", "Error in TodaysList checkSubOccurance. Error: " + e.ToString(), "OK");
            }
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SettingsPage(), false);
        }
    }
}
