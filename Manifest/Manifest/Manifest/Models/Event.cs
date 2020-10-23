using System;
using System.Collections.Generic;

namespace Manifest.Models
{
    public class Event
    {
        public string Title { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string CreatedBy { get; set; }
        public string Description { get; set; }
        public  List <Attendee> Attendees { get; set; }
    }
}
