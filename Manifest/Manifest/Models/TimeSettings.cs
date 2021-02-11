using System;
using System.Collections.Generic;

namespace Manifest.Models
{
    public class TimeSettings
    {
        public TimeSpan MorningStartTime { get; set; }
        public TimeSpan AfterNoonStartTime { get; set; }
        public TimeSpan EveningStartTime { get; set; }
        public TimeSpan NightStartTime { get; set; }
        public string TimeZone { get; set; }
        public TimeSpan DayStart { get; set; }
        public TimeSpan DayEnd { get; set; }
    }

    public class Times
    {
        public string evening_time {get; set;}
        public string morning_time { get; set; }
        public string afternoon_time { get; set; }
        public string night_time { get; set; }
        public string day_end { get; set; }
        public string day_start { get; set; }
        public string time_zone { get; set; }
    }

}
