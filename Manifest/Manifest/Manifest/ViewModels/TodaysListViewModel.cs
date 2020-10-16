using Manifest.Models;
using Manifest.Views;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Manifest.ViewModels
{
    public class TodaysListViewModel
    {
        private readonly INavigation Navigation;

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
                //await Shell.Current.GoToAsync($"SubOccuranceCarousalView?occuranceid={Tile.Id}");
                if (!tile.IsSublistAvailable)
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
                    //if(tile.IsPersistant) await Navigation.PushAsync(new SubOccuranceCarousalView(tile.Id, informStatus));
                    //else await Navigation.PushAsync(new SubOccuranceListView(tile.Id, informStatus));
                    await Navigation.PushAsync(new SubOccuranceCarousalView(tile.Id, informStatus));
                }
                
            }
        }

        public void ChangeToInProgress(TodaysListTile tile)
        {
            if (!tile.InProgress)
            {
                tile.InProgress = true;
            }
        }

        public void ChangeToComplete(TodaysListTile tile)
        {
            if (!tile.IsComplete)
            {
                tile.InProgress = false;
                tile.IsComplete = true;
            }
        }
    }
}
