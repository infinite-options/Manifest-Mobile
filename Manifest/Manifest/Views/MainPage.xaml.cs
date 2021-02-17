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
            barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
            title.Text = "Manifest";


            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = GetCurrentTime();
            barStackLayoutProperties.BackgroundColor = Color.Salmon;
            barStackLayoutRow.Height = 0;
            buttonStackLayoutRow.Height = lastRowHeight;
            
            Debug.WriteLine("AUTO SIGN IN");
            foreach (string key in Application.Current.Properties.Keys)
            {
                Debug.WriteLine("key: {0}, value: {1}", key, Application.Current.Properties[key]);
            }

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
            barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
            title.Text = "Manifest";


            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = GetCurrentTime();
            barStackLayoutProperties.BackgroundColor = Color.Salmon;
            barStackLayoutRow.Height = 0;
            buttonStackLayoutRow.Height = lastRowHeight;

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
                    var request = new OAuth2Request("GET", new Uri(Constant.GoogleUserInfoUrl), null, user.Account);
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

                    var facebookResponse = client.GetStringAsync(Constant.FacebookUserInfoUrl + user.Account.Properties["access_token"]);
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

                var RDSResponse = await client.PostAsync(Constant.LogInUrl, postContent);
                //var RDSResponse = await client.PostAsync(RdsConfig.BaseUrl + RdsConfig.UserIdFromEmailUrl, postContent);

                var responseContent = await RDSResponse.Content.ReadAsStringAsync();
                Debug.WriteLine(responseContent);
                var authetication = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                var session = JsonConvert.DeserializeObject<Session>(responseContent);
                if (RDSResponse.IsSuccessStatusCode)
                {
                    if (responseContent != null)
                    {
                        if (authetication.code.ToString() == Constant.EmailNotFound)
                        {
                            // Missing a Oops message you don't have an account
                            Application.Current.MainPage = new LogInPage();
                        }
                        if (authetication.code.ToString() == Constant.AutheticatedSuccesful)
                        {

                            try
                            {
                                Debug.WriteLine("USER AUTHENTICATED");
                                DateTime today = DateTime.Now;
                                DateTime expDate = today.AddDays(Constant.days);

                                Application.Current.Properties["userId"] = session.result[0].user_unique_id;
                                Application.Current.Properties["timeStamp"] = expDate;

                                if(platform == "GOOGLE")
                                {
                                    Application.Current.Properties["showCalendar"] = true;
                                    Application.Current.Properties["accessToken"] = user.Account.Properties["access_token"];
                                    Application.Current.Properties["refreshToken"] = user.Account.Properties["refresh_token"];
                                }

                                _ = Application.Current.SavePropertiesAsync();

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

                                    var clientResponse = await client.PostAsync(RdsConfig.BaseUrl + RdsConfig.addGuid, notificationContent);

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
                        if (authetication.code.ToString() == Constant.ErrorPlatform)
                        {
                            //var RDSCode = JsonConvert.DeserializeObject<RDSLogInMessage>(responseContent);
                            //test.Hide();
                            //Application.Current.MainPage = new LogInPage("Message", RDSCode.message);
                            
                        }

                        if (authetication.code.ToString() == Constant.ErrorUserDirectLogIn)
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

        void WhatAreYouCurrentlyDoingClick(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new FirstPulsePage();
        }

        void WhoAmIClick(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new WhoAmIPage();
        }

        void NoteToFutureSelfClick(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new FirstPulsePage();
        }

        void TodayListClick(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new TodaysListPage());
        }

    }
}
