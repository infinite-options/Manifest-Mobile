using Manifest.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Manifest.Login.Constants;
using Xamarin.Auth;
using Manifest.Login.Classes;
using Newtonsoft.Json;
using System.Net.Http;
using System.Windows.Input;
using Manifest.Models;
using Manifest.Login.Apple;
using Xamarin.Essentials;
using Manifest.Services;
using System.Diagnostics;

namespace Manifest.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command FacebookLoginCommand { get; }
        public Command GoogleLoginCommand { get; }
        public ICommand AppleLoginCommand { get; set; }

        public static string accessToken = null;
        public static string refreshToken = null;

        public static string appleUserEmail;
        public static string appleToken = null;

        public bool IsAppleSignInAvailable { get { return appleSignInService?.IsAvailable ?? false; } }
        public event EventHandler AppleError = delegate { };

        IAppleSignInService appleSignInService;

        //Adding a Repository object to push guid and uid to database as soon as login is verified
        private Repository Repository = Repository.Instance;

        public LoginViewModel()
        {
            FacebookLoginCommand = new Command(OnFacebookClicked);
            GoogleLoginCommand = new Command(OnGoogleClicked);

            if(Device.RuntimePlatform == Device.Android)
            {
                OnAndroidApple();
            }
            else
            {
                appleSignInService = DependencyService.Get<IAppleSignInService>();
                AppleLoginCommand = new Command(OnAppleClicked);
            }
        }

        public async void OnAndroidApple()
        {
            await Shell.Current.DisplayAlert("Service", "Apple log in is not available in this platform yet. Please use Google or Facebook to log in!", "OK");
            System.Diagnostics.Debug.WriteLine("Apple okay registered");
        }

        public async void OnFacebookClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            // await Shell.Current.GoToAsync($"//{nameof(TodaysList)}");
            // Console.WriteLine("YOU HAVE CLICKED THE FACEBOOK SIGN IN BUTTON");
            string clientID = string.Empty;
            string redirectURL = string.Empty;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientID = Constant.FacebookiOSClientID;
                    redirectURL = Constant.FacebookiOSRedirectUrl;
                    break;
                case Device.Android:
                    clientID = Constant.FacebookAndroidClientID;
                    redirectURL = Constant.FacebookAndroidRedirectUrl;
                    break;
            }

            var authenticator = new OAuth2Authenticator(clientID, Constant.FacebookScope, new Uri(Constant.FacebookAuthorizeUrl), new Uri(redirectURL), null, false);
            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();

            authenticator.Completed += FacebookAuthenticatorCompleted;
            authenticator.Error += FacebookAutheticatorError;

            presenter.Login(authenticator);

        }

        public async void OnGoogleClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            // await Shell.Current.GoToAsync($"//{nameof(TodaysList)}");
            // Console.WriteLine("YOU HAVE CLICKED THE GOOGLE SIGN IN BUTTON");
            System.Diagnostics.Debug.WriteLine("Google clicked");
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


        public async void FacebookAuthenticatorCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;

            if (authenticator != null)
            {
                authenticator.Completed -= FacebookAuthenticatorCompleted;
                authenticator.Error -= FacebookAutheticatorError;
            }

            if (e.IsAuthenticated)
            {
                System.Diagnostics.Debug.WriteLine("FACEBOOK TOKEN");
                System.Diagnostics.Debug.WriteLine("ACCESS_TOKEN: " + (string)e.Account.Properties["access_token"]);

                FacebookUserProfileAsync((string)e.Account.Properties["access_token"]);
            }
            else
            {
                await Shell.Current.DisplayAlert("Message", "Facebook was not able to authenticate your account", "OK");
            }
        }

        public async void FacebookUserProfileAsync(string accessToken)
        {
            // MECHANISM:

            // 1. RETRIVE TOKEN FROM SOCIAL LOGIN
            // 2. PASS THIS INFORMATION TO PARVA
            // 3. WAIT FOR A RESPONSE
            // 4. BASED ON THE RESPONSE I WOULD NEED TO REDIRECT THE USER TO THE CORRECT PAGE

            var client = new HttpClient();
            var socialLogInPost = new SocialLogInPost();

            var facebookResponse = client.GetStringAsync(Constant.FacebookUserInfoUrl + accessToken);
            var userData = facebookResponse.Result;

            FacebookResponse facebookData = JsonConvert.DeserializeObject<FacebookResponse>(userData);

            socialLogInPost.email = facebookData.email;
            socialLogInPost.social_id = facebookData.id;
            socialLogInPost.mobile_access_token = accessToken;
            socialLogInPost.mobile_refresh_token = "";
            socialLogInPost.signup_platform = "FACEBOOK";

            var socialLogInPostSerialized = JsonConvert.SerializeObject(socialLogInPost);
            var postContent = new StringContent(socialLogInPostSerialized, Encoding.UTF8, "application/json");

            System.Diagnostics.Debug.WriteLine(socialLogInPostSerialized);

            var RDSResponse = await client.PostAsync(Constant.LogInUrl, postContent);
            var responseContent = await RDSResponse.Content.ReadAsStringAsync();
            Session session = JsonConvert.DeserializeObject<Session>(responseContent);
            System.Diagnostics.Debug.WriteLine(responseContent);
            System.Diagnostics.Debug.WriteLine(RDSResponse.IsSuccessStatusCode);

            if (responseContent != null)
            {
                if (responseContent.Contains(Constant.EmailNotFound))
                {
                    System.Diagnostics.Debug.WriteLine("HERE IS WHERE WE NEED TO PUT A DISPLAY ALERT MESSAGE");
                    Console.WriteLine(responseContent);

                    string message = "It looks like you don't have an account yet. Please contact your personal TA to create your account!";
                    await Shell.Current.DisplayAlert("Message", message, "OK");
                }
                else if (responseContent.Contains(Constant.AutheticatedSuccesful))
                {
                    System.Diagnostics.Debug.WriteLine("WE WERE ABLE TO FIND YOUR ACOUNT IN OUR DATABASE");
                    Repository.Instance.SaveSession(session);
                    Application.Current.Properties["user_id"] = session.result[0].user_unique_id;
                    //Before navigating to the TodaysList, store the guid and uid
                    //await Repository.storeGUID(Preferences.Get("guid", ""), session.result[0].user_unique_id);
                    await Repository.storeGUID(GlobalVars.user_guid, session.result[0].user_unique_id);
                    await Shell.Current.GoToAsync($"//{nameof(TodaysList)}");
                }
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

            await Shell.Current.DisplayAlert("Authentication error: ", e.Message, "OK");
        }

        private async void GoogleAuthenticatorCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;

            if (authenticator != null)
            {
                authenticator.Completed -= GoogleAuthenticatorCompleted;
                authenticator.Error -= GoogleAuthenticatorError;
            }

            if (e.IsAuthenticated)
            {
                System.Diagnostics.Debug.WriteLine("GOOGLE TOKENS");
                System.Diagnostics.Debug.WriteLine("ACCESS_TOKEN: " + (string)e.Account.Properties["access_token"]);
                System.Diagnostics.Debug.WriteLine("ACCESS_REFRESH_TOKEN: " +(string)e.Account.Properties["refresh_token"]);

                GoogleUserProfileAsync((string)e.Account.Properties["access_token"], (string)e.Account.Properties["refresh_token"], e);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error", "Google was not able to autheticate your account", "OK");
            }
        }

        //Routing done in this function
        public async void GoogleUserProfileAsync(string accessToken, string refreshToken, AuthenticatorCompletedEventArgs e)
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
            System.Diagnostics.Debug.WriteLine("responseContent");
            Console.WriteLine(responseContent);
            Session session = JsonConvert.DeserializeObject<Session>(responseContent);

            System.Diagnostics.Debug.WriteLine(responseContent);
            System.Diagnostics.Debug.WriteLine(RDSResponse.IsSuccessStatusCode);

            if (RDSResponse.IsSuccessStatusCode)
            {
                if (responseContent != null)
                {
                    if (responseContent.Contains(Constant.EmailNotFound))
                    {
                        System.Diagnostics.Debug.WriteLine("HERE IS WHERE WE NEED TO PUT A DISPLAY ALERT MESSAGE");

                        string message = "It looks like you don't have an account yet. Please contact your persona TA to create your account!";
                        await Shell.Current.DisplayAlert("Message", message, "OK");
                    }
                    else if (responseContent.Contains(Constant.AutheticatedSuccesful))
                    {
                        System.Diagnostics.Debug.WriteLine("WE WERE ABLE TO FIND YOUR ACOUNT IN OUR DATABASE");
                        Repository.Instance.SaveSession(session);
                        Application.Current.Properties["user_id"] = session.result[0].user_unique_id;

                        //Before navigating to the TodaysList, store the guid and uid
                        //await Repository.storeGUID(Preferences.Get("guid", ""), session.result[0].user_unique_id);
                        await Repository.storeGUID(GlobalVars.user_guid, session.result[0].user_unique_id);
                        await Shell.Current.GoToAsync($"//{nameof(TodaysList)}");
                    }

                }
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

            await Shell.Current.DisplayAlert("Authentication error: ", e.Message, "OK");
        }

        public async void OnAppleClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            // await Shell.Current.GoToAsync($"//{nameof(TodaysList)}");
            // Console.WriteLine("YOU HAVE CLICKED THE APPLE SIGN IN BUTTON");
            var account = await appleSignInService.SignInAsync();
            if (account != null)
            {
                Preferences.Set(SplashScreenViewModel.LoggedInKey, true);
                await SecureStorage.SetAsync(SplashScreenViewModel.AppleUserIdKey, account.UserId);

                // System.Diagnostics.Debug.WriteLine($"Signed in!\n  Name: {account?.Name ?? string.Empty}\n  Email: {account?.Email ?? string.Empty}\n  UserId: {account?.UserId ?? string.Empty}");

                System.Diagnostics.Debug.WriteLine("APPLE TOKENS");
                System.Diagnostics.Debug.WriteLine("APPLE_EMAIL: " +(string)account.Email);
                System.Diagnostics.Debug.WriteLine("APPLE_USERID: " + (string)account.UserId);
                System.Diagnostics.Debug.WriteLine("APPLE_TOKEN: " + (string)account.Token);

                if (account.Email == null) { account.Email = ""; }
                if (account.Token == null) { account.Token = ""; }

                AppleUserProfileAsync(account.Email, account.UserId, account.Token);

            }
            else
            {
                await Shell.Current.DisplayAlert("Alert","It looks like you don't have an account with Apple","OK");
            }
        }

        public async void AppleUserProfileAsync(string appleEmail, string appleId, string appleToken)
        {
            System.Diagnostics.Debug.WriteLine("LINE 95");
            var client = new HttpClient();
            var socialLogInPost = new SocialLogInPost();

            socialLogInPost.email = appleEmail;
            socialLogInPost.social_id = appleId;
            socialLogInPost.mobile_access_token = appleToken;
            socialLogInPost.mobile_refresh_token = "";
            socialLogInPost.signup_platform = "APPLE";

            var socialLogInPostSerialized = JsonConvert.SerializeObject(socialLogInPost);

            System.Diagnostics.Debug.WriteLine(socialLogInPostSerialized);
            await Shell.Current.DisplayAlert("Alert", socialLogInPostSerialized, "OK");

            var postContent = new StringContent(socialLogInPostSerialized, Encoding.UTF8, "application/json");
            var RDSResponse = await client.PostAsync(Constant.LogInUrl, postContent);
            var responseContent = await RDSResponse.Content.ReadAsStringAsync();

            Session session = JsonConvert.DeserializeObject<Session>(responseContent);
            await Shell.Current.DisplayAlert("Alert", responseContent, "OK");
            System.Diagnostics.Debug.WriteLine(responseContent);
            if (RDSResponse.IsSuccessStatusCode)
            {
                if (responseContent != null)
                {
                    if (responseContent.Contains(Constant.EmailNotFound))
                    {
                        // Application.Current.MainPage = new SocialLogInSignUp(userName, "", appleUserEmail, appleToken, appleToken, "APPLE");
                        string message = "It looks like you don't have an account yet. Please contact your persona TA to create your account!";
                        await Shell.Current.DisplayAlert("Message", message, "OK");
                    }
                    if (responseContent.Contains(Constant.AutheticatedSuccesful))
                    {
                        //Application.Current.MainPage = new HomePage();
                        Application.Current.Properties["user_id"] = session.result[0].user_unique_id;
                        Repository.Instance.SaveSession(session);

                        //Before navigating to the TodaysList, store the guid and uid
                        //await Repository.storeGUID(Preferences.Get("guid", ""), session.result[0].user_unique_id);
                        await Repository.storeGUID(GlobalVars.user_guid, session.result[0].user_unique_id);
                        await Shell.Current.GoToAsync($"//{nameof(TodaysList)}");
                    }
                }
            }
        }
    }
}
