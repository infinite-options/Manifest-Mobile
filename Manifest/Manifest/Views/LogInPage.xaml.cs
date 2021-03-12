using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Manifest.LogIn.Classes;
using Manifest.Config;
using Xamarin.Forms;
using Xamarin.Auth;
using System.Windows.Input;
using Xamarin.Essentials;
using Manifest.LogIn.Apple;
using Manifest.Models;
using System.Linq;
using System.Diagnostics;
using Manifest.Notifications;

namespace Manifest.Views
{
    public partial class LogInPage : ContentPage
    {
        public event EventHandler SignIn;
        public bool createAccount = false;
        private string deviceId;                                                         // Initalized boolean value called createAccount
        string location;
        public LogInPage()                                                                                  // This is the class Constructor
        {
            InitializeComponent();                                                                          // This is a Xamarin default
            InitializeAppProperties();                                                                      // This refers to class below

            if (Device.RuntimePlatform == Device.Android)
            {
                appleLogInButton.IsEnabled = false;
            }
            else
            {
                InitializedAppleLogin();
                // Turns on Apple Login for Apple devices
            }
            GetCurrentLocation();


            if (Device.RuntimePlatform == Device.iOS)
            {
                deviceId = GlobalVars.user_guid;
                if (deviceId != null) { Debug.WriteLine("This is the iOS GUID from Log in: " + deviceId); }
            }
            else
            {
                deviceId = GlobalVars.user_guid;
                if (deviceId != null) { Debug.WriteLine("This is the Android GUID from Log in " + deviceId); }
            }
            if (deviceId != "")
            {
                Application.Current.Properties["guid"] = deviceId.Substring(5);
            }
            else
            {
                Application.Current.Properties["guid"] = "";
            }
            

        }

        public void InitializeAppProperties()                                                               // Initializes most (not all) Application.Current.Properties
        {                                                                                                   // You can create additional parameters on the fly
            // App keys
            Application.Current.Properties["location"] = "";
            Application.Current.Properties["userId"] = "";
            var today = DateTime.Now;
            Application.Current.Properties["timeStamp"] = today;
            Application.Current.Properties["accessToken"] = "";
            Application.Current.Properties["refreshToken"] = "";
            Application.Current.Properties["platform"] = "";
            // Color keys
            Application.Current.Properties["colorScheme"] = "classic";
            Application.Current.Properties["background"] = "#FDFDFD";
            Application.Current.Properties["header"] = "#889AB5";
            Application.Current.Properties["navBar"] = "#889AB5";
            Application.Current.Properties["goal"] = "#F8BE28";
            Application.Current.Properties["routine"] = "#F26D4B";
            Application.Current.Properties["event"] = "#67ABFC";
            Application.Current.Properties["showCalendar"] = false;
        }

        public void InitializedAppleLogin()
        {
            var vm = new LoginViewModel();
            vm.AppleError += AppleError;
            BindingContext = vm;
        }

