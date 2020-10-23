using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Manifest.Config;
using Manifest.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Manifest.Services.Google
{
    public class Calendar : ICalendarClient
    {
        HttpClient client = new HttpClient();

        public async Task<List<Event>> GetEventsList(string accessToken, DateTimeOffset dateTimeOffset)
        {

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

            string response = await MakeEventsRequest(fullURI, accessToken);
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
            return json;
        }
    }
}
