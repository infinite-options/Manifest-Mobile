﻿//This class is used to store Goals and Routines.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Newtonsoft.Json;

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
        public string TimeInterval { get; set; }
        public Color StatusColor { get; set; }
        public List<Occurance> commonTimeOccurs { get; set; }
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
        public int NumSubOccurances { get; set; }
        public int SubOccurancesCompleted { get; set; }
        //Added so we have the list of subOccurances
        public List<SubOccurance> subOccurances { get; set; }
        public Color GoalBackground { get; set; }
        public Color BorderColor { get; set; }
        public int borderWidth { get; set; }
        public Color textColor { get; set; }

        public string PicUrlUpdate
        {
            set
            {
                PicUrl = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PicUrl"));
            }
        }

        public void updateIsInProgress(bool updatedVal)
        {
            IsInProgress = updatedVal;
            PropertyChanged(this, new PropertyChangedEventArgs("IsInProgress"));
            if (updatedVal)
            {
                GoalBackground = Color.FromHex("#FFE07E");
                PropertyChanged(this, new PropertyChangedEventArgs("GoalBackground"));
            }
            else if(!IsComplete && !IsInProgress)
            {
                GoalBackground = Color.Transparent;
                PropertyChanged(this, new PropertyChangedEventArgs("GoalBackground"));
            }
        }

        public void updateIsComplete(bool updatedVal)
        {
            IsComplete = updatedVal;
            PropertyChanged(this, new PropertyChangedEventArgs("IsComplete"));
            if (updatedVal)
            {
                GoalBackground = Color.FromHex("#D3E6D3");
                PropertyChanged(this, new PropertyChangedEventArgs("GoalBackground"));
            }
            else if (!IsComplete && !IsInProgress)
            {
                GoalBackground = Color.Transparent;
                PropertyChanged(this, new PropertyChangedEventArgs("GoalBackground"));
            }
        }

        public void updateBorderWidth(int newWidth)
        {
            borderWidth = newWidth;
            PropertyChanged(this, new PropertyChangedEventArgs("borderWidth"));
        }

        public void updateStatusColor(string hexColor)
        {
            StatusColor = Color.FromHex(hexColor);
            if (StatusColor.Luminosity > 0.8) textColor = Color.Black;
            else textColor = Color.White;
        }

        public static void SortOccurances(List<Occurance> todaysOccurances)
        {
            todaysOccurances.Sort(delegate (Occurance a, Occurance b)
            {
                if (a.StartDayAndTime.TimeOfDay < b.StartDayAndTime.TimeOfDay) return -1;
                else if (a.StartDayAndTime.TimeOfDay == b.StartDayAndTime.TimeOfDay)
                {
                    if (a.IsEvent == true && b.IsEvent == false)
                    {
                        return -1;
                    }
                    else if (a.IsEvent == false && b.IsEvent == true)
                    {
                        return 1;
                    }
                    else if (a.IsPersistent == true && b.IsPersistent == false)
                    {
                        return -1;
                    }
                    else if (a.IsPersistent == false && b.IsPersistent == true)
                    {
                        return 1;
                    }
                    else if (a.EndDayAndTime.TimeOfDay < b.EndDayAndTime.TimeOfDay)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else return 1;
            });
        }
    }

    public class OccuranceResponse
    {
        public string message { get; set; }
        public List<OccuranceDto> result { get; set; }
    }

    public class OccuranceDto
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
        public List<SubOccuranceDto> actions_tasks { get; set; }
        //public List<Json> actions_tasks { get; set; }
    }

}