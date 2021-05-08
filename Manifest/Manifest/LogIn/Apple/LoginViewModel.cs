using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Manifest.Config;
using Manifest.Views;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using Manifest.Models;
using System.Diagnostics;
using System.Collections.Generic;

namespace Manifest.LogIn.Apple
{
    public class Info
    {
        public string customer_email { get; set; }
    }

    public class AppleUser
    {
        public string message { get; set; }
        public int code { get; set; }
        public IList<Info> result { get; set; }
        public string sql { get; set; }
    }

    public class AppleEmail
    {
        public string social_id { get; set; }
    }

    public class LoginViewModel
    {

        public static string apple_token = null;
        public static string apple_email = null;

        public bool IsAppleSignInAvailable { get { return appleSignInService?.IsAvailable ?? false; } }
        public ICommand SignInWithAppleCommand { get; set; }

        public event EventHandler AppleError = delegate { };

        IAppleSignInService appleSignInService = null;
        private string deviceId;

        public LoginViewModel()
        {
            appleSignInService = DependencyService.Get<IAppleSignInService>();
            SignInWithAppleCommand = new Command(OnAppleSignInRequest);
            if (Device.RuntimePlatform == Device.iOS)
            {
                deviceId = Preferences.Get("guid", null);
                if (deviceId != null) { Debug.WriteLine("This is iOS guid: " + deviceId); }
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

                    if (account.Token == null) { account.Token = ""; }
                    if (account.Email != null)
                    {
                        if (Application.Current.Properties.ContainsKey(account.UserId.ToString()))
                        {
                            //Application.Current.Properties[account.UserId.ToString()] = account.Email;
                            Debug.WriteLine((string)Application.Current.Properties[account.UserId.ToString()]);
                        }
                        else
                        {
                            Application.Current.Properties[account.UserId.ToString()] = account.Email;
                        }
                    }
                    if (account.Email == null) { account.Email = ""; }
                    if (account.Name == null) { account.Name = ""; }

                    if (Application.Current.Properties.ContainsKey(account.UserId.ToString()))
                    {
                        account.Email = (string)Application.Current.Properties[account.UserId.ToString()];
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
                        //Application.Current.MainPage = new MainPage(null, account, "APPLE");

                        var mainPage = new MainPage(null, account, "APPLE");

                        Application.Current.MainPage = new TodaysListPage();
                    }
                    else
                    {
                        var client = new HttpClient();
                        //var getAppleEmail = new AppleEmail();
                        //getAppleEmail.social_id = account.UserId;

                        //var socialLogInPostSerialized = JsonConvert.SerializeObject(getAppleEmail);

                        //System.Diagnostics.Debug.WriteLine(socialLogInPostSerialized);

                        //var postContent = new StringContent(socialLogInPostSerialized, Encoding.UTF8, "application/json");
                        //var RDSResponse = await client.PostAsync("https://tsx3rnuidi.execute-api.us-west-1.amazonaws.com/dev/api/v2/AppleEmail", postContent);
                        var userId = (string)Application.Current.Properties["userId"];
                        var RDSResponse = await client.GetAsync(AppConstants.BaseUrl + AppConstants.appleEmail + userId);
                        var responseContent = await RDSResponse.Content.ReadAsStringAsync();

                        System.Diagnostics.Debug.WriteLine(responseContent);
                        if (RDSResponse.IsSuccessStatusCode)
                        {
                            var data = JsonConvert.DeserializeObject<AppleUser>(responseContent);
                            Application.Current.Properties[account.UserId.ToString()] = data.message;
                            account.Email = (string)Application.Current.Properties[account.UserId.ToString()];
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
                            //Application.Current.MainPage = new MainPage(null, account, "APPLE");

                            var mainPage = new MainPage(null, account, "APPLE");

                            Application.Current.MainPage = new TodaysListPage();
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Ooops", "Our system is not working. We can't process your request at this moment", "OK");
                        }
                    }
                }
                else
                {
                    AppleError?.Invoke(this, default(EventArgs));
                }
            }
            catch (Exception apple)
            {
                await Application.Current.MainPage.DisplayAlert("Error", apple.Message, "OK");
            }
        }
    }
}
