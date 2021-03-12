using System;
namespace Manifest.Models
{
    public class Attendee : Person
    {
        public bool? Organizer { get; set; }
        public bool? Self { get; set; }
        public bool? Optional { get; set; }
        public string ResponseStatus { get; set; }
    }
}
