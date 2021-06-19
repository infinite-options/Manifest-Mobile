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
using Acr.UserDialogs;
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
                //InitializedAppleLogin();
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

        //public void InitializedAppleLogin()
        //{
        //    var vm = new LoginViewModel();
        //    vm.AppleError += AppleError;
        //    BindingContext = vm;
        //}

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
        public async void FacebookAuthenticatorCompleted(object sender, AuthenticatorCompletedEventArgs e)                    // Called when Facebook submitt is clicked.  Facebook send in "sender" (event handler calls) and "e" contains user parameters
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

                var client = new SignIn();
                UserDialogs.Instance.ShowLoading("We are processing your request...");
                var authenticationStatus  = await client.UserVerification(e, null, "FACEBOOK");
                if(authenticationStatus == "EMAIL WAS NOT FOUND")
                {
                    var account = new AppleAccount();
                    account.Email = "ioappletest@gmail.com";
                    account.UserId = "";
                    account.Name = "";
                    account.RealUserStatus = "";
                    account.Token = "";

                    authenticationStatus = await client.UserVerification(null, account, "APPLE");
                }

                ProcessRequest(authenticationStatus);


            }
            else
            {
                //Application.Current.MainPage = new LogInPage();
            }
        }

        public async void ProcessRequest(string code)
        {
            if(code != "")
            {
                if(code == "EMAIL WAS NOT FOUND")
                {
                    UserDialogs.Instance.HideLoading();
                    //await DisplayAlert("Great!", "We have found your account, and you have successfully sign in.", "OK");

                    //Application.Current.MainPage = new TodaysListPage();
                    await DisplayAlert("Oops", "We did not find the email associated with his account. Please ask your advisor to sign you up and try again.", "OK");
                }
                else if (code == "USER SIGNED IN SUCCESSFULLY AND DEVICE ID WAS REGISTERED SUCCESSFULLY")
                {
                    UserDialogs.Instance.HideLoading();
                    await Application.Current.SavePropertiesAsync();
                    //await DisplayAlert("Great!", "We have found your account, and you have successfully sign in. Your device is ready to accept real-time notifications.", "Continue");
                    Application.Current.MainPage = new TodaysListPage();
                }
                else if (code == "USER SIGNED IN SUCCESSFULLY AND DEVICE ID WAS NOT REGISTERED SUCCESSFULLY")
                {
                    UserDialogs.Instance.HideLoading();
                    await Application.Current.SavePropertiesAsync();
                    //await DisplayAlert("Great!", "We have found your account, and you have successfully sign in. However, we were not able to register your device to accept real-time notifications.", "Continue");
                    Application.Current.MainPage = new TodaysListPage();
                }
                else if (code == "SIGN IN WITH THE CORRECT VIA SOCIAL MEDIA ACCOUNT")
                {
                    UserDialogs.Instance.HideLoading();
                    await DisplayAlert("Oops", "Please sign in with the correct social media platform and try again", "OK");
                }
                else if (code == "ERROR WHEN CALLING ENDPOINT")
                {
                    UserDialogs.Instance.HideLoading();
                    await DisplayAlert("Oops", "We were not able to successfully retrieve your account. Please contact your advisor.", "OK");
                }
                else if (code == "SOMETHING FAILED IN THE USER VERIFICATION METHOD")
                {
                    UserDialogs.Instance.HideLoading();
                    await DisplayAlert("Oops", "We were not able to successfully retrieve your account. Please contact your advisor.", "OK");
                }
            }
            else
            {
                UserDialogs.Instance.HideLoading();
                await DisplayAlert("Oops", "We were not able to successfully retrieve your account. Please contact your advisor.", "OK");
            }
        }

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

                var client = new SignIn();
                UserDialogs.Instance.ShowLoading("We are processing your request...");
                var authenticationStatus = await client.UserVerification(e, null, "GOOGLE");
                if (authenticationStatus == "EMAIL WAS NOT FOUND")
                {
                    var account = new AppleAccount();
                    account.Email = "ioappletest@gmail.com";
                    account.UserId = "";
                    account.Name = "";
                    account.RealUserStatus = "";
                    account.Token = "";

                    authenticationStatus = await client.UserVerification(null, account, "APPLE");
                }
                ProcessRequest(authenticationStatus);
            }
            else
            {
                //Application.Current.MainPage = new LogInPage();
                //await DisplayAlert("Error", "Google was not able to autheticate your account", "OK");
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

        public async void AppleLogInClick(System.Object sender, System.EventArgs e)
        {
            if(Device.RuntimePlatform != Device.Android)
            {
                OnAppleSignInRequest();
            }
            else
            {
                await DisplayAlert("Oops", "This feature is currently in progress for Android users. We appreciate your patience.", "OK");

            }
        }

        public async void OnAppleSignInRequest()
        {
            try
            {
                IAppleSignInService appleSignInService = DependencyService.Get<IAppleSignInService>();
                var account = await appleSignInService.SignInAsync();

                if (account != null)
                {
                    Preferences.Set(App.LoggedInKey, true);
                    await SecureStorage.SetAsync(App.AppleUserIdKey, account.UserId);
                    string email = "";
                    if(account.Email != null)
                    {
                        await SecureStorage.SetAsync(account.UserId, account.Email);
                        Application.Current.Properties[account.UserId.ToString()] = account.Email;
                    }
                    else
                    {
                        email = await SecureStorage.GetAsync(account.UserId);

                        if (email == null)
                        {
                            if (Application.Current.Properties.ContainsKey(account.UserId.ToString()))
                            {
                                email = (string)Application.Current.Properties[account.UserId.ToString()];
                            }
                            else
                            {
                                email = "";
                            }
                        }
                        //Debug.WriteLine("EMAIL THAT WAS SAVED: " + email);

                        account.Email = email;
                       
                    }

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

                    var client = new SignIn();
                    UserDialogs.Instance.ShowLoading("We are processing your request...");
                    var authenticationStatus = await client.UserVerification(null, account, "APPLE");
                    if (authenticationStatus == "EMAIL WAS NOT FOUND")
                    {
                        account.Email = "ioappletest@gmail.com";
                        account.UserId = "";
                        account.Name = "";
                        account.RealUserStatus = "";
                        account.Token = "";

                        authenticationStatus = await client.UserVerification(null, account, "APPLE");
                    }
                    ProcessRequest(authenticationStatus);


                    //if (account.Email == null)
                    //{
                    //    if (Application.Current.Properties.ContainsKey(account.UserId.ToString()))
                    //    {
                    //        account.Email = (string)Application.Current.Properties[account.UserId.ToString()];
                    //        string url = AppConstants.BaseUrl + AppConstants.addGuid;
                    //        Debug.WriteLine("WRITE GUID: " + url);

                    //        if (Device.RuntimePlatform == Device.iOS)
                    //        {
                    //            deviceId = Preferences.Get("guid", "");
                    //            if (deviceId != null) { Debug.WriteLine("This is the iOS GUID from Log in: " + deviceId); }
                    //        }
                    //        else
                    //        {
                    //            deviceId = Preferences.Get("guid", "");
                    //            if (deviceId != null) { Debug.WriteLine("This is the Android GUID from Log in " + deviceId); }
                    //        }
                    //        if (deviceId != "")
                    //        {
                    //            Application.Current.Properties["guid"] = deviceId.Substring(5);
                    //        }
                    //        else
                    //        {
                    //            Application.Current.Properties["guid"] = "";
                    //        }

                    //        var client = new SignIn();
                    //        UserDialogs.Instance.ShowLoading("We are processing your request...");
                    //        var authenticationStatus = await client.UserVerification(null, account, "APPLE");
                    //        ProcessRequest(authenticationStatus);
                    //    }
                    //    else
                    //    {
                    //        await DisplayAlert("Oops","We were able to find your account. Please go to settings -> Apple ID -> Password & Security -> Apple Using Apple ID -> Manifest My Life -> Stop Using Apple ID","OK");
                    //    }
                    //}
                    //else
                    //{
                    //    Application.Current.Properties[account.UserId.ToString()] = account.Email.ToString();
                    //    string url = AppConstants.BaseUrl + AppConstants.addGuid;
                    //    Debug.WriteLine("WRITE GUID: " + url);

                    //    if (Device.RuntimePlatform == Device.iOS)
                    //    {
                    //        deviceId = Preferences.Get("guid", "");
                    //        if (deviceId != null) { Debug.WriteLine("This is the iOS GUID from Log in: " + deviceId); }
                    //    }
                    //    else
                    //    {
                    //        deviceId = Preferences.Get("guid", "");
                    //        if (deviceId != null) { Debug.WriteLine("This is the Android GUID from Log in " + deviceId); }
                    //    }
                    //    if (deviceId != "")
                    //    {
                    //        Application.Current.Properties["guid"] = deviceId.Substring(5);
                    //    }
                    //    else
                    //    {
                    //        Application.Current.Properties["guid"] = "";
                    //    }

                    //    var client = new SignIn();
                    //    UserDialogs.Instance.ShowLoading("We are processing your request...");
                    //    var authenticationStatus = await client.UserVerification(null, account, "APPLE");
                    //    ProcessRequest(authenticationStatus);
                    //}

                }
                else
                {
                    //AppleError?.Invoke(this, default(EventArgs));
                }
            }
            catch (Exception errorSignInRequest)
            {
                await Application.Current.MainPage.DisplayAlert("Error", errorSignInRequest.Message, "OK");
            }
        }

    }
}
