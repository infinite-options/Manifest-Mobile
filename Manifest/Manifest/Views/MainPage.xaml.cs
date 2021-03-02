using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Manifest.Config;
using Manifest.LogIn.Apple;
using Manifest.LogIn.Classes;
using Manifest.Models;
using Manifest.RDS;
using Newtonsoft.Json;
using Xamarin.Auth;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class MainPage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        string city;
        string time;
        string currentActivity;
        Occurance currOccurance;
        public List<Event> eventsToday;
        HttpClient client = new HttpClient();
        List<Occurance> todaysEvents = new List<Occurance>();
        List<Occurance> todaysOccurances = new List<Occurance>();
        // DICTIONARY KEYS WE HAVE ACCESS TO ARE:
        // 1. "location"
        // 2. "userId"
        // 3. "timeStamp"
        // 4. "accessToken"
        // 5. "refreshToken"

        public MainPage()
        {
            InitializeComponent();

            setting = false;
            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            scheduleFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            lobbyFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            supportFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);


            goalsEllipse.Fill = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["goal"]));
            routineEllipse.Fill = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["goal"]));
            pulseEllipse.Fill = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["goal"]));
            whoAmIEllipse.Fill = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["goal"]));
            infoEllipse.Fill = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["goal"]));
            centerEllipse.Fill = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["header"]));
            subCenterEllipse.Fill = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["goal"]));
            scheduleEllipse.Fill = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["goal"]));


            //barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
            title.Text = "Manifest";


            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = GetCurrentTime();
            Debug.WriteLine("AUTO SIGN IN");

            foreach (string key in Application.Current.Properties.Keys)
            {
                Debug.WriteLine("key: {0}, value: {1}", key, Application.Current.Properties[key]);
            }
            GetCurrOccurance();
        }

        public MainPage(AuthenticatorCompletedEventArgs googleFacebookAccount = null, AppleAccount appleCredentials = null, string platform = "")
        {
            InitializeComponent();
            UserVerification(googleFacebookAccount, appleCredentials, platform);
            setting = false;
            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            scheduleFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            lobbyFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            supportFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            //barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
            title.Text = "Manifest";


            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = GetCurrentTime();
            //barStackLayoutProperties.BackgroundColor = Color.Salmon;
            //barStackLayoutRow.Height = 0;
            //buttonStackLayoutRow.Height = lastRowHeight;

            

        }

        public async void UserVerification(AuthenticatorCompletedEventArgs user = null, AppleAccount appleCredentials = null, string platform = "")
        {
            try
            {
                var client = new HttpClient();
                var socialLogInPost = new SocialLogInPost();
                var googleData = new GoogleResponse();
                var facebookData = new FacebookResponse();

                if (platform == "GOOGLE")
                {
                    var request = new OAuth2Request("GET", new Uri(AppConstants.GoogleUserInfoUrl), null, user.Account);
                    var GoogleResponse = await request.GetResponseAsync();
                    var googelUserData = GoogleResponse.GetResponseText();

                    googleData = JsonConvert.DeserializeObject<GoogleResponse>(googelUserData);

                    socialLogInPost.email = googleData.email;
                    socialLogInPost.social_id = googleData.id;
                    socialLogInPost.mobile_access_token = user.Account.Properties["access_token"];
                    socialLogInPost.mobile_refresh_token = user.Account.Properties["refresh_token"];
                }
                else if (platform == "FACEBOOK")
                {

                    var facebookResponse = client.GetStringAsync(AppConstants.FacebookUserInfoUrl + user.Account.Properties["access_token"]);
                    var facebookUserData = facebookResponse.Result;

                    facebookData = JsonConvert.DeserializeObject<FacebookResponse>(facebookUserData);

                    socialLogInPost.email = facebookData.email;
                    socialLogInPost.social_id = facebookData.id;
                    socialLogInPost.mobile_access_token = user.Account.Properties["access_token"];
                    socialLogInPost.mobile_refresh_token = user.Account.Properties["access_token"];
                }
                else if (platform == "APPLE")
                {
                    socialLogInPost.email = appleCredentials.Email;
                    socialLogInPost.social_id = appleCredentials.UserId;
                    socialLogInPost.mobile_access_token = appleCredentials.Token;
                    socialLogInPost.mobile_refresh_token = appleCredentials.Token;
                }

                socialLogInPost.signup_platform = platform;

                var socialLogInPostSerialized = JsonConvert.SerializeObject(socialLogInPost);
                var postContent = new StringContent(socialLogInPostSerialized, Encoding.UTF8, "application/json");

                var RDSResponse = await client.PostAsync(AppConstants.BaseUrl + AppConstants.login, postContent);
                //var RDSResponse = await client.PostAsync(AppConstants.BaseUrl + AppConstants.UserIdFromEmailUrl, postContent);

                var responseContent = await RDSResponse.Content.ReadAsStringAsync();
                Debug.WriteLine(responseContent);
                var authetication = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                var session = JsonConvert.DeserializeObject<Session>(responseContent);
                if (RDSResponse.IsSuccessStatusCode)
                {
                    if (responseContent != null)
                    {
                        if (authetication.code.ToString() == AppConstants.EmailNotFound)
                        {
                            // Missing a Oops message you don't have an account
                            Application.Current.MainPage = new LogInPage();
                        }
                        if (authetication.code.ToString() == AppConstants.AutheticatedSuccesful)
                        {

                            try
                            {
                                Debug.WriteLine("USER AUTHENTICATED");
                                DateTime today = DateTime.Now;
                                DateTime expDate = today.AddDays(AppConstants.days);

                                Application.Current.Properties["userId"] = session.result[0].user_unique_id;
                                Application.Current.Properties["timeStamp"] = expDate; 
                                Application.Current.Properties["platform"] = platform;

                                if (platform == "GOOGLE")
                                {
                                    Application.Current.Properties["showCalendar"] = true;
                                    Application.Current.Properties["accessToken"] = user.Account.Properties["access_token"];
                                    Application.Current.Properties["refreshToken"] = user.Account.Properties["refresh_token"];
                                }

                                _ = Application.Current.SavePropertiesAsync();
                                GetCurrOccurance();
                                string id = (string)Application.Current.Properties["userId"];
                                string guid = (string)Application.Current.Properties["guid"];
                                Debug.WriteLine("GUID FROM MAIN PAGE: " + guid);
                                if (guid != "")
                                {
                                    NotificationPost notificationPost = new NotificationPost();

                                    notificationPost.user_unique_id = id;
                                    notificationPost.guid = guid;
                                    notificationPost.notification = "TRUE";

                                    var notificationSerializedObject = JsonConvert.SerializeObject(notificationPost);
                                    Debug.WriteLine("Notification JSON Object to send: " + notificationSerializedObject);

                                    var notificationContent = new StringContent(notificationSerializedObject, Encoding.UTF8, "application/json");

                                    var clientResponse = await client.PostAsync(AppConstants.BaseUrl + AppConstants.addGuid, notificationContent);

                                    Debug.WriteLine("Status code: " + clientResponse.IsSuccessStatusCode);

                                    if (clientResponse.IsSuccessStatusCode)
                                    {
                                        Debug.WriteLine("We have post the guid to the database");
                                    }
                                    else
                                    {
                                        await DisplayAlert("Ooops!", "Something went wrong. We are not able to send you notification at this moment", "OK");
                                    }
                                }

                                foreach (string key in Application.Current.Properties.Keys)
                                {
                                    Debug.WriteLine("key: {0}, value: {1}", key, Application.Current.Properties[key]);
                                }
                            }
                            catch (Exception s)
                            {
                                await DisplayAlert("Something went wrong with notifications","","OK");
                            }
                        }
                        if (authetication.code.ToString() == AppConstants.ErrorPlatform)
                        {
                            //var RDSCode = JsonConvert.DeserializeObject<RDSLogInMessage>(responseContent);
                            //test.Hide();
                            //Application.Current.MainPage = new LogInPage("Message", RDSCode.message);
                            
                        }

                        if (authetication.code.ToString() == AppConstants.ErrorUserDirectLogIn)
                        {
                            //test.Hide();
                            //Application.Current.MainPage = new LogInPage("Oops!", "You have an existing Serving Fresh account. Please use direct login");
                        }
                    }
                }
            }
            catch (Exception first)
            {
                Debug.WriteLine(first.Message);
            }
        }

        public string GetCurrentTime()
        {
            var currentTime = DateTime.Now;
            time = currentTime.ToString("MMMM d, yyyy");
            return time;
        }

        private async void GetCurrOccurance()
        {
            string userID = (string)Application.Current.Properties["userId"];
            string url = AppConstants.BaseUrl + AppConstants.goalsAndRoutinesUrl + "/" + userID;
            Debug.WriteLine("URL FROM GET CURR OCCURANCE: " + url);
            List<Occurance> todaysOccurances = await RdsConnect.getOccurances(url);
            if(todaysOccurances == null)
            {
                return;
            }
            //Debug.WriteLine("URL: " + url);
            //var response = await client.GetStringAsync(url);
            //Debug.WriteLine("Getting user. User info below:");
            //Debug.WriteLine(response);
            //OccuranceResponse occuranceResponse = JsonConvert.DeserializeObject<OccuranceResponse>(response);
            ////Debug.WriteLine(occuranceResponse);
            await CallGetEvents();
            DateTime dateTime = DateTime.Now;
            currOccurance = SortAndGetActivity(todaysOccurances, todaysEvents, dateTime.TimeOfDay);
            if (currOccurance== null)
            {
                currOccurance = new Occurance();
                currentActivity = "Free time";
                currOccurance.Title = "Go have fun!";
            }
            else if (currOccurance.IsEvent == true)
            {
                currentActivity = "Event";
            }
            else if (currOccurance.IsPersistent == true)
            {
                currentActivity = "Routine";
            }
            else if (currOccurance.IsPersistent == false)
            {
                currentActivity = "Goal";
            }
            CenterCircle.Text = currentActivity + ": " + currOccurance.Title;
            Debug.WriteLine("CurrOccurance = " + currOccurance.Title);
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
                return "FAILURE";
            }
            return "SUCCESS";
        }

        private List<Occurance> EventsToOccurances(List<Event> events)
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
            return todaysEvents;
        }

        private Occurance SortAndGetActivity(List<Occurance> occurances, List<Occurance> events, TimeSpan currDateTime)
        {
            Occurance curr = new Occurance();

            int i = 0;
            int j = 0;
            //First sort todaysOccurances
            occurances.Sort(delegate (Occurance a, Occurance b)
            {
                if (a.StartDayAndTime.TimeOfDay < b.StartDayAndTime.TimeOfDay) return -1;
                else return 1;
            });
            List<Occurance> merged = new List<Occurance>();
            //Debug.WriteLine("Num occurances = " + todaysOccurances.Count);
            //Debug.WriteLine("Num Event = " + todaysEvents.Count);
            while (i < occurances.Count || j < events.Count)
            {
                //xDebug.WriteLine(i.ToString() + j.ToString());
                if (i >= occurances.Count && j < events.Count)
                {
                    merged.Add(events[j]);
                    Debug.WriteLine(events[j].Title + " start time: " + events[j].StartDayAndTime);
                    j++;
                    continue;
                }
                else if (i < occurances.Count && j >= events.Count)
                {
                    merged.Add(occurances[i]);
                    Debug.WriteLine(occurances[i].Title + " start time: " + occurances[i].StartDayAndTime);
                    i++;
                    continue;
                }
                else if (occurances[i].StartDayAndTime.TimeOfDay < events[j].StartDayAndTime.TimeOfDay)
                {
                    merged.Add(occurances[i]);
                    Debug.WriteLine(occurances[i].Title + " start time: " + occurances[i].StartDayAndTime);
                    i++;
                }
                else
                {
                    merged.Add(events[j]);
                    Debug.WriteLine(events[j].Title + " start time: " + events[j].StartDayAndTime);
                    j++;
                }
            }
            todaysOccurances = merged;
            foreach (Occurance activity in merged)
            {
                if (activity.StartDayAndTime.TimeOfDay <= currDateTime && activity.EndDayAndTime.TimeOfDay >= currDateTime)
                {
                    curr = activity;
                    return curr;
                }
            } 
            return null;
        }
        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new SettingsPage("MainPage");
        }

        void RoutinesClick(System.Object sender, System.EventArgs e)
        {
            //Navigation.PushAsync(new TodaysListPage());
            Application.Current.MainPage = new NavigationPage(new RoutinePage());
        }

        void InfoClick(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new AboutMePage();
        }

        void GoalsClick(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new GoalsPage(DateTime.Now.ToString("t"), DateTime.Now.ToString("t")));
        }

        void goToEventsPage(Occurance eventOccurance)
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
                DisplayAlert("Error!", "There seems to be a problem displaying the current event", "OK");
            }
            else
            {
                Application.Current.MainPage = new NavigationPage();
                Navigation.PushAsync(new EventsPage(currEvent), false);
            }
        }

        void WhatAreYouCurrentlyDoingClick(System.Object sender, System.EventArgs e)
        {
           if (currentActivity == "Event")
            {
                goToEventsPage(currOccurance);
            }
           else if (currentActivity == "Routine")
            {
                Application.Current.MainPage = new NavigationPage(new RoutinePage());
            }
           else if (currentActivity == "Goal")
            {
                Application.Current.MainPage = new NavigationPage(new GoalsPage(DateTime.Now.ToString("t"), DateTime.Now.ToString("t")));
            }
            else
            {
                DisplayAlert("Message", "Nothing scheduled right now. Just relax and have fun!", "OK");
            }
        }

        void WhoAmIClick(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new WhoAmIPage();
        }

        void NoteToFutureSelfClick(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new FirstPulsePage());
        }

        void TodayListClick(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new TodaysListPage());
        }

    }
}
