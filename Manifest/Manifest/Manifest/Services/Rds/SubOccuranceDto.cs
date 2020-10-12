using System;
using System.Collections.Generic;
using System.Text;
using Manifest.Models;

namespace Manifest.Services.Rds
{
    class SubOccuranceDto
    {
        public string at_unique_id { get; set; }
        public string at_title { get; set; }
        public string goal_routine_id { get; set; }
        public int at_sequence { get; set; }
        public string is_available { get; set; }
        public string is_complete { get; set; }
        public string is_in_progress { get; set; }
        public string is_sublist_available { get; set; }
        public string is_must_do { get; set; }
        public string photo { get; set; }
        public string is_timed { get; set; }
        public string datetime_completed { get; set; }
        public string datetime_started { get; set; }
        public string expected_completion_time { get; set; }
        public string available_start_time { get; set; }
        public string available_end_time { get; set; }

        public SubOccurance ToSubOccurances()
        {
            SubOccurance Suboccurance = new SubOccurance();
            Suboccurance.Id = at_unique_id;
            Suboccurance.Title = at_title;
            Suboccurance.GoalRoutineID = goal_routine_id;
            Suboccurance.AtSequence = at_sequence;
            Suboccurance.IsAvailable = DataParser.ToBool(is_available);
            Suboccurance.IsComplete = DataParser.ToBool(is_complete);
            Suboccurance.IsInProgress = DataParser.ToBool(is_in_progress);
            Suboccurance.IsSublistAvailable = DataParser.ToBool(is_sublist_available);
            Suboccurance.IsMustDo = DataParser.ToBool(is_must_do);
            Suboccurance.PicUrl = photo;
            Suboccurance.IsTimed = DataParser.ToBool(is_timed);
            Suboccurance.DateTimeCompleted = DataParser.ToDateTime(datetime_completed);
            Suboccurance.DateTimeStarted = DataParser.ToDateTime(datetime_started);
            Suboccurance.ExpectedCompletionTime = DataParser.ToTimeSpan(expected_completion_time);
            Suboccurance.AvailableStartTime = DataParser.ToDateTime(available_start_time);
            Suboccurance.AvailableEndTime = DataParser.ToDateTime(available_end_time);

            return Suboccurance;
        }
    }
}
