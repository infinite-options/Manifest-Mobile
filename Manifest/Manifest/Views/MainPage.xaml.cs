using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Manifest.Config;
using Manifest.LogIn.Classes;
using Manifest.Models;
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

        public MainPage(string accessToken, string refreshToken, AuthenticatorCompletedEventArgs e)
        {
            InitializeComponent();
            UserVerification(accessToken,refreshToken,e);
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

        public async void UserVerification(string accessToken, string refreshToken, AuthenticatorCompletedEventArgs e)
        {
            var client = new HttpClient();
            var socialLogInPost = new SocialLogInPost();

            var request = new OAuth2Request("GET", new Uri(Constant.GoogleUserInfoUrl), null, e.Account);
            var GoogleResponse = await request.GetResponseAsync();
            var userData = GoogleResponse.GetResponseText();

            GoogleResponse googleData = JsonConvert.DeserializeObject<GoogleResponse>(userData);

            System.Diagnostics.Debug.WriteLine(userData);

            socialLogInPost.email = googleData.email;
            socialLogInPost.social_id = googleData.id;
            socialLogInPost.mobile_access_token = accessToken;
            socialLogInPost.mobile_refresh_token = refreshToken;
            socialLogInPost.signup_platform = "GOOGLE";

            var socialLogInPostSerialized = JsonConvert.SerializeObject(socialLogInPost);
            var postContent = new StringContent(socialLogInPostSerialized, Encoding.UTF8, "application/json");

            System.Diagnostics.Debug.WriteLine(socialLogInPostSerialized);

            var RDSResponse = await client.PostAsync(Constant.LogInUrl, postContent);
            var responseContent = await RDSResponse.Content.ReadAsStringAsync();
            Session session = JsonConvert.DeserializeObject<Session>(responseContent);


            DateTime today = DateTime.Now;
            DateTime expDate = today.AddDays(Constant.days);

            Application.Current.Properties["userId"] = session.result[0].user_unique_id;
            Application.Current.Properties["accessToken"] = accessToken;
            Application.Current.Properties["refreshToken"] = refreshToken;
            Application.Current.Properties["timeStamp"] = expDate;

            Debug.WriteLine("VERIFICATION");
            foreach(string key in Application.Current.Properties.Keys)
            {
                Debug.WriteLine("key: {0}, value: {1}", key, Application.Current.Properties[key]);
            }

            _ = Application.Current.SavePropertiesAsync();

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
            Application.Current.MainPage = new NavigationPage(new GoalsPage());
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
