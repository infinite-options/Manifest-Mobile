using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manifest.Config;
using Manifest.LogIn.Classes;
using Xamarin.Auth;
using Xamarin.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Manifest.Views
{
    public class History
    {
        public string message { get; set; }
        public object result { get; set; }
    }

    public class HeaderInfo
    {
        public string name { get; set; }
        public string date { get; set; }
    }

    public class ProgressRow
    {
        public string name { get; set; }
        public string strokeColorA {get;set;}
        public bool fillA { get; set; }
        public bool displayA { get; set; }
        public string strokeColorB { get; set; }
        public bool fillB { get; set; }
        public bool displayB { get; set; }
        public string strokeColorC { get; set; }
        public bool fillC { get; set; }
        public bool displayC { get; set; }
        public string strokeColorD { get; set; }
        public bool fillD { get; set; }
        public string strokeColorE { get; set; }
        public bool fillE { get; set; }
        public string strokeColorF { get; set; }
        public bool fillF { get; set; }
        public string strokeColorG { get; set; }
        public bool fillG { get; set; }
    }

    public partial class ProgressPage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        ObservableCollection<HeaderInfo> dates;
        ObservableCollection<ProgressRow> rows;
        string goalId;

        public ProgressPage(string goalId, string goalTitle)
        {
            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);


            goalNotStarted.Color = Color.FromHex((string)Application.Current.Properties["goal"]);
            goalStarted.Color = Color.FromHex((string)Application.Current.Properties["routine"]);
            goalCompleted.Color = Color.FromHex((string)Application.Current.Properties["routine"]);

            title.Text = "Progress";
            subTitle.Text = goalTitle;
            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();
            dates = new ObservableCollection<HeaderInfo>();
            rows = new ObservableCollection<ProgressRow>();
            if (Device.RuntimePlatform == Device.iOS)
            {
                titleGrid.Margin = new Thickness(0, 10, 0, 0);
            }
            GetGoalProgress(goalId);
            NavigationPage.SetHasNavigationBar(this, false);
        }


        //async void GetGoalProgress(string id)
        //{
        //    try
        //    {
        //        var client = new HttpClient();
        //        var startDate = DateTime.Now;
        //        var endDate = DateTime.Now;
        //        var userId = (string)Application.Current.Properties["userId"];
        //        for (int i = 6; i > 0; i--)
        //        {
        //            startDate = startDate.AddDays(-1);
        //        }
        //        Debug.WriteLine("START: " + startDate);
        //        Debug.WriteLine("END: " + endDate);

        //        //dates.Add(new HeaderInfo() { name = "Goals", date = "" });
        //        var arrayKeys = new List<string>();
        //        for (int i = 0; i < 7; i++)
        //        {
        //            var temp = startDate.AddDays(i);
        //            arrayKeys.Add(temp.ToString("yyyy-MM-dd"));
        //        }

        //        //DatesList.ItemsSource = dates;

        //        client.DefaultRequestHeaders.Add("start-date", startDate.ToString("yyyy-MM-dd"));
        //        client.DefaultRequestHeaders.Add("end-date", endDate.ToString("yyyy-MM-dd"));
        //        client.DefaultRequestHeaders.Add("goal_routine_id", id);
        //        var response = await client.GetAsync(AppConstants.BaseUrl + AppConstants.particularGoalHistory + userId);

        //        var actionNames = new Dictionary<string, string>();
        //        var actionNameArray = new List<string>();

        //        if (response.IsSuccessStatusCode)
        //        {

        //            var data = await response.Content.ReadAsStringAsync();

        //            var user = JsonConvert.DeserializeObject<History>(data);
        //            Debug.WriteLine("GOALS DATA: " + user.result);
        //            var dates = JsonConvert.DeserializeObject<IDictionary<string, object>>(user.result.ToString());
        //            var date = DateTime.Now;
        //            foreach (string key in arrayKeys)
        //            {
        //                var actionsArray = new List<int>();

        //                for (int i = 0; i < 3; i++)
        //                {
        //                    actionsArray.Add(-1);
        //                }

        //                if (dates.ContainsKey(key))
        //                {
        //                    var value = JsonConvert.DeserializeObject<IDictionary<string, object>>(dates[key].ToString());
        //                    if (value.ContainsKey("actions"))
        //                    {

        //                        var actions = JsonConvert.DeserializeObject<IDictionary<string, object>>(value["actions"].ToString());

        //                        var j = 0;
        //                        foreach(string subKey in actions.Keys)
        //                        {
        //                            if (!actionNames.ContainsKey(subKey))
        //                            {
        //                                actionNames.Add(subKey, "");
        //                                actionNameArray.Add(subKey);

        //                            }
        //                            if (actions[subKey].ToString() == "not started")
        //                            {
        //                                actionsArray[j] = -1;
        //                            }
        //                            else if (actions[subKey].ToString() == "in_progress")
        //                            {
        //                                actionsArray[j] = 0;
        //                            }
        //                            else if (actions[subKey].ToString() == "completed")
        //                            {
        //                                actionsArray[j] = 1;
        //                            }
        //                            j++;
        //                        }
        //                    }
        //                }

        //                rows.Add(new ProgressRow()
        //                {
        //                    name = date.DayOfWeek.ToString(),
        //                    strokeColorA = StrokeFill(actionsArray[0]),
        //                    fillA = Fill(actionsArray[0]),
        //                    strokeColorB = StrokeFill(actionsArray[1]),
        //                    fillB = Fill(actionsArray[1]),
        //                    strokeColorC = StrokeFill(actionsArray[2]),
        //                    fillC = Fill(actionsArray[2]),
        //                });
        //                date = date.AddDays(1);
        //            }


        //            if (0 < actionNameArray.Count)
        //            {
        //                actionA.Text = actionNameArray[0];
        //            }

        //            if (1 < actionNameArray.Count)
        //            {
        //                actionB.Text = actionNameArray[1];
        //            }


        //            if (2 < actionNameArray.Count)
        //            {
        //                actionC.Text = actionNameArray[2];
        //            }


        //            GoalsStatusList.ItemsSource = rows;

        //            //var dateItems = new List<string>();
        //            //var goalsItems = new List<string>();

        //            //goalsItems.Clear();
        //            //foreach (string key in dates.Keys)
        //            //{
        //            //    dateItems.Add(key);
        //            //}

        //            //foreach (string key in dates.Keys)
        //            //{
        //            //    var subSet = JsonConvert.DeserializeObject<IDictionary<string, object>>(dates[key].ToString());

        //            //    foreach (string subKey in subSet.Keys)
        //            //    {
        //            //        if (!goalsItems.Contains(subKey))
        //            //        {
        //            //            goalsItems.Add(subKey);
        //            //        }
        //            //    }
        //            //}

        //            //var grid = new List<List<int>>();
        //            //foreach (string key in dates.Keys)
        //            //{
        //            //    var item = JsonConvert.DeserializeObject<IDictionary<string, object>>(dates[key].ToString());
        //            //    var row = new List<int>();
        //            //    foreach (string subKey in goalsItems)
        //            //    {
        //            //        var value = 0;
        //            //        if (item.ContainsKey(subKey))
        //            //        {
        //            //            if (item[subKey].ToString() == "not started")
        //            //            {
        //            //                value = -1;
        //            //            }
        //            //            else if (item[subKey].ToString() == "in_progress")
        //            //            {
        //            //                value = 0;
        //            //            }
        //            //            else if (item[subKey].ToString() == "completed")
        //            //            {
        //            //                value = 1;
        //            //            }
        //            //        }
        //            //        else
        //            //        {
        //            //            value = -1;
        //            //        }
        //            //        row.Add(value);
        //            //    }
        //            //    grid.Add(row);
        //            //}

        //            ////Debug.WriteLine("ELEMENTS IN GRID: " + grid.Count);
        //            ////Debug.WriteLine("ELEMENTS IN GRID: " + grid[0].Count);

        //            //int j = 0;
        //            //rows.Clear();
        //            //foreach (string key in goalsItems)
        //            //{
        //            //    var element = new ProgressRow();
        //            //    element.name = key;

        //            //    if (0 < grid.Count)
        //            //    {
        //            //        element.strokeColorA = StrokeFill(grid[0][j]);
        //            //        element.fillA = Fill(grid[0][j]);
        //            //    }

        //            //    if (1 < grid.Count)
        //            //    {
        //            //        element.strokeColorB = StrokeFill(grid[1][j]);
        //            //        element.fillB = Fill(grid[1][j]);
        //            //    }

        //            //    if (2 < grid.Count)
        //            //    {
        //            //        element.strokeColorC = StrokeFill(grid[2][j]);
        //            //        element.fillC = Fill(grid[2][j]);
        //            //    }

        //            //    if (3 < grid.Count)
        //            //    {
        //            //        element.strokeColorD = StrokeFill(grid[3][j]);
        //            //        element.fillD = Fill(grid[3][j]);
        //            //    }

        //            //    if (4 < grid.Count)
        //            //    {
        //            //        element.strokeColorE = StrokeFill(grid[4][j]);
        //            //        element.fillE = Fill(grid[4][j]);
        //            //    }

        //            //    if (5 < grid.Count)
        //            //    {
        //            //        element.strokeColorF = StrokeFill(grid[5][j]);
        //            //        element.fillF = Fill(grid[5][j]);
        //            //    }

        //            //    if (6 < grid.Count)
        //            //    {
        //            //        element.strokeColorG = StrokeFill(grid[6][j]);
        //            //        element.fillG = Fill(grid[6][j]);
        //            //    }

        //            //    rows.Add(element);
        //            //    j++;
        //            //}

        //            //GoalsStatusList.ItemsSource = rows;
        //        }
        //        else
        //        {
        //            await DisplayAlert("Oops", "We weren't able to fulfill your request. Please try again later", "OK");
        //        }
        //    }
        //    catch (Exception progress)
        //    {
        //        await DisplayAlert("Oops", progress.Message, "OK");
        //    }
        //}


        async void GetGoalProgress(string id)
        {
            try
            {
                Debug.WriteLine("ID GOAL: " + id);
                var client = new HttpClient();
                var startDate = DateTime.Now;
                var endDate = DateTime.Now;
                var userId = (string)Application.Current.Properties["userId"];
                for (int i = 6; i > 0; i--)
                {
                    startDate = startDate.AddDays(-1);
                }
                Debug.WriteLine("START: " + startDate);
                Debug.WriteLine("END: " + endDate);

                var startDateToDisplay = DateTime.Now;
                //var arrayKeys = new List<string>();
                //for (int i = 7; i > 0; i--)
                //{
                    
                //    Debug.WriteLine("DATES: " + startDateToDisplay.ToString("yyyy-MM-dd"));
                //    arrayKeys.Add(startDateToDisplay.ToString("yyyy-MM-dd"));
                //    startDateToDisplay = startDateToDisplay.AddDays(-1);
                //}

                //DatesList.ItemsSource = dates;

                Debug.WriteLine("INPUT 1:" + startDate.ToString("yyyy-MM-dd"));

                Debug.WriteLine("INPUT 2:" + endDate.ToString("yyyy-MM-dd"));

                client.DefaultRequestHeaders.Add("start_date", startDate.ToString("yyyy-MM-dd"));
                client.DefaultRequestHeaders.Add("end_date", endDate.ToString("yyyy-MM-dd"));
                client.DefaultRequestHeaders.Add("goal_routine_id", id);

                var response = await client.GetAsync(AppConstants.BaseUrl + AppConstants.particularGoalHistory + userId);
                var actionNames = new Dictionary<string, string>();
                var actionNameArray = new List<string>();
                actionNameArray.Add("");
                actionNameArray.Add("");
                actionNameArray.Add("");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<History>(data);
                    Debug.WriteLine("GOALS DATA: " + user.result);
                    var dates = JsonConvert.DeserializeObject<IDictionary<string, object>>(user.result.ToString());
                    var dataDictionary = new Dictionary<string, object>();

                    //STEP 1:GET KEYS.
                    var arrayKeys = new List<DateTime>();
                    int count = 7;
                    foreach(string key in dates.Keys)
                    {
                        arrayKeys.Add(DateTime.Parse(key));
                        count--;
                    }

                    arrayKeys.Sort();

                    if(arrayKeys.Count != 0)
                    {
                        var firstDate = arrayKeys[0];
                        for (int i = count; i > 0; i--)
                        {
                            firstDate = firstDate.AddDays(-1);
                            arrayKeys.Add(firstDate);
                        }
                        arrayKeys.Sort();
                        var date = DateTime.Now;
                        var indexSubKeys = 0;
                        var first = false;
                        for(int key = 6; key >= 0; key--)
                        {
                            var actionsArray = new List<int>();

                            for (int i = 0; i < 3; i++)
                            {
                                actionsArray.Add(-1);
                            }
                            Debug.WriteLine("Key: " + arrayKeys[key]);
                            if (dates.ContainsKey(arrayKeys[key].ToString("yyyy-MM-dd")))
                            {
                                var value = JsonConvert.DeserializeObject<IDictionary<string, object>>(dates[arrayKeys[key].ToString("yyyy-MM-dd")].ToString());
                                if (value.ContainsKey("actions"))
                                {
                                    if (value["actions"] != null && value.Count ==2)
                                    {
                                        var actions = JsonConvert.DeserializeObject<IDictionary<string, object>>(value["actions"].ToString());
                                        var indexAction = 0;
                                        foreach (string subKey in actions.Keys)
                                        {
                                            if (!actionNames.ContainsKey(subKey))
                                            {
                                                actionNames.Add(subKey, "");
                                                if(indexSubKeys < 3)
                                                {
                                                    actionNameArray[indexSubKeys] = subKey;
                                                    indexSubKeys++;
                                                }
                                            }
                                            if (actions[subKey].ToString() == "not started")
                                            {
                                                if (indexAction < 3)
                                                {
                                                    actionsArray[indexAction] = -1;
                                                }
                                            }
                                            else if (actions[subKey].ToString() == "in_progress")
                                            {
                                                if (indexAction < 3)
                                                {
                                                    actionsArray[indexAction] = 0;
                                                }
                                            }
                                            else if (actions[subKey].ToString() == "completed")
                                            {
                                                if (indexAction < 3)
                                                {
                                                    actionsArray[indexAction] = 1;
                                                }
                                                
                                            }
                                            indexAction++;
                                        }
                                    }
                                }
                            }

                            var dateLabel = "";

                            if (!first)
                            {
                                dateLabel = "Today";
                                first = true;
                            }
                            else
                            {
                                dateLabel = date.DayOfWeek.ToString();
                            }
                            rows.Add(new ProgressRow()
                            {
                                name = dateLabel,
                                strokeColorA = StrokeFill(actionsArray[0]),
                                fillA = Fill(actionsArray[0]),
                                displayA = Display(actionNameArray[0]),
                                strokeColorB = StrokeFill(actionsArray[1]),
                                fillB = Fill(actionsArray[1]),
                                displayB = Display(actionNameArray[1]),
                                strokeColorC = StrokeFill(actionsArray[2]),
                                fillC = Fill(actionsArray[2]),
                                displayC = Display(actionNameArray[2]),
                            });

                            date = date.AddDays(-1);
                        }

                        if (0 < actionNameArray.Count)
                        {
                            actionA.Text = actionNameArray[0];
                        }

                        if (1 < actionNameArray.Count)
                        {
                            actionB.Text = actionNameArray[1];
                        }

                        if (2 < actionNameArray.Count)
                        {
                            actionC.Text = actionNameArray[2];
                        }

                        GoalsStatusList.ItemsSource = rows;
                    }
                    else
                    {
                        //Display an alert saying there is no data or display an empty grid.
                        await DisplayAlert("Oops", "Progress not available. Complete a goal to see your progress", "OK");
                        return;
                    }


                    //foreach(string key in dates.Keys)
                    //{
                    //    DateTime today = DateTime.Parse(key);
                    //    var localTime = today.ToLocalTime();
                    //    Debug.WriteLine("LOCAL TIME: " + localTime);
                    //    dataDictionary.Add(localTime.ToString("yyyy-MM-dd"), dates[key]);
                    //}

                }
                else
                {
                    await DisplayAlert("Oops", "We weren't able to fulfill your request. Please try again later", "OK");
                }
            }
            catch (Exception progress)
            {
                await DisplayAlert("Oops", progress.Message, "OK");
            }
        }


        bool Fill(int val)
        {
            bool color = false;
            if(val == -1)
            {
                color = false;
            }else if (val == 0)
            {
                color = false;
            }else if (val == 1)
            {
                color = true;
            }
            return color;
        }

        bool Display(string val)
        {
            if( val != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        string StrokeFill(int val)
        {
            string color = "#00000059";
            if (val == -1)
            {
                color = (string)Application.Current.Properties["goal"];
            }
            else if (val == 0)
            {
                color = (string)Application.Current.Properties["routine"];
            }
            else if (val == 1)
            {
                color = (string)Application.Current.Properties["routine"];
            }
            return color;
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Navigation.PopAsync(false);
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SettingsPage(), false);
        }
    }
}
