using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manifest.Config;
using Manifest.LogIn.Classes;
using Xamarin.Auth;
using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class RoutineStepsPage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        public RoutineStepsPage()
        {
            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
            calendarSwitch.IsToggled = (bool)Application.Current.Properties["showCalendar"];
            title.Text = "Routine Steps";
            subTitle.Text = "";
            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();


            NavigationPage.SetHasNavigationBar(this, false);
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Navigation.PopAsync(false);
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
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
                mainStackLayoutRow.Height = height;
                settingStackLayoutRow.Height = 0;
                barStackLayoutRow.Height = 0;
                setting = false;
            }
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
