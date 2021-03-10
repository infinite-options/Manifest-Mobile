using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using Manifest.Config;
using Manifest.Models;
using Manifest.RDS;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Manifest.Views
{
    public class Options
    {
        public string color { get; set; }
        public double height { get; set; }
        public string name { get; set; }


    }

    public partial class WhatIsImportantToMePage : ContentPage
    {
        public Models.User user;
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        string city;
        string time;
        string startDate;
        string endDate;
        ObservableCollection<HeaderInfo> dates;
        ObservableCollection<ProgressRow> rows;

        ObservableCollection<ProgressRow> routinesRow;

        ObservableCollection<Options> importantItems;
        ObservableCollection<Options> happyItems;
        ObservableCollection<Options> motivationItems;

        DateTime start;

        public WhatIsImportantToMePage()
        {
            InitializeComponent();
            setting = false;
            importantItems = new ObservableCollection<Options>();
            happyItems = new ObservableCollection<Options>();
            motivationItems = new ObservableCollection<Options>();
            dates = new ObservableCollection<HeaderInfo>();
            rows = new ObservableCollection<ProgressRow>();
            routinesRow = new ObservableCollection<ProgressRow>();
            GetUserProgress();
            GetUserRoutineProgress();

            goalNotStarted.Color = Color.FromHex((string)Application.Current.Properties["goal"]);
            goalStarted.Color = Color.FromHex((string)Application.Current.Properties["routine"]);
            goalCompleted.Color = Color.FromHex((string)Application.Current.Properties["routine"]);

            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            scheduleFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            lobbyFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            supportFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);


            if(Device.RuntimePlatform == Device.iOS)
            {
                devices.Width = 135;
            }
            else
            {
                devices.Width = 180;
            }

            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = GetCurrentTime();
            title.Text = "My Values";
            var userId = (string)Application.Current.Properties["userId"];
            initializeUser(userId);


        }

        private async void initializeUser(string uid)
        {
            try
            {
                string res = await RdsConnect.getUser(uid);
                if (res == "Failure")
                {
                    await DisplayAlert("Alert", "Error in getUser() in initializeUser() in AboutMePage", "OK");
                    return;
                }
                Debug.WriteLine("ABOUT ME: " + res);
                UserResponse userResponse = JsonConvert.DeserializeObject<UserResponse>(res);
                ToUser(userResponse);

                var startDate = DateTime.Parse(user.datetime);
                var startDateCopy = DateTime.Parse(user.datetime);
                var endDate = DateTime.Now;

                startDateLabel.Text = startDate.ToString("MM/dd/yyyy");
                endDateLabel.Text = endDate.ToString("MM/dd/yyyy");

                int totalDays = -1;
                while (startDateCopy < endDate)
                {
                    startDateCopy = startDateCopy.AddDays(1);
                    totalDays++;
                }

                //var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                //var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                start = startDate;
                rangeSliderAge.MinimumValue = 0;
                rangeSliderAge.LowerValue = 0;
                rangeSliderAge.MaximumValue = (float)totalDays;
                rangeSliderAge.UpperValue = (float)totalDays;

                GetHistoryData(startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            }
            catch (Exception a)
            {

            }

        }

        private void ToUser(UserResponse userResponse)
        {
            if (userResponse.result != null)
            {
                foreach (UserDto dto in userResponse.result)
                {
                    if (dto.user_email_id != null)
                    {
                        user = new Models.User()
                        {
                            FirstName = dto.user_first_name,
                            LastName = dto.user_last_name,
                            Email = dto.user_email_id,
                            HavePic = DataParser.ToBool(dto.have_pic),
                            PicUrl = dto.user_picture,
                            MessageCard = dto.message_card,
                            MessageDay = dto.message_day,
                            MajorEvents = dto.user_major_events,
                            MyHistory = dto.user_history,
                            TimeSettings = ToTimeSettings(dto),
                            user_birth_date = dto.user_birth_date,
                            datetime = dto.datetime,
                        };
                        break;
                    }
                }
            }
        }

        private TimeSettings ToTimeSettings(UserDto dto)
        {
            TimeSettings timeSettings = new TimeSettings();
            timeSettings.TimeZone = dto.time_zone;
            timeSettings.MorningStartTime = DataParser.ToTimeSpan(dto.morning_time);
            timeSettings.AfterNoonStartTime = DataParser.ToTimeSpan(dto.afternoon_time);
            timeSettings.EveningStartTime = DataParser.ToTimeSpan(dto.evening_time);
            timeSettings.NightStartTime = DataParser.ToTimeSpan(dto.night_time);
            timeSettings.DayStart = DataParser.ToTimeSpan(dto.day_start);
            timeSettings.DayEnd = DataParser.ToTimeSpan(dto.day_end);
            return timeSettings;
        }

        void rangeSliderAge_DragCompleted(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine("DRAG IS COMPLETED");

            var startDate = start.AddDays(rangeSliderAge.LowerValue);
            var endDate = start.AddDays(rangeSliderAge.UpperValue);

            startDateLabel.Text = startDate.ToString("MM/dd/yyyy");
            endDateLabel.Text = endDate.ToString("MM/dd/yyyy");

            GetHistoryData(startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
        }

        async void GetUserProgress()
        {
            try
            {
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
                dates.Clear();
                //dates.Add(new HeaderInfo() { name = "Goals", date = "" });
                for (int i = 0; i < 7; i++)
                {
                    var temp = startDate.AddDays(i);
                    if(i != 6)
                    {
                        dates.Add(new HeaderInfo() { name = temp.ToString("ddd"), date = temp.ToString("MM/dd") });
                    }
                    else
                    {
                        dates.Add(new HeaderInfo() { name = "Today", date = temp.ToString("MM/dd") });
                    }
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

                    var height = 0;

                    foreach (string key in dates.Keys)
                    {
                        var subSet = JsonConvert.DeserializeObject<IDictionary<string, object>>(dates[key].ToString());

                        foreach (string subKey in subSet.Keys)
                        {
                            if (!goalsItems.Contains(subKey))
                            {
                                goalsItems.Add(subKey);
                                height = height + 30;
                            }
                        }
                    }
                    GoalsStatusList.HeightRequest = height;

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

                        if (0 < grid.Count)
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
            }
            catch (Exception progress)
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
                for (int i = 6; i > 0; i--)
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


                client.DefaultRequestHeaders.Add("start-date", startDate.ToString("yyyy-MM-dd"));
                client.DefaultRequestHeaders.Add("end-date", endDate.ToString("yyyy-MM-dd"));

                var response = await client.GetAsync(AppConstants.BaseUrl + AppConstants.routineHistory + userId);

                if (response.IsSuccessStatusCode)
                {

                    var data = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("HISTORY DATA: " + data);
                    var user = JsonConvert.DeserializeObject<History>(data);
                    Debug.WriteLine("HISTORY DATA: " + user.result);
                    var dates = JsonConvert.DeserializeObject<IDictionary<string, object>>(user.result.ToString());



                    var dateItems = new List<string>();
                    var goalsItems = new List<string>();

                    
                    foreach (string key in dates.Keys)
                    {
                        dateItems.Add(key);
                    }

                    var height = 0;

                    foreach (string key in dates.Keys)
                    {
                        var subSet = JsonConvert.DeserializeObject<IDictionary<string, object>>(dates[key].ToString());

                        foreach (string subKey in subSet.Keys)
                        {
                            if (!goalsItems.Contains(subKey))
                            {
                                goalsItems.Add(subKey);
                                height = height + 30;
                            }
                        }
                    }

                    RoutinesStatusList.HeightRequest = height;

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

                    foreach (string key in goalsItems)
                    {
                        var element = new ProgressRow();
                        element.name = key;

                        if (0 < grid.Count)
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

                        routinesRow.Add(element);
                        j++;
                    }

                    RoutinesStatusList.ItemsSource = routinesRow;
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
            if (val == -1)
            {
                color = false;
            }
            else if (val == 0)
            {
                color = false;
            }
            else if (val == 1)
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
                color = (string)Application.Current.Properties["routine"];
            }
            return color;
        }

        public async void GetHistoryData(string startDate = null, string endDate = null)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("start-date", startDate);
                client.DefaultRequestHeaders.Add("end-date", endDate);

                var userId = (string)Application.Current.Properties["userId"];
                var response = await client.GetAsync(AppConstants.BaseUrl + AppConstants.history + userId);
                var data = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("IS CALL TO ENDPOINT SUCCESSFUL: " + response.IsSuccessStatusCode);
                Debug.WriteLine("RETURNED DATA: " + data);

                var parser = JsonConvert.DeserializeObject<UserHistory>(data);
                //Debug.WriteLine("FELLINGS: " + parser.result.feelings);
                Debug.WriteLine("HAPPY: " + parser.result.happy);
                Debug.WriteLine("MOTIVATION: " + parser.result.motivation);
                Debug.WriteLine("IMPORTANT: " + parser.result.important);

                //var feelings = JsonConvert.DeserializeObject<IDictionary<string, int>>(parser.result.feelings.ToString());
                var happy = JsonConvert.DeserializeObject<IDictionary<string, int>>(parser.result.happy.ToString());
                var motivation = JsonConvert.DeserializeObject<IDictionary<string, int>>(parser.result.motivation.ToString());
                var important = JsonConvert.DeserializeObject<IDictionary<string, int>>(parser.result.important.ToString());

                happyItems.Clear();

                var factor = 0.0;
                foreach (string key in happy.Keys)
                {
                    factor = Math.Max(factor, happy[key]);
                }

                var temp = factor;
                factor = 100 / factor;
                foreach (string key in happy.Keys)
                {
                    happyItems.Add(new Options { name = key, height = happy[key] * factor, color = (string)Application.Current.Properties["event"] });
                }

                factor = 0.0;
                foreach (string key in motivation.Keys)
                {
                    factor = Math.Max(factor, motivation[key]);
                }

                temp = factor;
                factor = 100 / factor;
                motivationItems.Clear();
                foreach (string key in motivation.Keys)
                {
                    motivationItems.Add(new Options { name = key, height = motivation[key] * factor, color = (string)Application.Current.Properties["routine"] });
                }
                factor = 0.0;
                foreach (string key in important.Keys)
                {
                    factor = Math.Max(factor, important[key]);
                }

                temp = factor;
                factor = 100 / factor;
                importantItems.Clear();
                foreach (string key in important.Keys)
                {
                    importantItems.Add(new Options { name = key, height = important[key] * factor, color = (string)Application.Current.Properties["goal"] });
                }
                ImportantList.ItemsSource = importantItems;
                HappyList.ItemsSource = happyItems;
                MotivationList.ItemsSource = motivationItems;
            }
            catch (Exception history)
            {
                await DisplayAlert("Oops",history.Message,"OK");
            }
        }

        public string GetCurrentTime()
        {
            var currentTime = DateTime.Now;
            time = currentTime.ToString("MMMM d, yyyy");
            return time;
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new SettingsPage("WhatIsImportantToMePage");
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new TodaysListPage()); ;
        }

        void TapGestureRecognizer_Tapped_1(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void TapGestureRecognizer_Tapped_2(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new AboutMePage();
        }

    }
}
