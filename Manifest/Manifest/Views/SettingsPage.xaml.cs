using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manifest.Config;
using Manifest.Interfaces;
using Manifest.LogIn.Classes;
using Xamarin.Auth;
using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class SettingsPage : ContentPage
    {

        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        string city;
        string time;
        string parentName;
        public SettingsPage(string name = null)
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

            title.Text = "Settings";

            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = GetCurrentTime();

            calendarSwitch.IsToggled = (bool)Application.Current.Properties["showCalendar"];
            Debug.WriteLine("AUTO SIGN IN");
            foreach (string key in Application.Current.Properties.Keys)
            {
                Debug.WriteLine("key: {0}, value: {1}", key, Application.Current.Properties[key]);
            }

            calendarSwitch.IsToggled = (bool)Application.Current.Properties["showCalendar"];

            NavigationPage.SetHasNavigationBar(this, false);

            parentName = name;


            string version = "";
            string build = "";
            version = DependencyService.Get<IAppVersionAndBuild>().GetVersionNumber();
            build = DependencyService.Get<IAppVersionAndBuild>().GetBuildNumber();

            appVersion.Text = "App version: " + version +", App build: " + build;


            var colorTheme = (string)Application.Current.Properties["colorScheme"];
            if(colorTheme == "retro")
            {
                retroScheme.BackgroundColor = Color.FromHex("#0C1E21");
                vibrantScheme.BackgroundColor = Color.Transparent;
                coolScheme.BackgroundColor = Color.Transparent;
                cottonScheme.BackgroundColor = Color.Transparent;
                classicScheme.BackgroundColor = Color.Transparent;
                retroLabel.TextColor = Color.FromHex("#FFFFFF");
                vibrantLabel.TextColor = Color.FromHex("#0C1E21");
                coolLabel.TextColor = Color.FromHex("#0C1E21");
                cottonLabel.TextColor = Color.FromHex("#0C1E21");
                classicLabel.TextColor = Color.FromHex("#0C1E21");
            }
            else if (colorTheme == "vibrant")
            {
                retroScheme.BackgroundColor = Color.Transparent;
                vibrantScheme.BackgroundColor = Color.FromHex("#0C1E21");
                coolScheme.BackgroundColor = Color.Transparent;
                cottonScheme.BackgroundColor = Color.Transparent;
                classicScheme.BackgroundColor = Color.Transparent;
                retroLabel.TextColor = Color.FromHex("#0C1E21");
                vibrantLabel.TextColor = Color.FromHex("#FFFFFF");
                coolLabel.TextColor = Color.FromHex("#0C1E21");
                cottonLabel.TextColor = Color.FromHex("#0C1E21");
                classicLabel.TextColor = Color.FromHex("#0C1E21");
            }
            else if (colorTheme == "cool")
            {
                retroScheme.BackgroundColor = Color.Transparent;
                vibrantScheme.BackgroundColor = Color.Transparent;
                coolScheme.BackgroundColor = Color.FromHex("#0C1E21");
                cottonScheme.BackgroundColor = Color.Transparent;
                classicScheme.BackgroundColor = Color.Transparent;
                retroLabel.TextColor = Color.FromHex("#0C1E21");
                vibrantLabel.TextColor = Color.FromHex("#0C1E21");
                coolLabel.TextColor = Color.FromHex("#FFFFFF");
                cottonLabel.TextColor = Color.FromHex("#0C1E21");
                classicLabel.TextColor = Color.FromHex("#0C1E21");
            }
            else if (colorTheme == "cotton")
            {
                retroScheme.BackgroundColor = Color.Transparent;
                vibrantScheme.BackgroundColor = Color.Transparent;
                coolScheme.BackgroundColor = Color.Transparent;
                cottonScheme.BackgroundColor = Color.FromHex("#0C1E21");
                classicScheme.BackgroundColor = Color.Transparent;
                retroLabel.TextColor = Color.FromHex("#0C1E21");
                vibrantLabel.TextColor = Color.FromHex("#0C1E21");
                coolLabel.TextColor = Color.FromHex("#0C1E21");
                cottonLabel.TextColor = Color.FromHex("#FFFFFF");
                classicLabel.TextColor = Color.FromHex("#0C1E21");
            }
            else if (colorTheme == "classic")
            {
                retroScheme.BackgroundColor = Color.Transparent;
                vibrantScheme.BackgroundColor = Color.Transparent;
                coolScheme.BackgroundColor = Color.Transparent;
                cottonScheme.BackgroundColor = Color.Transparent;
                classicScheme.BackgroundColor = Color.FromHex("#0C1E21");
                retroLabel.TextColor = Color.FromHex("#0C1E21");
                vibrantLabel.TextColor = Color.FromHex("#0C1E21");
                coolLabel.TextColor = Color.FromHex("#0C1E21");
                cottonLabel.TextColor = Color.FromHex("#0C1E21");
                classicLabel.TextColor = Color.FromHex("#FFFFFF");
            }
        }

        public string GetCurrentTime()
        {
            var currentTime = DateTime.Now;
            time = currentTime.ToString("MMMM d, yyyy");
            return time;
        }

        void Switch_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            if (calendarSwitch.IsToggled == false)
            {
                Debug.WriteLine("SET SHOW CALENDAR TO FALSE");
                Application.Current.Properties["showCalendar"] = false;
            }
            else
            {
                if ((bool)Application.Current.Properties["showCalendar"] == false)
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
                    clientId = AppConstants.GoogleiOSClientID;
                    redirectUri = AppConstants.GoogleRedirectUrliOS;
                    break;

                case Device.Android:
                    clientId = AppConstants.GoogleAndroidClientID;
                    redirectUri = AppConstants.GoogleRedirectUrlAndroid;
                    break;
            }

            var authenticator = new OAuth2Authenticator(clientId, string.Empty, AppConstants.GoogleScope, new Uri(AppConstants.GoogleAuthorizeUrl), new Uri(redirectUri), new Uri(AppConstants.GoogleAccessTokenUrl), null, true);
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
                vibrantScheme.BackgroundColor = Color.Transparent;
                coolScheme.BackgroundColor = Color.Transparent;
                cottonScheme.BackgroundColor = Color.Transparent;
                classicScheme.BackgroundColor = Color.Transparent;
                retroLabel.TextColor = Color.FromHex("#FFFFFF");
                vibrantLabel.TextColor = Color.FromHex("#0C1E21");
                coolLabel.TextColor = Color.FromHex("#0C1E21");
                cottonLabel.TextColor = Color.FromHex("#0C1E21");
                classicLabel.TextColor = Color.FromHex("#0C1E21");
                SaveColorScheme(selectedFrame.ClassId, "#F4F9E9", "#153243", "#EEF0EB", "#B4B8AB", "#284B63", "#F5948D");
            }
            else if (selectedFrame.ClassId == "vibrant")
            {
                retroScheme.BackgroundColor = Color.Transparent;
                vibrantScheme.BackgroundColor = Color.FromHex("#0C1E21");
                coolScheme.BackgroundColor = Color.Transparent;
                cottonScheme.BackgroundColor = Color.Transparent;
                classicScheme.BackgroundColor = Color.Transparent;
                retroLabel.TextColor = Color.FromHex("#0C1E21");
                vibrantLabel.TextColor = Color.FromHex("#FFFFFF");
                coolLabel.TextColor = Color.FromHex("#0C1E21");
                cottonLabel.TextColor = Color.FromHex("#0C1E21");
                classicLabel.TextColor = Color.FromHex("#0C1E21");
                SaveColorScheme(selectedFrame.ClassId, "#FFFFFF", "#4DC4B6", "#CBF3F0", "#F6A01F", "#482728", "#F8C069");
            }
            else if (selectedFrame.ClassId == "cool")
            {
                retroScheme.BackgroundColor = Color.Transparent;
                vibrantScheme.BackgroundColor = Color.Transparent;
                coolScheme.BackgroundColor = Color.FromHex("#0C1E21");
                cottonScheme.BackgroundColor = Color.Transparent;
                classicScheme.BackgroundColor = Color.Transparent;
                retroLabel.TextColor = Color.FromHex("#0C1E21");
                vibrantLabel.TextColor = Color.FromHex("#0C1E21");
                coolLabel.TextColor = Color.FromHex("#FFFFFF");
                cottonLabel.TextColor = Color.FromHex("#0C1E21");
                classicLabel.TextColor = Color.FromHex("#0C1E21");
                SaveColorScheme(selectedFrame.ClassId, "#FDFDFD", "#03182B", "#A7EEFF" , "#93A0AF", "#59A3B7", "#5AA6F5");
            }
            else if (selectedFrame.ClassId == "cotton")
            {
                retroScheme.BackgroundColor = Color.Transparent;
                vibrantScheme.BackgroundColor = Color.Transparent;
                coolScheme.BackgroundColor = Color.Transparent;
                cottonScheme.BackgroundColor = Color.FromHex("#0C1E21");
                classicScheme.BackgroundColor = Color.Transparent;
                retroLabel.TextColor = Color.FromHex("#0C1E21");
                vibrantLabel.TextColor = Color.FromHex("#0C1E21");
                coolLabel.TextColor = Color.FromHex("#0C1E21");
                cottonLabel.TextColor = Color.FromHex("#FFFFFF");
                classicLabel.TextColor = Color.FromHex("#0C1E21");
                SaveColorScheme(selectedFrame.ClassId, "#FCE4E0", "#F38375", "#F59C9C", "#EF6351", "#F6A399", "#7A5980");
            }
            else if (selectedFrame.ClassId == "classic")
            {
                retroScheme.BackgroundColor = Color.Transparent;
                vibrantScheme.BackgroundColor = Color.Transparent;
                coolScheme.BackgroundColor = Color.Transparent;
                cottonScheme.BackgroundColor = Color.Transparent;
                classicScheme.BackgroundColor = Color.FromHex("#0C1E21");
                retroLabel.TextColor = Color.FromHex("#0C1E21");
                vibrantLabel.TextColor = Color.FromHex("#0C1E21");
                coolLabel.TextColor = Color.FromHex("#0C1E21");
                cottonLabel.TextColor = Color.FromHex("#0C1E21");
                classicLabel.TextColor = Color.FromHex("#FFFFFF");
                SaveColorScheme(selectedFrame.ClassId, "#F2F7FC", "#889AB5", "#F8BE28", "#376DAC", "#F26D4B", "#67ABFC");
            }
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

            scheduleFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            lobbyFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            supportFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            logOutFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
        }

        void LogOutClick(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new LogInPage();
        }

        async void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            //REMOVE FROM THE STACK

            try
            {
                if (Navigation.NavigationStack.Count != 0)
                {
                    await Navigation.PopAsync(false);
                }
                else
                {
                    if (parentName == "MainPage")
                    {
                        Application.Current.MainPage = new MainPage();
                    }
                    else if (parentName == "AboutMePage")
                    {
                        Application.Current.MainPage = new AboutMePage();
                    }
                    else if (parentName == "WhoAmIPage")
                    {
                        Application.Current.MainPage = new WhoAmIPage();
                    }
                    else if (parentName == "FirstPulsePage")
                    {
                        Application.Current.MainPage = new FirstPulsePage();
                    }
                    else if (parentName == "WhatIsImportantToMePage")
                    {
                        Application.Current.MainPage = new WhatIsImportantToMePage();
                    }
                }
            }
            catch (Exception unknowPage)
            {
                await DisplayAlert("Notice", unknowPage.Message, "OK");
                Application.Current.MainPage = new MainPage();
            }
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new TodaysListPage());
        }

        void TapGestureRecognizer_Tapped_1(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void TapGestureRecognizer_Tapped_2(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new AboutMePage();
        }
    }
}
