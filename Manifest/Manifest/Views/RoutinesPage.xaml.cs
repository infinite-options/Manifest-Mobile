using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using Manifest.Config;
using Manifest.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class RoutinesPage : ContentPage
    {
        public ObservableCollection<Occurance> datagrid = new ObservableCollection<Occurance>();
        public List<Occurance> todaysRoutines;
        HttpClient client = new HttpClient();
        double deviceHeight = DeviceDisplay.MainDisplayInfo.Height;
        double deviceWidth = DeviceDisplay.MainDisplayInfo.Width;
        float rowHeight;


        public RoutinesPage(string userID)
        {
            InitializeComponent();
            todaysRoutines = new List<Occurance>();
            rowHeight = (float)(deviceHeight * 0.1);
            initialiseTodaysOccurances(userID);

        }

        private async void initialiseTodaysOccurances(string userID)
        {
            try
            {
                //Need to add userID
                string url = RdsConfig.BaseUrl + RdsConfig.goalsAndRoutinesUrl + "/" + userID;
                var response = await client.GetStringAsync(url);
                Debug.WriteLine("Getting user. User info below:");
                //Debug.WriteLine(response);
                OccuranceResponse occuranceResponse = JsonConvert.DeserializeObject<OccuranceResponse>(response);
                //Debug.WriteLine(occuranceResponse);
                ToOccurances(occuranceResponse);
                SortRoutines();
                CreateList();
            }
            catch (Exception e)
            {
                await DisplayAlert("Alert", "Error in TodaysListTest initialiseTodaysOccurances. Error: " + e.ToString(), "OK");
            }
        }

        //This function takes the response from the endpoint, and formats it into Occurances
        private void ToOccurances(OccuranceResponse occuranceResponse)
        {
            try
            {
                //Clear the occurances, as we are going to get new one now
                todaysRoutines.Clear();
                if (occuranceResponse.result == null || occuranceResponse.result.Count == 0)
                {
                    DisplayAlert("No tasks today", "OK", "Cancel");
                }
                foreach (OccuranceDto dto in occuranceResponse.result)
                {
                    //Only add routines
                    if (dto.is_displayed_today == "True" && dto.is_persistent == "True")
                    {
                        Occurance toAdd = new Occurance();
                        toAdd.Id = dto.gr_unique_id;
                        toAdd.Title = dto.gr_title;
                        toAdd.PicUrl = dto.photo;
                        toAdd.IsPersistent = DataParser.ToBool(dto.is_persistent);
                        toAdd.IsInProgress = DataParser.ToBool(dto.is_in_progress);
                        toAdd.IsComplete = DataParser.ToBool(dto.is_complete);
                        toAdd.IsSublistAvailable = DataParser.ToBool(dto.is_sublist_available);
                        toAdd.ExpectedCompletionTime = DataParser.ToTimeSpan(dto.expected_completion_time);
                        toAdd.CompletionTime = dto.expected_completion_time;
                        toAdd.DateTimeCompleted = DataParser.ToDateTime(dto.datetime_completed);
                        toAdd.DateTimeStarted = DataParser.ToDateTime(dto.datetime_started);
                        toAdd.StartDayAndTime = DataParser.ToDateTime(dto.start_day_and_time);
                        toAdd.EndDayAndTime = DataParser.ToDateTime(dto.end_day_and_time);
                        toAdd.Repeat = DataParser.ToBool(dto.repeat);
                        toAdd.RepeatEvery = dto.repeat_every;
                        toAdd.RepeatFrequency = dto.repeat_frequency;
                        toAdd.RepeatType = dto.repeat_type;
                        toAdd.RepeatOccurences = dto.repeat_occurences;
                        toAdd.RepeatEndsOn = DataParser.ToDateTime(dto.repeat_ends_on);
                        //toAdd.RepeatWeekDays = ParseRepeatWeekDays(repeat_week_days);
                        toAdd.UserId = dto.user_id;
                        toAdd.IsEvent = false;
                        todaysRoutines.Add(toAdd);
                    }
                }
                return;
            }
            catch (Exception e)
            {
                DisplayAlert("Alert", "Error in TodaysListTest ToOccurances(). Error: " + e.ToString(), "OK");
            }
        }

        private void SortRoutines()
        {
            todaysRoutines.Sort(delegate (Occurance a, Occurance b)
            {
                if (a.StartDayAndTime.TimeOfDay < b.StartDayAndTime.TimeOfDay) return -1;
                else return 1;
            });
        }

        private void CreateList()
        {
            foreach (Occurance toAdd in todaysRoutines)
            {
                //We want to get every subtask for that routine
                Grid newGrid = new Grid {
                    RowDefinitions = {
                        new RowDefinition { Height = new GridLength(rowHeight, GridUnitType.Absolute)}
                    },
                    Padding = 10,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                };
                Grid gridToAdd =
                    new Grid {
                        BackgroundColor = Color.FromHex("#FFBD27"),
                        RowDefinitions =
                        {
                            new RowDefinition { Height = new GridLength(3, GridUnitType.Star)},
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Star)}
                        },
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star)},
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)}
                        }
                    };
                gridToAdd.Children.Add(
                    new Label
                    {
                        Text = toAdd.Title,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.White,
                        TextDecorations = TextDecorations.Underline
                    }, 0, 0);
                gridToAdd.Children.Add(
                    new Label
                    {
                        Text = "This takes: " + toAdd.ExpectedCompletionTime.ToString(),
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.White,
                        TextDecorations = TextDecorations.Underline
                    }, 0, 1);
                gridToAdd.Children.Add(
                    new Image {
                        Source = toAdd.PicUrl
                    }, 1, 0);
                routines.Children.Add(gridToAdd);
            }
        }

        private void goToTodaysList(object sender, EventArgs args)
        {
            Application.Current.MainPage = new TodaysListTest((String)Application.Current.Properties["userID"]);
        }
        void navigateToAboutMe(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new AboutMePage();
        }
    }
}
