using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Manifest.Models
{
    public enum TileType
    {
        Event, Occurance
    }
    public class TodaysListTile : IComparable<TodaysListTile>, INotifyPropertyChanged
    {
        public string Id { get; set; }
        public TimeSpan AvailableStartTime { get; set; }
        public TimeSpan AvailableEndTime { get; set; }
        public DateTime ActualStartTime { get; set; }
        public DateTime ActualEndTime { get; set; }
        public Color BorderColor
        {
            get
            {
                if (!isComplete) return Color.Transparent;
                return Color.Black;
            }
        }
        public Color FrameBgColorComplete { set; private get; }
        public Color FrameBgColorInComplete { set; private get; }
        public Color FrameBgColor { get { return (isComplete) ? FrameBgColorComplete : FrameBgColorInComplete; } }
        public string Title { get; set; }
        public TileType Type { get; set; }
        public string SubTitle { get; set; }
        public bool IsSublistAvailable { get; set; }
        public string TimeDifference { get; set; }
        
        private bool inProgress;
        public bool InProgress { get { return this.inProgress; } set { this.inProgress = value; OnPropertyChanged(); } }
        public bool IsPersistant { get; set; }
        public string Photo { get; set; }
        public ICommand TouchCommand { get; set; }

        private bool isComplete;
        public bool IsComplete
        {
            get { return this.isComplete; }
            set
            {
                this.isComplete = value;
                OnPropertyChanged("IsComplete");
                OnPropertyChanged("BorderColor");
                OnPropertyChanged("FrameBgColor");
                OnPropertyChanged("IsNotComplete");
            }
        }

        public bool IsNotComplete => !isComplete;
        public event PropertyChangedEventHandler PropertyChanged;

        public TodaysListTile() { }

        public TodaysListTile(bool inProgress, bool isComplete)
        {
            this.inProgress = inProgress;
            this.isComplete = isComplete;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public int CompareTo(TodaysListTile that)
        {
            return AvailableStartTime.CompareTo(that.AvailableStartTime);
        }
    }
}
