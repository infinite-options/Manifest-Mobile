//This class is used to store what days a Routine/Goal is available. Each Occurance has a RepeatWeekDays property, to show what days
//a task is available on

using System;
namespace Manifest.Models
{
    public class RepeatWeekDays
    {
        public bool Friday { get; set; }
        public bool Monday { get; set; }
        public bool Sunday { get; set; }
        public bool Tuesday { get; set; }
        public bool Saturday { get; set; }
        public bool Thursday { get; set; }
        public bool Wednesday { get; set; }
    }
}
