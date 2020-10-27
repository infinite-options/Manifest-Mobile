using Manifest.Models;
using Manifest.Services;
using Manifest.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Manifest.ViewModels
{
    [QueryProperty("OccuranceId", "occuranceid")]
    public class SubOccuranceCarousalViewModel : BaseViewModel
    {
        readonly WeakReference<SubOccuranceCarousalView> MainPageWeakReference;
        readonly Repository repository = Repository.Instance;
        InformStatus informStatus;
        public SubOccuranceCarousalViewModel(WeakReference<SubOccuranceCarousalView> MainPageWeakReference, InformStatus informStatus) : base()
        {
            this.MainPageWeakReference = MainPageWeakReference;
            this.informStatus = informStatus;
        }

        private readonly Repository Repository = Repository.Instance;
        private string occuranceId;
        private int completed = 0;
        public string OccuranceId { get { return occuranceId; } set
            {
                occuranceId = value;
                LoadData(value);
            }
        }

        internal void InformParent()
        {
            informStatus?.Invoke(completed, subOccurances.Count);
        }

        private Occurance occurance = null;
        public Occurance Occurance {
            get { return occurance; }
            set
            {
                occurance = value;
                OnPropertyChanged("Occurance");
            }
        }
        private List<SubOccurance> subOccurances = null;
        public List<SubOccurance> SubOccurances
        {
            get { return subOccurances; }
            set
            {
                subOccurances = value;
                OnPropertyChanged("SubOccurances");
            }
        }

        public ObservableCollection<SubOccuranceCarousalViewTile> Tiles = new ObservableCollection<SubOccuranceCarousalViewTile>();

        private async void LoadData(string occuranceId)
        {
            Occurance = Repository.GetOccuranceById(occuranceId);
            SubOccurances = Repository.GetSubOccurances(occuranceId);
            MainPageWeakReference.TryGetTarget(out SubOccuranceCarousalView mainPage);
            int index = await CreateTiles();
            if (completed== SubOccurances.Count)
            {
                mainPage.ChangeButtonToDone();
            }
            mainPage.LoadUI(index);
        }

        private async Task<int> CreateTiles(){
            int startIndex = -1;
            for(int i=0; i<subOccurances.Count; i++)
            {
                SubOccurance sub = subOccurances[i];
                if(startIndex==-1 && !sub.IsComplete) { startIndex = i; }
                if (sub.IsComplete) completed += 1;

                Tiles.Add(new SubOccuranceCarousalViewTile()
                {
                    Title = sub.Title,
                    PicUrl = sub.PicUrl,
                    IsComplete = sub.IsComplete,
                    SubTitle = Occurance.Title
                });
            }
            return startIndex >= 0 ? startIndex : subOccurances.Count - 1;
        }

        public async Task<SubOccurance> GetUpdatedItem(int index)
        {
            SubOccurance subOccurance = MarkCompleted(index);
            completed += 1;
            informStatus?.Invoke(completed, SubOccurances.Count);
            if (completed == SubOccurances.Count)
            {
                MainPageWeakReference.TryGetTarget(out SubOccuranceCarousalView mainPage);
                mainPage.ChangeButtonToDone();
            }
            return subOccurance;
        }

        private SubOccurance MarkCompleted(int index)
        {
            SubOccurance subOccurance = SubOccurances[index];
            Tiles[index].IsComplete = true;
            if (subOccurance.IsComplete == true)
            {
                return subOccurance;
            }
            subOccurance.IsComplete = true;
            _ = repository.UpdateSubOccurance(subOccurance);
            return subOccurance;
        }
    }
}
