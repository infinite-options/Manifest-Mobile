using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Manifest.Models
{
    public class SubOccurance : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public string Id { get; set; }
        public string Title { get; set; }
        public string GoalRoutineID { get; set; }
        public int AtSequence { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsComplete { get; set; }
        public bool IsInProgress { get; set; }
        public bool IsSublistAvailable { get; set; }
        public bool IsMustDo { get; set; }
        public string PicUrl { get; set; }
        public bool IsTimed { get; set; }
        public DateTime DateTimeCompleted { get; set; }
        public DateTime DateTimeStarted { get; set; }
        public TimeSpan ExpectedCompletionTime { get; set; }
        public DateTime AvailableStartTime { get; set; }
        public DateTime AvailableEndTime { get; set; }

        public void updateIsInProgress(bool updatedVal)
        {
            IsInProgress = updatedVal;
            PropertyChanged(this, new PropertyChangedEventArgs("IsInProgress"));
        }

        public void updateIsComplete(bool updatedVal)
        {
            IsComplete = updatedVal;
            PropertyChanged(this, new PropertyChangedEventArgs("IsComplete"));
        }
    }

    public class SubOccuranceDto
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
    }

    public class SubOccuranceResponse
    {
        public string message { get; set; }
        public List<SubOccuranceDto> result { get; set; }
    }
}
