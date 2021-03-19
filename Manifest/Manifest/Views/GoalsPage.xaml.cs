using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Manifest.Config;
using Manifest.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Auth;
using Manifest.LogIn.Classes;
using Manifest.RDS;
using Xamarin.Forms.Shapes;

namespace Manifest.Views
{
    public partial class GoalsPage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        double deviceHeight = DeviceDisplay.MainDisplayInfo.Height;
        double deviceWidth = DeviceDisplay.MainDisplayInfo.Width;
        List<Occurance> currentOccurances;
        HttpClient client = new HttpClient();
        List<OccuranceDto> todayOccurs = new List<OccuranceDto>();
        List<Occurance> goalsInRange = new List<Occurance>();
        Dictionary<System.Object, Occurance> occuranceDict = new Dictionary<System.Object, Occurance>();
        Dictionary<System.Object, Occurance> occuranceFrameDict = new Dictionary<System.Object, Occurance>();
        public string startTime = "6:00 AM";
        public string endTime = "7:00 AM";
        Occurance chosenOccurance = null;
        int prevIndex = -1;
        Frame prevFrame = null;
        bool goalsExist = false;
        Color selectedBorderColor = Color.Blue;
        Color unselectedBorderColor = Color.FromHex("#FFBD27");

        public GoalsPage(string start, string end)
        {

            InitializeComponent();
            try
            {


                startTime = start;
                endTime = end;
                getGoals();

                currentOccurances = new List<Occurance>();
                //call the goals endpoint and fill the currentOccurances list with the returned goals

                //Debug.WriteLine("occuranceList count: " + occuranceList.Count.ToString());
                //fill the dictionary with the title of the occurance and the occurance itself
                //occuranceDict = new Dictionary<string, Occurance>();

                //foreach (Occurance occurance in occuranceList)
                //{
                //    occuranceDict.Add(occurance.Title, occurance);
                //}
                //temporary for testing


                setting = false;
                height = mainStackLayoutRow.Height;
                lastRowHeight = barStackLayoutRow.Height;

                mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
                frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
                nextButton.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

                //barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
                title.Text = "Goals";
                subTitle.Text = "Choose a goal to pursue";
                var helperObject = new MainPage();
                locationTitle.Text = (string)Application.Current.Properties["location"];
                dateTitle.Text = helperObject.GetCurrentTime();
                if (Device.RuntimePlatform == Device.iOS)
                {
                    titleGrid.Margin = new Thickness(0, 10, 0, 0);
                }
                NavigationPage.SetHasNavigationBar(this, false);
            }
            catch (Exception goalPage)
            {
                
            }
        }

        //void Button_Clicked(System.Object sender, System.EventArgs e)
        //{
        //    Navigation.PushAsync(new GoalsSpecialPage(),false);
        //}

        async void getGoals()
        {
            try
            {
                string url = AppConstants.BaseUrl + AppConstants.goalsActInstrUrl + "/" + Application.Current.Properties["userId"];
                currentOccurances = await RdsConnect.getOccurances(url);
                //var response = await client.GetStringAsync(url);

                //OccuranceResponse occuranceResponse = JsonConvert.DeserializeObject<OccuranceResponse>(response);
                setGoals();
            }
            catch (Exception goals)
            {

            }
        }

