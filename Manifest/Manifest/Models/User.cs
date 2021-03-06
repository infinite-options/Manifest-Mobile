using System.Collections.Generic;

namespace Manifest.Models
{
    public class User
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public bool HavePic { get; set; }
        public string Id { get; set; }
        public string LastName { get; set; }
        public string MessageCard { get; set; }
        public string MessageDay { get; set; }
        public string MajorEvents { get; set; }
        public string MyHistory { get; set; }
        public string PicUrl { get; set; }
        public List<Person> ImportantPeople { get; set; }
        public TimeSettings TimeSettings { get; set; }
        public string user_birth_date { get; set; }
    }

    public class UserResponse
    {
        public string message { get; set; }
        public List<UserDto> result { get; set; }
    }
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
        public string user_birth_date { get; set; }
        public string user_history { get; set; }
        public string user_major_events { get; set; }

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
    }

}
