using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace Manifest.Models
{
    public class SubOccuranceCarousalViewTile : IComparable<SubOccuranceCarousalViewTile>, INotifyPropertyChanged
    {
        public string Title { get; set; }
        public string PicUrl { get; set; }
        public string SubTitle { get; set; }
        public string CompleteTime { get; set; }
        private bool isComplete;
        public bool IsComplete { get { return isComplete; } set 
            { 
                if (isComplete != value) {
                    isComplete = value;
                    OnPropertyChanged("IsComplete");
                } 
            } }

        public event PropertyChangedEventHandler PropertyChanged;
        public int CompareTo(SubOccuranceCarousalViewTile other)
        {
            return 0;
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
