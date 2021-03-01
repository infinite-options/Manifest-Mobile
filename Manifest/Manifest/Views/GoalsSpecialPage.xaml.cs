using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using Manifest.Config;
using Manifest.LogIn.Classes;
using Manifest.Models;
using Newtonsoft.Json;
using Xamarin.Auth;
using Xamarin.Essentials;
using Xamarin.Forms;
using Manifest.RDS;

namespace Manifest.Views
{
    public partial class GoalsSpecialPage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        //public List<SubOccurance> subTasks;
        public List<SubOccurance> subTasks;
        HttpClient client = new HttpClient();

        public ObservableCollection<SubOccurance> datagrid = new ObservableCollection<SubOccurance>();

        public int numTasks;
        public int numCompleted;

        public Occurance parent;

        double deviceHeight = DeviceDisplay.MainDisplayInfo.Height;
        double deviceWidth = DeviceDisplay.MainDisplayInfo.Width;
        Dictionary<Label, SubOccurance> subOccDict;
        Occurance passedOccurance;


        public GoalsSpecialPage(Occurance occurance)
        {
            passedOccurance = occurance;
            subOccDict = new Dictionary<Label, SubOccurance>();
            subTasks = occurance.subOccurances;
            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            goalFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            actionFrame1.BackgroundColor = Color.FromHex((string)Application.Current.Properties["routine"]);
            actionFrame2.BackgroundColor = Color.FromHex((string)Application.Current.Properties["goal"]);
            actionFrame3.BackgroundColor = Color.FromHex((string)Application.Current.Properties["event"]);

            //barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);

            title.Text = "Goals";
            subTitle.Text = occurance.Title;
            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();

            //parent = occurance;
            numTasks = 0;
            numCompleted = 0;
            string occuranceID = occurance.Id;
            //subTaskList.ItemsSource = datagrid;
            //initializeSubTasks(occuranceID);
            goalLabel.Text = occurance.Title;


            foreach (SubOccurance subOccur in subTasks)
            {
                Debug.WriteLine(subOccur.Title + " status: in progress: " + subOccur.IsInProgress + " completed: " + subOccur.IsComplete);
            }

            checkPlatform();
            NavigationPage.SetHasNavigationBar(this, false);

            if (subTasks.Count == 1)
            {
                actionLabel2.Text = subTasks[0].Title;
                subOccDict.Add(actionLabel2, subTasks[0]);
                actionFrame3.IsVisible = false;
                actionFrame1.IsVisible = false;
                leftArrow.IsVisible = false;
                rightArrow.IsVisible = false;

                //Image routineComplete = new Image();
                //Binding completeVisible = new Binding("IsComplete");
                //completeVisible.Source = todaysRoutines[i];
                //routineComplete.BindingContext = todaysRoutines[i];
                //routineComplete.Source = "greencheckmark.png";
                //routineComplete.SetBinding(Image.IsVisibleProperty, completeVisible);
                //routineComplete.HorizontalOptions = LayoutOptions.End;
                Binding progVisible = new Binding("IsInProgress");
                progVisible.Source = subTasks[0];
                InProgress2.BindingContext = subTasks[0];
                InProgress2.Source = "yellowclock.png";
                InProgress2.SetBinding(Image.IsVisibleProperty, progVisible);

                Binding completeVisible = new Binding("IsComplete");
                completeVisible.Source = subTasks[0];
                IsComplete2.BindingContext = subTasks[0];
                IsComplete2.Source = "greencheckmark.png";
                IsComplete2.SetBinding(Image.IsVisibleProperty, completeVisible);
            }
            else if (subTasks.Count == 2)
            {
                Debug.WriteLine("first: " + subTasks[0].Title + " second: " + subTasks[1].Title);
                actionLabel1.Text = subTasks[0].Title;
                subOccDict.Add(actionLabel1, subTasks[0]);

                actionLabel3.Text = subTasks[1].Title;
                subOccDict.Add(actionLabel3, subTasks[1]);
                actionFrame2.IsVisible = false;
                downArrow.IsVisible = false;

                Binding progVisible1 = new Binding("IsInProgress");
                progVisible1.Source = subTasks[0];
                InProgress1.BindingContext = subTasks[0];
                InProgress1.Source = "yellowclock.png";
                InProgress1.SetBinding(Image.IsVisibleProperty, progVisible1);

                Binding completeVisible1 = new Binding("IsComplete");
                completeVisible1.Source = subTasks[0];
                IsComplete1.BindingContext = subTasks[0];
                IsComplete1.Source = "greencheckmark.png";
                IsComplete1.SetBinding(Image.IsVisibleProperty, completeVisible1);

                Binding progVisible3 = new Binding("IsInProgress");
                progVisible3.Source = subTasks[1];
                InProgress3.BindingContext = subTasks[1];
                InProgress3.Source = "yellowclock.png";
                InProgress3.SetBinding(Image.IsVisibleProperty, progVisible3);

                Binding completeVisible3 = new Binding("IsComplete");
                completeVisible3.Source = subTasks[1];
                IsComplete3.BindingContext = subTasks[1];
                IsComplete3.Source = "greencheckmark.png";
                IsComplete3.SetBinding(Image.IsVisibleProperty, completeVisible3);

            }
            else if (subTasks.Count >= 3)
            {
                Debug.WriteLine("subtask 3 entered");
                actionLabel1.Text = subTasks[0].Title;
                subOccDict.Add(actionLabel1, subTasks[0]);

                actionLabel2.Text = subTasks[1].Title;
                subOccDict.Add(actionLabel2, subTasks[1]);

                actionLabel3.Text = subTasks[2].Title;
                subOccDict.Add(actionLabel3, subTasks[2]);

                Binding progVisible1 = new Binding("IsInProgress");
                progVisible1.Source = subTasks[0];
                InProgress1.BindingContext = subTasks[0];
                InProgress1.Source = "yellowclock.png";
                InProgress1.SetBinding(Image.IsVisibleProperty, progVisible1);

                Binding completeVisible1 = new Binding("IsComplete");
                completeVisible1.Source = subTasks[0];
                IsComplete1.BindingContext = subTasks[0];
                IsComplete1.Source = "greencheckmark.png";
                IsComplete1.SetBinding(Image.IsVisibleProperty, completeVisible1);

                Binding progVisible2 = new Binding("IsInProgress");
                progVisible2.Source = subTasks[1];
                InProgress2.BindingContext = subTasks[1];
                InProgress2.Source = "yellowclock.png";
                InProgress2.SetBinding(Image.IsVisibleProperty, progVisible2);

                Binding completeVisible2 = new Binding("IsComplete");
                completeVisible2.Source = subTasks[1];
                IsComplete2.BindingContext = subTasks[1];
                IsComplete2.Source = "greencheckmark.png";
                IsComplete2.SetBinding(Image.IsVisibleProperty, completeVisible2);

                Binding progVisible3 = new Binding("IsInProgress");
                progVisible3.Source = subTasks[2];
                InProgress3.BindingContext = subTasks[2];
                InProgress3.Source = "yellowclock.png";
                InProgress3.SetBinding(Image.IsVisibleProperty, progVisible3);

                Binding completeVisible3 = new Binding("IsComplete");
                completeVisible3.Source = subTasks[2];
                IsComplete3.BindingContext = subTasks[2];
                IsComplete3.Source = "greencheckmark.png";
                IsComplete3.SetBinding(Image.IsVisibleProperty, completeVisible3);
            }
            else Navigation.PopAsync();
        }

