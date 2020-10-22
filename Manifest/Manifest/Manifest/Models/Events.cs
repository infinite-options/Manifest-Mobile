using System;
using System.Collections.Generic;

namespace Manifest.Models
{
    public class Events
    {
        public string Title { get; set; }
        public DateTime Time { get; set; }
        public string CreatedBy { get; set; }
        public string Description { get; set; }
        public  List <Person> Attendees { get; set; }
    }
}
