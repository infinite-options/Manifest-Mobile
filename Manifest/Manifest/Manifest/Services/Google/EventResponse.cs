using Manifest.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manifest.Services.Google
{
    public partial class EventResponse
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("etag")]
        public string Etag { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("updated")]
        public DateTime Updated { get; set; }

        [JsonProperty("timeZone")]
        public string TimeZone { get; set; }

        [JsonProperty("accessRole")]
        public string AccessRole { get; set; }

        [JsonProperty("defaultReminders")]
        public List<EventsDefaultReminder> DefaultReminders { get; set; }

        [JsonProperty("nextPageToken")]
        public string NextPageToken { get; set; }

        [JsonProperty("nextSyncToken")]
        public string NextSyncToken { get; set; }

        [JsonProperty("items")]
        public List<EventDto> Items { get; set; }


        public List<Event> ToEvents()
        {
            List<Event> events = new List<Event>();
            foreach(EventDto dto in Items)
            {
                events.Add(dto.ToEvent());
            }
            return events;
        }
    }
}
