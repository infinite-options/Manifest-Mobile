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

            calendarSwitch.IsToggled = (bool) Application.Current.Properties["showCalendar"];
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
                var responseContent = await RDSResponse.Content.ReadAsStringAsync();
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
                                    calendarSwitch.IsToggled = true;
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

                                    var clientResponse = await client.PostAsync("https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev/api/v2/updateGuid/add", notificationContent);

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
            if (setting == false)
            {
                // DISPLAY SETTINGS UI
                title.Text = "Settings";
                mainStackLayoutRow.Height = 0;
                settingStackLayoutRow.Height = height;
                barStackLayoutRow.Height = 70;
                setting = true;
            }
            else
            {
                // HIDE SETTINGS UI
                title.Text = "Manifest";
                mainStackLayoutRow.Height = height;
                settingStackLayoutRow.Height = 0;
                barStackLayoutRow.Height = 0;
                setting = false;
            }
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            frameColor.BackgroundColor = Color.Pink;
            title.Text = "Manifest";
            subTitle.Text = "Sub title has changed";
        }

        void TapGestureRecognizer_Tapped_1(System.Object sender, System.EventArgs e)
        {
            frameColor.BackgroundColor = Color.Green;
            title.Text = "Today";
            subTitle.Text = "Sub title has changed";
        }

        void TapGestureRecognizer_Tapped_2(System.Object sender, System.EventArgs e)
        {
            frameColor.BackgroundColor = Color.Black;
            title.Text = "Goals";
            subTitle.Text = "Sub title has changed";
        }

        void TapGestureRecognizer_Tapped_3(System.Object sender, System.EventArgs e)
        {
            frameColor.BackgroundColor = Color.Red;
            title.Text = "Get Crasfty";
            subTitle.Text = "Sub title has changed";
        }

        void TapGestureRecognizer_Tapped_4(System.Object sender, System.EventArgs e)
        {
            frameColor.BackgroundColor = Color.Yellow;
            title.Text = "What's important to me";
            subTitle.Text = "Sub title has changed";
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

        }

        void WhoAmIClick(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new WhoAmIPage();
        }

        void NoteToFutureSelfClick(System.Object sender, System.EventArgs e)
        {

        }

        void TodayListClick(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new TodaysListPage());
        }

        void Switch_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            if(calendarSwitch.IsToggled == false)
            {
                Debug.WriteLine("SET SHOW CALENDAR TO FALSE");
                Application.Current.Properties["showCalendar"] = false;
            }
            else
            {
                if((bool)Application.Current.Properties["showCalendar"]== false)
                {
                    GoogleLogInClick();
                }
            }
        }

        public void GoogleLogInClick()
        {
            string clientId = string.Empty;
            string redirectUri = string.Empty;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = Constant.GoogleiOSClientID;
                    redirectUri = Constant.GoogleRedirectUrliOS;
                    break;

                case Device.Android:
                    clientId = Constant.GoogleAndroidClientID;
                    redirectUri = Constant.GoogleRedirectUrlAndroid;
                    break;
            }

            var authenticator = new OAuth2Authenticator(clientId, string.Empty, Constant.GoogleScope, new Uri(Constant.GoogleAuthorizeUrl), new Uri(redirectUri), new Uri(Constant.GoogleAccessTokenUrl), null, true);
            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();

            authenticator.Completed += GoogleAuthenticatorCompleted;
            authenticator.Error += GoogleAuthenticatorError;

            AuthenticationState.Authenticator = authenticator;
            presenter.Login(authenticator);
        }

        private async void GoogleAuthenticatorError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;

            if (authenticator != null)
            {
                authenticator.Completed -= GoogleAuthenticatorCompleted;
                authenticator.Error -= GoogleAuthenticatorError;
            }

            await DisplayAlert("Authentication error: ", e.Message, "OK");
        }

        private async void GoogleAuthenticatorCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Enter GoogleAuthenticatorCompleted");
            var authenticator = sender as OAuth2Authenticator;

            if (authenticator != null)
            {
                authenticator.Completed -= GoogleAuthenticatorCompleted;
                authenticator.Error -= GoogleAuthenticatorError;
            }

            if (e.IsAuthenticated)
            {
                Application.Current.Properties["showCalendar"] = true;
                Application.Current.Properties["accessToken"] = e.Account.Properties["access_token"];
                Application.Current.Properties["refreshToken"] = e.Account.Properties["refresh_token"];
            }
            else
            {
                await DisplayAlert("Error", "Google was not able to autheticate your account", "OK");
            }
        }

        void SetColorScheme(System.Object sender, System.EventArgs e)
        {
            var selectedFrame = (Frame)sender;
            Debug.WriteLine("Frame ClassId " + selectedFrame.ClassId);
            if (selectedFrame.ClassId == "retro")
            {
                retroScheme.BackgroundColor = Color.FromHex("#0C1E21");
                vibrantScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                coolScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                cottonScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                classicScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                retroLabel.TextColor = Color.FromHex("#FFFFFF");
                vibrantLabel.TextColor = Color.FromHex("#0C1E21");
                coolLabel.TextColor = Color.FromHex("#0C1E21");
                cottonLabel.TextColor = Color.FromHex("#0C1E21");
                classicLabel.TextColor = Color.FromHex("#0C1E21");
                SaveColorScheme(selectedFrame.ClassId, "#F4F9E9", "#153243", "#B4B8AB", "#EEF0EB", "#284B63", "#F5948D");
            }
            else if (selectedFrame.ClassId == "vibrant")
            {
                retroScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                vibrantScheme.BackgroundColor = Color.FromHex("#0C1E21");
                coolScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                cottonScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                classicScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                retroLabel.TextColor = Color.FromHex("#0C1E21");
                vibrantLabel.TextColor = Color.FromHex("#FFFFFF");
                coolLabel.TextColor = Color.FromHex("#0C1E21");
                cottonLabel.TextColor = Color.FromHex("#0C1E21");
                classicLabel.TextColor = Color.FromHex("#0C1E21");
                SaveColorScheme(selectedFrame.ClassId, "#FFFFFF", "#4DC4B6", "#F6A01F", "#CBF3F0", "#482728", "#F8C069");
            }
            else if (selectedFrame.ClassId == "cool")
            {
                retroScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                vibrantScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                coolScheme.BackgroundColor = Color.FromHex("#0C1E21");
                cottonScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                classicScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                retroLabel.TextColor = Color.FromHex("#0C1E21");
                vibrantLabel.TextColor = Color.FromHex("#0C1E21");
                coolLabel.TextColor = Color.FromHex("#FFFFFF");
                cottonLabel.TextColor = Color.FromHex("#0C1E21");
                classicLabel.TextColor = Color.FromHex("#0C1E21");
                SaveColorScheme(selectedFrame.ClassId, "#FDFDFD", "#03182B", "#93A0AF", "#A7EEFF", "#59A3B7", "#5AA6F5");
            }
            else if (selectedFrame.ClassId == "cotton")
            {
                retroScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                vibrantScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                coolScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                cottonScheme.BackgroundColor = Color.FromHex("#0C1E21");
                classicScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                retroLabel.TextColor = Color.FromHex("#0C1E21");
                vibrantLabel.TextColor = Color.FromHex("#0C1E21");
                coolLabel.TextColor = Color.FromHex("#0C1E21");
                cottonLabel.TextColor = Color.FromHex("#FFFFFF");
                classicLabel.TextColor = Color.FromHex("#0C1E21");
                SaveColorScheme(selectedFrame.ClassId, "#FCE4E0", "#F38375", "#EF6351", "#F59C9C", "#F6A399", "#7A5980");
            }
            else if (selectedFrame.ClassId == "classic")
            {
                retroScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                vibrantScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                coolScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                cottonScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                classicScheme.BackgroundColor = Color.FromHex("#0C1E21");
                retroLabel.TextColor = Color.FromHex("#0C1E21");
                vibrantLabel.TextColor = Color.FromHex("#0C1E21");
                coolLabel.TextColor = Color.FromHex("#0C1E21");
                cottonLabel.TextColor = Color.FromHex("#0C1E21");
                classicLabel.TextColor = Color.FromHex("#FFFFFF");
                SaveColorScheme(selectedFrame.ClassId, "#F2F7FC", "#9DB2CB", "#376DAC", "#F8BE28", "#F26D4B", "#67ABFC");
            }
        }

        void LogOutClick(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new LogInPage();
        }

        void SaveColorScheme(string colorSchemeName, string backgroudColor, string headerColor, string navBarColor, string goalColor, string routineColor, string eventColor)
        {
            Application.Current.Properties["colorScheme"] = colorSchemeName;
            Application.Current.Properties["background"] = backgroudColor;
            Application.Current.Properties["header"] = headerColor;
            Application.Current.Properties["navBar"] = navBarColor;
            Application.Current.Properties["goal"] = goalColor;
            Application.Current.Properties["routine"] = routineColor;
            Application.Current.Properties["event"] = eventColor;

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
            logOutFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
        }
    }
}
