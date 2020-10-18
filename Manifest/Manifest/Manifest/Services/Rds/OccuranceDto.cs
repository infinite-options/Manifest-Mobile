using Manifest.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Manifest.Services.Rds
{
    class OccuranceDto
    {
        public string gr_unique_id { get; set; }
        public string gr_title { get; set; }
        public string user_id { get; set; }
        public string is_available { get; set; }
        public string is_complete { get; set; }
        public string is_in_progress { get; set; }
        public string is_displayed_today { get; set; }
        public string is_persistent { get; set; }
        public string is_sublist_available { get; set; }
        public string is_timed { get; set; }
        public string photo { get; set; }
        public string start_day_and_time { get; set; }
        public string end_day_and_time { get; set; }
        public string repeat { get; set; }
        public string repeat_type { get; set; }
        public string repeat_ends_on { get; set; }
        public int repeat_occurences { get; set; }
        public int repeat_every { get; set; }
        public string repeat_frequency { get; set; }
        public string repeat_week_days { get; set; }
        public string datetime_started { get; set; }
        public string datetime_completed { get; set; }
        public string expected_completion_time { get; set; }
        public object completed { get; set; }


        public Occurance ToOccurance()
        {
            Occurance occurance = new Occurance();

            occurance.Id = gr_unique_id;
            occurance.Title = gr_title;
            occurance.PicUrl = photo;
            occurance.IsPersistent = DataParser.ToBool(is_persistent);
            occurance.IsInProgress = DataParser.ToBool(is_in_progress);
            occurance.IsComplete = DataParser.ToBool(is_complete);
            occurance.IsSublistAvailable = DataParser.ToBool(is_sublist_available);
            occurance.ExpectedCompletionTime = DataParser.ToTimeSpan(expected_completion_time);
            occurance.DateTimeCompleted = DataParser.ToDateTime(datetime_completed);
            occurance.DateTimeStarted = DataParser.ToDateTime(datetime_started);
            occurance.StartDayAndTime = DataParser.ToDateTime(start_day_and_time);
            occurance.EndDayAndTime = DataParser.ToDateTime(end_day_and_time);
            occurance.Repeat = DataParser.ToBool(repeat);
            occurance.RepeatEvery = repeat_every;
            occurance.RepeatFrequency = repeat_frequency;
            occurance.RepeatType = repeat_type;
            occurance.RepeatOccurences = repeat_occurences;
            if (!string.IsNullOrWhiteSpace(repeat_ends_on)) occurance.RepeatEndsOn = DataParser.ToDateTime(repeat_ends_on);
            occurance.RepeatWeekDays = ParseRepeatWeekDays(repeat_week_days);
            occurance.UserId = user_id;
            return occurance;
        }


        internal RepeatWeekDays ParseRepeatWeekDays(string str)
        {
            try
            {
                RepeatWeekDays repeatWeekDays = JsonConvert.DeserializeObject<RepeatWeekDays>(str);
                return repeatWeekDays;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return null;
        }

        public class UpdateOccuranceDataType
        {
            public string id { get; set; }
            public DateTime datetime_completed { get; set; }
            public DateTime datetime_started { get; set; }
            public bool is_in_progress { get; set; }
            public bool is_complete { get; set; }

        }

        internal static string ToUpdateOccuranceString(Occurance occur)
        {
            var dto = new UpdateOccuranceDataType()
            {
                id = occur.Id,
                datetime_completed = occur.DateTimeCompleted,
                datetime_started = occur.DateTimeStarted,
                is_in_progress = occur.IsInProgress,
                is_complete = occur.IsComplete
            };
            return JsonConvert.SerializeObject(dto);
        }
    }
}
