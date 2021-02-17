using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Manifest.Config;
using Manifest.LogIn.Classes;
using Manifest.Models;
using Newtonsoft.Json;
using Xamarin.Auth;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class RoutinePage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        string city;
        string time;
        
        public ObservableCollection<Occurance> datagrid = new ObservableCollection<Occurance>();
        public List<Occurance> todaysRoutines;
        HttpClient client = new HttpClient();
        double deviceHeight = DeviceDisplay.MainDisplayInfo.Height;
        double deviceWidth = DeviceDisplay.MainDisplayInfo.Width;
        float rowHeight;

        public RoutinePage()
        {
            InitializeComponent();
            try{


                setting = false;
                height = mainStackLayoutRow.Height;
                lastRowHeight = barStackLayoutRow.Height;

                mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
                frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
                barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
                
                title.Text = "Routines";

                var helperObject = new MainPage();
                locationTitle.Text = (string)Application.Current.Properties["location"];
                dateTitle.Text = helperObject.GetCurrentTime();

                NavigationPage.SetHasNavigationBar(this, false);
                todaysRoutines = new List<Occurance>();
                rowHeight = (float)(deviceHeight * 0.06);
                var userID = (string)Application.Current.Properties["userId"];
                initialiseTodaysOccurances(userID);
            }
            catch (Exception routine)
            {

            }
        }


        private async void initialiseTodaysOccurances(string userID)
        {
            try
            {
                //Need to add userID
                string url = RdsConfig.BaseUrl + RdsConfig.getRoutines + "/" + userID;
                var response = await client.GetStringAsync(url);
                Debug.WriteLine("Getting user. User info below:");
                Debug.WriteLine(response);
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
                        if (dto.actions_tasks == null)
                        {
                            Debug.WriteLine("Actions and tasks are null");
                        }
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
                        toAdd.NumSubOccurances = 0;
                        toAdd.SubOccurancesCompleted = 0;
                        toAdd.subOccurances = GetSubOccurances(dto.actions_tasks, toAdd);
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

        private List<SubOccurance> GetSubOccurances(List<SubOccuranceDto> actions_tasks, Occurance parent)
        {
            List<SubOccurance> subTasks = new List<SubOccurance>();
            if (actions_tasks == null || actions_tasks.Count == 0)
            {
                return subTasks;
            }
            foreach(SubOccuranceDto dto in actions_tasks)
            {
                parent.NumSubOccurances++;
                //numTasks++;
                SubOccurance toAdd = new SubOccurance();
                toAdd.Id = dto.at_unique_id;
                toAdd.Title = dto.at_title;
                toAdd.GoalRoutineID = dto.goal_routine_id;
                toAdd.AtSequence = dto.at_sequence;
                toAdd.IsAvailable = DataParser.ToBool(dto.is_available);
                toAdd.IsComplete = DataParser.ToBool(dto.is_complete);
                if (toAdd.IsComplete)
                {
                    parent.SubOccurancesCompleted++;
                }
                toAdd.IsInProgress = DataParser.ToBool(dto.is_in_progress);
                toAdd.IsSublistAvailable = DataParser.ToBool(dto.is_sublist_available);
                toAdd.IsMustDo = DataParser.ToBool(dto.is_must_do);
                toAdd.PicUrl = dto.photo;
                toAdd.IsTimed = DataParser.ToBool(dto.is_timed);
                toAdd.DateTimeCompleted = DataParser.ToDateTime(dto.datetime_completed);
                toAdd.DateTimeStarted = DataParser.ToDateTime(dto.datetime_started);
                toAdd.ExpectedCompletionTime = DataParser.ToTimeSpan(dto.expected_completion_time);
                toAdd.AvailableStartTime = DataParser.ToDateTime(dto.available_start_time);
                toAdd.AvailableEndTime = DataParser.ToDateTime(dto.available_end_time);
                subTasks.Add(toAdd);
                Debug.WriteLine(toAdd.Id);
            }

            return subTasks;
        }

        private void SortRoutines()
        {
            todaysRoutines.Sort(delegate (Occurance a, Occurance b)
            {
                if (a.StartDayAndTime.TimeOfDay < b.StartDayAndTime.TimeOfDay) return -1;
                else return 1;
            });
        }

        private async void CreateList()
        {

            TapGestureRecognizer doneRecognizer = new TapGestureRecognizer();
            doneRecognizer.NumberOfTapsRequired = 1;
            doneRecognizer.Tapped += subTaskComplete;

            TapGestureRecognizer helpRecognizer = new TapGestureRecognizer();
            helpRecognizer.NumberOfTapsRequired = 1;
            helpRecognizer.Tapped += helpNeeded;

            TapGestureRecognizer routineRecognizer = new TapGestureRecognizer();
            routineRecognizer.NumberOfTapsRequired = 1;
            routineRecognizer.Tapped += routineTapped;

            for (int i = 0; i < todaysRoutines.Count; i++)
            {
                Occurance toAdd = todaysRoutines[i];
                //We want to get every subtask for that routine
                Grid newGrid = new Grid {
                    RowDefinitions = {
                        new RowDefinition { Height = new GridLength(rowHeight, GridUnitType.Absolute)}
                    },
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    RowSpacing = 10
                };

                //Add Complete and In Progress images
                Image routineComplete = new Image();
                Binding completeVisible = new Binding("IsComplete");
                completeVisible.Source = todaysRoutines[i];
                routineComplete.BindingContext = todaysRoutines[i];
                routineComplete.Source = "greencheckmark.png";
                routineComplete.SetBinding(Image.IsVisibleProperty, completeVisible);
                routineComplete.HorizontalOptions = LayoutOptions.End;

                Image routineInProgress = new Image();
                Binding inProgressVisible = new Binding("IsInProgress");
                inProgressVisible.Source = todaysRoutines[i];
                routineInProgress.BindingContext = todaysRoutines[i];
                routineInProgress.Source = "yellowclock.png";
                routineInProgress.SetBinding(Image.IsVisibleProperty, inProgressVisible);
                routineInProgress.HorizontalOptions = LayoutOptions.End;



                newGrid.BindingContext = toAdd;
                Grid gridToAdd =
                    new Grid {
                        BackgroundColor = Color.FromHex("#FFBD27"),
                        RowDefinitions =
                        {
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Star)},
                            new RowDefinition { Height = new GridLength(2, GridUnitType.Star)},
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Star)}
                        },
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star)},
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)}
                        }
                    };
                float fontsize = rowHeight / 4;
                gridToAdd.Children.Add(
                    new Label
                    {
                        Text = toAdd.StartDayAndTime.TimeOfDay.ToString() + " - " + toAdd.EndDayAndTime.TimeOfDay.ToString(),
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.White
                    }, 0, 0);
                gridToAdd.Children.Add(
                    new Label
                    {
                        Text = toAdd.Title,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.White,
                        TextDecorations = TextDecorations.Underline,
                        FontSize = fontsize
                    }, 0, 1);
                gridToAdd.Children.Add(
                    new Label
                    {
                        Text = "This takes: " + toAdd.ExpectedCompletionTime.ToString(),
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.White,
                        TextDecorations = TextDecorations.Underline
                    }, 0, 2);
                gridToAdd.Children.Add(
                    new Image {
                        Source = toAdd.PicUrl,
                        HeightRequest = rowHeight,
                        Aspect = Aspect.AspectFit
                    }, 1, 2, 0, 3);

                gridToAdd.Children.Add(routineComplete, 1, 2, 0, 3);
                gridToAdd.Children.Add(routineInProgress, 1, 2, 0, 3);
                Frame gridFrame = new Frame {
                    BackgroundColor = Color.FromHex("#FFBD27"),
                    CornerRadius = 15,
                    Content = gridToAdd,
                    Padding = 10,
                    GestureRecognizers = {routineRecognizer}
                };
                gridFrame.BindingContext = toAdd;
                newGrid.Children.Add(gridFrame,0,0);
                int rowToAdd = 1;
                //Now, we have to get the subtasks and add them to our grid
                //var subTasks = await initializeSubTasks(i);
                var subTasks = toAdd.subOccurances;
                //Initialize if the task is complete or not
                if (toAdd.NumSubOccurances == toAdd.SubOccurancesCompleted && toAdd.NumSubOccurances > 0)
                {
                    toAdd.updateIsComplete(true);
                }
                else if (toAdd.SubOccurancesCompleted > 0)
                {
                    toAdd.updateIsInProgress(true);
                }
                Debug.WriteLine("SubTasks Completed = " + toAdd.SubOccurancesCompleted);

                foreach (SubOccurance subTask in subTasks)
                {
                    float subGridHeight = rowHeight / (float)1.5;
                    newGrid.RowDefinitions.Add(new RowDefinition
                    {
                        Height = new GridLength(subGridHeight, GridUnitType.Absolute)
                    });
                    Grid subGrid =
                    new Grid
                    {
                        RowDefinitions =
                        {
                            //new RowDefinition { Height = new GridLength(3, GridUnitType.Star)},
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Star)}
                        },
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)},
                            new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star)}
                        }
                    };
                    subGrid.BindingContext = subTask;
                    Grid icons = new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)},
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)}
                        }
                    };
                    icons.Children.Add(
                        new Image
                        {
                            Source = "greencheckmark.png",
                            GestureRecognizers = { doneRecognizer },
                            BindingContext = subTask
                        },0,0
                    );
                    icons.Children.Add(
                        new Image
                        {
                            Source = "dots.png",
                            GestureRecognizers = { helpRecognizer },
                            BindingContext = subTask
                        },1,0);
                    subGrid.Children.Add(icons, 0, 0);
                    subGrid.Children.Add(
                        new Frame
                        {
                            CornerRadius = 15,
                            BackgroundColor = Color.FromHex("#FFBD27"),
                            Content =
                            new Label
                            {
                                Text = subTask.Title,
                                TextColor = Color.Black,
                                BackgroundColor = Color.FromHex("#FFBD27")
                            }
                        }
                        ,1,0
                        );
                    Image isComplete = new Image();
                    Binding isVisible = new Binding("IsComplete");
                    isVisible.Source = subTask;
                    isComplete.BindingContext = subTask;
                    isComplete.Source = "greencheckmark.png";
                    isComplete.SetBinding(Image.IsVisibleProperty, isVisible);
                    isComplete.HorizontalOptions = LayoutOptions.End;
                    subGrid.Children.Add(
                        isComplete, 1, 0
                        );
                    newGrid.Children.Add(subGrid, 0, rowToAdd);
                    rowToAdd++;

                }
                routines.Children.Add(newGrid);
            }
        }

        //This function makes a call to the database to get all the sub tasks for the given occurance, and displays it on the device
        private async Task<List<SubOccurance>> initializeSubTasks(int parentIndex)
        {
            string occuranceID = todaysRoutines[parentIndex].Id;
            string url = RdsConfig.BaseUrl + RdsConfig.actionAndTaskUrl + '/' + occuranceID;
            var response = await client.GetStringAsync(url);
            SubOccuranceResponse subOccuranceResponse = JsonConvert.DeserializeObject<SubOccuranceResponse>(response);
            var toReturn = ToSubOccurances(subOccuranceResponse, parentIndex);
            return toReturn;

        }

        //This function converts the response we got from the endpoint to a list of SubOccurance's
        private List<SubOccurance> ToSubOccurances(SubOccuranceResponse subOccuranceResponse, int parent)
        {
            //Clear the occurances, as we are going to get new one now
            List<SubOccurance> subTasks = new List<SubOccurance>();
            if (subOccuranceResponse.result == null || subOccuranceResponse.result.Count == 0)
            {
                return subTasks;
            }
            foreach (SubOccuranceDto dto in subOccuranceResponse.result)
            {
                todaysRoutines[parent].NumSubOccurances++;
                //numTasks++;
                SubOccurance toAdd = new SubOccurance();
                toAdd.Id = dto.at_unique_id;
                toAdd.Title = dto.at_title;
                toAdd.GoalRoutineID = dto.goal_routine_id;
                toAdd.AtSequence = dto.at_sequence;
                toAdd.IsAvailable = DataParser.ToBool(dto.is_available);
                toAdd.IsComplete = DataParser.ToBool(dto.is_complete);
                if (toAdd.IsComplete)
                {
                    todaysRoutines[parent].SubOccurancesCompleted++;
                }
                toAdd.IsInProgress = DataParser.ToBool(dto.is_in_progress);
                toAdd.IsSublistAvailable = DataParser.ToBool(dto.is_sublist_available);
                toAdd.IsMustDo = DataParser.ToBool(dto.is_must_do);
                toAdd.PicUrl = dto.photo;
                toAdd.IsTimed = DataParser.ToBool(dto.is_timed);
                toAdd.DateTimeCompleted = DataParser.ToDateTime(dto.datetime_completed);
                toAdd.DateTimeStarted = DataParser.ToDateTime(dto.datetime_started);
                toAdd.ExpectedCompletionTime = DataParser.ToTimeSpan(dto.expected_completion_time);
                toAdd.AvailableStartTime = DataParser.ToDateTime(dto.available_start_time);
                toAdd.AvailableEndTime = DataParser.ToDateTime(dto.available_end_time);
                subTasks.Add(toAdd);
                Debug.WriteLine(toAdd.Id);
            }
            return subTasks;
        }

        public async void subTaskComplete(object sender, EventArgs args)
        {
            Debug.WriteLine("Task tapped");
            Image myvar = (Image)sender;
            SubOccurance currOccurance = myvar.BindingContext as SubOccurance;

            //Get the parent of the sender
            Grid mygrid = (Grid)myvar.Parent;

            if (mygrid == null)
            {
                Debug.WriteLine("Parent is null");
            }

            //Now we got the parent element we want
            Grid parent = (Grid)mygrid.Parent;

            if (parent == null)
            {
                Debug.WriteLine("Grandparent is null");
            }

            Grid grandparent = (Grid)parent.Parent;

            Occurance parentOccurance = grandparent.BindingContext as Occurance;

            string url = RdsConfig.BaseUrl + RdsConfig.updateActionAndTask;
            if (currOccurance.IsComplete == false)
            {
                Debug.WriteLine("Should be changed to in complete");
                currOccurance.updateIsInProgress(false);
                currOccurance.updateIsComplete(true);
                //numCompleted++;
                currOccurance.DateTimeCompleted = DateTime.Now;
                UpdateOccurance updateOccur = new UpdateOccurance()
                {
                    id = currOccurance.Id,
                    datetime_completed = currOccurance.DateTimeCompleted,
                    datetime_started = currOccurance.DateTimeStarted,
                    is_in_progress = currOccurance.IsInProgress,
                    is_complete = currOccurance.IsComplete
                };
                string toSend = updateOccur.updateOccurance();
                var content = new StringContent(toSend);
                var res = await client.PostAsync(url, content);
                if (res.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Successfully completed the subtask");
                }

                //Now update the parent
                parentOccurance.SubOccurancesCompleted++;
                url = RdsConfig.BaseUrl + RdsConfig.updateGoalAndRoutine;
                if (parentOccurance.NumSubOccurances == parentOccurance.SubOccurancesCompleted)
                {
                    parentOccurance.updateIsInProgress(false);
                    parentOccurance.updateIsComplete(true);
                    parentOccurance.DateTimeCompleted = DateTime.Now;
                    updateOccur = new UpdateOccurance()
                    {
                        id = parentOccurance.Id,
                        datetime_completed = parentOccurance.DateTimeCompleted,
                        datetime_started = parentOccurance.DateTimeStarted,
                        is_in_progress = parentOccurance.IsInProgress,
                        is_complete = parentOccurance.IsComplete
                    };
                    toSend = updateOccur.updateOccurance();
                    content = new StringContent(toSend);
                    res = await client.PostAsync(url, content);
                    if (res.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Wrote to the datebase");
                    }
                    else
                    {
                        Debug.WriteLine("Some error");
                        Debug.WriteLine(toSend);
                        Debug.WriteLine(res.ToString());
                    }
                }
                else if (parentOccurance.NumSubOccurances > parentOccurance.SubOccurancesCompleted && parentOccurance.SubOccurancesCompleted > 0)
                {
                    parentOccurance.updateIsInProgress(true);
                    parentOccurance.DateTimeCompleted = DateTime.Now;
                    updateOccur = new UpdateOccurance()
                    {
                        id = parentOccurance.Id,
                        datetime_completed = parentOccurance.DateTimeCompleted,
                        datetime_started = parentOccurance.DateTimeStarted,
                        is_in_progress = parentOccurance.IsInProgress,
                        is_complete = parentOccurance.IsComplete
                    };
                    toSend = updateOccur.updateOccurance();
                    content = new StringContent(toSend);
                    res = await client.PostAsync(url, content);
                    if (res.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Wrote to the datebase");
                    }
                    else
                    {
                        Debug.WriteLine("Some error");
                        Debug.WriteLine(toSend);
                        Debug.WriteLine(res.ToString());
                    }
                }
            }
                //if (numCompleted == numTasks)
                //{
                //    parent.updateIsInProgress(false);
                //    parent.updateIsComplete(true);
                //    UpdateOccurance parentOccur = new UpdateOccurance()
                //    {
                //        id = parent.Id,
                //        datetime_completed = parent.DateTimeCompleted,
                //        datetime_started = parent.DateTimeStarted,
                //        is_in_progress = parent.IsInProgress,
                //        is_complete = parent.IsComplete
                //    };
                //    string toSendParent = parentOccur.updateOccurance();
                //    var parentContent = new StringContent(toSendParent);
                //    string parenturl = RdsConfig.BaseUrl + RdsConfig.updateGoalAndRoutine;
                //    var res = await client.PostAsync(parenturl, parentContent);
                //    if (res.IsSuccessStatusCode)
                //    {
                //        Debug.WriteLine("Parent is now complete");
                //    }
                //    else
                //    {
                //        Debug.WriteLine("Error updating parent");
                //    }
                //}
                //else if (parent.IsInProgress == false)
                //{
                //    parent.updateIsInProgress(true);
                //    UpdateOccurance parentOccur = new UpdateOccurance()
                //    {
                //        id = parent.Id,
                //        datetime_completed = parent.DateTimeCompleted,
                //        datetime_started = parent.DateTimeStarted,
                //        is_in_progress = parent.IsInProgress,
                //        is_complete = parent.IsComplete
                //    };
                //    string toSendParent = parentOccur.updateOccurance();
                //    var parentContent = new StringContent(toSendParent);
                //    string parenturl = RdsConfig.BaseUrl + RdsConfig.updateGoalAndRoutine;
                //    var res = await client.PostAsync(parenturl, parentContent);
                //    if (res.IsSuccessStatusCode)
                //    {
                //        Debug.WriteLine("Parent is now in progress");
                //    }
                //    else
                //    {
                //        Debug.WriteLine("Error updating parent");
                //    }
                //}
            }

        public void helpNeeded(object sender, EventArgs args)
        {
            Debug.WriteLine("Help button pressed. Help needed for subTask");
        }

        public async void routineTapped(object sender, EventArgs args)
        {
            Frame myvar = (Frame)sender;
            Occurance currOccurance = myvar.BindingContext as Occurance;
            //Now check if the currOccurance has any subtasks
            if (currOccurance.NumSubOccurances == 0)
            {
                string url = RdsConfig.BaseUrl + RdsConfig.updateGoalAndRoutine;
                if (currOccurance.IsComplete == false && currOccurance.IsInProgress == false)
                {
                    currOccurance.updateIsInProgress(true);
                    currOccurance.DateTimeStarted = DateTime.Now;
                    Debug.WriteLine("Should be changed to in progress. InProgress = " + currOccurance.IsInProgress);
                    //string toSend = updateOccurance(currOccurance);
                    UpdateOccurance updateOccur = new UpdateOccurance()
                    {
                        id = currOccurance.Id,
                        datetime_completed = currOccurance.DateTimeCompleted,
                        datetime_started = currOccurance.DateTimeStarted,
                        is_in_progress = currOccurance.IsInProgress,
                        is_complete = currOccurance.IsComplete
                    };
                    string toSend = updateOccur.updateOccurance();
                    var content = new StringContent(toSend);
                    var res = await client.PostAsync(url, content);
                    if (res.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Wrote to the datebase");
                    }
                    else
                    {
                        Debug.WriteLine("Some error");
                        Debug.WriteLine(toSend);
                        Debug.WriteLine(res.ToString());
                    }
                
                }
                else if (currOccurance.IsInProgress == true && currOccurance.IsComplete == false)
                {
                    Debug.WriteLine("Should be changed to in complete");
                    currOccurance.updateIsInProgress(false);
                    currOccurance.updateIsComplete(true);
                    currOccurance.DateTimeCompleted = DateTime.Now;
                    UpdateOccurance updateOccur = new UpdateOccurance()
                    {
                        id = currOccurance.Id,
                        datetime_completed = currOccurance.DateTimeCompleted,
                        datetime_started = currOccurance.DateTimeStarted,
                        is_in_progress = currOccurance.IsInProgress,
                        is_complete = currOccurance.IsComplete
                    };
                    string toSend = updateOccur.updateOccurance();
                    var content = new StringContent(toSend);
                    _ = await client.PostAsync(url, content);

                }
            }
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushAsync(new RoutineStepsPage(),false);
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SettingsPage(), false);
        }
    }
}
