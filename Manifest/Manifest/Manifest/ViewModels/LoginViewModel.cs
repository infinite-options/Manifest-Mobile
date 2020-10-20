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

namespace Manifest.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command FacebookLoginCommand { get; }
        public Command GoogleLoginCommand { get; }
        public Command AppleLoginCommand { get; }

        public static string accessToken = null;
        public static string refreshToken = null;

        public LoginViewModel()
        {
            FacebookLoginCommand = new Command(OnFacebookClicked);
            GoogleLoginCommand = new Command(OnGoogleClicked);
            AppleLoginCommand = new Command(OnAppleClicked);
        }

        public async void OnFacebookClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            // await Shell.Current.GoToAsync($"//{nameof(TodaysList)}");
            // Console.WriteLine("YOU HAVE CLICKED THE FACEBOOK SIGN IN BUTTON");
            
        }

        public async void OnGoogleClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            // await Shell.Current.GoToAsync($"//{nameof(TodaysList)}");
            // Console.WriteLine("YOU HAVE CLICKED THE GOOGLE SIGN IN BUTTON");

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
                if (accessToken == null && refreshToken == null)
                {
                    accessToken = e.Account.Properties["access_token"];
                    refreshToken = e.Account.Properties["refresh_token"];

                    Application.Current.Properties["access_token"] = accessToken;
                    Application.Current.Properties["refresh_token"] = refreshToken;

                    GoogleUserProfileAsync(accessToken, refreshToken, e);
                }
                else if (!refreshToken.Equals(e.Account.Properties["refresh_token"]) && !accessToken.Equals(e.Account.Properties["access_token"]))
                {
                    DateTime today = DateTime.Now;
                    DateTime expirationDate = today.AddDays(Constant.days);
                    Application.Current.Properties["time_stamp"] = expirationDate;

                    accessToken = e.Account.Properties["access_token"];
                    refreshToken = e.Account.Properties["refresh_token"];

                    UpdateTokensPost updateTokens = new UpdateTokensPost();
                    updateTokens.access_token = accessToken;
                    updateTokens.refresh_token = refreshToken;
                    updateTokens.uid = (string)Application.Current.Properties["user_id"];
                    updateTokens.social_timestamp = expirationDate.ToString("yyyy-MM-dd HH:mm:ss");

                    var updatePostSerilizedObject = JsonConvert.SerializeObject(updateTokens);
                    var updatePostContent = new StringContent(updatePostSerilizedObject, Encoding.UTF8, "application/json");

                    var client = new HttpClient();
                    var RDSrespose = await client.PostAsync(Constant.UpdateTokensUrl, updatePostContent);

                    if (RDSrespose.IsSuccessStatusCode)
                    {
                        GoogleUserProfileAsync(accessToken, refreshToken, e);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Unable to update google tokens");
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error", "Google was not able to autheticate your account", "OK");
            }
        }

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
            socialLogInPost.password = "";
            socialLogInPost.token = refreshToken;
            socialLogInPost.signup_platform = "GOOGLE";

            var socialLogInPostSerialized = JsonConvert.SerializeObject(socialLogInPost);
            var postContent = new StringContent(socialLogInPostSerialized, Encoding.UTF8, "application/json");

            System.Diagnostics.Debug.WriteLine(socialLogInPostSerialized);

            var RDSResponse = await client.PostAsync(Constant.LogInUrl, postContent);
            var responseContent = await RDSResponse.Content.ReadAsStringAsync();

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
                    if (responseContent.Contains(Constant.AutheticatedSuccesful))
                    {
                        System.Diagnostics.Debug.WriteLine("WE WERE ABLE TO FIND YOUR ACOUNT IN OUR DATABASE");

                        Application.Current.Properties["user_id"] = session.result[0].user_unique_id;
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

            System.Diagnostics.Debug.WriteLine(e.Message);
        }

        public async void OnAppleClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            // await Shell.Current.GoToAsync($"//{nameof(TodaysList)}");
            // Console.WriteLine("YOU HAVE CLICKED THE APPLE SIGN IN BUTTON");
        }
    }
}
