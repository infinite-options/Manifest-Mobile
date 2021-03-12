using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Manifest.Config;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Manifest.RDS
{
    public class GoogleAPI
    {
        public static async Task<bool> RefreshToken(string refreshToken)
        {
            string clientId = null;
            string redirectUri = null;

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

            var values = new Dictionary<string, string> {
            { "refresh_token", refreshToken},
            { "client_id", clientId },
            { "grant_type", "refresh_token"}
            };



            var content = new FormUrlEncodedContent(values);


            var client = new HttpClient();
            var response = await client.PostAsync(AppConstants.GoogleAccessTokenUrl, content);
            //var response = responseTask.Result;
            var json = await response.Content.ReadAsStringAsync();

            JObject jsonParsed = JObject.Parse(json);
            Debug.WriteLine("Refrersh token response:\n" + jsonParsed.ToString());
            if (jsonParsed["error"] != null)
            {
                Console.WriteLine(jsonParsed.ToString());
                System.Diagnostics.Trace.WriteLine(jsonParsed.ToString());
                //Repository.Instance.ClearSession();
                //await Shell.Current.GoToAsync($"//LoginPage");
                return false;
            }

            try
            {
                Debug.WriteLine("Manifest.Services.Google.Calendar: Updating new access Token: " + jsonParsed["access_token"].ToString());
                Application.Current.Properties["accessToken"] = jsonParsed["access_token"].ToString();
                if (jsonParsed["refresh_token"] != null)
                {
                    Application.Current.Properties["refreshToken"] = jsonParsed["refresh_token"].ToString();
                }
                //Now, write to database

                var accessInfo = new Dictionary<string, string> {
            { "user_unique_id", (string)Application.Current.Properties["userId"]},
            { "mobile_refresh_token", (string)Application.Current.Properties["refreshToken"] },
            { "mobile_access_token", (string)Application.Current.Properties["accessToken"]}
            };
                var update = JsonConvert.SerializeObject(accessInfo);
                var toSend = new StringContent(update);

                string url = AppConstants.BaseUrl + AppConstants.updateAccessRefresh;

                var res = await client.PostAsync(url, toSend);
                if (res.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Updated the access and refresh tokens");
                }
                else
                {
                    Debug.WriteLine("Was not able to update access and refresh tokens");
                }
            }
            catch (NullReferenceException e)
            {
                Debug.WriteLine(e.StackTrace);
                //Repository.Instance.ClearSession();
                //await Shell.Current.GoToAsync($"//LoginPage");
                return false;
            }

            //Debug.WriteLine("Manifest.Services.Google.Calendar: " + json);
            return true;

        }
    }
}
