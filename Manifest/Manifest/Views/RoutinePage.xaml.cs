using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Manifest.Config;
using Manifest.LogIn.Classes;
using Manifest.Models;
using Manifest.RDS;
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
            catch (Exception e)
            {
                DisplayAlert("Error", "Error in InitializeComponent, RoutinePage:\n" + e.ToString(), "OK");
            }
        }


        private async void initialiseTodaysOccurances(string userID)
        {
            try
            {
                //Need to add userID
                string url = RdsConfig.BaseUrl + RdsConfig.getRoutines + "/" + userID;
                todaysRoutines = await RdsConnect.getOccurances(url);
                SortRoutines();
                CreateList();
            }
            catch (Exception e)
            {
                await DisplayAlert("Alert", "Error in TodaysListTest initialiseTodaysOccurances. Error: " + e.ToString(), "OK");
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
                string timespan = toAdd.StartDayAndTime.ToString("hh:mm tt") + " - " + toAdd.EndDayAndTime.ToString("hh:mm tt");
                gridToAdd.Children.Add(
                    new Label
                    {
                        Text = timespan,
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
                string res = await RdsConnect.updateOccurance(currOccurance, false, true, url);
                if (res == "Failure")
                {
                    await DisplayAlert("Error", "There was an error writing to the database.", "OK");
                }

                //Now update the parent
                parentOccurance.SubOccurancesCompleted++;
                url = RdsConfig.BaseUrl + RdsConfig.updateGoalAndRoutine;
                if (parentOccurance.NumSubOccurances == parentOccurance.SubOccurancesCompleted)
                {
                    res = await RdsConnect.updateOccurance(parentOccurance, false, true, url);
                    if (res == "Failure")
                    {
                        await DisplayAlert("Error", "There was an error writing to the database.", "OK");
                    }
                    Debug.WriteLine("Wrote to the datebase");
                }
                else if (parentOccurance.IsInProgress == false && parentOccurance.IsComplete == false)
                {
                    res = await RdsConnect.updateOccurance(parentOccurance, true, false, url);
                    if (res == "Failure")
                    {
                        await DisplayAlert("Error", "There was an error writing to the database.", "OK");
                    }
                }
            }
        }

        public async void helpNeeded(object sender, EventArgs args)
        {
            Debug.WriteLine("Help button pressed. Help needed for subTask");
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
            //If there are instructions, navigate to the instruction page
            if (currOccurance.instructions.Count > 0)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new RoutineStepsPage(currOccurance, parentOccurance), false);
            }
            else
            {
                await DisplayAlert("Note", "No instructions for this task", "OK");
            }

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
                    string res = await RdsConnect.updateOccurance(currOccurance, true, false, url);
                    if (res == "Failure")
                    {
                        await DisplayAlert("Error", "There was an error writing to the database.", "OK");
                    }
                    Debug.WriteLine("Wrote to the datebase");

                }
                else if (currOccurance.IsInProgress == true && currOccurance.IsComplete == false)
                {
                    string res = await RdsConnect.updateOccurance(currOccurance, false, true, url);
                    if (res == "Failure")
                    {
                        await DisplayAlert("Error", "There was an error writing to the database.", "OK");
                    }
                    Debug.WriteLine("Wrote to the datebase");
                }
            }
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        // void Button_Clicked(System.Object sender, System.EventArgs e)
        // {
        //     Application.Current.MainPage.Navigation.PushAsync(new RoutineStepsPage(),false);
        // }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SettingsPage(), false);
        }
    }
}