        void checkPlatform()
        {
            goalFrame.HeightRequest = deviceHeight / 14;
            goalFrame.WidthRequest = goalFrame.HeightRequest;
            goalFrame.CornerRadius = (int)(deviceHeight / 21);
            goalLabel.FontSize = deviceHeight / 70;

            progIcon.HeightRequest = deviceHeight / 28;
            progIcon.WidthRequest = deviceHeight / 28;
            //progIcon.CornerRadius = (int)(deviceHeight / 100);
            progLabel.FontSize = deviceHeight / 140;
            //leftArrow.WidthRequest = deviceWidth / 3;
            //double holder = leftArrow.Width;
            //leftArrow.Margin = new Thickness(deviceWidth / 6, 0, 0, 0);
            //leftArrow.WidthRequest = holder;
            //rightArrow.Margin = new Thickness(-deviceWidth / 9, 0, 0, 0);
            //rightArrow.WidthRequest = holder;

            actionFrame1.HeightRequest = deviceWidth / 10;
            actionFrame1.WidthRequest = deviceWidth / 10;
            actionFrame1.CornerRadius = (int)(deviceWidth / 13.5);
            actionLabel1.FontSize = deviceWidth / 45;

            InProgress1.HeightRequest = deviceWidth / 18;
            InProgress1.WidthRequest = deviceWidth / 18;
            IsComplete1.HeightRequest = deviceWidth / 18;
            IsComplete1.WidthRequest = deviceWidth / 18;

            actionFrame2.HeightRequest = deviceWidth / 10;
            actionFrame2.WidthRequest = deviceWidth / 10;
            actionFrame2.CornerRadius = (int)(deviceWidth / 13.5);
            actionLabel2.FontSize = deviceWidth / 45;

            InProgress2.HeightRequest = deviceWidth / 18;
            InProgress2.WidthRequest = deviceWidth / 18;
            IsComplete2.HeightRequest = deviceWidth / 18;
            IsComplete2.WidthRequest = deviceWidth / 18;

            actionFrame3.HeightRequest = deviceWidth / 10;
            actionFrame3.WidthRequest = deviceWidth / 10;
            actionFrame3.CornerRadius = (int)(deviceWidth / 13.5);
            actionLabel3.FontSize = deviceWidth / 45;

            InProgress3.HeightRequest = deviceWidth / 18;
            InProgress3.WidthRequest = deviceWidth / 18;
            IsComplete3.HeightRequest = deviceWidth / 18;
            IsComplete3.WidthRequest = deviceWidth / 18;
        }

