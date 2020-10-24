using Manifest.Models;
using Manifest.Services;
using Manifest.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Manifest.ViewModels
{
    public class TodaysListViewModel
    {
        private readonly INavigation Navigation;
        private readonly Repository repository = Repository.Instance;

        public TodaysListViewModel(INavigation Navigation)
        {
            this.Navigation = Navigation;
        }

#pragma warning disable CS1998 
        public async Task OnTileTapped(TodaysListTile tile)
#pragma warning restore CS1998
        {
            if (tile.Type == TileType.Occurance)
            {
                List<SubOccurance> subOccurances = repository.GetSubOccurances(tile.Id);
                if (!tile.IsSublistAvailable || subOccurances.Count==0)
                {
                    if (tile.IsNotComplete && tile.InProgress) ChangeToComplete(tile);
                    else if (tile.IsNotComplete && !tile.InProgress) ChangeToInProgress(tile);
                }
                else
                {
                    InformStatus informStatus = async (int completed, int total) =>
                    {
                        if (completed == total) ChangeToComplete(tile);
                        else if (completed > 0) ChangeToInProgress(tile);
                    };
                    if (tile.IsPersistant) await Navigation.PushAsync(new SubOccuranceCarousalView(tile.Id, informStatus));
                    else await Navigation.PushAsync(new SubOccuranceListView(tile.Id, informStatus));
                }

            }else if( tile.Type == TileType.Event)
            {
                await Navigation.PushAsync(new EventsPage(tile.Id));
            }
        }

        public void ChangeToInProgress(TodaysListTile tile)
        {
            if (!tile.InProgress && !tile.IsComplete)
            {
                tile.InProgress = true;
                tile.ActualStartTime = DateTime.Now;
                Occurance occurance = repository.GetOccuranceById(tile.Id);
                occurance.IsInProgress = true;
                occurance.DateTimeStarted = tile.ActualStartTime;
                _ = repository.UpdateOccurance(occurance);
            }
        }

        public void ChangeToComplete(TodaysListTile tile)
        {
            if (!tile.IsComplete)
            {
                tile.InProgress = false;
                tile.IsComplete = true;
                tile.ActualEndTime = DateTime.Now;
                Occurance occurance = repository.GetOccuranceById(tile.Id);
                occurance.IsInProgress = false;
                occurance.IsInProgress = false;
                occurance.DateTimeCompleted = tile.ActualEndTime;
                tile.ActualStartTime = DateTime.Now;
                _ = repository.UpdateOccurance(occurance);
            }
        }
    }
}