        public async void GetCurrentLocation()
        {
            try
            {
                
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);

                    var placemark = placemarks?.FirstOrDefault();
                    if (placemark != null)
                    {
                        var geocodeAddress =
                            $"AdminArea:       {placemark.AdminArea}\n" +
                            $"CountryCode:     {placemark.CountryCode}\n" +
                            $"CountryName:     {placemark.CountryName}\n" +
                            $"FeatureName:     {placemark.FeatureName}\n" +
                            $"Locality:        {placemark.Locality}\n" +
                            $"PostalCode:      {placemark.PostalCode}\n" +
                            $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                            $"SubLocality:     {placemark.SubLocality}\n" +
                            $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                            $"Thoroughfare:    {placemark.Thoroughfare}\n";

                        Debug.WriteLine(geocodeAddress);
                        Application.Current.Properties["location"] = "";

                        Application.Current.Properties["location"] = placemark.Locality + ", " + placemark.AdminArea;
                    }
                    Debug.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                }
              
            }
            catch (Exception c)
            {
                // Handle not supported on device exception
                Debug.WriteLine("LOCATION MESSAGE CA:" +  c.Message);
            }

        }

        // Facebook
        // comes from LoginPage.xaml or App.xaml.cs
        public void FacebookLogInClick(System.Object sender, System.EventArgs e)                                        // Linked to Clicked="FacebookLogInClick" from LogInPage.xaml
        {
            string clientID = string.Empty;
            string redirectURL = string.Empty;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientID = AppConstants.FacebookiOSClientID;
                    redirectURL = AppConstants.FacebookiOSRedirectUrl;
                    break;
                case Device.Android:
                    clientID = AppConstants.FacebookAndroidClientID;
                    redirectURL = AppConstants.FacebookAndroidRedirectUrl;
                    break;
            }

            var authenticator = new OAuth2Authenticator(clientID, AppConstants.FacebookScope, new Uri(AppConstants.FacebookAuthorizeUrl), new Uri(redirectURL), null, false);  // Initializes variable authenticator
            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();

            // Is this like clicking a button?  ie I just clicked FacebookAuthenticatorCompleted and then that function ran?
            authenticator.Completed += FacebookAuthenticatorCompleted;                                                  // += Creates a button handler (like OnClick in xaml).  Assignment to submit button on the Facebook page
            authenticator.Error += FacebookAutheticatorError;                                                           // Assignment to Cancel button on the Facebook page

            presenter.Login(authenticator);                                                                             // Calls Facebook and invokes Facebook UI.  Authenticator contains app_uid,etc
        }


        // Verifies Facebook Authenticated
        public void FacebookAuthenticatorCompleted(object sender, AuthenticatorCompletedEventArgs e)                    // Called when Facebook submitt is clicked.  Facebook send in "sender" (event handler calls) and "e" contains user parameters
        {
            var authenticator = sender as OAuth2Authenticator;                                                          // Casting sender an an OAuth2Authenticator type

            if (authenticator != null)
            {
                authenticator.Completed -= FacebookAuthenticatorCompleted;
                authenticator.Error -= FacebookAutheticatorError;
            }

            if (e.IsAuthenticated)                                                                                      // How does this statement work?
            {
                string url = AppConstants.BaseUrl + AppConstants.addGuid;
                Debug.WriteLine("WRITE GUID: " + url);

                if (Device.RuntimePlatform == Device.iOS)
                {
                    deviceId = Preferences.Get("guid", "");
                    if (deviceId != null) { Debug.WriteLine("This is the iOS GUID from Log in: " + deviceId); }
                }
                else
                {
                    deviceId = Preferences.Get("guid", "");
                    if (deviceId != null) { Debug.WriteLine("This is the Android GUID from Log in " + deviceId); }
                }
                if (deviceId != "")
                {
                    Application.Current.Properties["guid"] = deviceId.Substring(5);
                }
                else
                {
                    Application.Current.Properties["guid"] = "";
                }
                Application.Current.MainPage = new MainPage(e, null, "FACEBOOK");
            }
            else
            {
                Application.Current.MainPage = new LogInPage();
            }
        }

        // Closes button handlers if they click Cancel in Facebook
        private async void FacebookAutheticatorError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= FacebookAuthenticatorCompleted;
                authenticator.Error -= FacebookAutheticatorError;
            }

            await DisplayAlert("Authentication error: ", e.Message, "OK");
        }

        public void GoogleLogInClick(System.Object sender, System.EventArgs e)
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
                string url = AppConstants.BaseUrl + AppConstants.addGuid;
                Debug.WriteLine("WRITE GUID: " + url);

                if (Device.RuntimePlatform == Device.iOS)
                {
                    deviceId = Preferences.Get("guid", "");
                    if (deviceId != null) { Debug.WriteLine("This is the iOS GUID from Log in: " + deviceId); }
                }
                else
                {
                    deviceId = Preferences.Get("guid", "");
                    if (deviceId != null) { Debug.WriteLine("This is the Android GUID from Log in " + deviceId); }
                }
                if (deviceId != "")
                {
                    Application.Current.Properties["guid"] = deviceId.Substring(5);
                }
                else
                {
                    Application.Current.Properties["guid"] = "";
                }

                //foreach (string key in e.Account.Properties.Keys)
                //{
                //    Debug.WriteLine("Key: {0}, value: {1}", e.Account.Properties[key], e.Account.Properties[key]);
                //}

                Application.Current.MainPage = new MainPage(e, null, "GOOGLE");
            }
            else
            {
                Application.Current.MainPage = new LogInPage();
                await DisplayAlert("Error", "Google was not able to autheticate your account", "OK");
            }
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

        public void AppleLogInClick(System.Object sender, System.EventArgs e)
        {
            SignIn?.Invoke(sender, e);
            var c = (ImageButton)sender;
            c.Command?.Execute(c.CommandParameter);
        }

        public void InvokeSignInEvent(object sender, EventArgs e)
            => SignIn?.Invoke(sender, e);

        private async void AppleError(object sender, EventArgs e)
        {
            await DisplayAlert("Error", "We weren't able to set an account for you", "OK");
        }
    }
}
