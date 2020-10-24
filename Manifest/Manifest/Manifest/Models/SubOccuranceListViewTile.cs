using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Manifest.Models
{
    public class SubOccuranceListViewTile : IComparable<SubOccuranceListViewTile>, INotifyPropertyChanged
    {
        public string TileId { get; set; }
        public string Title { get; set; }
        public string OccuranceTitle { get; set; }
        public string OccurancePicUrl { get; set; }
        public string PicUrl { get; set; }
        public string SubTitle { get; set; }

        private bool isInProgress;
        public bool IsInProgress { 
            get { return isInProgress; }
            set
            {
                if (isInProgress != value)
                {
                    isInProgress = value;
                    OnPropertyChanged("IsInProgress");
                }
            }
        }

        private bool isComplete;
        public bool IsComplete { 
            get { return isComplete; } 
            set
            {
                if (isComplete != value)
                {
                    isComplete = value;
                    OnPropertyChanged("IsComplete");
                }
            }
        }
        public ICommand TouchCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public int CompareTo(SubOccuranceListViewTile other)
        {
            return 0;
        }
    }
}
