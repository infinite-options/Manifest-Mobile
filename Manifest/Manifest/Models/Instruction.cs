using System;
using System.ComponentModel;

namespace Manifest.Models
{
    public class Instruction : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public string unique_id { get; set; }
        public string title { get; set; }
        public string at_id { get; set; }
        public int IsSequence { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsComplete { get; set; }
        public bool IsInProgress { get; set; }
        public bool IsTimed { get; set; }
        public string Photo { get; set; }
        public TimeSpan expected_completion_time { get; set; }

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

    public class InstructionDto
    {
        public string unique_id { get; set; }
        public string title { get; set; }
        public string at_id { get; set; }
        public string is_sequence { get; set; }
        public string is_available { get; set; }
        public string is_complete { get; set; }
        public string is_in_progress { get; set; }
        public string is_timed { get; set; }
        public string photo { get; set; }
        public string expected_completion_time { get; set; }
    }
}
