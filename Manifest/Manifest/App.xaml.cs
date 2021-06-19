using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;
using Manifest.Views;
using Manifest.Config;
using Xamarin.Essentials;
using Manifest.LogIn.Apple;
using Manifest.Models;

namespace Manifest
{
    public partial class App : Application
    {
        public const string LoggedInKey = "LoggedIn";
        public const string AppleUserIdKey = "AppleUserIdKey";
        string userId;

        public App()
        {
            InitializeComponent();

            if (Application.Current.Properties.ContainsKey("userId"))
            {
                if (Application.Current.Properties.ContainsKey("timeStamp"))
                {
                    var time = (DateTime)Application.Current.Properties["timeStamp"];
                    if (time != null)
                    {
                        DateTime today = DateTime.Now;
                        DateTime expTime = (DateTime)Application.Current.Properties["timeStamp"];

                        if (today <= expTime)
                        {
                            //MainPage = new MainPage();
                            MainPage = new TodaysListPage();
                        }
                        else
                        {
                            MainPage = new LogInPage();
                        }
                    }
                    else
                    {
                        MainPage = new LogInPage();
                    }
                }
                else
                {
                    MainPage = new LogInPage();
                }
            }
            else
            {
                MainPage = new LogInPage();
            }
        }

        // Initialization function that checks if a user has logged in through Apple
        protected override async void OnStart()
        {
            try
            {
                var appleSignInService = DependencyService.Get<IAppleSignInService>();

                if (appleSignInService != null)
                {
                    userId = await SecureStorage.GetAsync(AppleUserIdKey);
                    if (appleSignInService.IsAvailable && !string.IsNullOrEmpty(userId))
                    {
                        var credentialState = await appleSignInService.GetCredentialStateAsync(userId);
                        switch (credentialState)
                        {
                            case AppleSignInCredentialState.Authorized:
                                break;
                            case AppleSignInCredentialState.NotFound:
                            case AppleSignInCredentialState.Revoked:
                                SecureStorage.Remove(AppleUserIdKey);
                                Preferences.Set(LoggedInKey, false);
                                MainPage = new LogInPage();
                                break;
                        }
                    }
                }
            }
            catch (Exception resetStorage)
            {
                string error = resetStorage.Message;
                Application.Current.Properties.Clear();   // Resets user info in the app.  Use for debug
                SecureStorage.RemoveAll(); // Resets user info in the app.  Use for debug
                MainPage = new LogInPage();
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {

        }
    }
}
