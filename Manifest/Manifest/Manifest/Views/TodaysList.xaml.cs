using Manifest.Models;
using Manifest.Services;
using Manifest.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Manifest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TodaysList : ContentPage
    {
        private readonly TodaysListTileGroup EarlyMorning = new TodaysListTileGroup("Early Morning", "moon.png");
        private readonly TodaysListTileGroup Morning = new TodaysListTileGroup("Morning", "sunrisemid.png");
        private readonly TodaysListTileGroup AfterNoon = new TodaysListTileGroup("Afternoon", "sunrisemid.png");
        private readonly TodaysListTileGroup Evening = new TodaysListTileGroup("Evening", "sunrisemid.png");
        private readonly TodaysListTileGroup Night = new TodaysListTileGroup("Night", "sunrisemid.png");

        private TimeSettings TimeSettings;
        ObservableCollection<TodaysListTileGroup> Items { get; set; }
        private readonly TodaysListViewModel ViewModel;
        readonly Repository repository = Repository.Instance;
        public TodaysList()
        {
            // Test Case: DataFactory.Instance.GetDataClient().GetSubOccurances("300-000049");
            InitializeComponent();
            ViewModel = new TodaysListViewModel(Navigation);
            var task = repository.GetUser("100-000028");
            task.Wait();
            User user = task.Result;
            TimeSettings = user.TimeSettings;
            BindingContext = ViewModel;
            LoadUI(repository.GetAllOccurances(user.Id));
        }

        private async Task<TodaysListTile> ToTile(Occurance occurance)
        {
            string subTitle;
            string startTime = occurance.StartDayAndTime.ToString("hh:mm tt");
            string endTime = occurance.EndDayAndTime.ToString("hh:mm tt");

            TimeSpan TimeTaken = occurance.EndDayAndTime - occurance.StartDayAndTime;
            if (occurance.IsPersistent)
            {
                subTitle = "This takes: " + ((int)TimeTaken.TotalMinutes).ToString() + " minutes";
            }
            else
            {
                subTitle = "This is available from: \n" + startTime + " to " + endTime;
            }

            TodaysListTile tile = new TodaysListTile(occurance.IsInProgress, occurance.IsComplete)
            {
                Id = occurance.Id,
                AvailableStartTime = occurance.StartDayAndTime.TimeOfDay,
                AvailableEndTime = occurance.EndDayAndTime.TimeOfDay,
                Title = occurance.Title,
                SubTitle = subTitle,
                IsSublistAvailable = occurance.IsSublistAvailable,
                Type = TileType.Occurance,
                Photo = occurance.PicUrl,
                FrameBgColorComplete = Color.FromHex("#E9E8E8"),
                FrameBgColorInComplete = Color.FromHex("#FFFFFF")
            };

            tile.TouchCommand = new Command(async () => ViewModel.OnTileTapped(tile));
            return tile;
        }

        private void FillGroup(TodaysListTile tile)
        {
            if (tile.AvailableStartTime < TimeSettings.MorningStartTime)
            {
                EarlyMorning.Add(tile);
            }
            else if (tile.AvailableStartTime < TimeSettings.AfterNoonStartTime)
            {
                Morning.Add(tile);
            }
            else if (tile.AvailableStartTime < TimeSettings.EveningStartTime)
            {
                AfterNoon.Add(tile);
            }
            else if (tile.AvailableStartTime < TimeSettings.NightStartTime)
            {
                Evening.Add(tile);
            }
            else
            {
                Night.Add(tile);
            }
        }

        private async Task<List<TodaysListTile>> PopulateTilesAsync(List<Occurance> occurances)
        {
            List<TodaysListTile> tiles = new List<TodaysListTile>();

            List<Task<TodaysListTile>> tasks = occurances.Select(occur => ToTile(occur)).ToList();

            while (tasks.Count() > 0)
            {
                var completedTask = await Task.WhenAny(tasks).ConfigureAwait(false);

                tasks.Remove(completedTask);
                var tile = await completedTask;
                tiles.Add(tile);
            }
            return tiles;
        }

        private void MergeTiles(List<TodaysListTile> tiles1, List<TodaysListTile> tiles2)
        {
            foreach (TodaysListTile item in tiles1)
            {
                tiles2.Add(item);
            }
        }

        private async Task LoadTilesAsync(List<Occurance> occurances)
        {
            Task<List<TodaysListTile>> occuranceTask = PopulateTilesAsync(occurances);

            List<TodaysListTile> tiles = await occuranceTask;

            tiles.Sort();

            foreach (TodaysListTile tile in tiles)
            {
                FillGroup(tile);
            }

        }

        private void LoadGroups(ObservableCollection<TodaysListTileGroup> groups)
        {
            if (EarlyMorning.Count > 0) groups.Add(EarlyMorning);
            if (Morning.Count > 0) groups.Add(Morning);
            if (AfterNoon.Count > 0) groups.Add(AfterNoon);
            if (Evening.Count > 0) groups.Add(Evening);
            if (Night.Count > 0) groups.Add(Night);
            Items = groups;
            MainThread.BeginInvokeOnMainThread(() => {
                TodaysListCollectionView.ItemsSource = Items;
            });
        }

        private async void LoadUI(List<Occurance> occurances)
        {
            await LoadTilesAsync(occurances);
            TodaysListCollectionView.Header = new Label
            {
                Text = DateTime.Now.DayOfWeek.ToString(),
                FontSize = 40,
                Padding = new Thickness(0, 40, 0, 0),
                Margin = new Thickness(0, 40, 0, 0)
            };
            LoadGroups(new ObservableCollection<TodaysListTileGroup>());
        }
    }
}