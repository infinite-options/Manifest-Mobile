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
using Xamarin.CommunityToolkit.UI.Views;
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
                string url = AppConstants.BaseUrl + AppConstants.getRoutines + "/" + userID;
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
                Expander routineExpander = new Expander();
                //We want to get every subtask for that routine
                Grid newGrid = new Grid {
                    RowDefinitions = {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star)}
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

                Color routineColor = new Color();
                if (toAdd.StartDayAndTime.TimeOfDay > DateTime.Now.TimeOfDay || toAdd.EndDayAndTime.TimeOfDay < DateTime.Now.TimeOfDay)
                {
                    routineColor = Color.FromHex("#889AB5");
                }
                else
                {
                    routineColor = Color.FromHex("#F26D4B");
                }

                newGrid.BindingContext = toAdd;
                Grid gridToAdd =
                    new Grid {
                        BackgroundColor = routineColor,
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
                        Text = "This takes: " + toAdd.ExpectedCompletionTime.ToString(@"%h") +"hrs " + toAdd.ExpectedCompletionTime.ToString(@"mm") + "min",
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.White,
                        //TextDecorations = TextDecorations.Underline
                    }, 0, 2);
                Grid routineImage = new Grid()
                {
                    BackgroundColor = routineColor,
                    RowDefinitions =
                        {
                            new RowDefinition { Height = new GridLength(3, GridUnitType.Star)},
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Star)},
                        }
                };
                routineImage.Children.Add(
                    new Image
                    {
                        Source = toAdd.PicUrl,
                        HeightRequest = rowHeight,
                        Aspect = Aspect.AspectFit
                    },0,0);
                if (toAdd.NumSubOccurances > 0)
                {
                    //Add sublist image to routine
                    routineImage.Children.Add(
                        new Image
                        {
                            Source = "sublist.png",
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.End,
                            Aspect = Aspect.AspectFit,
                            //HeightRequest = rowHeight / 4
                        }, 0, 1);
                }
                gridToAdd.Children.Add(
                    routineImage, 1, 2, 0, 3);

                gridToAdd.Children.Add(routineComplete, 1, 2, 0, 3);
                gridToAdd.Children.Add(routineInProgress, 1, 2, 0, 3);

                

                Frame gridFrame = new Frame {
                    BackgroundColor = routineColor,
                    CornerRadius = 15,
                    Content = gridToAdd,
                    Padding = 10,
                    GestureRecognizers = {routineRecognizer}
                };
                gridFrame.BindingContext = toAdd;
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
                routineExpander.Header = gridFrame;
                newGrid.Children.Add(routineExpander, 0, 0);
                if (toAdd.NumSubOccurances != 0)
                {
                    Grid routineExpanderChildren = new Grid() {
                        Padding = new Thickness(5, 10, 5, 10)
                    };
                    int rowToAdd = 0;
                    foreach (SubOccurance subTask in subTasks)
                    {
                        float subGridHeight = rowHeight / (float)1.5;
                        routineExpanderChildren.RowDefinitions.Add(new RowDefinition
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
                            }, 0, 0
                        );
                        icons.Children.Add(
                            new Image
                            {
                                Source = "instructionsHelp.png",
                                GestureRecognizers = { helpRecognizer },
                                BindingContext = subTask
                            }, 1, 0);
                        subGrid.Children.Add(icons, 0, 0);
                        subGrid.Children.Add(
                            new Frame
                            {
                                CornerRadius = 15,
                                BackgroundColor = routineColor,
                                Content =
                                new Label
                                {
                                    Text = subTask.Title,
                                    TextColor = Color.White,
                                    BackgroundColor = routineColor
                                }
                            }
                            , 1, 0
                            );
                        Image isComplete = new Image();
                        Binding isVisible = new Binding("IsComplete");
                        isVisible.Source = subTask;
                        isComplete.BindingContext = subTask;
                        isComplete.Source = "greencheckmark.png";
                        isComplete.SetBinding(Image.IsVisibleProperty, isVisible);
                        isComplete.HorizontalOptions = LayoutOptions.End;

                        Image isInProgress = new Image();
                        Binding isInProgressVisible = new Binding("IsInProgress");
                        isInProgressVisible.Source = subTask;
                        isInProgress.BindingContext = subTask;
                        isInProgress.Source = "yellowclock.png";
                        isInProgress.SetBinding(Image.IsVisibleProperty, isInProgressVisible);
                        isInProgress.HorizontalOptions = LayoutOptions.End;
                        subGrid.Children.Add(
                            isComplete, 1, 0
                            );
                        subGrid.Children.Add(
                            isInProgress, 1, 0
                            );
                        routineExpanderChildren.Children.Add(subGrid, 0, rowToAdd);
                        rowToAdd++;

                    }
                    routineExpander.Content = routineExpanderChildren;
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

            string url = AppConstants.BaseUrl + AppConstants.updateActionAndTask;
            if (currOccurance.IsComplete == false && currOccurance.IsInProgress == true)
            {
                string res = await RdsConnect.updateOccurance(currOccurance, false, true, url);
                if (res == "Failure")
                {
                    await DisplayAlert("Error", "There was an error writing to the database.", "OK");
                }

                //Now update the parent
                parentOccurance.SubOccurancesCompleted++;
                url = AppConstants.BaseUrl + AppConstants.updateGoalAndRoutine;
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
            else if (currOccurance.IsComplete == false && currOccurance.IsInProgress == false)
            {
                string res = await RdsConnect.updateOccurance(currOccurance, true, false, url);
                if (res == "Failure")
                {
                    await DisplayAlert("Error", "There was an error writing to the database.", "OK");
                }

                //Now update the parent
                //parentOccurance.SubOccurancesCompleted++;
                url = AppConstants.BaseUrl + AppConstants.updateGoalAndRoutine;
                if (parentOccurance.IsInProgress == false && parentOccurance.IsComplete == false)
                {
                    res = await RdsConnect.updateOccurance(parentOccurance, true, false, url);
                    if (res == "Failure")
                    {
                        await DisplayAlert("Error", "There was an error writing to the database.", "OK");
                    }
                }
            }
            else if (currOccurance.IsComplete == true && currOccurance.IsInProgress == false)
            {
                string res = await RdsConnect.updateOccurance(currOccurance, false, false, url);
                if (res == "Failure")
                {
                    await DisplayAlert("Error", "There was an error writing to the database.", "OK");
                }
                //Need to update instructions
                foreach (Instruction instruction in currOccurance.instructions)
                {
                    res = await RdsConnect.updateInstruction(false, instruction);
                    if (res == "FAILURE")
                    {
                        Debug.WriteLine("Failed to update instruction");
                    }
                }
                
                //Now update the parent
                parentOccurance.SubOccurancesCompleted--;
                url = AppConstants.BaseUrl + AppConstants.updateGoalAndRoutine;
                if (parentOccurance.SubOccurancesCompleted == 0)
                {
                    res = await RdsConnect.updateOccurance(parentOccurance, false, false, url);
                    if (res == "Failure")
                    {
                        await DisplayAlert("Error", "There was an error writing to the database.", "OK");
                    }
                }
                else if (parentOccurance.SubOccurancesCompleted < parentOccurance.NumSubOccurances && parentOccurance.SubOccurancesCompleted > 0)
                {
                    res = await RdsConnect.updateOccurance(parentOccurance, true, false, url);
                    if (res == "Failure")
                    {
                        await DisplayAlert("Error", "There was an error writing to the database.", "OK");
                    }
                }

            }
        }


        private async void resetSubOccurance(SubOccurance currOccurance, Occurance parentOccurance)
        {
            string url = AppConstants.BaseUrl + AppConstants.updateActionAndTask;
            string res = await RdsConnect.updateOccurance(currOccurance, false, false, url);
            Debug.WriteLine("Suoccurance update response: " + res);
            if (res == "Failure")
            {
                await DisplayAlert("Error", "There was an error writing to the database.", "OK");
            }
            //Need to update instructions
            foreach (Instruction instruction in currOccurance.instructions)
            {
                res = await RdsConnect.updateInstruction(false, instruction);
                if (res == "FAILURE")
                {
                    Debug.WriteLine("Failed to update instruction");
                }
            }

            //Now update the parent
            parentOccurance.SubOccurancesCompleted--;
            url = AppConstants.BaseUrl + AppConstants.updateGoalAndRoutine;
            if (parentOccurance.SubOccurancesCompleted == 0)
            {
                res = await RdsConnect.updateOccurance(parentOccurance, false, false, url);
                if (res == "Failure")
                {
                    await DisplayAlert("Error", "There was an error writing to the database.", "OK");
                }
            }
            else if (parentOccurance.SubOccurancesCompleted < parentOccurance.NumSubOccurances && parentOccurance.SubOccurancesCompleted > 0)
            {
                res = await RdsConnect.updateOccurance(parentOccurance, true, false, url);
                if (res == "Failure")
                {
                    await DisplayAlert("Error", "There was an error writing to the database.", "OK");
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
            string url = AppConstants.BaseUrl + AppConstants.updateGoalAndRoutine;
            if (currOccurance.NumSubOccurances == 0 && !(currOccurance.IsInProgress == false && currOccurance.IsComplete == true))
            {
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
            else if (currOccurance.IsInProgress == false && currOccurance.IsComplete == true)
            {
                bool reset = await DisplayAlert("Warning", "Do you want to reset this routine? All subtasks and instructions will be reset if you do.", "No", "Yes");
                if (reset == false)
                {
                    if (currOccurance.IsSublistAvailable == true)
                    {
                        foreach (SubOccurance subtask in currOccurance.subOccurances)
                        {
                            resetSubOccurance(subtask, currOccurance);
                        }
                    }
                    else
                    {
                        string res = await RdsConnect.updateOccurance(currOccurance, false, false, url);
                        if (res == "Failure")
                        {
                            await DisplayAlert("Error", "There was an error writing to the database.", "OK");
                        }
                        Debug.WriteLine("Wrote to the datebase");
                    }
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

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new TodaysListPage(null, null));
        }

        void Button_Clicked_1(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();

        }
        void Button_Clicked_2(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new AboutMePage();
        }
    }
}
