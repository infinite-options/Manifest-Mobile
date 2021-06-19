using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Manifest.Config;
using Manifest.Interfaces;
using Manifest.LogIn.Classes;
using Manifest.Models;
using Manifest.RDS;
using Newtonsoft.Json;
using Xamarin.Auth;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Manifest.Views.AboutMePage;

namespace Manifest.Views
{
    public partial class WhoAmIPage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        string city;
        string time;
        public Models.User user;
        ObservableCollection<Person> family = new ObservableCollection<Person>();
        ObservableCollection<Person> friend = new ObservableCollection<Person>();
        public WhoAmIPage()
        {
            InitializeComponent();
            GetCurrentLocation();
            setting = false;
            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            scheduleFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            lobbyFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            supportFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            title.Text = "Settings";
            if (Device.RuntimePlatform == Device.iOS)
            {
                titleGrid.Margin = new Thickness(0, 10, 0, 0);
            }
            //frameMessageCard.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            //frameMessageDay.BackgroundColor = Color.FromHex((string)Application.Current.Properties["goal"]);
            //frameMajorEvent.BackgroundColor = Color.FromHex((string)Application.Current.Properties["routine"]);
            //frameMyHistory.BackgroundColor = Color.FromHex((string)Application.Current.Properties["event"]);

            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = GetCurrentTime();

            string version = "";
            string build = "";
            version = DependencyService.Get<IAppVersionAndBuild>().GetVersionNumber();
            build = DependencyService.Get<IAppVersionAndBuild>().GetBuildNumber();

            appVersion.Text = "App version: " + version + ", App build: " + build;
            //Debug.WriteLine("SIGNED IN WITH APPLE: USERID: " + userInfo);
            //var helperObject = new MainPage();
            //locationTitle.Text = (string)Application.Current.Properties["location"];
            //dateTitle.Text = helperObject.GetCurrentTime();
            var userId = (string)Application.Current.Properties["userId"];

            Debug.WriteLine("SIGNED IN WITH APPLE: USERID: " + userId);

            initializeUser(userId);
        }

        public string GetCurrentTime()
        {
            var currentTime = DateTime.Now;
            var time = currentTime.ToString("MMMM d, yyyy");
            return time;
        }

