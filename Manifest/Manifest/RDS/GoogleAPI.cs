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
                    clientId = Constant.GoogleiOSClientID;
                    redirectUri = Constant.GoogleRedirectUrliOS;
                    break;

                case Device.Android:
                    clientId = Constant.GoogleAndroidClientID;
                    redirectUri = Constant.GoogleRedirectUrlAndroid;
                    break;
            }

            var values = new Dictionary<string, string> {
            { "refresh_token", refreshToken},
            { "client_id", clientId },
            { "grant_type", "refresh_token"}
            };



            var content = new FormUrlEncodedContent(values);


            var client = new HttpClient();
            var response = await client.PostAsync(Constant.GoogleAccessTokenUrl, content);
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

                //Repository.Instance.UpdateAccessToken(jsonParsed["access_token"].ToString());
            }
            catch (NullReferenceException e)
            {
                Debug.WriteLine(e.StackTrace);
                //Repository.Instance.ClearSession();
                //await Shell.Current.GoToAsync($"//LoginPage");
                return false;
            }

            Debug.WriteLine("Manifest.Services.Google.Calendar: " + json);
            return true;

        }
    }
}
