using System;
using System.Collections.Generic;
using System.Text;

namespace Manifest.Services
{
    internal class CalendarFactory
    {
        private static readonly Lazy<CalendarFactory> lazy = new Lazy<CalendarFactory>(() => new CalendarFactory());
        private static readonly ICalendarClient googleClient = new Google.Calendar();

        public static CalendarFactory Instance { get { return lazy.Value; } }

        public ICalendarClient GetClient(string name)
        {
            switch (name)
            {
                case "GOOGLE":
                    return googleClient;
                default:
                    return null;
            }
        }
    }
}