        public async void GetCurrentLocation()
        {
            try
            {

                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);

                    var placemark = placemarks?.FirstOrDefault();
                    if (placemark != null)
                    {
                        var geocodeAddress =
                            $"AdminArea:       {placemark.AdminArea}\n" +
                            $"CountryCode:     {placemark.CountryCode}\n" +
                            $"CountryName:     {placemark.CountryName}\n" +
                            $"FeatureName:     {placemark.FeatureName}\n" +
                            $"Locality:        {placemark.Locality}\n" +
                            $"PostalCode:      {placemark.PostalCode}\n" +
                            $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                            $"SubLocality:     {placemark.SubLocality}\n" +
                            $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                            $"Thoroughfare:    {placemark.Thoroughfare}\n";

                        Debug.WriteLine(geocodeAddress);
                        Application.Current.Properties["location"] = "";

                        Application.Current.Properties["location"] = placemark.Locality + ", " + placemark.AdminArea;
                    }
                    Debug.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                }

            }
            catch (Exception c)
            {
                // Handle not supported on device exception
                Debug.WriteLine("LOCATION MESSAGE CA:" + c.Message);
            }

        }

        private async void initializeUser(string uid)
        {
            try
            {
                string res = await RdsConnect.getUser(uid);
                if (res == "Failure")
                {
                    //await DisplayAlert("Alert", "Error in getUser() in initializeUser() in AboutMePage", "OK");
                }
                Debug.WriteLine("ABOUT ME: " + res);
                UserResponse userResponse = JsonConvert.DeserializeObject<UserResponse>(res);
                ToUser(userResponse);
                //userID.Text = (String)Application.Current.Properties["userID"];

                userName.Text = user.FirstName + " " + user.LastName;
                //userImage.Source = user.PicUrl;
                if (user.user_birth_date == null)
                {
                    userBirthDate.IsVisible = false;
                    //userBirthDate.Text = "Date of birth: N/A";
                }
                else
                {
                    try
                    {
                        var date = DateTime.Parse(user.user_birth_date);
                        //Debug.WriteLine("DATE OF BIRTH: " + date.ToString("dd MMMM yyyy"));
                        userBirthDate.Text = "Date of birth: " + date.ToString("MMM dd yyyy");
                    }
                    catch (Exception birthDate)
                    {
                        //await DisplayAlert("Oops", birthDate.Message, "OK");
                        Debug.WriteLine("Error: " + birthDate.Message);
                        userBirthDate.IsVisible = false;
                    }
                }

                if(user.MessageCard != null)
                {
                    messageCard.Text = user.MessageDay;
                }

                if (user.MessageDay != null)
                {
                    messageDay.Text = user.MessageCard;
                }

                if (user.MajorEvents != null)
                {
                    majorEvents.Text = user.MajorEvents;
                }

                if (user.MyHistory != null)
                {
                    myHistory.Text = user.MyHistory;
                }

                if (user.PicUrl != null && user.PicUrl != "")
                {
                    userPic.Source = user.PicUrl;
                }

                //double height = Math.Max(messageCard.Text.Length + messageCard.Text.Length, messageDay.Text.Length + messageDay.Text.Length);
                //Debug.WriteLine("HEIGHT: " + height);
                //frameMessageCard.HeightRequest = height;
                //frameMessageDay.HeightRequest = height;

                //height = Math.Max(majorEvents.Text.Length + majorEvents.Text.Length, myHistory.Text.Length + myHistory.Text.Length);
                //Debug.WriteLine("HEIGHT: " + height);
                //frameMajorEvent.HeightRequest = height;
                //frameMyHistory.HeightRequest = height;

            }
            catch (Exception a)
            {

            }

        }

        private void ToUser(UserResponse userResponse)
        {
            family.Clear();
            friend.Clear();
            var familyCounter = 1;
            var friendCounter = 1;
            var height1 = 30;
            var height2 = 20;
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
                            user_birth_date = dto.user_birth_date
                        };
                    }
                    else
                    {
                        var firstName = "";
                        foreach (char a in dto.people_name)
                        {
                            if (a != ' ')
                            {
                                firstName += a;
                            }
                            else
                            {
                                break;
                            }
                        }
                        Person toAdd = new Person()
                        {
                            Name = firstName,
                            Relation = dto.relation_type,
                            PicUrl = dto.ta_picture,
                            Id = dto.ta_people_id,
                            PhoneNumber = dto.ta_phone
                        };
                        if (toAdd.PicUrl == null || toAdd.PicUrl == "")
                        {
                            toAdd.PicUrl = "aboutme.png";
                        }
                        if(toAdd.Relation == "Family")
                        {
                            //family.Add(toAdd);
                            if (familyCounter <= 5)
                            {
                                var myStack = new StackLayout();
                                myStack.HorizontalOptions = LayoutOptions.Center;
                                myStack.WidthRequest = 40;
                                myStack.HeightRequest = 40;

                                var tap = new TapGestureRecognizer();
                                tap.Tapped += TapGestureRecognizer_Tapped_1;
                                myStack.GestureRecognizers.Add(tap);

                                var myFrame = new Frame();
                                myFrame.IsClippedToBounds = true;
                                myFrame.Padding = 0;
                                myFrame.WidthRequest = 40;
                                myFrame.HeightRequest = 40;
                                myFrame.HasShadow = false;
                                myFrame.CornerRadius = 20;
                                myFrame.BorderColor = Color.Black;

                                var myImage = new Image();
                                myImage.Source = toAdd.PicUrl;
                                myImage.Aspect = Aspect.AspectFill;

                                myFrame.Content = myImage;

                                myStack.Children.Add(myFrame);

                                

                                var name = new CustomizeFontLabel();
                                name.HorizontalTextAlignment = TextAlignment.Center;
                                name.Text = toAdd.Name;
                                name.TextColor = Color.Black;
                                name.FontSize = 9;

                                myStack.Children.Add(name);

                                var phone = new CustomizeFontLabel();
                                phone.HorizontalTextAlignment = TextAlignment.Center;
                                phone.Text = toAdd.PhoneNumber;
                                phone.IsVisible = false;

                                myStack.Children.Add(phone);
                                familyMembersList.Children.Add(myStack);
                                //height1 += 60;
                            }
                            familyCounter++;
                        }
                        if (toAdd.Relation == "Friends" || toAdd.Relation == "Friend")
                        {
                            //friend.Add(toAdd);
                            if (friendCounter <= 5)
                            {
                                var myStack = new StackLayout();
                                myStack.HorizontalOptions = LayoutOptions.Center;
                                myStack.WidthRequest = 40;
                                myStack.HeightRequest = 40;

                                var tap = new TapGestureRecognizer();
                                tap.Tapped += TapGestureRecognizer_Tapped_2;
                                myStack.GestureRecognizers.Add(tap);

                                var myFrame = new Frame();
                                myFrame.IsClippedToBounds = true;
                                myFrame.Padding = 0;
                                myFrame.WidthRequest = 40;
                                myFrame.HeightRequest = 40;
                                myFrame.HasShadow = false;
                                myFrame.CornerRadius = 20;
                                myFrame.BorderColor = Color.Black;

                                var myImage = new Image();
                                myImage.Source = toAdd.PicUrl;
                                myImage.Aspect = Aspect.AspectFill;

                                myFrame.Content = myImage;

                                myStack.Children.Add(myFrame);



                                var name = new CustomizeFontLabel();
                                name.HorizontalTextAlignment = TextAlignment.Center;
                                name.Text = toAdd.Name;
                                name.TextColor = Color.Black;
                                name.FontSize = 9;

                                myStack.Children.Add(name);

                                var phone = new CustomizeFontLabel();
                                phone.HorizontalTextAlignment = TextAlignment.Center;
                                phone.Text = toAdd.PhoneNumber;
                                phone.IsVisible = false;

                                myStack.Children.Add(phone);
                                friendMembersList.Children.Add(myStack);
                                //height2 += 60;
                            }
                            friendCounter++;
                        }
                    }
                }
                //familyMembersList.ItemsSource = family;
                //friendMembersList.ItemsSource = friend;
                //familyMembersList.HeightRequest = height1;
                //friendMembersList.HeightRequest = height2;
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

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage.Navigation.PopModalAsync(false);
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new SettingsPage("WhoAmIPage");
        }

        void Button_Clicked_1(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new TodaysListPage());
        }

        void Button_Clicked_2(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void Button_Clicked_3(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new AboutMePage();
        }

        void TapGestureRecognizer_Tapped_1(System.Object sender, System.EventArgs e)
        {
            var myStack = (StackLayout)sender;
            var phone = (Label)myStack.Children[2];
            PerformCall(phone.Text);
        }

        async void PerformCall(string phoneNumber)
        {
            if (phoneNumber == "")
            {
                await Application.Current.MainPage.DisplayAlert("Sorry!", $"Hmmm... We don't have a phone number on file", "OK");
            }
            else
            {
                //Console.WriteLine("ZZZZZZZZZZZZZZZ");
                Debug.WriteLine("Manifest.ViewModels.AboutViewModel: Dialing Number:" + phoneNumber);
                //Console.WriteLine("ZZZZZZZZZZZZZZZ");
                try
                {
                    PhoneDialer.Open(phoneNumber);
                    Debug.WriteLine("IN ABOUTVIEWMODEL. LAUNCHING PHONE");
                }
                catch (Exception e)
                {
                    await DisplayAlert("Error", "Unable to perform a phone call", "OK");
                }
                //await Launcher.OpenAsync(new Uri("tel:" + phoneNumber));
            }
        }

        void TapGestureRecognizer_Tapped_2(System.Object sender, System.EventArgs e)
        {
            var myStack = (StackLayout)sender;
            var phone = (Label)myStack.Children[2];
            PerformCall(phone.Text);
        }

        async void Slider_DragCompleted(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine("VALUE1: " + (int)slider1.Value);
            Debug.WriteLine("X VALUE: " + slider1.Width);
            var f = slider1.Width / 120;
            value1.Text = ((int)slider1.Value).ToString();
            value1.Margin = new Thickness(11 + f* (int)slider1.Value, 8, 0, -8);

            await SendRequest("anxiety_scale", ((int)slider1.Value).ToString());
        }

        async void Slider_DragCompleted_1(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine("VALUE2: " + (int)slider2.Value);
            var f = slider2.Width / 120;
            value2.Text = ((int)slider2.Value).ToString();
            value2.Margin = new Thickness(11 + f * (int)slider2.Value, 8, 0, -8);
            await SendRequest("mood_scale", ((int)slider2.Value).ToString());
        }

        public async Task<bool> SendRequest(string category, string option)
        {
            var client = new HttpClient();
            var feedback = new Assessment();

            feedback.user_id = (string)Application.Current.Properties["userId"];
            feedback.category = category;
            feedback.name = option;

            var feedbackJSON = JsonConvert.SerializeObject(feedback);

            Debug.WriteLine(feedbackJSON);

            var postContent = new StringContent(feedbackJSON, Encoding.UTF8, "application/json");
            var rdsResponse = await client.PostAsync(AppConstants.BaseUrl + AppConstants.addPulse, postContent);

            return rdsResponse.IsSuccessStatusCode;
        }

        void LogOut(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new LogInPage();
        }
    }
}
