using System;
using System.Collections.Generic;
using System.Text;

namespace Manifest.Models
{
    public class SubOccurance
    {
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
    }
}
