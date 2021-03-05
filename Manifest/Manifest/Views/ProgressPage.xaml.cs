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
        public string strokeColorB { get; set; }
        public bool fillB { get; set; }
        public string strokeColorC { get; set; }
        public bool fillC { get; set; }
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

        public ProgressPage()
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
            goalCompleted.Color = Color.FromHex((string)Application.Current.Properties["event"]);

            title.Text = "Progress";
            subTitle.Text = "Your progress";
            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();
            dates = new ObservableCollection<HeaderInfo>();
            rows = new ObservableCollection<ProgressRow>();
            GetUserProgress();
            GetUserRoutineProgress();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        async void GetUserProgress()
        {
            try
            {
                var client = new HttpClient();
                var startDate = DateTime.Now;
                var endDate = DateTime.Now;
                var userId = (string)Application.Current.Properties["userId"];
                for (int i = 7; i > 0; i--)
                {
                    startDate = startDate.AddDays(-1);
                }
                Debug.WriteLine("START: " + startDate);
                Debug.WriteLine("END: " + endDate);

                //dates.Add(new HeaderInfo() { name = "Goals", date = "" });
                for (int i = 0; i < 7; i++)
                {
                    var temp = startDate.AddDays(i);

                    dates.Add(new HeaderInfo() { name = temp.ToString("ddd"), date = temp.ToString("MM/dd") });
                }

                DatesList.ItemsSource = dates;

                client.DefaultRequestHeaders.Add("start-date", startDate.ToString("yyyy-MM-dd"));
                client.DefaultRequestHeaders.Add("end-date", endDate.ToString("yyyy-MM-dd"));

                var response = await client.GetAsync(AppConstants.BaseUrl + AppConstants.goalsHistory + userId);

                if (response.IsSuccessStatusCode)
                {

                    var data = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("HISTORY DATA: " + data);
                    var user = JsonConvert.DeserializeObject<History>(data);
                    Debug.WriteLine("HISTORY DATA: " + user.result);
                    var dates = JsonConvert.DeserializeObject<IDictionary<string, object>>(user.result.ToString());



                    var dateItems = new List<string>();
                    var goalsItems = new List<string>();

                    goalsItems.Clear();
                    foreach (string key in dates.Keys)
                    {
                        dateItems.Add(key);
                    }

                    foreach (string key in dates.Keys)
                    {
                        var subSet = JsonConvert.DeserializeObject<IDictionary<string, object>>(dates[key].ToString());
      
                        foreach (string subKey in subSet.Keys)
                        {
                            if (!goalsItems.Contains(subKey))
                            {
                                goalsItems.Add(subKey);
                            }
                        }
                    }

                    var grid = new List<List<int>>();
                    foreach (string key in dates.Keys)
                    {
                        var item = JsonConvert.DeserializeObject<IDictionary<string, object>>(dates[key].ToString());
                        var row = new List<int>();
                        foreach (string subKey in goalsItems)
                        {
                            var value = 0;
                            if (item.ContainsKey(subKey))
                            {
                                if (item[subKey].ToString() == "not started")
                                {
                                    value = -1;
                                }
                                else if (item[subKey].ToString() == "in_progress")
                                {
                                    value = 0;
                                }
                                else if (item[subKey].ToString() == "completed")
                                {
                                    value = 1;
                                }
                            }
                            else
                            {
                                value = -1;
                            }
                            row.Add(value);
                        }
                        grid.Add(row);
                    }

                    //Debug.WriteLine("ELEMENTS IN GRID: " + grid.Count);
                    //Debug.WriteLine("ELEMENTS IN GRID: " + grid[0].Count);

                    int j = 0;
                    rows.Clear();
                    foreach (string key in goalsItems)
                    {
                        var element = new ProgressRow();
                        element.name = key;

                        if(0 < grid.Count)
                        {
                            element.strokeColorA = StrokeFill(grid[0][j]);
                            element.fillA = Fill(grid[0][j]);
                        }

                        if (1 < grid.Count)
                        {
                            element.strokeColorB = StrokeFill(grid[1][j]);
                            element.fillB = Fill(grid[1][j]);
                        }

                        if (2 < grid.Count)
                        {
                            element.strokeColorC = StrokeFill(grid[2][j]);
                            element.fillC = Fill(grid[2][j]);
                        }

                        if (3 < grid.Count)
                        {
                            element.strokeColorD = StrokeFill(grid[3][j]);
                            element.fillD = Fill(grid[3][j]);
                        }

                        if (4 < grid.Count)
                        {
                            element.strokeColorE = StrokeFill(grid[4][j]);
                            element.fillE = Fill(grid[4][j]);
                        }

                        if (5 < grid.Count)
                        {
                            element.strokeColorF = StrokeFill(grid[5][j]);
                            element.fillF = Fill(grid[5][j]);
                        }

                        if (6 < grid.Count)
                        {
                            element.strokeColorG = StrokeFill(grid[6][j]);
                            element.fillG = Fill(grid[6][j]);
                        }

                        rows.Add(element);
                        j++;
                    }

                    GoalsStatusList.ItemsSource = rows;
                }
                else
                {
                    await DisplayAlert("Oops", "We weren't able to fulfill your request. Please try again later", "OK");
                }
            }catch(Exception progress)
            {
                await DisplayAlert("Oops", progress.Message, "OK");
            }
        }

        async void GetUserRoutineProgress()
        {
            try
            {
                var client = new HttpClient();
                var startDate = DateTime.Now;
                var endDate = DateTime.Now;
                var userId = (string)Application.Current.Properties["userId"];
                for (int i = 7; i > 0; i--)
                {
                    startDate = startDate.AddDays(-1);
                }
                Debug.WriteLine("START: " + startDate);
                Debug.WriteLine("END: " + endDate);

                //dates.Add(new HeaderInfo() { name = "Goals", date = "" });
                //for (int i = 0; i < 7; i++)
                //{
                //    var temp = startDate.AddDays(i);

                //    dates.Add(new HeaderInfo() { name = temp.ToString("ddd"), date = temp.ToString("MM/dd") });
                //}

                //DatesList.ItemsSource = dates;

                client.DefaultRequestHeaders.Add("start-date", startDate.ToString("yyyy-MM-dd"));
                client.DefaultRequestHeaders.Add("end-date", endDate.ToString("yyyy-MM-dd"));

                var response = await client.GetAsync(AppConstants.BaseUrl + AppConstants.routineHistory + userId);
                var data = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("ROUTINE DATA: " + data);
                //if (response.IsSuccessStatusCode)
                //{

                //    var data = await response.Content.ReadAsStringAsync();
                //    Debug.WriteLine("HISTORY DATA: " + data);
                //    var user = JsonConvert.DeserializeObject<History>(data);
                //    Debug.WriteLine("HISTORY DATA: " + user.result);
                //    var dates = JsonConvert.DeserializeObject<IDictionary<string, object>>(user.result.ToString());



                //    var dateItems = new List<string>();
                //    var goalsItems = new List<string>();


                //    foreach (string key in dates.Keys)
                //    {
                //        dateItems.Add(key);
                //    }

                //    foreach (string key in dates.Keys)
                //    {
                //        var subSet = JsonConvert.DeserializeObject<IDictionary<string, object>>(dates[key].ToString());

                //        foreach (string subKey in subSet.Keys)
                //        {
                //            if (!goalsItems.Contains(subKey))
                //            {
                //                goalsItems.Add(subKey);
                //            }
                //        }
                //    }

                //    var grid = new List<List<int>>();
                //    foreach (string key in dates.Keys)
                //    {
                //        var item = JsonConvert.DeserializeObject<IDictionary<string, object>>(dates[key].ToString());
                //        var row = new List<int>();
                //        foreach (string subKey in goalsItems)
                //        {
                //            var value = 0;
                //            if (item.ContainsKey(subKey))
                //            {
                //                if (item[subKey].ToString() == "not started")
                //                {
                //                    value = -1;
                //                }
                //                else if (item[subKey].ToString() == "in_progress")
                //                {
                //                    value = 0;
                //                }
                //                else if (item[subKey].ToString() == "completed")
                //                {
                //                    value = 1;
                //                }
                //            }
                //            else
                //            {
                //                value = -1;
                //            }
                //            row.Add(value);
                //        }
                //        grid.Add(row);
                //    }

                //    //Debug.WriteLine("ELEMENTS IN GRID: " + grid.Count);
                //    //Debug.WriteLine("ELEMENTS IN GRID: " + grid[0].Count);

                //    int j = 0;
                //    rows.Clear();
                //    foreach (string key in goalsItems)
                //    {
                //        var element = new ProgressRow();
                //        element.name = key;

                //        if (0 < grid.Count)
                //        {
                //            element.strokeColorA = StrokeFill(grid[0][j]);
                //            element.fillA = Fill(grid[0][j]);
                //        }

                //        if (1 < grid.Count)
                //        {
                //            element.strokeColorB = StrokeFill(grid[1][j]);
                //            element.fillB = Fill(grid[1][j]);
                //        }

                //        if (2 < grid.Count)
                //        {
                //            element.strokeColorC = StrokeFill(grid[2][j]);
                //            element.fillC = Fill(grid[2][j]);
                //        }

                //        if (3 < grid.Count)
                //        {
                //            element.strokeColorD = StrokeFill(grid[3][j]);
                //            element.fillD = Fill(grid[3][j]);
                //        }

                //        if (4 < grid.Count)
                //        {
                //            element.strokeColorE = StrokeFill(grid[4][j]);
                //            element.fillE = Fill(grid[4][j]);
                //        }

                //        if (5 < grid.Count)
                //        {
                //            element.strokeColorF = StrokeFill(grid[5][j]);
                //            element.fillF = Fill(grid[5][j]);
                //        }

                //        if (6 < grid.Count)
                //        {
                //            element.strokeColorG = StrokeFill(grid[6][j]);
                //            element.fillG = Fill(grid[6][j]);
                //        }

                //        rows.Add(element);
                //        j++;
                //    }

                //    RoutinesStatusList.ItemsSource = rows;
                //}
                //else
                //{
                //    await DisplayAlert("Oops", "We weren't able to fulfill your request. Please try again later", "OK");
                //}
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
                color = (string)Application.Current.Properties["event"];
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
