using System;
using System.Collections.Generic;
using System.Text;

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
}
