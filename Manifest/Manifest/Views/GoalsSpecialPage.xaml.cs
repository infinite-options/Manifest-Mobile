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

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
            calendarSwitch.IsToggled = (bool)Application.Current.Properties["showCalendar"];
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

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            if (setting == false)
            {
                // DISPLAY SETTINGS UI
                title.Text = "Settings";
                mainStackLayoutRow.Height = 0;
                settingStackLayoutRow.Height = height;
                barStackLayoutRow.Height = 70;
                setting = true;
            }
            else
            {
                // HIDE SETTINGS UI
                mainStackLayoutRow.Height = height;
                settingStackLayoutRow.Height = 0;
                barStackLayoutRow.Height = 0;
                setting = false;
            }
        }

        void Switch_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            if (calendarSwitch.IsToggled == false)
            {
                Debug.WriteLine("SET SHOW CALENDAR TO FALSE");
                Application.Current.Properties["showCalendar"] = false;
            }
            else
            {
                if ((bool)Application.Current.Properties["showCalendar"] == false)
                {
                    GoogleLogInClick();
                }
            }
        }

        public void GoogleLogInClick()
        {
            string clientId = string.Empty;
            string redirectUri = string.Empty;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = Constant.GoogleiOSClientID;
                    redirectUri = Constant.GoogleRedirectUrliOS;
                    break;

                case Device.Android:
                    clientId = Constant.GoogleAndroidClientID;
                    redirectUri = Constant.GoogleRedirectUrlAndroid;
                    break;
            }

            var authenticator = new OAuth2Authenticator(clientId, string.Empty, Constant.GoogleScope, new Uri(Constant.GoogleAuthorizeUrl), new Uri(redirectUri), new Uri(Constant.GoogleAccessTokenUrl), null, true);
            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();

            authenticator.Completed += GoogleAuthenticatorCompleted;
            authenticator.Error += GoogleAuthenticatorError;

            AuthenticationState.Authenticator = authenticator;
            presenter.Login(authenticator);
        }

        private async void GoogleAuthenticatorError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;

            if (authenticator != null)
            {
                authenticator.Completed -= GoogleAuthenticatorCompleted;
                authenticator.Error -= GoogleAuthenticatorError;
            }

            await DisplayAlert("Authentication error: ", e.Message, "OK");
        }

        private async void GoogleAuthenticatorCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Enter GoogleAuthenticatorCompleted");
            var authenticator = sender as OAuth2Authenticator;

            if (authenticator != null)
            {
                authenticator.Completed -= GoogleAuthenticatorCompleted;
                authenticator.Error -= GoogleAuthenticatorError;
            }

            if (e.IsAuthenticated)
            {
                Application.Current.Properties["showCalendar"] = true;
                Application.Current.Properties["accessToken"] = e.Account.Properties["access_token"];
                Application.Current.Properties["refreshToken"] = e.Account.Properties["refresh_token"];
            }
            else
            {
                await DisplayAlert("Error", "Google was not able to autheticate your account", "OK");
            }
        }

        void SetColorScheme(System.Object sender, System.EventArgs e)
        {
            var selectedFrame = (Frame)sender;
            Debug.WriteLine("Frame ClassId " + selectedFrame.ClassId);
            if (selectedFrame.ClassId == "retro")
            {
                retroScheme.BackgroundColor = Color.FromHex("#0C1E21");
                vibrantScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                coolScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                cottonScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                classicScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                retroLabel.TextColor = Color.FromHex("#FFFFFF");
                vibrantLabel.TextColor = Color.FromHex("#0C1E21");
                coolLabel.TextColor = Color.FromHex("#0C1E21");
                cottonLabel.TextColor = Color.FromHex("#0C1E21");
                classicLabel.TextColor = Color.FromHex("#0C1E21");
                SaveColorScheme(selectedFrame.ClassId, "#F4F9E9", "#153243", "#B4B8AB", "#EEF0EB", "#284B63", "#F5948D");
            }
            else if (selectedFrame.ClassId == "vibrant")
            {
                retroScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                vibrantScheme.BackgroundColor = Color.FromHex("#0C1E21");
                coolScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                cottonScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                classicScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                retroLabel.TextColor = Color.FromHex("#0C1E21");
                vibrantLabel.TextColor = Color.FromHex("#FFFFFF");
                coolLabel.TextColor = Color.FromHex("#0C1E21");
                cottonLabel.TextColor = Color.FromHex("#0C1E21");
                classicLabel.TextColor = Color.FromHex("#0C1E21");
                SaveColorScheme(selectedFrame.ClassId, "#FFFFFF", "#4DC4B6", "#F6A01F", "#CBF3F0", "#482728", "#F8C069");
            }
            else if (selectedFrame.ClassId == "cool")
            {
                retroScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                vibrantScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                coolScheme.BackgroundColor = Color.FromHex("#0C1E21");
                cottonScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                classicScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                retroLabel.TextColor = Color.FromHex("#0C1E21");
                vibrantLabel.TextColor = Color.FromHex("#0C1E21");
                coolLabel.TextColor = Color.FromHex("#FFFFFF");
                cottonLabel.TextColor = Color.FromHex("#0C1E21");
                classicLabel.TextColor = Color.FromHex("#0C1E21");
                SaveColorScheme(selectedFrame.ClassId, "#FDFDFD", "#03182B", "#93A0AF", "#A7EEFF", "#59A3B7", "#5AA6F5");
            }
            else if (selectedFrame.ClassId == "cotton")
            {
                retroScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                vibrantScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                coolScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                cottonScheme.BackgroundColor = Color.FromHex("#0C1E21");
                classicScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                retroLabel.TextColor = Color.FromHex("#0C1E21");
                vibrantLabel.TextColor = Color.FromHex("#0C1E21");
                coolLabel.TextColor = Color.FromHex("#0C1E21");
                cottonLabel.TextColor = Color.FromHex("#FFFFFF");
                classicLabel.TextColor = Color.FromHex("#0C1E21");
                SaveColorScheme(selectedFrame.ClassId, "#FCE4E0", "#F38375", "#EF6351", "#F59C9C", "#F6A399", "#7A5980");
            }
            else if (selectedFrame.ClassId == "classic")
            {
                retroScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                vibrantScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                coolScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                cottonScheme.BackgroundColor = Color.FromHex("#F2F7FC");
                classicScheme.BackgroundColor = Color.FromHex("#0C1E21");
                retroLabel.TextColor = Color.FromHex("#0C1E21");
                vibrantLabel.TextColor = Color.FromHex("#0C1E21");
                coolLabel.TextColor = Color.FromHex("#0C1E21");
                cottonLabel.TextColor = Color.FromHex("#0C1E21");
                classicLabel.TextColor = Color.FromHex("#FFFFFF");
                SaveColorScheme(selectedFrame.ClassId, "#F2F7FC", "#9DB2CB", "#376DAC", "#F8BE28", "#F26D4B", "#67ABFC");
            }
        }

        void LogOutClick(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new LogInPage();
        }

        void SaveColorScheme(string colorSchemeName, string backgroudColor, string headerColor, string navBarColor, string goalColor, string routineColor, string eventColor)
        {
            Application.Current.Properties["colorScheme"] = colorSchemeName;
            Application.Current.Properties["background"] = backgroudColor;
            Application.Current.Properties["header"] = headerColor;
            Application.Current.Properties["navBar"] = navBarColor;
            Application.Current.Properties["goal"] = goalColor;
            Application.Current.Properties["routine"] = routineColor;
            Application.Current.Properties["event"] = eventColor;

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
            logOutFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
        }

    }
}
