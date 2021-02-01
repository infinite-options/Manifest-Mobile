//This class is used to store Goals and Routines.

using System;
using System.ComponentModel;
namespace Manifest.Models
{
    public class Occurance : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public string Id { get; set; }
        public string Title { get; set; }
        public string UserId { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsComplete { get; set; }
        public bool IsInProgress { get; set; }
        public bool IsDisplayedToday { get; set; }
        public bool IsPersistent { get; set; }
        public bool IsSublistAvailable { get; set; }
        public bool IsTimed { get; set; }
        public string PicUrl { get; set; }
        public DateTime StartDayAndTime { get; set; }
        public DateTime EndDayAndTime { get; set; }
        public bool Repeat { get; set; }
        public string RepeatType { get; set; }
        public DateTime RepeatEndsOn { get; set; }
        public int RepeatOccurences { get; set; }
        public int RepeatEvery { get; set; }
        public string RepeatFrequency { get; set; }
        public RepeatWeekDays RepeatWeekDays { get; set; }
        public DateTime DateTimeStarted { get; set; }
        public DateTime DateTimeCompleted { get; set; }
        public TimeSpan ExpectedCompletionTime { get; set; }
        public string CompletionTime { get; set; }
        public Object Completed { get; set; }
        //Added so that we can convert events to the same datatype
        public bool IsEvent { get; set; }
        public string Description { get; set; }

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
}