        //This function takes the response from the endpoint, and formats it into Occurances
        private async void setGoals()
        {
            try
            {
                if (startTime == endTime)
                {
                    foreach (Occurance dto in currentOccurances)
                    {
                        if (ToDateTime(dto.StartDayAndTime.ToString("t")).TimeOfDay <= ToDateTime(startTime).TimeOfDay && ToDateTime(dto.EndDayAndTime.ToString("t")).TimeOfDay >= ToDateTime(endTime).TimeOfDay)
                        {
                            Debug.WriteLine(dto.Title + " passes");
                            goalsInRange.Add(dto);
                        }
                    }
                }
                else
                {
                    foreach (Occurance dto in currentOccurances)
                    {
                        if (ToDateTime(dto.StartDayAndTime.ToString("t")).TimeOfDay >= ToDateTime(startTime).TimeOfDay && ToDateTime(dto.EndDayAndTime.ToString("t")).TimeOfDay <= ToDateTime(endTime).TimeOfDay)
                        {
                            Debug.WriteLine(dto.Title + " status: in progress: " + dto.IsInProgress + " completed: " + dto.IsComplete);
                            goalsInRange.Add(dto);
                        }

                    }
                }

                Debug.WriteLine("number of today goals: " + todayOccurs.Count.ToString());
                Debug.WriteLine("number of ranged goals: " + goalsInRange.Count.ToString());

                if (goalsInRange.Count == 0)
                {
                    var response = await DisplayAlert("Notice", "There are no goals schedule for you at this time. Press 'See Future Goals' to see upcoming goals or 'Return To Lobby.'", "See Future Goals", "Return To Lobby");
                    if (response)
                    {
                        foreach (Occurance dto in currentOccurances)
                        {
                            goalsInRange.Add(dto);
                        }
                        if (goalsInRange.Count == 0)
                        {
                            goalsExist = false;
                            await DisplayAlert("Oops", "Our records show that you have no future goals.", "OK");
                            Application.Current.MainPage = new MainPage();
                            return;
                        }
                    }
                    else
                    {
                        Application.Current.MainPage = new MainPage();
                        return;
                    }
                }
                goalsExist = true;
                if (goalsInRange.Count == 1)
                {
                    text1.Text = goalsInRange[0].Title;
                    occuranceDict.Add(text1.Text, goalsInRange[0]);

                    if (goalsInRange[0].IsInProgress)
                    {
                        goal1.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[0].IsComplete)
                    {
                        goal1.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal1.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    stack2.IsEnabled = false;
                    stack2.IsVisible = false;

                    stack3.IsEnabled = false;
                    stack3.IsVisible = false;

                    stack4.IsEnabled = false;
                    stack4.IsVisible = false;

                    stack5.IsEnabled = false;
                    stack5.IsVisible = false;

                    stack6.IsEnabled = false;
                    stack6.IsVisible = false;

                    stack7.IsEnabled = false;
                    stack7.IsVisible = false;
                }
                else if (goalsInRange.Count == 2)
                {
                    text1.Text = goalsInRange[0].Title;
                    occuranceDict.Add(text1.Text, goalsInRange[0]);

                    text2.Text = goalsInRange[1].Title;
                    occuranceDict.Add(text2.Text, goalsInRange[1]);

                    if(Device.RuntimePlatform == Device.iOS)
                    {
                        stack2.Margin = new Thickness(50, 0, 0, 0);
                    }
                    else
                    {
                        stack2.Margin = new Thickness(0.2, 0, 0, 0);
                    }

                    if (goalsInRange[0].IsInProgress)
                    {
                        goal1.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[0].IsComplete)
                    {
                        goal1.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal1.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[1].IsInProgress)
                    {
                        goal2.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[1].IsComplete)
                    {
                        goal2.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal2.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    stack3.IsEnabled = false;
                    stack3.IsVisible = false;

                    stack4.IsEnabled = false;
                    stack4.IsVisible = false;

                    stack5.IsEnabled = false;
                    stack5.IsVisible = false;

                    stack6.IsEnabled = false;
                    stack6.IsVisible = false;

                    stack7.IsEnabled = false;
                    stack7.IsVisible = false;
                }
                else if (goalsInRange.Count == 3)
                {
                    text1.Text = goalsInRange[0].Title;
                    occuranceDict.Add(text1.Text, goalsInRange[0]);

                    text2.Text = goalsInRange[1].Title;
                    occuranceDict.Add(text2.Text, goalsInRange[1]);

                    text3.Text = goalsInRange[2].Title;
                    occuranceDict.Add(text3.Text, goalsInRange[2]);

                    stack2.Margin = new Thickness(50, 0, 0, 0);

                    if (goalsInRange[0].IsInProgress)
                    {
                        goal1.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }else if (goalsInRange[0].IsComplete)
                    {
                        goal1.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal1.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[1].IsInProgress)
                    {
                        goal2.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[1].IsComplete)
                    {
                        goal2.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal2.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[2].IsInProgress)
                    {
                        goal3.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[2].IsComplete)
                    {
                        goal3.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal3.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    stack4.IsEnabled = false;
                    stack4.IsVisible = false;

                    stack5.IsEnabled = false;
                    stack5.IsVisible = false;

                    stack6.IsEnabled = false;
                    stack6.IsVisible = false;

                    stack7.IsEnabled = false;
                    stack7.IsVisible = false;
                }
                else if (goalsInRange.Count == 4)
                {
                    text1.Text = goalsInRange[0].Title;
                    occuranceDict.Add(text1.Text, goalsInRange[0]);

                    text2.Text = goalsInRange[1].Title;
                    occuranceDict.Add(text2.Text, goalsInRange[1]);

                    text3.Text = goalsInRange[2].Title;
                    occuranceDict.Add(text3.Text, goalsInRange[2]);

                    text4.Text = goalsInRange[3].Title;
                    occuranceDict.Add(text4.Text, goalsInRange[3]);

                    stack2.Margin = new Thickness(70, 0, 0, 0);

                    if (goalsInRange[0].IsInProgress)
                    {
                        goal1.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[0].IsComplete)
                    {
                        goal1.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal1.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[1].IsInProgress)
                    {
                        goal2.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[1].IsComplete)
                    {
                        goal2.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal2.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[2].IsInProgress)
                    {
                        goal3.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[2].IsComplete)
                    {
                        goal3.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal3.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[3].IsInProgress)
                    {
                        goal4.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[3].IsComplete)
                    {
                        goal4.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal4.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    stack5.IsEnabled = false;
                    stack5.IsVisible = false;

                    stack6.IsEnabled = false;
                    stack6.IsVisible = false;

                    stack7.IsEnabled = false;
                    stack7.IsVisible = false;
                }
                else if (goalsInRange.Count == 5)
                {
                    text1.Text = goalsInRange[0].Title;
                    occuranceDict.Add(text1.Text, goalsInRange[0]);

                    text2.Text = goalsInRange[1].Title;
                    occuranceDict.Add(text2.Text, goalsInRange[1]);

                    text3.Text = goalsInRange[2].Title;
                    occuranceDict.Add(text3.Text, goalsInRange[2]);

                    text4.Text = goalsInRange[3].Title;
                    occuranceDict.Add(text4.Text, goalsInRange[3]);

                    text5.Text = goalsInRange[4].Title;
                    occuranceDict.Add(text5.Text, goalsInRange[4]);

                    stack2.Margin = new Thickness(0,0,0,0);
                    stack5.Margin = new Thickness(15,5,0,0);

                    if (goalsInRange[0].IsInProgress)
                    {
                        goal1.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[0].IsComplete)
                    {
                        goal1.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal1.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[1].IsInProgress)
                    {
                        goal2.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[1].IsComplete)
                    {
                        goal2.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal2.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[2].IsInProgress)
                    {
                        goal3.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[2].IsComplete)
                    {
                        goal3.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal3.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[3].IsInProgress)
                    {
                        goal4.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[3].IsComplete)
                    {
                        goal4.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal4.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[4].IsInProgress)
                    {
                        goal5.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[4].IsComplete)
                    {
                        goal5.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal5.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    stack6.IsEnabled = false;
                    stack6.IsVisible = false;

                    stack7.IsEnabled = false;
                    stack7.IsVisible = false;
                }
                else if (goalsInRange.Count == 6)
                {
                    text1.Text = goalsInRange[0].Title;
                    occuranceDict.Add(text1.Text, goalsInRange[0]);

                    text2.Text = goalsInRange[1].Title;
                    occuranceDict.Add(text2.Text, goalsInRange[1]);

                    text3.Text = goalsInRange[2].Title;
                    occuranceDict.Add(text3.Text, goalsInRange[2]);

                    text4.Text = goalsInRange[3].Title;
                    occuranceDict.Add(text4.Text, goalsInRange[3]);

                    text5.Text = goalsInRange[4].Title;
                    occuranceDict.Add(text5.Text, goalsInRange[4]);

                    text6.Text = goalsInRange[5].Title;
                    occuranceDict.Add(text6.Text, goalsInRange[5]);

                    if (goalsInRange[0].IsInProgress)
                    {
                        goal1.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[0].IsComplete)
                    {
                        goal1.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal1.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[1].IsInProgress)
                    {
                        goal2.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[1].IsComplete)
                    {
                        goal2.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal2.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[2].IsInProgress)
                    {
                        goal3.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[2].IsComplete)
                    {
                        goal3.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal3.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[3].IsInProgress)
                    {
                        goal4.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[3].IsComplete)
                    {
                        goal4.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal4.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[4].IsInProgress)
                    {
                        goal5.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[4].IsComplete)
                    {
                        goal5.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal5.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[5].IsInProgress)
                    {
                        goal6.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[5].IsComplete)
                    {
                        goal6.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal6.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    stack7.IsEnabled = false;
                    stack7.IsVisible = false;
                }
                else if (goalsInRange.Count == 7)
                {
                    text1.Text = goalsInRange[0].Title;
                    occuranceDict.Add(text1.Text, goalsInRange[0]);

                    text2.Text = goalsInRange[1].Title;
                    occuranceDict.Add(text2.Text, goalsInRange[1]);
 
                    text3.Text = goalsInRange[2].Title;
                    occuranceDict.Add(text3.Text, goalsInRange[2]);

                    text4.Text = goalsInRange[3].Title;
                    occuranceDict.Add(text4.Text, goalsInRange[3]);

                    text5.Text = goalsInRange[4].Title;
                    occuranceDict.Add(text5.Text, goalsInRange[4]);

                    text6.Text = goalsInRange[5].Title;
                    occuranceDict.Add(text6.Text, goalsInRange[5]);

                    text7.Text = goalsInRange[6].Title;
                    occuranceDict.Add(text7.Text, goalsInRange[6]);

                    if (goalsInRange[0].IsInProgress)
                    {
                        goal1.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[0].IsComplete)
                    {
                        goal1.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal1.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[1].IsInProgress)
                    {
                        goal2.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[1].IsComplete)
                    {
                        goal2.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal2.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[2].IsInProgress)
                    {
                        goal3.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[2].IsComplete)
                    {
                        goal3.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal3.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[3].IsInProgress)
                    {
                        goal4.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[3].IsComplete)
                    {
                        goal4.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal4.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[4].IsInProgress)
                    {
                        goal5.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[3].IsComplete)
                    {
                        goal5.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal5.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[5].IsInProgress)
                    {
                        goal6.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[5].IsComplete)
                    {
                        goal6.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal6.Fill = new SolidColorBrush(Color.Transparent);
                    }

                    if (goalsInRange[6].IsInProgress)
                    {
                        goal7.Fill = new SolidColorBrush(Color.FromHex("#FFE07E"));
                    }
                    else if (goalsInRange[6].IsComplete)
                    {
                        goal7.Fill = new SolidColorBrush(Color.FromHex("#D3E6D3"));
                    }
                    else
                    {
                        goal7.Fill = new SolidColorBrush(Color.Transparent);
                    }
                }

                gridDisplay.IsVisible = true;
                return;
            }
            catch (Exception e)
            {
                DisplayAlert("Alert", "Error in TodaysListTest ToOccurances(). Error: " + e.ToString(), "OK");
            }
        }



        //This function convert a string to a DateTime
        private DateTime ToDateTime(string dateString)
        {
            try
            {
                Debug.WriteLine("inToDateTime: " + dateString);
                return DateTime.Parse(dateString);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in ToDateTime function in TodaysList class. " + e.ToString());
                Debug.WriteLine(dateString);
            }
            return new DateTime();
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SettingsPage(), false);
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            int navigationStackCount = Application.Current.MainPage.Navigation.NavigationStack.Count;
            Debug.WriteLine("navigationStackCount: " + navigationStackCount.ToString());

            if (navigationStackCount > 2)
            {
                Navigation.PopToRootAsync(false);
            }
            else if (navigationStackCount != 1)
            {
                Navigation.PopAsync(false);
                
            }
            else
            {
                Application.Current.MainPage = new MainPage();
            }
        }

        void navigatetoTodaysList(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new TodaysListPage();
        }

        async void nextClicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                Debug.WriteLine(chosenOccurance.Title + ", InProgress = " + chosenOccurance.IsInProgress.ToString() + ", IsComplete = " + chosenOccurance.IsComplete.ToString());
                if (chosenOccurance == null)
                {
                    await DisplayAlert("Note", "Please select a goal.", "OK");
                    return;
                }

                if (goalsExist == false) return;
                //if a goal has only one subtask, navigate directly to steps page
                if (chosenOccurance != null && chosenOccurance.IsSublistAvailable == false)
                {
                    string url2 = AppConstants.BaseUrl + AppConstants.updateGoalAndRoutine;
                    if (!chosenOccurance.IsComplete)
                    {
                        bool complete = await DisplayAlert("Note", "This Goal has no subtasks. Click OK if you want to proceed and finish the goal", "Cancel", "OK");
                        if (chosenOccurance.IsInProgress == true)
                        {
                            await RdsConnect.updateOccurance(chosenOccurance, false, true, url2);
                        }
                        else if (chosenOccurance.IsInProgress == false && chosenOccurance.IsComplete == false)
                        {
                            await RdsConnect.updateOccurance(chosenOccurance, true, false, url2);
                        }
                    }
                    else
                    {
                        bool reset = await DisplayAlert("Note", "This Goal has no subtasks, and it has already been completed. Click OK if you want to reset it ", "Cancel", "OK");
                        if (!reset)
                        {
                            await RdsConnect.updateOccurance(chosenOccurance, false, false, url2);
                        }
                    }
                }
                else if (chosenOccurance != null && chosenOccurance.IsSublistAvailable == true && chosenOccurance.subOccurances.Count == 1 && chosenOccurance.IsComplete == false)
                {
                    //var nextPage = new GoalStepsPage(chosenOccurance, chosenOccurance.subOccurances[0], "#F8BE28");
                    
                    await Navigation.PushAsync(new GoalStepsPage(chosenOccurance, chosenOccurance.subOccurances[0], "#F8BE28"),false);
                }
                else if (chosenOccurance != null && chosenOccurance.IsSublistAvailable == true)
                    await Navigation.PushAsync(new GoalsSpecialPage(chosenOccurance),false);
                //else if (chosenOccurance != null && chosenOccurance.IsSublistAvailable == false)
                //    await DisplayAlert("Error", "this goal doesn't have subtasks", "OK");
                else await DisplayAlert("Oops", "please select a goal first", "OK");
            }
            catch(Exception goals)
            {
                await DisplayAlert("Oops",goals.Message,"OK");
            }
        }



        //only one used when the user clicks a goal circle
        void navigatetoActionsFrame(System.Object sender, System.EventArgs e)
        {
            var myStack = (StackLayout)sender;
            var key = "";
            if(myStack.ClassId == "1")
            {
                key = text1.Text;
                goal1.Stroke = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["header"]));
                goal2.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal3.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal4.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal5.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal6.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal7.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
            }
            else if (myStack.ClassId == "2")
            {
                key = text2.Text;
                goal1.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal2.Stroke = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["header"]));
                goal3.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal4.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal5.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal6.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal7.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
            }
            else if (myStack.ClassId == "3")
            {
                key = text3.Text;
                goal1.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal2.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal3.Stroke = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["header"]));
                goal4.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal5.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal6.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal7.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
            }
            else if (myStack.ClassId == "4")
            {
                key = text4.Text;
                goal1.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal2.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal3.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal4.Stroke = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["header"]));
                goal5.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal6.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal7.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
            }
            else if (myStack.ClassId == "5")
            {
                key = text5.Text;
                goal1.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal2.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal3.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal4.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal5.Stroke = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["header"]));
                goal6.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal7.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
            }
            else if (myStack.ClassId == "6")
            {
                key = text6.Text;
                goal1.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal2.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal3.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal4.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal5.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal6.Stroke = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["header"]));
                goal7.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
            }
            else if (myStack.ClassId == "7")
            {
                key = text7.Text;
                goal1.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal2.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal3.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal4.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal5.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal6.Stroke = new SolidColorBrush(Color.FromHex("#FFBD27"));
                goal7.Stroke = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["header"]));
            }
            try
            {
                chosenOccurance = occuranceDict[key];
            }
            catch (Exception keyIssue)
            {
                Debug.WriteLine(keyIssue.Message);
            }
        }
    }
}
