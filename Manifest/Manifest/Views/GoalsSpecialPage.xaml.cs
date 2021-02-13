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
    public partial class GoalsSpecialPage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        //public List<SubOccurance> subTasks;
        public List<SubOccuranceDto> subTasks;
        HttpClient client = new HttpClient();

        public ObservableCollection<SubOccurance> datagrid = new ObservableCollection<SubOccurance>();

        public int numTasks;
        public int numCompleted;

        public Occurance parent;

        double deviceHeight = DeviceDisplay.MainDisplayInfo.Height;
        double deviceWidth = DeviceDisplay.MainDisplayInfo.Width;
        Dictionary<string, SubOccuranceDto> subOccDict;



        public GoalsSpecialPage(OccuranceDto occurance)
        {
            subOccDict = new Dictionary<string, SubOccuranceDto>();
            subTasks = occurance.actions_tasks;
            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;

            frameColor.BackgroundColor = Color.FromHex("#9DB2CB");
            title.Text = "Goals";
            subTitle.Text = occurance.gr_title;
            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();

            //parent = occurance;
            numTasks = 0;
            numCompleted = 0;
            string occuranceID = occurance.gr_unique_id;
            //subTaskList.ItemsSource = datagrid;
            //initializeSubTasks(occuranceID);
            goal.Text = occurance.gr_title;
            foreach (SubOccuranceDto subOccur in subTasks)
            {
                subOccDict.Add(subOccur.at_title, subOccur);
                Debug.WriteLine("suboccurance title: " + subOccur.at_title);
            }

            checkPlatform();
            NavigationPage.SetHasNavigationBar(this, false);

            if (subTasks.Count == 1)
            {
                action2.Text = subTasks[0].at_title;
                action3.IsVisible = false;
                action1.IsVisible = false;
                leftArrow.IsVisible = false;
                rightArrow.IsVisible = false;
            }
            else if (subTasks.Count == 2)
            {
                Debug.WriteLine("first: " + subTasks[0].at_title + " second: " + subTasks[1].at_title);
                action1.Text = subTasks[0].at_title;
                action3.Text = subTasks[1].at_title;
                action2.IsVisible = false;
                downArrow.IsVisible = false;
            }
            else if (subTasks.Count == 3)
            {
                action1.Text = subTasks[0].at_title;
                action2.Text = subTasks[1].at_title;
                action3.Text = subTasks[2].at_title;
            }
            else Navigation.PopAsync();
        }

        void checkPlatform()
        {
            goal.HeightRequest = deviceHeight / 12;
            goal.WidthRequest = goal.HeightRequest;
            goal.CornerRadius = (int)(deviceHeight / 24);
            goal.FontSize = deviceHeight / 70;

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

            action1.HeightRequest = deviceWidth / 7;
            action1.WidthRequest = deviceWidth / 7;
            action1.CornerRadius = (int)(deviceWidth / 14);
            action1.FontSize = deviceWidth / 45;

            action2.HeightRequest = deviceWidth / 7;
            action2.WidthRequest = deviceWidth / 7;
            action2.CornerRadius = (int)(deviceWidth / 14);
            action2.FontSize = deviceWidth / 45;

            action3.HeightRequest = deviceWidth / 7;
            action3.WidthRequest = deviceWidth / 7;
            action3.CornerRadius = (int)(deviceWidth / 14);
            action3.FontSize = deviceWidth / 45;
        }

        void goBackToGoals(System.Object sender, System.EventArgs e)
        {
            Navigation.PopAsync(false);
        }

        void goToSteps(System.Object sender, System.EventArgs e)
        {
            Button receiving = (Button)sender;
            if (receiving.Text != null && receiving.Text != "" && subOccDict[receiving.Text].instructions_steps.Count != 0)
                Navigation.PushAsync(new GoalStepsPage(subTitle.Text, subOccDict[receiving.Text]), false);
            else if (subOccDict[receiving.Text].instructions_steps.Count == 0)
                DisplayAlert("Oops", "there are no instructions available for this action", "OK");
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
            Application.Current.MainPage = new TodaysListPage();
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
            string url = RdsConfig.BaseUrl + RdsConfig.updateActionAndTask;
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
                    string parenturl = RdsConfig.BaseUrl + RdsConfig.updateGoalAndRoutine;
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
                    string parenturl = RdsConfig.BaseUrl + RdsConfig.updateGoalAndRoutine;
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
    }
}