        void goBackToGoals(System.Object sender, System.EventArgs e)
        {
            Navigation.PopAsync(false);
        }

        async void goToSteps(System.Object sender, System.EventArgs e)
        {
            string url = AppConstants.BaseUrl + AppConstants.updateActionAndTask;

            Label receiving = (Label)sender;
            if (receiving == actionLabel1 && receiving.Text != null && receiving.Text != "" && subOccDict[receiving].instructions.Count != 0 && IsComplete1.IsVisible == false)
                await Navigation.PushAsync(new GoalStepsPage(passedOccurance, subOccDict[receiving], actionFrame1.BackgroundColor.ToHex().ToString()), false);
            else if (receiving == actionLabel2 && receiving.Text != null && receiving.Text != "" && subOccDict[receiving].instructions.Count != 0 && IsComplete2.IsVisible == false)
                await Navigation.PushAsync(new GoalStepsPage(passedOccurance, subOccDict[receiving], actionFrame2.BackgroundColor.ToHex().ToString()), false);
            else if (receiving == actionLabel3 && receiving.Text != null && receiving.Text != "" && subOccDict[receiving].instructions.Count != 0 && IsComplete3.IsVisible == false)
                await Navigation.PushAsync(new GoalStepsPage(passedOccurance, subOccDict[receiving], actionFrame3.BackgroundColor.ToHex().ToString()), false);
            else if (subOccDict[receiving].instructions.Count == 0 && subOccDict[receiving].IsComplete == false)
            {
                Debug.WriteLine("last elseif entered");
                if (subOccDict[receiving].IsInProgress == true)
                    await RdsConnect.updateOccurance(subOccDict[receiving], false, true, url);
                else await RdsConnect.updateOccurance(subOccDict[receiving], true, false, url);

                bool onlyInProgress = false;
                foreach (SubOccurance subOccur in passedOccurance.subOccurances)
                {
                    if (subOccur.IsComplete == false && subOccur.Title != subOccDict[receiving].Title)
                        onlyInProgress = true;
                }

                string url2 = AppConstants.BaseUrl + AppConstants.updateGoalAndRoutine;
                if (onlyInProgress == true)
                    await RdsConnect.updateOccurance(passedOccurance, true, false, url2);
                else await RdsConnect.updateOccurance(passedOccurance, false, true, url2);
            }
                //DisplayAlert("Oops", "there are no instructions available for this action", "OK");
        }

