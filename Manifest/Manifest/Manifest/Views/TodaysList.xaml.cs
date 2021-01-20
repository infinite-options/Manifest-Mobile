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
            InitializeComponent();
            BindingContext = ViewModel = new TodaysListViewModel(Navigation);
            RefreshViewInstance.Command = new Command(async () => { await Refresh(); RefreshViewInstance.IsRefreshing = false; });
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    TodaysListCollectionView.Margin= new Thickness(20,0,20,52);
                    break;
                case Device.Android:
                    TodaysListCollectionView.Margin = new Thickness(20, 0, 20, 0);
                    break;
                case Device.UWP:
                default:
                    TodaysListCollectionView.Margin = new Thickness(20, 0, 20, 52);
                    break;
            }
            System.Diagnostics.Debug.WriteLine("IN TodaysList INITIALIZER");
        }

        protected override void OnAppearing()
        {
            System.Diagnostics.Debug.WriteLine("Started OnAppearing");
            var user = repository.LoadUserData();
            TimeSettings = user.TimeSettings;
            // Calling both the endpoint
            var OccurancesTask = repository.GetAllOccurances(user.Id);
            var EventsTask= repository.GetEvents();
            // Waiting for those endpoints
            OccurancesTask.Wait();
            EventsTask.Wait();
            LoadUI(OccurancesTask.Result, EventsTask.Result);
            System.Diagnostics.Debug.WriteLine("Finished OnAppearing");
        }

        private async Task ClearGroups()
        {
            System.Diagnostics.Debug.WriteLine("Started ClearGroups");
            EarlyMorning.Clear();
            Morning.Clear();
            AfterNoon.Clear();
            Evening.Clear();
            Night.Clear();
            System.Diagnostics.Debug.WriteLine("Finished ClearGroups");
        }

        private async Task<TodaysListTile> ToTile(Occurance occurance)
        {
            System.Diagnostics.Debug.WriteLine("Started ToTile Occurance");
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
                subTitle = "This is available from:\n" + startTime + " to " + endTime;
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
                IsPersistant = occurance.IsPersistent,
                FrameBgColorComplete = Color.FromHex("#E9E8E8"),
                FrameBgColorInComplete = Color.FromHex("#FFFFFF")
            };

            tile.TouchCommand = new Command(async () => await ViewModel.OnTileTapped(tile)); //Earlier no await here. Could be error
            System.Diagnostics.Debug.WriteLine("Finished ToTile Occurance");
            return tile;
        }

        private async Task<TodaysListTile> ToTile(Event _event)
        {
            System.Diagnostics.Debug.WriteLine("Started ToTile Event");
            TodaysListTile eventTile = new TodaysListTile()
            {
                Id = _event.Id,
                Type = TileType.Event,
                AvailableEndTime = _event.EndTime.LocalDateTime.TimeOfDay,
                AvailableStartTime = _event.StartTime.LocalDateTime.TimeOfDay,
                TimeDifference = _event.StartTime.LocalDateTime.ToString("h:mm tt") + " - " + _event.EndTime.LocalDateTime.ToString("h:mm tt"),
                Title = _event.Title,
                SubTitle = _event.Description,
                Photo = "calendarFive.png"
            };
            eventTile.TouchCommand = new Command(async () => await ViewModel.OnTileTapped(eventTile)); //Earlier no await here. Could be error
            System.Diagnostics.Debug.WriteLine("Finished ToTile Event");
            return eventTile;
        }

        private void FillGroup(TodaysListTile tile)
        {
            System.Diagnostics.Debug.WriteLine("Started FillGroup");
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
            System.Diagnostics.Debug.WriteLine("Finished FillGroup");
        }

        private async Task<List<TodaysListTile>> PopulateTilesAsync(List<Occurance> occurances)
        {
            System.Diagnostics.Debug.WriteLine("Started PopulateTileAsync Occurance");
            List<TodaysListTile> tiles = new List<TodaysListTile>();

            List<Task<TodaysListTile>> tasks = occurances.Select(occur => ToTile(occur)).ToList();

            while (tasks.Count() > 0)
            {
                System.Diagnostics.Debug.WriteLine("Continuously called");
                var completedTask = await Task.WhenAny(tasks).ConfigureAwait(false);

                tasks.Remove(completedTask);
                var tile = await completedTask;
                tiles.Add(tile);
            }
            System.Diagnostics.Debug.WriteLine("Finished PopulateTileAsync Occurance");
            return tiles;
        }

        private async Task<List<TodaysListTile>> PopulateTilesAsync(List<Event> events)
        {
            System.Diagnostics.Debug.WriteLine("Started PopulateTileAsync Event");
            List<TodaysListTile> tiles = new List<TodaysListTile>();

            List<Task<TodaysListTile>> tasks = events.Select(occur => ToTile(occur)).ToList();

            while (tasks.Count() > 0)
            {
                var completedTask = await Task.WhenAny(tasks).ConfigureAwait(false);

                tasks.Remove(completedTask);
                var tile = await completedTask;
                tiles.Add(tile);
            }
            System.Diagnostics.Debug.WriteLine("Finished PopulateTileAsync Event");
            return tiles;
        }

        private void MergeTiles(List<TodaysListTile> tiles1, List<TodaysListTile> tiles2)
        {
            System.Diagnostics.Debug.WriteLine("Started MergeTiles");
            foreach (TodaysListTile item in tiles1)
            {
                tiles2.Add(item);
            }
            System.Diagnostics.Debug.WriteLine("Finished MergeTiles");
        }

        private async Task LoadTilesAsync(List<Occurance> occurances)
        {
            System.Diagnostics.Debug.WriteLine("Started LoadTilesAsync Occurance");
            Task<List<TodaysListTile>> occuranceTask = PopulateTilesAsync(occurances);

            List<TodaysListTile> tiles = await occuranceTask;

            tiles.Sort();

            foreach (TodaysListTile tile in tiles)
            {
                FillGroup(tile);
            }
            System.Diagnostics.Debug.WriteLine("Finished LoadTilesAsync Occurance");

        }

        private async Task LoadTilesAsync(List<Event> events)
        {
            System.Diagnostics.Debug.WriteLine("Started LoadTilesAsync Event");
            Task<List<TodaysListTile>> eventsTask = PopulateTilesAsync(events);

            List<TodaysListTile> tiles = await eventsTask;

            tiles.Sort();

            foreach (TodaysListTile tile in tiles)
            {
                FillGroup(tile);
            }
            System.Diagnostics.Debug.WriteLine("Finished LoadTilesAsync Event");
        }

        private void LoadGroups(ObservableCollection<TodaysListTileGroup> groups)
        {
            System.Diagnostics.Debug.WriteLine("Started LoadGroups");
            if (EarlyMorning.Count > 0) groups.Add(EarlyMorning);
            if (Morning.Count > 0) groups.Add(Morning);
            if (AfterNoon.Count > 0) groups.Add(AfterNoon);
            if (Evening.Count > 0) groups.Add(Evening);
            if (Night.Count > 0) groups.Add(Night);
            Items = groups;
            MainThread.BeginInvokeOnMainThread(() => {
                System.Diagnostics.Debug.WriteLine("BeginInvokeOnMainThread called");
                TodaysListCollectionView.ItemsSource = Items;
            });
            System.Diagnostics.Debug.WriteLine("Finished LoadGroups");
        }

        private async void LoadUI(List<Occurance> occurances, List<Event> events)
        {
            System.Diagnostics.Debug.WriteLine("Started LoadUI");
            await ClearGroups();
            var occuranceLoadtask = LoadTilesAsync(occurances);
            var eventsloadTask = LoadTilesAsync(events);
            await occuranceLoadtask;
            await eventsloadTask;
            TodaysListCollectionView.Header = new Label
            {
                Text = DateTime.Now.DayOfWeek.ToString(),
                FontSize = 40,
                Padding = new Thickness(0, 80, 0, 0),
                //Margin = new Thickness(0, 80, 0, 0)
            };
            LoadGroups(new ObservableCollection<TodaysListTileGroup>());
            System.Diagnostics.Debug.WriteLine("Finished LoadUI");
        }

        private async Task Refresh()
        {
            System.Diagnostics.Debug.WriteLine("Started Refresh");
            RefreshViewInstance.IsRefreshing = true;
            var user = repository.RefreshUserData();
            TimeSettings = user.TimeSettings;
            // Calling both the endpoint
            var OccurancesTask = repository.GetAllFreshOccurances(user.Id);
            var EventsTask = repository.GetFreshEvents();
            // Waiting for those endpoints
            OccurancesTask.Wait();
            EventsTask.Wait();
            LoadUI(OccurancesTask.Result, EventsTask.Result);
            System.Diagnostics.Debug.WriteLine("Finished Refresh");
        }
    }
}