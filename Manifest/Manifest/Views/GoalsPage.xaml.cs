using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Manifest.Config;
using Manifest.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

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
        List<OccuranceDto> goalsInRange = new List<OccuranceDto>();
        Dictionary<string, OccuranceDto> occuranceDict = new Dictionary<string, OccuranceDto>();
        public string startTime = "6:00 AM";
        public string endTime = "7:00 AM";

        public GoalsPage(string start, string end)
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

            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;

            frameColor.BackgroundColor = Color.FromHex("#9DB2CB");
            title.Text = "Goals";
            subTitle.Text = "Choose a goal to pursue";
            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();

            
            //if (goalsInRange.Count == 1)
            //{
            //    setProperties1();
            //    show7();
            //    first7.Text = goalsInRange[0].gr_title;
            //}
            //else if (goalsInRange.Count == 2)
            //{
            //    setProperties2();
            //    show7();
            //    first7.Text = goalsInRange[0].gr_title;
            //    second7.Text = goalsInRange[1].gr_title;
            //}
            //else if (goalsInRange.Count == 3)
            //{
            //    setProperties3();
            //    show3();
            //    first7.Text = goalsInRange[0].gr_title;
            //    second7.Text = goalsInRange[1].gr_title;
            //    third7.Text = goalsInRange[2].gr_title;
            //}
            //else if (goalsInRange.Count == 4)
            //{
            //    setProperties4();
            //    show4();
            //    first4.Text = goalsInRange[0].gr_title;
            //    second4.Text = goalsInRange[1].gr_title;
            //    third4.Text = goalsInRange[2].gr_title;
            //    fourth4.Text = goalsInRange[3].gr_title;
            //}
            //else if (goalsInRange.Count == 5)
            //{
            //    setProperties5();
            //    show5();
            //    first5.Text = goalsInRange[0].gr_title;
            //    second5.Text = goalsInRange[1].gr_title;
            //    third5.Text = goalsInRange[2].gr_title;
            //    fourth5.Text = goalsInRange[3].gr_title;
            //    fifth5.Text = goalsInRange[4].gr_title;
            //}
            //else if (goalsInRange.Count == 6)
            //{
            //    setProperties6();
            //    show6();
            //    first6.Text = goalsInRange[0].gr_title;
            //    second6.Text = goalsInRange[1].gr_title;
            //    third6.Text = goalsInRange[2].gr_title;
            //    fourth6.Text = goalsInRange[3].gr_title;
            //    fifth6.Text = goalsInRange[4].gr_title;
            //    sixth6.Text = goalsInRange[5].gr_title;
            //}
            //else
            //{
            //    setProperties7();
            //    show7();
            //    first7.Text = goalsInRange[0].gr_title;
            //    second7.Text = goalsInRange[1].gr_title;
            //    third7.Text = goalsInRange[2].gr_title;
            //    fourth7.Text = goalsInRange[3].gr_title;
            //    fifth7.Text = goalsInRange[4].gr_title;
            //    sixth7.Text = goalsInRange[5].gr_title;
            //    seventh7.Text = goalsInRange[6].gr_title;
            //}

            NavigationPage.SetHasNavigationBar(this, false);
        }

        //void Button_Clicked(System.Object sender, System.EventArgs e)
        //{
        //    Navigation.PushAsync(new GoalsSpecialPage(),false);
        //}

        async void getGoals()
        {
            string url = RdsConfig.BaseUrl + RdsConfig.goalsActInstrUrl + "/" + Application.Current.Properties["userId"];
            var response = await client.GetStringAsync(url);

            OccuranceResponse occuranceResponse = JsonConvert.DeserializeObject<OccuranceResponse>(response);
            ToOccurances(occuranceResponse);
        }

        //This function takes the response from the endpoint, and formats it into Occurances
        private void ToOccurances(OccuranceResponse occuranceResponse)
        {
            try
            {
                //Clear the occurances, as we are going to get new one now
                //todaysOccurances.Clear();
                if (occuranceResponse.result == null || occuranceResponse.result.Count == 0)
                {
                    DisplayAlert("No tasks today", "OK", "Cancel");
                }

                //sort by end time
                //List<OccuranceDto> todayOccurs = new List<OccuranceDto>();
                foreach (OccuranceDto dto in occuranceResponse.result)
                {
                    if (dto.is_displayed_today == "True")
                    {
                        if (todayOccurs.Count == 0)
                            todayOccurs.Add(dto);
                        else
                        {
                            for (int i = 0; i < todayOccurs.Count; i++)
                            {
                                if (i == todayOccurs.Count - 1 && DateTime.Parse(dto.end_day_and_time).TimeOfDay > DateTime.Parse(todayOccurs[i].end_day_and_time).TimeOfDay)
                                {
                                    todayOccurs.Add(dto);
                                    break;
                                }
                                //else if (DateTime.Parse(dto.end_day_and_time).TimeOfDay > DateTime.Parse(todayOccurs[i].end_day_and_time).TimeOfDay)
                                //{
                                //    todayOccurs.Insert(i+1, dto);
                                //    break;
                                //}
                                else if (DateTime.Parse(dto.end_day_and_time).TimeOfDay <= DateTime.Parse(todayOccurs[i].end_day_and_time).TimeOfDay)
                                {
                                    todayOccurs.Insert(i, dto);
                                    break;
                                }
                            }
                        }

                    }
                }

                //sort by start time
                for (int j = 0; j < todayOccurs.Count - 1; j++)
                {
                    for (int i = j + 1; i < todayOccurs.Count; i++)
                    {
                        if (DateTime.Parse(todayOccurs[i].start_day_and_time).TimeOfDay < DateTime.Parse(todayOccurs[j].start_day_and_time).TimeOfDay)
                        {
                            OccuranceDto temp = todayOccurs[i];
                            todayOccurs[i] = todayOccurs[j];
                            todayOccurs[j] = temp;
                        }
                    }
                }

                Debug.WriteLine("passed sorting");
                foreach (OccuranceDto dto in todayOccurs)
                {
                    Debug.WriteLine("starting: " + DateTime.Parse(dto.start_day_and_time).TimeOfDay + " ending: " + DateTime.Parse(dto.end_day_and_time).TimeOfDay);
                }

                if (startTime == endTime)
                {
                    foreach (OccuranceDto dto in todayOccurs)
                    {
                        if (ToDateTime(ToDateTime(dto.start_day_and_time).ToString("t")).TimeOfDay <= ToDateTime(startTime).TimeOfDay && ToDateTime(ToDateTime(dto.end_day_and_time).ToString("t")).TimeOfDay >= ToDateTime(endTime).TimeOfDay)
                        {
                            Debug.WriteLine(dto.gr_title + " passes");
                            goalsInRange.Add(dto);
                        }
                    }
                }
                else
                {
                    foreach (OccuranceDto dto in todayOccurs)
                    {
                        if (ToDateTime(ToDateTime(dto.start_day_and_time).ToString("t")).TimeOfDay >= ToDateTime(startTime).TimeOfDay && ToDateTime(ToDateTime(dto.end_day_and_time).ToString("t")).TimeOfDay <= ToDateTime(endTime).TimeOfDay)
                        {
                            Debug.WriteLine(dto.gr_title + " passes");
                            goalsInRange.Add(dto);
                        }
                    }
                }

                Debug.WriteLine("number of today goals: " + todayOccurs.Count.ToString());
                Debug.WriteLine("number of ranged goals: " + goalsInRange.Count.ToString());

                if (goalsInRange.Count == 0)
                {
                    DisplayAlert("Oops", "no goals available at this time", "OK");
                }
                else if (goalsInRange.Count == 1)
                {
                    setProperties1();
                    show7();
                    //first7.Text = goalsInRange[0].gr_title;
                    text1.Text = goalsInRange[0].gr_title;
                }
                else if (goalsInRange.Count == 2)
                {
                    setProperties2();
                    show7();
                    //first7.Text = goalsInRange[0].gr_title;
                    //second7.Text = goalsInRange[1].gr_title;
                    text1.Text = goalsInRange[0].gr_title;
                    text2.Text = goalsInRange[1].gr_title;
                }
                else if (goalsInRange.Count == 3)
                {
                    setProperties3();
                    show3();
                    //first7.Text = goalsInRange[0].gr_title;
                    //second7.Text = goalsInRange[1].gr_title;
                    //third7.Text = goalsInRange[2].gr_title;
                    text1.Text = goalsInRange[0].gr_title;
                    text2.Text = goalsInRange[1].gr_title;
                    text3.Text = goalsInRange[2].gr_title;
                }
                else if (goalsInRange.Count == 4)
                {
                    setProperties4();
                    show7();
                    text1.Text = goalsInRange[0].gr_title;
                    text2.Text = goalsInRange[1].gr_title;
                    text3.Text = goalsInRange[2].gr_title;
                    text4.Text = goalsInRange[3].gr_title;
                }
                else if (goalsInRange.Count == 5)
                {
                    setProperties5();
                    show7();
                    //first5.Text = goalsInRange[0].gr_title;
                    text1.Text = goalsInRange[0].gr_title;
                    text2.Text = goalsInRange[1].gr_title;
                    text3.Text = goalsInRange[2].gr_title;
                    text4.Text = goalsInRange[3].gr_title;
                    text5.Text = goalsInRange[4].gr_title;
                }
                else if (goalsInRange.Count == 6)
                {
                    setProperties6();
                    show7();
                    text1.Text = goalsInRange[0].gr_title;
                    text2.Text = goalsInRange[1].gr_title;
                    text3.Text = goalsInRange[2].gr_title;
                    text4.Text = goalsInRange[3].gr_title;
                    text5.Text = goalsInRange[4].gr_title;
                    text6.Text = goalsInRange[5].gr_title;
                }
                else
                {
                    setProperties7();
                    show7();
                    //first7.Text = goalsInRange[0].gr_title;
                    //second7.Text = goalsInRange[1].gr_title;
                    //third7.Text = goalsInRange[2].gr_title;
                    //fourth7.Text = goalsInRange[3].gr_title;
                    //fifth7.Text = goalsInRange[4].gr_title;
                    //sixth7.Text = goalsInRange[5].gr_title;
                    //seventh7.Text = goalsInRange[6].gr_title;
                    text1.Text = goalsInRange[0].gr_title;
                    text2.Text = goalsInRange[1].gr_title;
                    text3.Text = goalsInRange[2].gr_title;
                    text4.Text = goalsInRange[3].gr_title;
                    text5.Text = goalsInRange[4].gr_title;
                    text6.Text = goalsInRange[5].gr_title;
                    text7.Text = goalsInRange[6].gr_title;
                }

                foreach (OccuranceDto occurance in goalsInRange)
                {
                    occuranceDict.Add(occurance.gr_title, occurance);
                }

                //foreach (OccuranceDto dto in todayOccurs)
                //{
                //    if (dto.is_displayed_today == "True")
                //    {
                //        Debug.WriteLine("occurance tracked");
                //        Occurance toAdd = new Occurance();
                //        toAdd.Id = dto.gr_unique_id;
                //        toAdd.Title = dto.gr_title;
                //        toAdd.PicUrl = dto.photo;
                //        toAdd.IsPersistent = ToBool(dto.is_persistent);
                //        toAdd.IsInProgress = ToBool(dto.is_in_progress);
                //        toAdd.IsComplete = ToBool(dto.is_complete);
                //        toAdd.IsSublistAvailable = ToBool(dto.is_sublist_available);
                //        toAdd.ExpectedCompletionTime = ToTimeSpan(dto.expected_completion_time);
                //        toAdd.CompletionTime = "This takes " + dto.expected_completion_time;
                //        toAdd.DateTimeCompleted = ToDateTime(dto.datetime_completed);
                //        toAdd.DateTimeStarted = ToDateTime(dto.datetime_started);
                //        toAdd.StartDayAndTime = ToDateTime(dto.start_day_and_time);
                //        toAdd.EndDayAndTime = ToDateTime(dto.end_day_and_time);
                //        toAdd.TimeInterval = DateTime.Parse(dto.start_day_and_time).ToString("t") + "-" + DateTime.Parse(dto.end_day_and_time).ToString("t");

                //        if (DateTime.Now.TimeOfDay >= toAdd.StartDayAndTime.TimeOfDay && DateTime.Now.TimeOfDay <= toAdd.EndDayAndTime.TimeOfDay)
                //            toAdd.StatusColor = Color.FromHex("#FFBD27");
                //        else toAdd.StatusColor = Color.FromHex("#9DB2CB");

                //        Debug.WriteLine("start time: " + dto.start_day_and_time);

                //        toAdd.Repeat = ToBool(dto.repeat);
                //        toAdd.RepeatEvery = dto.repeat_every;
                //        toAdd.RepeatFrequency = dto.repeat_frequency;
                //        toAdd.RepeatType = dto.repeat_type;
                //        toAdd.RepeatOccurences = dto.repeat_occurences;
                //        toAdd.RepeatEndsOn = ToDateTime(dto.repeat_ends_on);
                //        //toAdd.RepeatWeekDays = ParseRepeatWeekDays(repeat_week_days);
                //        toAdd.UserId = dto.user_id;
                //        toAdd.IsEvent = false;


                //        todaysOccurances.Add(toAdd);


                //        //todaysOccurances.Add(toAdd);
                //        //need a isPursueAGoal variable to know if the thing being clicked needs to be navigated to
                //        //send in list of goals for the pursue a goal into goals page instead of calling the endpoint again
                //        //
                //        //store the incoming occurance in listOccur, if it doesn't have the same time as the last occurance, clear listOccur,
                //        //if the incoming occurance is at the same hour and minutes as the one in listOccur, add the time to listTime (that will be passed to goals.xaml.cs)
                //    }
                //}


                return;
            }
            catch (Exception e)
            {
                DisplayAlert("Alert", "Error in TodaysListTest ToOccurances(). Error: " + e.ToString(), "OK");
            }
        }

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


        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            int navigationStackCount = Application.Current.MainPage.Navigation.NavigationStack.Count;

            if (navigationStackCount != 1)
            {
                Navigation.PopAsync(false);
                
            }
            else
            {
                Application.Current.MainPage = new MainPage();
            }
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
        }


        void setProperties1()
        {
            AbsoluteLayout.SetLayoutBounds(first7, new Rectangle(0.5, 0.5, deviceWidth * 0.35, deviceWidth * 0.35));
            first7.CornerRadius = (int)((deviceWidth * 0.35) / 2);
            //first7.Text = "Goal 1";
            //first7.FontSize = deviceWidth / 23;
            text1.FontSize = deviceWidth / 23;

            AbsoluteLayout.SetLayoutBounds(second7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(third7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(fourth7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(fifth7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(sixth7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(seventh7, new Rectangle(0, 0, 0, 0));
            fourth7.HeightRequest = 0;
            fifth7.HeightRequest = 0;
            sixth7.HeightRequest = 0;
            seventh7.HeightRequest = 0;

            //next7.HeightRequest = deviceHeight * 0.025;
            //next7.WidthRequest = deviceWidth * 0.14;
            //next7.CornerRadius = (int)((deviceHeight * 0.025) / 2);
        }

        void setProperties2()
        {
            AbsoluteLayout.SetLayoutBounds(first7, new Rectangle(0.10, 0.07, deviceWidth * 0.23, deviceWidth * 0.23));
            first7.HeightRequest = deviceWidth * 0.23;
            first7.WidthRequest = deviceWidth * 0.23;
            first7.CornerRadius = (int)((deviceWidth * 0.23) / 2);
            //first7.Text = "Goal 1";
            text1.FontSize = deviceWidth / 28;

            AbsoluteLayout.SetLayoutBounds(second7, new Rectangle(0.85, 0.46, deviceWidth * 0.30, deviceWidth * 0.30));
            second7.HeightRequest = deviceWidth * 0.30;
            second7.WidthRequest = deviceWidth * 0.30;
            second7.CornerRadius = (int)((deviceWidth * 0.30) / 2);
            //second7.Text = "Goal 2";
            //second7.FontSize = deviceWidth / 24;
            text2.Text = "Goal 2";
            text2.FontSize = deviceWidth / 24;

            AbsoluteLayout.SetLayoutBounds(third7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(fourth7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(fifth7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(sixth7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(seventh7, new Rectangle(0, 0, 0, 0));
            fourth7.HeightRequest = 0;
            fifth7.HeightRequest = 0;
            sixth7.HeightRequest = 0;
            seventh7.HeightRequest = 0;

            //next7.HeightRequest = deviceHeight * 0.025;
            //next7.WidthRequest = deviceWidth * 0.14;
            //next7.CornerRadius = (int)((deviceHeight * 0.025) / 2);
        }

        void setProperties3()
        {
            AbsoluteLayout.SetLayoutBounds(first7, new Rectangle(0.10, 0.07, deviceWidth * 0.23, deviceWidth * 0.23));
            first7.HeightRequest = deviceWidth * 0.23;
            first7.WidthRequest = deviceWidth * 0.23;
            first7.CornerRadius = (int)((deviceWidth * 0.23) / 2);
            //first7.Text = "Goal 1";
            //first7.FontSize = deviceWidth / 28;
            text1.FontSize = deviceWidth / 28;

            AbsoluteLayout.SetLayoutBounds(second7, new Rectangle(0.85, 0.45, deviceWidth * 0.30, deviceWidth * 0.30));
            second7.HeightRequest = deviceWidth * 0.30;
            second7.WidthRequest = deviceWidth * 0.30;
            second7.CornerRadius = (int)((deviceWidth * 0.30) / 2);
            //second7.Text = "Goal 2";
            //second7.FontSize = deviceWidth / 24;
            text2.FontSize = deviceWidth / 24;

            AbsoluteLayout.SetLayoutBounds(third7, new Rectangle(0.14, 0.80, deviceWidth * 0.22, deviceWidth * 0.22));
            third7.HeightRequest = deviceWidth * 0.22;
            third7.WidthRequest = deviceWidth * 0.22;
            third7.CornerRadius = (int)((deviceWidth * 0.22) / 2);
            //third7.Text = "Goal 3";
            //third7.FontSize = deviceWidth / 28;
            text3.Text = "Goal 3";
            text3.FontSize = deviceWidth / 28;

            AbsoluteLayout.SetLayoutBounds(fourth7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(fifth7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(sixth7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(seventh7, new Rectangle(0, 0, 0, 0));
            fourth7.HeightRequest = 0;
            fifth7.HeightRequest = 0;
            sixth7.HeightRequest = 0;
            seventh7.HeightRequest = 0;

            //next7.HeightRequest = deviceHeight * 0.025;
            //next7.WidthRequest = deviceWidth * 0.14;
            //next7.CornerRadius = (int)((deviceHeight * 0.025) / 2);
        }

        void setProperties4()
        {
            AbsoluteLayout.SetLayoutBounds(first7, new Rectangle(0.10, 0.07, deviceWidth * 0.23, deviceWidth * 0.23));
            first7.HeightRequest = deviceWidth * 0.23;
            first7.WidthRequest = deviceWidth * 0.23;
            first7.CornerRadius = (int)((deviceWidth * 0.23) / 2);
            //first7.Text = "Goal 1";
            //first7.FontSize = deviceWidth / 25;
            //first7.Text = "Goal 1";
            text1.FontSize = deviceWidth / 25;

            AbsoluteLayout.SetLayoutBounds(second7, new Rectangle(0.85, 0.45, deviceWidth * 0.30, deviceWidth * 0.30));
            second7.HeightRequest = deviceWidth * 0.30;
            second7.WidthRequest = deviceWidth * 0.30;
            second7.CornerRadius = (int)((deviceWidth * 0.30) / 2);
            //second7.Text = "Goal 2";
            //second7.FontSize = deviceWidth / 22;
            text2.FontSize = deviceWidth / 22;

            AbsoluteLayout.SetLayoutBounds(third7, new Rectangle(0.14, 0.81, deviceWidth * 0.22, deviceWidth * 0.22));
            third7.HeightRequest = deviceWidth * 0.22;
            third7.WidthRequest = deviceWidth * 0.22;
            third7.CornerRadius = (int)((deviceWidth * 0.22) / 2);
            //third7.Text = "Goal 3";
            //third7.FontSize = deviceWidth / 26;
            text3.FontSize = deviceWidth / 26;

            AbsoluteLayout.SetLayoutBounds(fourth7, new Rectangle(0.82, 0.84, deviceWidth * 0.16, deviceWidth * 0.16));
            fourth7.HeightRequest = deviceWidth * 0.16;
            fourth7.WidthRequest = deviceWidth * 0.16;
            fourth7.CornerRadius = (int)((deviceWidth * 0.16) / 2);
            //fourth7.Text = "Goal 4";
            //fourth7.FontSize = deviceWidth / 33;
            text4.FontSize = deviceWidth / 33;

            AbsoluteLayout.SetLayoutBounds(fifth7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(sixth7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(seventh7, new Rectangle(0, 0, 0, 0));

            //next4.HeightRequest = deviceHeight * 0.025;
            //next4.WidthRequest = deviceWidth * 0.14;
            //next4.CornerRadius = (int)((deviceHeight * 0.025) / 2);
        }

        void setProperties5()
        {
            AbsoluteLayout.SetLayoutBounds(first7, new Rectangle(0.10, 0.03, deviceWidth * 0.21, deviceWidth * 0.21));
            first5.HeightRequest = deviceWidth * 0.21;
            first5.WidthRequest = deviceWidth * 0.21;
            first7.CornerRadius = (int)((deviceWidth * 0.21) / 2);
            //first5.Text = "Goal 1";
            //first7.FontSize = deviceWidth / 26;
            //text.Text = "Goal 1";
            text1.FontSize = deviceWidth / 26;

            AbsoluteLayout.SetLayoutBounds(second7, new Rectangle(0.75, 0.32, deviceWidth * 0.26, deviceWidth * 0.26));
            second7.HeightRequest = deviceWidth * 0.26;
            second7.WidthRequest = deviceWidth * 0.26;
            second7.CornerRadius = (int)((deviceWidth * 0.26) / 2);
            //second7.Text = "Goal 2";
            //second7.FontSize = deviceWidth / 24;
            text2.FontSize = deviceWidth / 24;

            AbsoluteLayout.SetLayoutBounds(third7, new Rectangle(0.12, 0.66, deviceWidth * 0.21, deviceWidth * 0.21));
            third7.HeightRequest = deviceWidth * 0.21;
            third7.WidthRequest = deviceWidth * 0.21;
            third7.CornerRadius = (int)((deviceWidth * 0.21) / 2);
            //third7.Text = "Goal 3";
            //third7.FontSize = deviceWidth / 27;
            text3.FontSize = deviceWidth / 27;

            AbsoluteLayout.SetLayoutBounds(fourth7, new Rectangle(0.9, 0.68, deviceWidth * 0.16, deviceWidth * 0.16));
            fourth7.HeightRequest = deviceWidth * 0.16;
            fourth7.WidthRequest = deviceWidth * 0.16;
            fourth7.CornerRadius = (int)((deviceWidth * 0.16) / 2);
            //fourth7.Text = "Goal 4";
            //fourth7.FontSize = deviceWidth / 33;
            text4.FontSize = deviceWidth / 33;

            AbsoluteLayout.SetLayoutBounds(fifth7, new Rectangle(0.65, 0.93, deviceWidth * 0.17, deviceWidth * 0.17));
            fifth7.HeightRequest = deviceWidth * 0.17;
            fifth7.WidthRequest = deviceWidth * 0.17;
            fifth7.CornerRadius = (int)((deviceWidth * 0.17) / 2);
            //fifth7.Text = "Goal 5";
            //fifth7.FontSize = deviceWidth / 32;
            text5.FontSize = deviceWidth / 32;

            AbsoluteLayout.SetLayoutBounds(sixth7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(seventh7, new Rectangle(0, 0, 0, 0));

            //next5.HeightRequest = deviceHeight * 0.025;
            //next5.WidthRequest = deviceWidth * 0.14;
            //next5.CornerRadius = (int)((deviceHeight * 0.025) / 2);
        }

        void setProperties6()
        {
            AbsoluteLayout.SetLayoutBounds(first7, new Rectangle(0.10, 0.05, deviceWidth * 0.20, deviceWidth * 0.20));
            first7.HeightRequest = deviceWidth * 0.20;
            first7.WidthRequest = deviceWidth * 0.20;
            first7.CornerRadius = (int)((deviceWidth * 0.20) / 2);
            //first7.Text = "Goal 1";
            //first7.FontSize = deviceWidth / 26;
            text1.FontSize = deviceWidth / 26;

            AbsoluteLayout.SetLayoutBounds(second7, new Rectangle(0.91, 0.23, deviceWidth * 0.26, deviceWidth * 0.26));
            second7.HeightRequest = deviceWidth * 0.26;
            second7.WidthRequest = deviceWidth * 0.26;
            second7.CornerRadius = (int)((deviceWidth * 0.26) / 2);
            //second7.Text = "Goal 2";
            //second7.FontSize = deviceWidth / 25;
            text2.FontSize = deviceWidth / 25;

            AbsoluteLayout.SetLayoutBounds(third7, new Rectangle(0.08, 0.49, deviceWidth * 0.22, deviceWidth * 0.22));
            third7.HeightRequest = deviceWidth * 0.22;
            third7.WidthRequest = deviceWidth * 0.22;
            third7.CornerRadius = (int)((deviceWidth * 0.22) / 2);
            //third7.Text = "Goal 3";
            //third7.FontSize = deviceWidth / 26;
            text3.FontSize = deviceWidth / 26;

            AbsoluteLayout.SetLayoutBounds(fourth7, new Rectangle(0.78, 0.64, deviceWidth * 0.17, deviceWidth * 0.17));
            fourth7.HeightRequest = deviceWidth * 0.17;
            fourth7.WidthRequest = deviceWidth * 0.17;
            fourth7.CornerRadius = (int)((deviceWidth * 0.17) / 2);
            //fourth7.Text = "Goal 4";
            //fourth7.FontSize = deviceWidth / 31;
            text4.FontSize = deviceWidth / 31;

            AbsoluteLayout.SetLayoutBounds(fifth7, new Rectangle(0.11, 0.83, deviceWidth * 0.16, deviceWidth * 0.16));
            fifth7.HeightRequest = deviceWidth * 0.16;
            fifth7.WidthRequest = deviceWidth * 0.16;
            fifth7.CornerRadius = (int)((deviceWidth * 0.16) / 2);
            //fifth7.Text = "Goal 5";
            //fifth7.FontSize = deviceWidth / 34;
            text5.FontSize = deviceWidth / 34;

            AbsoluteLayout.SetLayoutBounds(sixth7, new Rectangle(0.68, 0.93, deviceWidth * 0.17, deviceWidth * 0.17));
            sixth7.HeightRequest = deviceWidth * 0.17;
            sixth7.WidthRequest = deviceWidth * 0.17;
            sixth7.CornerRadius = (int)((deviceWidth * 0.17) / 2);
            //sixth7.Text = "Goal 6";
            //sixth7.FontSize = deviceWidth / 31;
            text6.FontSize = deviceWidth / 31;

            AbsoluteLayout.SetLayoutBounds(seventh7, new Rectangle(0, 0, 0, 0));

            //next6.HeightRequest = deviceHeight * 0.025;
            //next6.WidthRequest = deviceWidth * 0.14;
            //next6.CornerRadius = (int)((deviceHeight * 0.025) / 2);
        }

        void setProperties7()
        {
            AbsoluteLayout.SetLayoutBounds(first7, new Rectangle(0.10, 0.05, deviceWidth * 0.20, deviceWidth * 0.20));
            first7.HeightRequest = deviceWidth * 0.20;
            first7.WidthRequest = deviceWidth * 0.20;
            first7.CornerRadius = (int)((deviceWidth * 0.20) / 2);
            //first7.Text = "Goal 1";
            //first7.FontSize = deviceWidth / 27;
            text1.FontSize = deviceWidth / 27;

            AbsoluteLayout.SetLayoutBounds(second7, new Rectangle(0.90, 0.23, deviceWidth * 0.26, deviceWidth * 0.26));
            second7.HeightRequest = deviceWidth * 0.26;
            second7.WidthRequest = deviceWidth * 0.26;
            second7.CornerRadius = (int)((deviceWidth * 0.26) / 2);
            //second7.Text = "Goal 2";
            //second7.FontSize = deviceWidth / 25;
            text2.FontSize = deviceWidth / 25;

            AbsoluteLayout.SetLayoutBounds(third7, new Rectangle(0.09, 0.39, deviceWidth * 0.17, deviceWidth * 0.17));
            third7.HeightRequest = deviceWidth * 0.17;
            third7.WidthRequest = deviceWidth * 0.17;
            third7.CornerRadius = (int)((deviceWidth * 0.17) / 2);
            //third7.Text = "Goal 3";
            //third7.FontSize = deviceWidth / 31;
            text3.FontSize = deviceWidth / 31;

            AbsoluteLayout.SetLayoutBounds(fourth7, new Rectangle(0.78, 0.63, deviceWidth * 0.17, deviceWidth * 0.17));
            fourth7.HeightRequest = deviceWidth * 0.17;
            fourth7.WidthRequest = deviceWidth * 0.17;
            fourth7.CornerRadius = (int)((deviceWidth * 0.17) / 2);
            //fourth7.Text = "Goal 4";
            //fourth7.FontSize = deviceWidth / 30;
            text4.FontSize = deviceWidth / 30;

            AbsoluteLayout.SetLayoutBounds(fifth7, new Rectangle(0.10, 0.64, deviceWidth * 0.13, deviceWidth * 0.13));
            fifth7.HeightRequest = deviceWidth * 0.13;
            fifth7.WidthRequest = deviceWidth * 0.13;
            fifth7.CornerRadius = (int)((deviceWidth * 0.13) / 2);
            //fifth7.Text = "Goal 5";
            //fifth7.FontSize = deviceWidth / 39;
            text5.FontSize = deviceWidth / 39;

            AbsoluteLayout.SetLayoutBounds(sixth7, new Rectangle(0.93, 0.91, deviceWidth * 0.17, deviceWidth * 0.17));
            sixth7.HeightRequest = deviceWidth * 0.17;
            sixth7.WidthRequest = deviceWidth * 0.17;
            sixth7.CornerRadius = (int)((deviceWidth * 0.17) / 2);
            //sixth7.Text = "Goal 6";
            //sixth7.FontSize = deviceWidth / 31;
            text6.FontSize = deviceWidth / 31;

            AbsoluteLayout.SetLayoutBounds(seventh7, new Rectangle(0.31, 0.92, deviceWidth * 0.19, deviceWidth * 0.19));
            seventh7.HeightRequest = deviceWidth * 0.19;
            seventh7.WidthRequest = deviceWidth * 0.19;
            seventh7.CornerRadius = (int)((deviceWidth * 0.19) / 2);
            //seventh7.Text = "Goal 7";
            //seventh7.FontSize = deviceWidth / 30;
            text7.FontSize = deviceWidth / 30;

            //next7.HeightRequest = deviceHeight * 0.025;
            //next7.WidthRequest = deviceWidth * 0.14;
            //next7.CornerRadius = (int)((deviceHeight * 0.025) / 2);
        }


        void show7()
        {
            goals7.IsVisible = true;
            goals6.IsVisible = false;
            goals3.IsVisible = false;
            goals4.IsVisible = false;
            goals5.IsVisible = false;
        }

        void show6()
        {
            goals7.IsVisible = false;
            goals6.IsVisible = true;
            goals3.IsVisible = false;
            goals4.IsVisible = false;
            goals5.IsVisible = false;
        }

        void show5()
        {
            goals7.IsVisible = false;
            goals6.IsVisible = false;
            goals3.IsVisible = false;
            goals4.IsVisible = false;
            goals5.IsVisible = true;
        }

        void show4()
        {
            goals7.IsVisible = false;
            goals6.IsVisible = false;
            goals3.IsVisible = false;
            goals5.IsVisible = false;
            goals4.IsVisible = true;
        }

        void show3()
        {
            goals7.IsVisible = true;
            goals6.IsVisible = false;
            goals3.IsVisible = false;
            goals5.IsVisible = false;
            goals4.IsVisible = false;
        }

        void clickedShow7(System.Object sender, System.EventArgs e)
        {
            //show7();
            setProperties7();
            show7();
        }

        void clickedShow6(System.Object sender, System.EventArgs e)
        {
            //show6();
            setProperties6();
            show7();
        }

        void clickedShow5(System.Object sender, System.EventArgs e)
        {
            //show5();
            setProperties5();
            show7();
        }

        void clickedShow4(System.Object sender, System.EventArgs e)
        {
            //show4();
            setProperties4();
            show7();
        }

        void clickedShow3(System.Object sender, System.EventArgs e)
        {
            //show3();
            setProperties3();
            show3();
        }

        void clickedShow2(System.Object sender, System.EventArgs e)
        {
            //show3();
            setProperties2();
            show3();
        }

        void clickedShow1(System.Object sender, System.EventArgs e)
        {
            //show3();
            setProperties1();
            show3();
        }

        void navigatetoTodaysList(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new TodaysListPage();
        }

        void navigatetoActions(System.Object sender, System.EventArgs e)
        {
            Label receiving = (Label)sender;

            if (occuranceDict[receiving.Text].is_sublist_available == "True")
                Navigation.PushAsync(new GoalsSpecialPage(occuranceDict[receiving.Text]));
            else DisplayAlert("Error", "this goal doesn't have subtasks", "OK");
            //Application.Current.MainPage = new GoalsSpecialPage();
        }
    }
}