        async void goToStepsFrame(System.Object sender, System.EventArgs e)
        {
            string url = AppConstants.BaseUrl + AppConstants.updateActionAndTask;

            Frame receiving = (Frame)sender;

            if (receiving == actionFrame1 && actionLabel1.Text != null && actionLabel1.Text != "" && subOccDict[actionLabel1].instructions.Count != 0 && IsComplete1.IsVisible == false)
                await Navigation.PushAsync(new GoalStepsPage(passedOccurance, subOccDict[actionLabel1], actionFrame1.BackgroundColor.ToHex().ToString()), false);
            else if (receiving == actionFrame2 && actionLabel2.Text != null && actionLabel2.Text != "" && subOccDict[actionLabel2].instructions.Count != 0 && IsComplete2.IsVisible == false)
                await Navigation.PushAsync(new GoalStepsPage(passedOccurance, subOccDict[actionLabel2], actionFrame2.BackgroundColor.ToHex().ToString()), false);
            else if (receiving == actionFrame3 && actionLabel3.Text != null && actionLabel3.Text != "" && subOccDict[actionLabel3].instructions.Count != 0 && IsComplete3.IsVisible == false)
                await Navigation.PushAsync(new GoalStepsPage(passedOccurance, subOccDict[actionLabel3], actionFrame3.BackgroundColor.ToHex().ToString()), false);
            else if (receiving == actionFrame1 && actionLabel1.Text != null && actionLabel1.Text != "" && subOccDict[actionLabel1].IsComplete == false)
            {
                Debug.WriteLine("first else if entered");
                if (subOccDict[actionLabel1].IsInProgress == true)
                {
                    await RdsConnect.updateOccurance(subOccDict[actionLabel1], false, true, url);

                    bool onlyInProgress = false;
                    foreach (SubOccurance subOccur in passedOccurance.subOccurances)
                    {
                        if (subOccur.IsComplete == false && subOccur.Title != subOccDict[actionLabel1].Title)
                            onlyInProgress = true;
                    }

                    string url2 = AppConstants.BaseUrl + AppConstants.updateGoalAndRoutine;
                    if (onlyInProgress == true)
                        await RdsConnect.updateOccurance(passedOccurance, true, false, url2);
                    else await RdsConnect.updateOccurance(passedOccurance, false, true, url2);
                }
                else await RdsConnect.updateOccurance(subOccDict[actionLabel1], true, false, url);
            }
            else if (receiving == actionFrame2 && actionLabel2.Text != null && actionLabel2.Text != "" && subOccDict[actionLabel2].IsComplete == false)
            {
                Debug.WriteLine("second else if entered");
                if (subOccDict[actionLabel2].IsInProgress == true)
                {
                    await RdsConnect.updateOccurance(subOccDict[actionLabel2], false, true, url);

                    bool onlyInProgress = false;
                    foreach (SubOccurance subOccur in passedOccurance.subOccurances)
                    {
                        if (subOccur.IsComplete == false && subOccur.Title != subOccDict[actionLabel2].Title)
                            onlyInProgress = true;
                    }

                    string url2 = AppConstants.BaseUrl + AppConstants.updateGoalAndRoutine;
                    if (onlyInProgress == true)
                        await RdsConnect.updateOccurance(passedOccurance, true, false, url2);
                    else await RdsConnect.updateOccurance(passedOccurance, false, true, url2);
                }
                else await RdsConnect.updateOccurance(subOccDict[actionLabel2], true, false, url);

                
            }
            else if (receiving == actionFrame3 && actionLabel3.Text != null && actionLabel3.Text != "" && subOccDict[actionLabel3].IsComplete == false)
            {
                Debug.WriteLine("third else if entered");
                if (subOccDict[actionLabel3].IsInProgress == true)
                {
                    await RdsConnect.updateOccurance(subOccDict[actionLabel3], false, true, url);

                    bool onlyInProgress = false;
                    foreach (SubOccurance subOccur in passedOccurance.subOccurances)
                    {
                        if (subOccur.IsComplete == false && subOccur.Title != subOccDict[actionLabel3].Title)
                            onlyInProgress = true;
                    }

                    string url2 = AppConstants.BaseUrl + AppConstants.updateGoalAndRoutine;
                    if (onlyInProgress == true)
                        await RdsConnect.updateOccurance(passedOccurance, true, false, url2);
                    else await RdsConnect.updateOccurance(passedOccurance, false, true, url2);
                }
                else await RdsConnect.updateOccurance(subOccDict[actionLabel3], true, false, url);
            }

        }

