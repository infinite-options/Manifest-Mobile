using Manifest.Models;
using System;
using System.Diagnostics;

namespace Manifest.Services.Rds
{
    public class UserDto
    {
        // User Data
        public string user_have_pic { get; set; }
        public string message_card { get; set; }
        public string message_day { get; set; }
        public string user_picture { get; set; }
        public string user_first_name { get; set; }
        public string user_last_name { get; set; }
        public string user_email_id { get; set; }
        public string evening_time { get; set; }
        public string morning_time { get; set; }
        public string afternoon_time { get; set; }
        public string night_time { get; set; }
        public string day_end { get; set; }
        public string day_start { get; set; }
        public string time_zone { get; set; }

        // Important People or TA details below
        public string ta_people_id { get; set; }
        public string email_id { get; set; }
        public string people_name { get; set; }
        public string have_pic { get; set; }
        public string ta_picture { get; set; }
        public string important { get; set; }
        public string user_unique_id { get; set; }
        public string relation_type { get; set; }
        public string ta_phone { get; set; }

        internal User ToUser()
        {
            if (user_email_id == null) { return null; }

            return new User()
            {
                FirstName = user_first_name,
                LastName = user_last_name,
                Email = user_email_id,
                HavePic = DataParser.ToBool(have_pic),
                PicUrl = user_picture,
                MessageCard = message_card,
                MessageDay = message_day,
                TimeSettings = ToTimeSettings()
            };
        }


        internal Person ToPerson()
        {
            if (people_name == null || ta_people_id == null) { return null; }

            return new Person()
            {
                Name = people_name,
                Relation = relation_type,
                PicUrl = ta_picture,
                Id = ta_people_id,
                PhoneNumber = ta_phone
            };
        }

        internal Person ToPerson(string userId)
        {
            Person person = ToPerson();
            person.UserId = userId;
            return person;
        }

        internal TimeSettings ToTimeSettings()
        {
            TimeSettings timeSettings = new TimeSettings();
            timeSettings.TimeZone = time_zone;
            timeSettings.MorningStartTime = DataParser.ToTimeSpan(morning_time);
            timeSettings.AfterNoonStartTime = DataParser.ToTimeSpan(afternoon_time);
            timeSettings.EveningStartTime = DataParser.ToTimeSpan(evening_time);
            timeSettings.NightStartTime = DataParser.ToTimeSpan(night_time);
            timeSettings.DayStart = DataParser.ToTimeSpan(day_start);
            timeSettings.DayEnd = DataParser.ToTimeSpan(day_end);
            return timeSettings;
        }
    }
}