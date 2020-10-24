using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Manifest.Config;
using Manifest.Models;
using Manifest.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace Manifest.Services.Google
{
    public class Calendar : ICalendarClient
    {
        HttpClient client = new HttpClient();

        public async Task<bool> UseAccessToken(string accessToken)
        {

            //Make HTTP Request
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(Constants.GoogleUserInfoUrl);
            request.Method = HttpMethod.Get;

            //Format Headers of Request with included Token
            string bearerString = string.Format("Bearer {0}", accessToken);
            request.Headers.Add("Authorization", bearerString);
            request.Headers.Add("Accept", "application/json");
            var client = new HttpClient();
            var task = client.SendAsync(request);
            task.Wait();
            HttpResponseMessage response = task.Result;
            HttpContent content = response.Content;
            var json = await content.ReadAsStringAsync();
            JObject jsonParsed = JObject.Parse(json);

            if (jsonParsed["error"] != null)
                return false;

            return true;
        }


        public async Task<bool> RefreshToken(string refreshToken)
        {
            string clientId = null;
            string redirectUri = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = Constants.GoogleiOSClientID;
                    redirectUri = Constants.GoogleRedirectUrliOS;
                    break;

                case Device.Android:
                    clientId = Constants.GoogleAndroidClientID;
                    redirectUri = Constants.GoogleRedirectUrlAndroid;
                    break;
            }

            var values = new Dictionary<string, string> {
            { "refresh_token", refreshToken},
            { "client_id", clientId },
            { "grant_type", "refresh_token"}
            };

            var content = new FormUrlEncodedContent(values);


            var client = new HttpClient();
            var responseTask = client.PostAsync(Constants.GoogleAccessTokenUrl, content);
            responseTask.Wait();
            var response = responseTask.Result;
            var json = await response.Content.ReadAsStringAsync();

            JObject jsonParsed = JObject.Parse(json);

            if (jsonParsed["error"] != null)
            {
                Console.WriteLine(jsonParsed.ToString());
                System.Diagnostics.Trace.WriteLine(jsonParsed.ToString());
                Repository.Instance.ClearSession();
                await Shell.Current.GoToAsync($"//LoginPage");
                return false;
            }

            try
            {
                Debug.WriteLine("Manifest.Services.Google.Calendar: Updating new access Token: " + jsonParsed["access_token"].ToString());

                Repository.Instance.UpdateAccessToken(jsonParsed["access_token"].ToString());
            }
            catch (NullReferenceException e)
            {
                Debug.WriteLine(e.StackTrace);
                Repository.Instance.ClearSession();
                await Shell.Current.GoToAsync($"//LoginPage");
                return false;
            }

            Debug.WriteLine("Manifest.Services.Google.Calendar: "+json);

            return true;

        }

        public async Task<List<Event>> GetEventsList(DateTimeOffset dateTimeOffset)
        {
            string accessToken = Repository.Instance.GetAccessToken();

            int publicYear = dateTimeOffset.Year;
            int publicMonth = dateTimeOffset.Month;
            int publicDay = dateTimeOffset.Day;

            string timeZoneOffset = dateTimeOffset.ToString();
            string[] timeZoneOffsetParsed = timeZoneOffset.Split('-');
            int timeZoneNum = Int32.Parse(timeZoneOffsetParsed[1].Substring(0, 2));


            //Make HTTP Request
            string baseUri = Constants.GoogleCalendarUrl + "?orderBy=startTime&singleEvents=true&";

            string monthString;
            string dayString;
            string paddedTimeZoneNum;

            //----------  ADD ZERO PADDING AND UTC FIX


            if (timeZoneNum < 10)
            {
                paddedTimeZoneNum = timeZoneNum.ToString().PadLeft(2, '0');

            }
            else
            {
                paddedTimeZoneNum = timeZoneNum.ToString();
            }

            if (publicMonth < 10)
            {
                monthString = publicMonth.ToString().PadLeft(2, '0');

            }
            else
            {
                monthString = publicMonth.ToString();
            }

            if (publicDay < 10)
            {
                dayString = publicDay.ToString().PadLeft(2, '0');

            }
            else
            {
                dayString = publicDay.ToString();
            }

            string timeMaxMin = String.Format("timeMax={0}-{1}-{2}T23%3A59%3A59-{3}%3A00&timeMin={0}-{1}-{2}T00%3A00%3A01-{3}%3A00", publicYear, monthString, dayString, paddedTimeZoneNum);

            string fullURI = baseUri + timeMaxMin;
            string response = "{}";
            try
            {
                response = await MakeEventsRequest(fullURI, accessToken);
            }
            catch (InvalidAccessTokenException e)
            {
                Debug.WriteLine(e);
                if (await RefreshToken(Repository.Instance.GetRefreshToken()))
                {
                    accessToken = Repository.Instance.GetAccessToken();
                    response = await MakeEventsRequest(fullURI, accessToken);
                }
            }
            EventResponse eventResponse = JsonConvert.DeserializeObject<EventResponse>(response);
            List<Event> events = eventResponse.ToEvents();
            return events;
        }


        private async Task<string> MakeEventsRequest(string uri, string accessToken)
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(uri);
            request.Method = HttpMethod.Get;

            //Format Headers of Request with included Token
            string bearerString = string.Format("Bearer {0}", accessToken);
            request.Headers.Add("Authorization", bearerString);
            request.Headers.Add("Accept", "application/json");

            Debug.WriteLine("Manifest.Services.Google.Calendar: Making request to "+ uri);

            Task<HttpResponseMessage> responseTask = client.SendAsync(request);
            responseTask.Wait();
            HttpResponseMessage response = responseTask.Result;
            HttpContent content = response.Content;
            var json = await content.ReadAsStringAsync();
            Debug.WriteLine("Manifest.Services.Google.Calendar.Response:\n" + json);
            JObject jsonParsed = JObject.Parse(json);

            if (jsonParsed.ContainsKey("error"))
            {
                throw new InvalidAccessTokenException();
            }
            return json;
        }
    }
}
