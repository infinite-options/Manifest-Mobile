using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Manifest.Models
{
    public class Event
    {
        public string Title { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string CreatedBy { get; set; }
        public string Description { get; set; }
        public List<Attendee> Attendees { get; set; }
        public string Id { get; set; }
    }

    //This class is used to read in the json response from the google endpoint
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
            if (Items == null)
            {
                return events;
            }
            foreach (EventDto dto in Items)
            {
                events.Add(dto.ToEvent());
            }
            return events;
        }
    }

    //Used to convert from EventResponse to Event
    public class EventDto
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("etag")]
        public string Etag { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("htmlLink", NullValueHandling = NullValueHandling.Ignore)]
        public string HtmlLink { get; set; }

        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset Created { get; set; }

        [JsonProperty("updated", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset Updated { get; set; }

        [JsonProperty("summary", NullValueHandling = NullValueHandling.Ignore)]
        public string EventName { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public string Location { get; set; }

        [JsonProperty("colorId", NullValueHandling = NullValueHandling.Ignore)]
        public string ColorID { get; set; }

        [JsonProperty("creator", NullValueHandling = NullValueHandling.Ignore)]
        public EventsCreator Creator { get; set; }

        [JsonProperty("organizer", NullValueHandling = NullValueHandling.Ignore)]
        public EventsCreator Organizer { get; set; }

        [JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
        public EventsEnd Start { get; set; }

        [JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
        public EventsEnd End { get; set; }

        [JsonProperty("iCalUID", NullValueHandling = NullValueHandling.Ignore)]
        public string ICalUid { get; set; }

        [JsonProperty("sequence", NullValueHandling = NullValueHandling.Ignore)]
        public long? Sequence { get; set; }

        [JsonProperty("attendees", NullValueHandling = NullValueHandling.Ignore)]
        public List<EventsAttendee> Attendees { get; set; }

        [JsonProperty("hangoutLink", NullValueHandling = NullValueHandling.Ignore)]
        public Uri HangoutLink { get; set; }

        [JsonProperty("conferenceData", NullValueHandling = NullValueHandling.Ignore)]
        public EventsConferenceData ConferenceData { get; set; }

        [JsonProperty("guestsCanModify", NullValueHandling = NullValueHandling.Ignore)]
        public bool? GuestsCanModify { get; set; }

        [JsonProperty("reminders", NullValueHandling = NullValueHandling.Ignore)]
        public EventsReminders Reminders { get; set; }

        [JsonProperty("recurrence", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Recurrence { get; set; }

        [JsonProperty("recurringEventId", NullValueHandling = NullValueHandling.Ignore)]
        public string RecurringEventId { get; set; }

        [JsonProperty("originalStartTime", NullValueHandling = NullValueHandling.Ignore)]
        public EventsOriginalStartTime OriginalStartTime { get; set; }

        public Event ToEvent()
        {
            Event _event = new Event()
            {
                Id = Id,
                Description = Description,
                Title = EventName,
                StartTime = Start.DateTime,
                EndTime = End.DateTime
            };

            List<Attendee> attendees = new List<Attendee>();
            if (Attendees != null)
            {
                foreach (EventsAttendee eventsAttendee in Attendees)
                {
                    attendees.Add(eventsAttendee.ToAttendee());
                }
            }
            _event.Attendees = attendees;

            return _event;
        }

    }

    public class EventsDefaultReminder
    {
        [JsonProperty("method")]
        public string method { get; set; }

        [JsonProperty("minutes")]
        public int minutes { get; set; }
    }

    public class EventsAttendee
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("displayName", NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayName { get; set; }

        [JsonProperty("organizer", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Organizer { get; set; }

        [JsonProperty("responseStatus")]
        public string ResponseStatus { get; set; }

        [JsonProperty("self", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Self { get; set; }

        [JsonProperty("resource", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Resource { get; set; }

        [JsonProperty("optional", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Optional { get; set; }

        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string Comment { get; set; }

        public Attendee ToAttendee()
        {
            Attendee attendee = new Attendee()
            {
                Name = DisplayName,
                Organizer = Organizer,
                Email = Email,
                Self = Self,
                ResponseStatus = ResponseStatus
            };

            return attendee;
        }
    }

    public class EventsConferenceData
    {
        [JsonProperty("entryPoints")]
        public EventsEntryPoint[] EntryPoints { get; set; }

        [JsonProperty("conferenceSolution")]
        public EventsConferenceSolution ConferenceSolution { get; set; }

        [JsonProperty("conferenceId", NullValueHandling = NullValueHandling.Ignore)]
        public string ConferenceId { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("createRequest", NullValueHandling = NullValueHandling.Ignore)]
        public EventsCreateRequest CreateRequest { get; set; }
    }

    public class EventsConferenceSolution
    {
        [JsonProperty("key")]
        public EventsKey Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("iconUri")]
        public string IconUri { get; set; }
    }

    public class EventsKey
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class EventsCreateRequest
    {
        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("conferenceSolutionKey")]
        public EventsKey ConferenceSolutionKey { get; set; }

        [JsonProperty("status")]
        public EventsStatusClass Status { get; set; }
    }

    public class EventsStatusClass
    {
        [JsonProperty("statusCode")]
        public string StatusCode { get; set; }
    }

    public class EventsEntryPoint
    {
        [JsonProperty("entryPointType")]
        public string EntryPointType { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }

        [JsonProperty("pin", NullValueHandling = NullValueHandling.Ignore)]
        public string Pin { get; set; }

        [JsonProperty("regionCode", NullValueHandling = NullValueHandling.Ignore)]
        public string RegionCode { get; set; }
    }

    public class EventsCreator
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("displayName", NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayName { get; set; }

        [JsonProperty("self", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Self { get; set; }
    }

    public class EventsEnd
    {
        [JsonProperty("dateTime")]
        public DateTimeOffset DateTime { get; set; }

        [JsonProperty("timeZone", NullValueHandling = NullValueHandling.Ignore)]
        public string TimeZone { get; set; }
    }

    public class EventsOriginalStartTime
    {
        [JsonProperty("dateTime")]
        public DateTimeOffset DateTime { get; set; }
    }

    public class EventsReminders
    {
        [JsonProperty("useDefault")]
        public bool UseDefault { get; set; }
    }
}
