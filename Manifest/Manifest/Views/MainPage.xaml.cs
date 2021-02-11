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

            frameColor.BackgroundColor = Color.FromHex("#9DB2CB");
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

            frameColor.BackgroundColor = Color.FromHex("#9DB2CB");
            title.Text = "Manifest";


            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = GetCurrentTime();
            barStackLayoutProperties.BackgroundColor = Color.Salmon;
            barStackLayoutRow.Height = 0;
            buttonStackLayoutRow.Height = lastRowHeight;

        }

        public async void UserVerification(AuthenticatorCompletedEventArgs user = null, AppleAccount appleCredentials = null, string platform = "")
        {
            //var client = new HttpClient();
            //var socialLogInPost = new SocialLogInPost();

            //var request = new OAuth2Request("GET", new Uri(Constant.GoogleUserInfoUrl), null, e.Account);
            //var GoogleResponse = await request.GetResponseAsync();
            //var userData = GoogleResponse.GetResponseText();

            //GoogleResponse googleData = JsonConvert.DeserializeObject<GoogleResponse>(userData);

            //System.Diagnostics.Debug.WriteLine(userData);

            //socialLogInPost.email = googleData.email;
            //socialLogInPost.social_id = googleData.id;
            //socialLogInPost.mobile_access_token = accessToken;
            //socialLogInPost.mobile_refresh_token = refreshToken;
            //socialLogInPost.signup_platform = "GOOGLE";

            //var socialLogInPostSerialized = JsonConvert.SerializeObject(socialLogInPost);
            //var postContent = new StringContent(socialLogInPostSerialized, Encoding.UTF8, "application/json");

            //System.Diagnostics.Debug.WriteLine(socialLogInPostSerialized);

            //var RDSResponse = await client.PostAsync(Constant.LogInUrl, postContent);
            //var responseContent = await RDSResponse.Content.ReadAsStringAsync();
            //Session session = JsonConvert.DeserializeObject<Session>(responseContent);


            //DateTime today = DateTime.Now;
            //DateTime expDate = today.AddDays(Constant.days);

            //Application.Current.Properties["userId"] = session.result[0].user_unique_id;
            //Application.Current.Properties["accessToken"] = accessToken;
            //Application.Current.Properties["refreshToken"] = refreshToken;
            //Application.Current.Properties["timeStamp"] = expDate;

            //Debug.WriteLine("VERIFICATION");
            //foreach(string key in Application.Current.Properties.Keys)
            //{
            //    Debug.WriteLine("key: {0}, value: {1}", key, Application.Current.Properties[key]);
            //}

            //_ = Application.Current.SavePropertiesAsync();


            try
            {
                //var progress = UserDialogs.Instance.Loading("Loading...");
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
                    socialLogInPost.signup_platform = "GOOGLE";
                }
                else if (platform == "FACEBOOK")
                {
                    var facebookResponse = client.GetStringAsync(Constant.FacebookUserInfoUrl + user.Account.Properties["access_token"]);
                    var facebookUserData = facebookResponse.Result;

                    facebookData = JsonConvert.DeserializeObject<FacebookResponse>(facebookUserData);

                    await DisplayAlert("Sign in to your Google Account", "", "");
                    var googleSignInClient = new LogInPage();
                    var o = new object();
                    var e = new EventArgs();
                    googleSignInClient.GoogleLogInClick(o, e);
                    return;
                }
                else if (platform == "APPLE")
                {
                    await DisplayAlert("Sign in to your Google Account", "", "");
                    var googleSignInClient = new LogInPage();
                    var o = new object();
                    var e = new EventArgs();
                    googleSignInClient.GoogleLogInClick(o, e);
                    return;
                }


                var socialLogInPostSerialized = JsonConvert.SerializeObject(socialLogInPost);
                var postContent = new StringContent(socialLogInPostSerialized, Encoding.UTF8, "application/json");

                //var test = UserDialogs.Instance.Loading("Loading...");
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
                           
                            //if (platform == "GOOGLE")
                            //{
                            //    Application.Current.MainPage = new SocialSignUp(googleData.id, googleData.given_name, googleData.family_name, googleData.email, accessToken, refreshToken, "GOOGLE");
                            //}
                            //else if (platform == "FACEBOOK")
                            //{
                            //    Application.Current.MainPage = new SocialSignUp(facebookData.id, facebookData.name, "", facebookData.email, accessToken, accessToken, "FACEBOOK");
                            //}
                            //else if (platform == "APPLE")
                            //{
                            //    Application.Current.MainPage = new SocialSignUp(appleCredentials.UserId, appleCredentials.Name, "", appleCredentials.Email, appleCredentials.Token, appleCredentials.Token, "APPLE");
                            //}
                        }
                        if (authetication.code.ToString() == Constant.AutheticatedSuccesful)
                        {

                            try
                            {
                                DateTime today = DateTime.Now;
                                DateTime expDate = today.AddDays(Constant.days);

                                Application.Current.Properties["userId"] = session.result[0].user_unique_id;
                                Application.Current.Properties["accessToken"] = user.Account.Properties["access_token"]; ;
                                Application.Current.Properties["refreshToken"] = user.Account.Properties["refresh_token"];
                                Application.Current.Properties["timeStamp"] = expDate;

                                Debug.WriteLine("VERIFICATION");
                                foreach (string key in Application.Current.Properties.Keys)
                                {
                                    Debug.WriteLine("key: {0}, value: {1}", key, Application.Current.Properties[key]);
                                }

                                _ = Application.Current.SavePropertiesAsync();

                                //var writeGuid = new RdsConnect();
                                //var guid = new GlobalVars();



                                //if (Device.RuntimePlatform == Device.iOS)
                                //{
                                //    deviceId = Preferences.Get("guid", null);
                                //    if (deviceId != null) { Debug.WriteLine("This is the iOS GUID from Log in: " + deviceId); }
                                //}
                                //else
                                //{
                                //    deviceId = Preferences.Get("guid", null);
                                //    if (deviceId != null) { Debug.WriteLine("This is the Android GUID from Log in " + deviceId); }
                                //}
                                var guid = (string)Application.Current.Properties["guid"];
                                if (guid != "")
                                {
                                    NotificationPost notificationPost = new NotificationPost();

                                    notificationPost.user_unique_id = (string)Application.Current.Properties["user_id"];
                                    notificationPost.guid = (string)Application.Current.Properties["guid"];
                                    notificationPost.notification = "TRUE";

                                    var notificationSerializedObject = JsonConvert.SerializeObject(notificationPost);
                                    Debug.WriteLine("Notification JSON Object to send: " + notificationSerializedObject);

                                    var notificationContent = new StringContent(notificationSerializedObject, Encoding.UTF8, "application/json");

                                    var clientResponse = await client.PostAsync("https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev/api/v2/updateGuid/add", notificationContent);

                                    Debug.WriteLine("Status code: " + clientResponse.IsSuccessStatusCode);

                                    if (clientResponse.IsSuccessStatusCode)
                                    {
                                        System.Diagnostics.Debug.WriteLine("We have post the guid to the database");
                                    }
                                    else
                                    {
                                        await DisplayAlert("Ooops!", "Something went wrong. We are not able to send you notification at this moment", "OK");
                                    }
                                }


                                //writeGuid.storeGUID(guid.user_guid, (string)Application.Current.Properties["userID"]);
                            }
                            catch (Exception s)
                            {
                                await DisplayAlert("Something went wrong with notifications","","OK");
                            }

                            //try
                            //{
                            //    var data = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                            //    Application.Current.Properties["user_id"] = data.result[0].customer_uid;

                            //    UpdateTokensPost updateTokesPost = new UpdateTokensPost();
                            //    updateTokesPost.uid = data.result[0].customer_uid;
                            //    if (platform == "GOOGLE")
                            //    {
                            //        updateTokesPost.mobile_access_token = accessToken;
                            //        updateTokesPost.mobile_refresh_token = refreshToken;
                            //    }
                            //    else if (platform == "FACEBOOK")
                            //    {
                            //        updateTokesPost.mobile_access_token = accessToken;
                            //        updateTokesPost.mobile_refresh_token = accessToken;
                            //    }
                            //    else if (platform == "APPLE")
                            //    {
                            //        updateTokesPost.mobile_access_token = appleCredentials.Token;
                            //        updateTokesPost.mobile_refresh_token = appleCredentials.Token;
                            //    }

                            //    var updateTokesPostSerializedObject = JsonConvert.SerializeObject(updateTokesPost);
                            //    var updateTokesContent = new StringContent(updateTokesPostSerializedObject, Encoding.UTF8, "application/json");
                            //    var updateTokesResponse = await client.PostAsync(Constant.UpdateTokensUrl, updateTokesContent);
                            //    var updateTokenResponseContent = await updateTokesResponse.Content.ReadAsStringAsync();

                            //    if (updateTokesResponse.IsSuccessStatusCode)
                            //    {
                            //        var user = new RequestUserInfo();
                            //        user.uid = data.result[0].customer_uid;

                            //        var requestSelializedObject = JsonConvert.SerializeObject(user);
                            //        var requestContent = new StringContent(requestSelializedObject, Encoding.UTF8, "application/json");

                            //        var clientRequest = await client.PostAsync(Constant.GetUserInfoUrl, requestContent);

                            //        if (clientRequest.IsSuccessStatusCode)
                            //        {
                            //            var userSfJSON = await clientRequest.Content.ReadAsStringAsync();
                            //            var userProfile = JsonConvert.DeserializeObject<UserInfo>(userSfJSON);

                            //            DateTime today = DateTime.Now;
                            //            DateTime expDate = today.AddDays(Constant.days);

                            //            Application.Current.Properties["user_id"] = data.result[0].customer_uid;
                            //            Application.Current.Properties["time_stamp"] = expDate;
                            //            Application.Current.Properties["platform"] = platform;
                            //            Application.Current.Properties["user_email"] = userProfile.result[0].customer_email;
                            //            Application.Current.Properties["user_first_name"] = userProfile.result[0].customer_first_name;
                            //            Application.Current.Properties["user_last_name"] = userProfile.result[0].customer_last_name;
                            //            Application.Current.Properties["user_phone_num"] = userProfile.result[0].customer_phone_num;
                            //            Application.Current.Properties["user_address"] = userProfile.result[0].customer_address;
                            //            Application.Current.Properties["user_unit"] = userProfile.result[0].customer_unit;
                            //            Application.Current.Properties["user_city"] = userProfile.result[0].customer_city;
                            //            Application.Current.Properties["user_state"] = userProfile.result[0].customer_state;
                            //            Application.Current.Properties["user_zip_code"] = userProfile.result[0].customer_zip;
                            //            Application.Current.Properties["user_latitude"] = userProfile.result[0].customer_lat;
                            //            Application.Current.Properties["user_longitude"] = userProfile.result[0].customer_long;

                            //            _ = Application.Current.SavePropertiesAsync();
                            //            await CheckVersion();

                            //            if (Device.RuntimePlatform == Device.iOS)
                            //            {
                            //                deviceId = Preferences.Get("guid", null);
                            //                if (deviceId != null) { Debug.WriteLine("This is the iOS GUID from Log in: " + deviceId); }
                            //            }
                            //            else
                            //            {
                            //                deviceId = Preferences.Get("guid", null);
                            //                if (deviceId != null) { Debug.WriteLine("This is the Android GUID from Log in " + deviceId); }
                            //            }

                            //            if (deviceId != null)
                            //            {
                            //                NotificationPost notificationPost = new NotificationPost();

                            //                notificationPost.uid = (string)Application.Current.Properties["user_id"];
                            //                notificationPost.guid = deviceId.Substring(5);
                            //                Application.Current.Properties["guid"] = deviceId.Substring(5);
                            //                notificationPost.notification = "TRUE";

                            //                var notificationSerializedObject = JsonConvert.SerializeObject(notificationPost);
                            //                Debug.WriteLine("Notification JSON Object to send: " + notificationSerializedObject);

                            //                var notificationContent = new StringContent(notificationSerializedObject, Encoding.UTF8, "application/json");

                            //                var clientResponse = await client.PostAsync(Constant.NotificationsUrl, notificationContent);

                            //                Debug.WriteLine("Status code: " + clientResponse.IsSuccessStatusCode);

                            //                if (clientResponse.IsSuccessStatusCode)
                            //                {
                            //                    System.Diagnostics.Debug.WriteLine("We have post the guid to the database");
                            //                }
                            //                else
                            //                {
                            //                    await DisplayAlert("Ooops!", "Something went wrong. We are not able to send you notification at this moment", "OK");
                            //                }
                            //            }
                            //            test.Hide();
                            //            //Application.Current.MainPage = new SelectionPage();
                            //        }
                            //        else
                            //        {
                            //            test.Hide();
                            //            await DisplayAlert("Alert!", "Our internal system was not able to retrieve your user information. We are working to solve this issue.", "OK");
                            //        }
                            //    }
                            //    else
                            //    {
                            //        test.Hide();
                            //        await DisplayAlert("Oops", "We are facing some problems with our internal system. We weren't able to update your credentials", "OK");
                            //    }
                            //    test.Hide();
                            //}
                            //catch (Exception second)
                            //{
                            //    Debug.WriteLine(second.Message);
                            //}
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
                mainStackLayoutRow.Height = 0;
                settingStackLayoutRow.Height = height;
                setting = true;
            }
            else
            {
                // HIDE SETTINGS UI
                mainStackLayoutRow.Height = height;
                settingStackLayoutRow.Height = 0;
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
            //Application.Current.MainPage = new NavigationPage(new GoalsPage());
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
    }
}