        void progressClicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ProgressPage());
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Navigation.PopAsync(false);
        }

        //This function makes a call to the database to get all the sub tasks for the given occurance, and displays it on the device
        //private async void initializeSubTasks(string occuranceID)
        //{
        //    string url = RdsConfig.BaseUrl + RdsConfig.actionAndTaskUrl + '/' + occuranceID;
        //    var response = await client.GetStringAsync(url);
        //    Debug.WriteLine("subocc response: " + response);
        //    SubOccuranceResponse subOccuranceResponse = JsonConvert.DeserializeObject<SubOccuranceResponse>(response);
        //    ToSubOccurances(subOccuranceResponse);
        //    //CreateList();
        //}

        //This function converts the response we got from the endpoint to a list of SubOccurance's
        //private void ToSubOccurances(SubOccuranceResponse subOccuranceResponse)
        //{
        //    //Clear the occurances, as we are going to get new one now
        //    //subTasks.Clear();
        //    if (subOccuranceResponse.result == null || subOccuranceResponse.result.Count == 0)
        //    {
        //        DisplayAlert("No tasks today", "OK", "Cancel");
        //    }
        //    foreach (SubOccuranceDto dto in subOccuranceResponse.result)
        //    {
        //        numTasks++;
        //        SubOccurance toAdd = new SubOccurance();
        //        toAdd.Id = dto.at_unique_id;
        //        toAdd.Title = dto.at_title;
        //        Debug.WriteLine("title: " + toAdd.Title);
        //        toAdd.GoalRoutineID = dto.goal_routine_id;
        //        toAdd.AtSequence = dto.at_sequence;
        //        toAdd.IsAvailable = ToBool(dto.is_available);
        //        toAdd.IsComplete = ToBool(dto.is_complete);
        //        if (toAdd.IsComplete)
        //        {
        //            numCompleted++;
        //        }
        //        toAdd.IsInProgress = ToBool(dto.is_in_progress);
        //        toAdd.IsSublistAvailable = ToBool(dto.is_sublist_available);
        //        toAdd.IsMustDo = ToBool(dto.is_must_do);
        //        toAdd.PicUrl = dto.photo;
        //        toAdd.IsTimed = ToBool(dto.is_timed);
        //        toAdd.DateTimeCompleted = ToDateTime(dto.datetime_completed);
        //        toAdd.DateTimeStarted = ToDateTime(dto.datetime_started);
        //        toAdd.ExpectedCompletionTime = ToTimeSpan(dto.expected_completion_time);
        //        toAdd.AvailableStartTime = ToDateTime(dto.available_start_time);
        //        toAdd.AvailableEndTime = ToDateTime(dto.available_end_time);
        //        //subTasks.Add(toAdd);
        //        Debug.WriteLine(toAdd.Id);
        //        Debug.WriteLine("ToSubOcc inside count: " + subTasks.Count.ToString());
        //        //subOccDict.Add(toAdd.Title, toAdd);
        //        if (numTasks == 1)
        //            action1.Text = toAdd.Title;
        //        else if (numTasks == 2)
        //            action2.Text = toAdd.Title;
        //        else action3.Text = toAdd.Title;
        //    }
        //    Debug.WriteLine("final inside count: " + subTasks.Count.ToString());
        //}

        //This function converts a string to a bool
        private bool ToBool(string boolString)
        {
            if (String.IsNullOrEmpty(boolString))
            {
                return false;
            }
            try
            {
                return Boolean.Parse(boolString);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in ToBool function in TodaysList class");
                return false;
            }
        }

        //This function converts a string to a TimeSpan
        private TimeSpan ToTimeSpan(string timeString)
        {
            if (String.IsNullOrEmpty(timeString))
            {
                return new TimeSpan();
            }
            try
            {
                return TimeSpan.Parse(timeString);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in ToTimeSpan function in TodaysList class");
            }
            return new TimeSpan();
        }

        //This function convert a string to a DateTime
        private DateTime ToDateTime(string dateString)
        {
            try
            {
                return DateTime.Parse(dateString);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in ToDateTime function in TodaysList class");
            }
            return new DateTime();
        }

        //private void CreateList()
        //{
        //    for (int i = 0; i < subTasks.Count; i++)
        //    {
        //        this.datagrid.Add(subTasks[i]);
        //    }
        //}


        private void goToTodaysList(object sender, EventArgs args)
        {
            Application.Current.MainPage = new TodaysListPage(null, null);
        }

        //public class UpdateSubOccuranceDataType
        //{
        //    public string id { get; set; }
        //    public DateTime datetime_completed { get; set; }
        //    public DateTime datetime_started { get; set; }
        //    public bool is_in_progress { get; set; }
        //    public bool is_complete { get; set; }

        //}

        //private string updateSubOccurance(SubOccurance toUpdate)
        //{
        //    var toSend = new UpdateSubOccuranceDataType()
        //    {
        //        id = toUpdate.Id,
        //        datetime_completed = toUpdate.DateTimeCompleted,
        //        datetime_started = toUpdate.DateTimeStarted,
        //        is_in_progress = toUpdate.IsInProgress,
        //        is_complete = toUpdate.IsComplete
        //    };
        //    return JsonConvert.SerializeObject(toSend);
        //}


        //This function will be called whenever a subOccurance is tapped. It will set the subOccurance to InProgress or IsComplete.
        //If all subOccurance's are complete, it sets the parent occurance to IsComplete
        async void subOccuranceTapped(object sender, EventArgs args)
        {
            Debug.WriteLine("Task tapped");
            Grid myvar = (Grid)sender;
            SubOccurance currOccurance = myvar.BindingContext as SubOccurance;
            string url = AppConstants.BaseUrl + AppConstants.updateActionAndTask;
            //if (currOccurance.IsInProgress == false && currOccurance.IsComplete == false)
            //{
            //    currOccurance.updateIsInProgress(true);
            //    currOccurance.DateTimeStarted = DateTime.Now;
            //    Debug.WriteLine("Should be changed to in progress. InProgress = " + currOccurance.IsInProgress);
            //    UpdateOccurance updateOccur = new UpdateOccurance()
            //    {
            //        id = currOccurance.Id,
            //        datetime_completed = currOccurance.DateTimeCompleted,
            //        datetime_started = currOccurance.DateTimeStarted,
            //        is_in_progress = currOccurance.IsInProgress,
            //        is_complete = currOccurance.IsComplete
            //    };
            //    string toSend = updateOccur.updateOccurance();
            //    var content = new StringContent(toSend);
            //    var res = await client.PostAsync(url, content);
            //    if (res.IsSuccessStatusCode)
            //    {
            //        Debug.WriteLine("Wrote to the datebase");
            //    }
            //    else
            //    {
            //        Debug.WriteLine("Some error");
            //        Debug.WriteLine(toSend);
            //        Debug.WriteLine(res.ToString());
            //    }
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
            //    res = await client.PostAsync(parenturl, parentContent);
            //    if (res.IsSuccessStatusCode)
            //    {
            //        Debug.WriteLine("Parent is now inProgress");
            //    }
            //    else
            //    {
            //        Debug.WriteLine("Error updating parent");
            //    }
            //}
            if (currOccurance.IsComplete == false)
            {
                Debug.WriteLine("Should be changed to in complete");
                currOccurance.updateIsInProgress(false);
                currOccurance.updateIsComplete(true);
                numCompleted++;
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

                if (numCompleted == numTasks)
                {
                    parent.updateIsInProgress(false);
                    parent.updateIsComplete(true);
                    UpdateOccurance parentOccur = new UpdateOccurance()
                    {
                        id = parent.Id,
                        datetime_completed = parent.DateTimeCompleted,
                        datetime_started = parent.DateTimeStarted,
                        is_in_progress = parent.IsInProgress,
                        is_complete = parent.IsComplete
                    };
                    string toSendParent = parentOccur.updateOccurance();
                    var parentContent = new StringContent(toSendParent);
                    string parenturl = AppConstants.BaseUrl + AppConstants.updateGoalAndRoutine;
                    var res = await client.PostAsync(parenturl, parentContent);
                    if (res.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Parent is now complete");
                    }
                    else
                    {
                        Debug.WriteLine("Error updating parent");
                    }
                }
                else if (parent.IsInProgress == false)
                {
                    parent.updateIsInProgress(true);
                    UpdateOccurance parentOccur = new UpdateOccurance()
                    {
                        id = parent.Id,
                        datetime_completed = parent.DateTimeCompleted,
                        datetime_started = parent.DateTimeStarted,
                        is_in_progress = parent.IsInProgress,
                        is_complete = parent.IsComplete
                    };
                    string toSendParent = parentOccur.updateOccurance();
                    var parentContent = new StringContent(toSendParent);
                    string parenturl = AppConstants.BaseUrl + AppConstants.updateGoalAndRoutine;
                    var res = await client.PostAsync(parenturl, parentContent);
                    if (res.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Parent is now in progress");
                    }
                    else
                    {
                        Debug.WriteLine("Error updating parent");
                    }
                }

            }
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SettingsPage(), false);
        }
    }
}
