using System;
using System.Collections.Generic;

namespace Manifest.Models
{

    public class Session
    {
        public string message { get; set; }
        public int code { get; set; }
        public IList<Result> result { get; set; }
    }

    public class Result
    {
        public string user_unique_id { get; set; }
        public string user_timestamp { get; set; }
        public string user_email_id { get; set; }
        public string user_first_name { get; set; }
        public string user_last_name { get; set; }
        public string user_have_pic { get; set; }
        public object message_card { get; set; }
        public object message_day { get; set; }
        public string user_picture { get; set; }
        public string google_auth_token { get; set; }
        public string google_refresh_token { get; set; }
        public string mobile_auth_token { get; set; }
        public string mobile_refresh_token { get; set; }
        public string morning_time { get; set; }
        public string afternoon_time { get; set; }
        public string evening_time { get; set; }
        public string night_time { get; set; }
        public string day_start { get; set; }
        public string day_end { get; set; }
        public string time_zone { get; set; }
        public object user_social_media { get; set; }
    }
}
