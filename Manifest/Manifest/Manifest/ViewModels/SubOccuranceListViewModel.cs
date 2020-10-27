using Manifest.Models;
using Manifest.Services;
using Manifest.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Manifest.ViewModels
{
    public class SubOccuranceListViewModel : BaseViewModel
    {
        public SubOccuranceListViewModel(WeakReference<SubOccuranceListView> MainPageWeakReference, InformStatus informStatus) : base()
        {
            this.MainPageWeakReference = MainPageWeakReference;
            this.informStatus = informStatus;
        }

        public WeakReference<SubOccuranceListView> MainPageWeakReference { get; }

        internal InformStatus informStatus;

        private readonly Repository Repository = Repository.Instance;
        private string occuranceId;
        private int completed = 0;
        public string OccuranceId
        {
            get { return occuranceId; }
            set
            {
                occuranceId = value;
                LoadData(value);
            }
        }

        private Occurance occurance = null;
        public Occurance Occurance
        {
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

        internal void InformParent()
        {
            informStatus?.Invoke(completed, subOccurances.Count);
        }

        public INavigation Navigation { get; internal set; }

        public ObservableCollection<SubOccuranceListViewTile> Tiles = new ObservableCollection<SubOccuranceListViewTile>();

        private async void LoadData(string occuranceId)
        {
            Occurance = Repository.GetOccuranceById(occuranceId);
            SubOccurances = Repository.GetSubOccurances(occuranceId);
            MainPageWeakReference.TryGetTarget(out SubOccuranceListView mainPage);
            int index = await CreateTiles();
            if (completed == SubOccurances.Count)
            {
                mainPage.ChangeButtonToDone();
            }
            mainPage.LoadUI(index);
        }


        private async Task<int> CreateTiles()
        {
            int startIndex = -1;
            for (int i = 0; i < subOccurances.Count; i++)
            {
                SubOccurance sub = subOccurances[i];
                if (startIndex == -1 && !sub.IsComplete) { startIndex = i; }
                if (sub.IsComplete) completed += 1;
                SubOccuranceListViewTile tile = new SubOccuranceListViewTile()
                {
                    TileId = ""+i,
                    Title = sub.Title,
                    PicUrl = sub.PicUrl,
                    IsComplete = sub.IsComplete,
                    IsInProgress = sub.IsInProgress,
                    SubTitle = Occurance.Title,
                    OccuranceTitle = Occurance.Title,
                    OccurancePicUrl = Occurance.PicUrl
                };
                tile.TouchCommand = new Command(() => { OnTouch(int.Parse(tile.TileId)); });
                Tiles.Add(tile);
            }
            return startIndex >= 0 ? startIndex : subOccurances.Count - 1;
        }


        public async Task OnTouch(int index)
        {
            if (subOccurances[index].IsComplete) { return; }
            MainPageWeakReference.TryGetTarget(out SubOccuranceListView mainPage);
            if (index == 0)
            {
                bool success = await ChangeToComplete(index);
                if (!success)
                {
                    await mainPage.DisplayAlert("Oops!", "Something Went wrong while updating the server. Please try again later", "OK");
                }
            }
            else
            {
                if (!subOccurances[index - 1].IsComplete)
                {
                    await mainPage.DisplayAlert("Oops!", "Please complete all steps before marking this step as done", "OK");
                    return;
                }
                bool success = await ChangeToComplete(index);
                if (!success) {
                    await mainPage.DisplayAlert("Oops!", "Something Went wrong while updating the server. Please try again later", "OK"); 
                }
            }
            if (completed == subOccurances.Count)
            {
                mainPage.ChangeButtonToDone();
            }
            informStatus(completed, subOccurances.Count);
        }

        private async Task<bool> ChangeToComplete(int index)
        {
            try
            {
                subOccurances[index].IsComplete = true;
                _ =  await Repository.UpdateSubOccurance(subOccurances[index]);
                Tiles[index].IsComplete = true;
                completed += 1;
                return true;
            }catch(Exception e)
            {
                subOccurances[index].IsComplete = false;
                Debug.WriteLine(e.StackTrace);
                return false;
            }
        }
    }
}
