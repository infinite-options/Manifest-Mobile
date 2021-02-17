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
                barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
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
                string url = RdsConfig.BaseUrl + RdsConfig.goalsActInstrUrl + "/" + Application.Current.Properties["userId"];
                var response = await client.GetStringAsync(url);

                OccuranceResponse occuranceResponse = JsonConvert.DeserializeObject<OccuranceResponse>(response);
                ToOccurances(occuranceResponse);
            }
            catch (Exception goals)
            {

            }
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
                foreach (OccuranceDto dto in occuranceResponse.result)
                {
                    //Only add routines
                    if (dto.is_displayed_today == "True")
                    {
                        Occurance toAdd = new Occurance();
                        if (dto.actions_tasks == null)
                        {
                            Debug.WriteLine("Actions and tasks are null");
                        }
                        toAdd.Id = dto.gr_unique_id;
                        toAdd.Title = dto.gr_title;
                        toAdd.PicUrl = dto.photo;
                        toAdd.IsPersistent = DataParser.ToBool(dto.is_persistent);
                        toAdd.IsInProgress = DataParser.ToBool(dto.is_in_progress);
                        toAdd.IsComplete = DataParser.ToBool(dto.is_complete);
                        toAdd.IsSublistAvailable = DataParser.ToBool(dto.is_sublist_available);
                        toAdd.ExpectedCompletionTime = DataParser.ToTimeSpan(dto.expected_completion_time);
                        toAdd.CompletionTime = dto.expected_completion_time;
                        toAdd.DateTimeCompleted = DataParser.ToDateTime(dto.datetime_completed);
                        toAdd.DateTimeStarted = DataParser.ToDateTime(dto.datetime_started);
                        toAdd.StartDayAndTime = DataParser.ToDateTime(dto.start_day_and_time);
                        toAdd.EndDayAndTime = DataParser.ToDateTime(dto.end_day_and_time);
                        toAdd.Repeat = DataParser.ToBool(dto.repeat);
                        toAdd.RepeatEvery = dto.repeat_every;
                        toAdd.RepeatFrequency = dto.repeat_frequency;
                        toAdd.RepeatType = dto.repeat_type;
                        toAdd.RepeatOccurences = dto.repeat_occurences;
                        toAdd.RepeatEndsOn = DataParser.ToDateTime(dto.repeat_ends_on);
                        //toAdd.RepeatWeekDays = ParseRepeatWeekDays(repeat_week_days);
                        toAdd.UserId = dto.user_id;
                        toAdd.IsEvent = false;
                        toAdd.NumSubOccurances = 0;
                        toAdd.SubOccurancesCompleted = 0;
                        toAdd.subOccurances = GetSubOccurances(dto.actions_tasks, toAdd);
                        currentOccurances.Add(toAdd);
                    }
                }

                //sort by start time
                for (int j = 0; j < currentOccurances.Count - 1; j++)
                {
                    for (int i = j + 1; i < currentOccurances.Count; i++)
                    {
                        if (currentOccurances[i].StartDayAndTime.TimeOfDay < currentOccurances[j].StartDayAndTime.TimeOfDay)
                        {
                            Occurance temp = currentOccurances[i];
                            currentOccurances[i] = currentOccurances[j];
                            currentOccurances[j] = temp;
                        }
                    }
                }

                Debug.WriteLine("passed sorting");
                foreach (OccuranceDto dto in todayOccurs)
                {
                    Debug.WriteLine("starting: " + DateTime.Parse(dto.start_day_and_time).TimeOfDay + " ending: " + DateTime.Parse(dto.end_day_and_time).TimeOfDay);
                }

                //if (startTime == endTime)
                //{
                //    foreach (OccuranceDto dto in todayOccurs)
                //    {
                //        if (ToDateTime(ToDateTime(dto.start_day_and_time).ToString("t")).TimeOfDay <= ToDateTime(startTime).TimeOfDay && ToDateTime(ToDateTime(dto.end_day_and_time).ToString("t")).TimeOfDay >= ToDateTime(endTime).TimeOfDay)
                //        {
                //            Debug.WriteLine(dto.gr_title + " passes");
                //            goalsInRange.Add(dto);
                //        }
                //    }
                //}
                //else
                //{
                //    foreach (OccuranceDto dto in todayOccurs)
                //    {
                //        if (ToDateTime(ToDateTime(dto.start_day_and_time).ToString("t")).TimeOfDay >= ToDateTime(startTime).TimeOfDay && ToDateTime(ToDateTime(dto.end_day_and_time).ToString("t")).TimeOfDay <= ToDateTime(endTime).TimeOfDay)
                //        {
                //            Debug.WriteLine(dto.gr_title + " passes");
                //            goalsInRange.Add(dto);
                //        }
                //    }
                //}

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
                            Debug.WriteLine(dto.Title + " passes");
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
                    text1.Text = goalsInRange[0].Title;
                    occuranceDict.Add(text1, goalsInRange[0]);
                    //occuranceDict.Add(first7, goalsInRange[0]);
                }
                else if (goalsInRange.Count == 2)
                {
                    setProperties2();
                    show7();
                    //first7.Text = goalsInRange[0].gr_title;
                    //second7.Text = goalsInRange[1].gr_title;
                    text1.Text = goalsInRange[0].Title;
                    occuranceDict.Add(text1, goalsInRange[0]);
                    //occuranceDict.Add(first7, goalsInRange[0]);

                    text2.Text = goalsInRange[1].Title;
                    occuranceDict.Add(text2, goalsInRange[1]);
                    //occuranceDict.Add(second7, goalsInRange[1]);
                }
                else if (goalsInRange.Count == 3)
                {
                    setProperties3();
                    show3();
                    //first7.Text = goalsInRange[0].gr_title;
                    //second7.Text = goalsInRange[1].gr_title;
                    //third7.Text = goalsInRange[2].gr_title;
                    text1.Text = goalsInRange[0].Title;
                    occuranceDict.Add(text1, goalsInRange[0]);
                    //occuranceDict.Add(first7, goalsInRange[0]);

                    text2.Text = goalsInRange[1].Title;
                    occuranceDict.Add(text2, goalsInRange[1]);
                    //occuranceDict.Add(second7, goalsInRange[1]);

                    text3.Text = goalsInRange[2].Title;
                    occuranceDict.Add(text3, goalsInRange[2]);
                    //occuranceDict.Add(third7, goalsInRange[2]);

                }
                else if (goalsInRange.Count == 4)
                {
                    setProperties4();
                    show7();
                    text1.Text = goalsInRange[0].Title;
                    occuranceDict.Add(text1, goalsInRange[0]);
                    //occuranceDict.Add(first7, goalsInRange[0]);

                    text2.Text = goalsInRange[1].Title;
                    occuranceDict.Add(text2, goalsInRange[1]);
                    //occuranceDict.Add(second7, goalsInRange[1]);

                    text3.Text = goalsInRange[2].Title;
                    occuranceDict.Add(text3, goalsInRange[2]);
                    //occuranceDict.Add(third7, goalsInRange[2]);

                    text4.Text = goalsInRange[3].Title;
                    occuranceDict.Add(text4, goalsInRange[3]);
                    //occuranceDict.Add(fourth7, goalsInRange[3]);

                }
                else if (goalsInRange.Count == 5)
                {
                    setProperties5();
                    show7();
                    //first5.Text = goalsInRange[0].gr_title;
                    text1.Text = goalsInRange[0].Title;
                    occuranceDict.Add(text1, goalsInRange[0]);
                    //occuranceDict.Add(first7, goalsInRange[0]);

                    text2.Text = goalsInRange[1].Title;
                    occuranceDict.Add(text2, goalsInRange[1]);
                    //occuranceDict.Add(second7, goalsInRange[1]);

                    text3.Text = goalsInRange[2].Title;
                    occuranceDict.Add(text3, goalsInRange[2]);
                    //occuranceDict.Add(third7, goalsInRange[2]);

                    text4.Text = goalsInRange[3].Title;
                    occuranceDict.Add(text4, goalsInRange[3]);
                    //occuranceDict.Add(fourth7, goalsInRange[3]);

                    text5.Text = goalsInRange[4].Title;
                    occuranceDict.Add(text5, goalsInRange[4]);
                    //occuranceDict.Add(fifth7, goalsInRange[4]);

                }
                else if (goalsInRange.Count == 6)
                {
                    setProperties6();
                    show7();
                    text1.Text = goalsInRange[0].Title;
                    occuranceDict.Add(text1, goalsInRange[0]);
                    //occuranceDict.Add(first7, goalsInRange[0]);

                    text2.Text = goalsInRange[1].Title;
                    occuranceDict.Add(text2, goalsInRange[1]);
                    //occuranceDict.Add(second7, goalsInRange[1]);

                    text3.Text = goalsInRange[2].Title;
                    occuranceDict.Add(text3, goalsInRange[2]);
                    //occuranceDict.Add(third7, goalsInRange[2]);

                    text4.Text = goalsInRange[3].Title;
                    occuranceDict.Add(text4, goalsInRange[3]);
                    //occuranceDict.Add(fourth7, goalsInRange[3]);

                    text5.Text = goalsInRange[4].Title;
                    occuranceDict.Add(text5, goalsInRange[4]);
                    //occuranceDict.Add(fifth7, goalsInRange[4]);

                    text6.Text = goalsInRange[5].Title;
                    occuranceDict.Add(text6, goalsInRange[5]);
                    //occuranceDict.Add(sixth7, goalsInRange[5]);

                }
                else
                {
                    setProperties7();
                    show7();
                    text1.Text = goalsInRange[0].Title;
                    occuranceDict.Add(text1, goalsInRange[0]);
                    //occuranceDict.Add(first7, goalsInRange[0]);

                    text2.Text = goalsInRange[1].Title;
                    occuranceDict.Add(text2, goalsInRange[1]);
                    //occuranceDict.Add(second7, goalsInRange[1]);

                    text3.Text = goalsInRange[2].Title;
                    occuranceDict.Add(text3, goalsInRange[2]);
                    //occuranceDict.Add(third7, goalsInRange[2]);

                    text4.Text = goalsInRange[3].Title;
                    occuranceDict.Add(text4, goalsInRange[3]);
                    //occuranceDict.Add(fourth7, goalsInRange[3]);

                    text5.Text = goalsInRange[4].Title;
                    occuranceDict.Add(text5, goalsInRange[4]);
                    //occuranceDict.Add(fifth7, goalsInRange[4]);

                    text6.Text = goalsInRange[5].Title;
                    occuranceDict.Add(text6, goalsInRange[5]);
                    //occuranceDict.Add(sixth7, goalsInRange[5]);

                    text7.Text = goalsInRange[6].Title;
                    occuranceDict.Add(text7, goalsInRange[6]);
                    //occuranceDict.Add(seventh7, goalsInRange[6]);

                }

                //foreach (OccuranceDto occurance in goalsInRange)
                //{
                //    occuranceDict.Add(occurance.gr_unique_id, occurance);
                //}

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

        private List<SubOccurance> GetSubOccurances(List<SubOccuranceDto> actions_tasks, Occurance parent)
        {
            List<SubOccurance> subTasks = new List<SubOccurance>();
            if (actions_tasks == null || actions_tasks.Count == 0)
            {
                return subTasks;
            }
            foreach (SubOccuranceDto dto in actions_tasks)
            {
                parent.NumSubOccurances++;
                //numTasks++;
                SubOccurance toAdd = new SubOccurance();
                toAdd.Id = dto.at_unique_id;
                toAdd.Title = dto.at_title;
                toAdd.GoalRoutineID = dto.goal_routine_id;
                toAdd.AtSequence = dto.at_sequence;
                toAdd.IsAvailable = DataParser.ToBool(dto.is_available);
                toAdd.IsComplete = DataParser.ToBool(dto.is_complete);
                if (toAdd.IsComplete)
                {
                    parent.SubOccurancesCompleted++;
                }
                toAdd.IsInProgress = DataParser.ToBool(dto.is_in_progress);
                toAdd.IsSublistAvailable = DataParser.ToBool(dto.is_sublist_available);
                toAdd.IsMustDo = DataParser.ToBool(dto.is_must_do);
                toAdd.PicUrl = dto.photo;
                toAdd.IsTimed = DataParser.ToBool(dto.is_timed);
                toAdd.DateTimeCompleted = DataParser.ToDateTime(dto.datetime_completed);
                toAdd.DateTimeStarted = DataParser.ToDateTime(dto.datetime_started);
                toAdd.ExpectedCompletionTime = DataParser.ToTimeSpan(dto.expected_completion_time);
                toAdd.AvailableStartTime = DataParser.ToDateTime(dto.available_start_time);
                toAdd.AvailableEndTime = DataParser.ToDateTime(dto.available_end_time);
                toAdd.instructions = GetInstructions(dto.instructions_steps);
                subTasks.Add(toAdd);
                Debug.WriteLine(toAdd.Id);
            }

            return subTasks;
        }

        private List<Instruction> GetInstructions(List<InstructionDto> instruction_steps)
        {
            List<Instruction> instructions = new List<Instruction>();
            if (instruction_steps.Count == 0 || instruction_steps == null)
            {
                return instructions;
            }
            foreach (InstructionDto dto in instruction_steps)
            {
                Instruction toAdd = new Instruction();
                toAdd.unique_id = dto.unique_id;
                toAdd.title = dto.title;
                toAdd.at_id = dto.at_id;
                toAdd.IsSequence = int.Parse(dto.is_sequence);
                toAdd.IsAvailable = DataParser.ToBool(dto.is_available);
                toAdd.IsComplete = DataParser.ToBool(dto.is_complete);
                toAdd.IsInProgress = DataParser.ToBool(dto.is_in_progress);
                toAdd.IsTimed = DataParser.ToBool(dto.is_timed);
                toAdd.Photo = dto.photo;
                toAdd.expected_completion_time = DataParser.ToTimeSpan(dto.expected_completion_time);
                instructions.Add(toAdd);
            }

            return instructions;
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
            Debug.WriteLine("navigationStackCount: " + navigationStackCount.ToString());


            if (navigationStackCount > 2)
            {
                //Navigation.RemovePage(Navigation.NavigationStack(2));
                Navigation.PopToRootAsync(false);
                //Navigation.PopAsync(false);
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

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SettingsPage(),false);
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

        void nextClicked(System.Object sender, System.EventArgs e)
        {
            //if a goal has only one subtask, navigate directly to steps page
            if (chosenOccurance != null && chosenOccurance.IsSublistAvailable == true && chosenOccurance.subOccurances.Count == 1)
            {
                Navigation.PushAsync(new GoalStepsPage(chosenOccurance.Title, chosenOccurance.subOccurances[0], "#F8BE28"));
            }
            else if (chosenOccurance != null && chosenOccurance.IsSublistAvailable == true)
                Navigation.PushAsync(new GoalsSpecialPage(chosenOccurance));
            else if (chosenOccurance != null && chosenOccurance.IsSublistAvailable == false)
                DisplayAlert("Error", "this goal doesn't have subtasks", "OK");
            else DisplayAlert("Oops", "please select a goal first", "OK");
        }

        void navigatetoActions(System.Object sender, System.EventArgs e)
        {
            //int navigationStackCount = Application.Current.MainPage.Navigation.NavigationStack.Count;
            //if (navigationStackCount > 2)
            //    Navigation.PopAsync();

            Label receiving = (Label)sender;
            chosenOccurance = occuranceDict[receiving];
            //if (occuranceDict[receiving].is_sublist_available == "True")
            //    Navigation.PushAsync(new GoalsSpecialPage(occuranceDict[receiving]));
            //else DisplayAlert("Error", "this goal doesn't have subtasks", "OK");
            //Application.Current.MainPage = new GoalsSpecialPage();
            if (receiving == text1)
            {
                first7.BackgroundColor = Color.FromHex("#FFBD27");
                second7.BackgroundColor = Color.Transparent;
                third7.BackgroundColor = Color.Transparent;
                fourth7.BackgroundColor = Color.Transparent;
                fifth7.BackgroundColor = Color.Transparent;
                sixth7.BackgroundColor = Color.Transparent;
                seventh7.BackgroundColor = Color.Transparent;
            }
            else if (receiving == text2)
            {
                first7.BackgroundColor = Color.Transparent;
                second7.BackgroundColor = Color.FromHex("#FFBD27");
                third7.BackgroundColor = Color.Transparent;
                fourth7.BackgroundColor = Color.Transparent;
                fifth7.BackgroundColor = Color.Transparent;
                sixth7.BackgroundColor = Color.Transparent;
                seventh7.BackgroundColor = Color.Transparent;
            }
            else if (receiving == text3)
            {
                first7.BackgroundColor = Color.Transparent;
                second7.BackgroundColor = Color.Transparent;
                third7.BackgroundColor = Color.FromHex("#FFBD27"); 
                fourth7.BackgroundColor = Color.Transparent;
                fifth7.BackgroundColor = Color.Transparent;
                sixth7.BackgroundColor = Color.Transparent;
                seventh7.BackgroundColor = Color.Transparent;
            }
            else if (receiving == text4)
            {
                first7.BackgroundColor = Color.Transparent;
                second7.BackgroundColor = Color.Transparent;
                third7.BackgroundColor = Color.Transparent;
                fourth7.BackgroundColor = Color.FromHex("#FFBD27");
                fifth7.BackgroundColor = Color.Transparent;
                sixth7.BackgroundColor = Color.Transparent;
                seventh7.BackgroundColor = Color.Transparent;
            }
            else if (receiving == text5)
            {
                first7.BackgroundColor = Color.Transparent;
                second7.BackgroundColor = Color.Transparent;
                third7.BackgroundColor = Color.Transparent;
                fourth7.BackgroundColor = Color.Transparent;
                fifth7.BackgroundColor = Color.FromHex("#FFBD27"); 
                sixth7.BackgroundColor = Color.Transparent;
                seventh7.BackgroundColor = Color.Transparent;
            }
            else if (receiving == text6)
            {
                first7.BackgroundColor = Color.Transparent;
                second7.BackgroundColor = Color.Transparent;
                third7.BackgroundColor = Color.Transparent;
                fourth7.BackgroundColor = Color.Transparent;
                fifth7.BackgroundColor = Color.Transparent;
                sixth7.BackgroundColor = Color.FromHex("#FFBD27"); 
                seventh7.BackgroundColor = Color.Transparent;
            }
            else
            {
                first7.BackgroundColor = Color.Transparent;
                second7.BackgroundColor = Color.Transparent; 
                third7.BackgroundColor = Color.Transparent;
                fourth7.BackgroundColor = Color.Transparent;
                fifth7.BackgroundColor = Color.Transparent;
                sixth7.BackgroundColor = Color.Transparent;
                seventh7.BackgroundColor = Color.FromHex("#FFBD27");
            }
        }


        void navigatetoActionsFrame(System.Object sender, System.EventArgs e)
        {
            //int navigationStackCount = Application.Current.MainPage.Navigation.NavigationStack.Count;
            //if (navigationStackCount > 2)
            //    Navigation.PopAsync();

            Frame receiving = (Frame)sender;
            
            if (receiving == first7)
            {
                chosenOccurance = occuranceDict[text1];
                first7.BackgroundColor = Color.FromHex("#FFBD27");
                second7.BackgroundColor = Color.Transparent;
                third7.BackgroundColor = Color.Transparent;
                fourth7.BackgroundColor = Color.Transparent;
                fifth7.BackgroundColor = Color.Transparent;
                sixth7.BackgroundColor = Color.Transparent;
                seventh7.BackgroundColor = Color.Transparent;
            }
            else if (receiving == second7)
            {
                chosenOccurance = occuranceDict[text2];
                first7.BackgroundColor = Color.Transparent;
                second7.BackgroundColor = Color.FromHex("#FFBD27");
                third7.BackgroundColor = Color.Transparent;
                fourth7.BackgroundColor = Color.Transparent;
                fifth7.BackgroundColor = Color.Transparent;
                sixth7.BackgroundColor = Color.Transparent;
                seventh7.BackgroundColor = Color.Transparent;
            }
            else if (receiving == third7)
            {
                chosenOccurance = occuranceDict[text3];
                first7.BackgroundColor = Color.Transparent;
                second7.BackgroundColor = Color.Transparent;
                third7.BackgroundColor = Color.FromHex("#FFBD27");
                fourth7.BackgroundColor = Color.Transparent;
                fifth7.BackgroundColor = Color.Transparent;
                sixth7.BackgroundColor = Color.Transparent;
                seventh7.BackgroundColor = Color.Transparent;
            }
            else if (receiving == fourth7)
            {
                chosenOccurance = occuranceDict[text4];
                first7.BackgroundColor = Color.Transparent;
                second7.BackgroundColor = Color.Transparent;
                third7.BackgroundColor = Color.Transparent;
                fourth7.BackgroundColor = Color.FromHex("#FFBD27");
                fifth7.BackgroundColor = Color.Transparent;
                sixth7.BackgroundColor = Color.Transparent;
                seventh7.BackgroundColor = Color.Transparent;
            }
            else if (receiving == fifth7)
            {
                chosenOccurance = occuranceDict[text5];
                first7.BackgroundColor = Color.Transparent;
                second7.BackgroundColor = Color.Transparent;
                third7.BackgroundColor = Color.Transparent;
                fourth7.BackgroundColor = Color.Transparent;
                fifth7.BackgroundColor = Color.FromHex("#FFBD27");
                sixth7.BackgroundColor = Color.Transparent;
                seventh7.BackgroundColor = Color.Transparent;
            }
            else if (receiving == sixth7)
            {
                chosenOccurance = occuranceDict[text6];
                first7.BackgroundColor = Color.Transparent;
                second7.BackgroundColor = Color.Transparent;
                third7.BackgroundColor = Color.Transparent;
                fourth7.BackgroundColor = Color.Transparent;
                fifth7.BackgroundColor = Color.Transparent;
                sixth7.BackgroundColor = Color.FromHex("#FFBD27");
                seventh7.BackgroundColor = Color.Transparent;
            }
            else if (receiving == seventh7)
            {
                chosenOccurance = occuranceDict[text7];
                first7.BackgroundColor = Color.Transparent;
                second7.BackgroundColor = Color.Transparent;
                third7.BackgroundColor = Color.Transparent;
                fourth7.BackgroundColor = Color.Transparent;
                fifth7.BackgroundColor = Color.Transparent;
                sixth7.BackgroundColor = Color.Transparent;
                seventh7.BackgroundColor = Color.FromHex("#FFBD27");
            }
            else DisplayAlert("Error", "this goal doesn't have subtasks", "OK");

        }
    }
}
