using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Manifest.Models;
using Manifest.RDS;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Auth;
using Manifest.Config;
using Manifest.LogIn.Classes;

namespace Manifest.Views
{
    public partial class AboutMePage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;

        public class UserResponse
        {
            public string message { get; set; }
            public List<UserDto> result { get; set; }
        }
        public class UserDto
        {
            // User Data
            public string user_have_pic { get; set; }
            public string message_card { get; set; }
            public string message_day { get; set; }
            public string user_picture { get; set; }
            public string user_first_name { get; set; }
            public string user_last_name { get; set; }
            public string user_email_id { get; set; }
            public string evening_time { get; set; }
            public string morning_time { get; set; }
            public string afternoon_time { get; set; }
            public string night_time { get; set; }
            public string day_end { get; set; }
            public string day_start { get; set; }
            public string time_zone { get; set; }

            // Important People or TA details below
            public string ta_people_id { get; set; }
            public string email_id { get; set; }
            public string people_name { get; set; }
            public string have_pic { get; set; }
            public string ta_picture { get; set; }
            public string important { get; set; }
            public string user_unique_id { get; set; }
            public string relation_type { get; set; }
            public string ta_phone { get; set; }
        }

        public Models.User user;
        List<Person> importantPeople = new List<Person>();
        double deviceHeight = DeviceDisplay.MainDisplayInfo.Height;
        double deviceWidth = DeviceDisplay.MainDisplayInfo.Width;

        float bigCircleHW;
        float smallCircleHW;
        float bigCircleRadius;
        float smallCircleRadius;
        float bigImageCircleHW;
        float bigImageCircleRadius;

        public ObservableCollection<Grid> datagrid = new ObservableCollection<Grid>();

        public AboutMePage()
        {
            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
            calendarSwitch.IsToggled = (bool)Application.Current.Properties["showCalendar"];
            title.Text = "About me";
            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();

            bigCircleHW = (float)(deviceWidth * 0.102);
            bigCircleRadius = (float)(bigCircleHW * 0.75);
            smallCircleHW = (float)(deviceWidth * 0.075);
            smallCircleRadius = (float)(smallCircleHW * 0.5);
            bigImageCircleHW = (float)(deviceWidth * 0.11);
            bigImageCircleRadius = (float)(bigCircleHW * 0.5);
            userImageFrame.CornerRadius = bigImageCircleRadius;
            userImageFrame.HeightRequest = bigImageCircleHW;
            userImageFrame.WidthRequest = bigImageCircleHW;
            whoAmIButton.CornerRadius = bigCircleRadius;
            whoAmIButton.HeightRequest = bigCircleHW;
            whoAmIButton.WidthRequest = bigCircleHW;
            whatMotivatesMeButton.CornerRadius = bigCircleRadius;
            whatMotivatesMeButton.HeightRequest = bigCircleHW;
            whatMotivatesMeButton.WidthRequest = bigCircleHW;
            Debug.WriteLine("In aboutME page");
            //Debug.WriteLine(Application.Current.Properties["userID"]);
            //Debug.WriteLine(Application.Current.Properties["time_stamp"]);
            initializeUser((String)Application.Current.Properties["userId"]);
            //userAdvisors.ItemTemplate = datagrid;


        }


        private async void initializeUser(string uid)
        {
            try
            {
                string res = await RdsConnect.getUser(uid);
                if (res == "Failure")
                {
                    await DisplayAlert("Alert", "Error in getUser() in initializeUser() in AboutMePage", "OK");
                }
                UserResponse userResponse = JsonConvert.DeserializeObject<UserResponse>(res);
                ToUser(userResponse);
                //userID.Text = (String)Application.Current.Properties["userID"];
                userName.Text = user.FirstName + " " + user.LastName;
                userImage.Source = user.PicUrl;
                CreateList();
            }
            catch (Exception a)
            {

            }

        }

        private void ToUser(UserResponse userResponse)
        {
            //User newUser = null;
            importantPeople.Clear();
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
                            TimeSettings = ToTimeSettings(dto)
                        };
                    }
                    else
                    {
                        Person toAdd = new Person()
                        {
                            Name = dto.people_name,
                            Relation = dto.relation_type,
                            PicUrl = dto.ta_picture,
                            Id = dto.ta_people_id,
                            PhoneNumber = dto.ta_phone
                        };
                        if (toAdd.PicUrl == null || toAdd.PicUrl == "")
                        {
                            toAdd.PicUrl = "aboutme.png";
                        }
                        importantPeople.Add(toAdd);
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

        public async void callAdvisor(object sender, EventArgs args)
        {
            Debug.WriteLine("Tap registered");
            Frame myvar = (Frame)sender;
            Person advisor = myvar.BindingContext as Person;
            string phoneNumber = advisor.PhoneNumber;
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

        private void CreateList()
        {
            int i = 0;
            int row = 0;
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.NumberOfTapsRequired = 1;
            tapGestureRecognizer.Tapped += callAdvisor;
            while (i < importantPeople.Count)
            {
                //ObservableCollection<Person> temp = new ObservableCollection<Person>();
                Grid tempGrid = new Grid
                {
                    RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(60, GridUnitType.Absolute) },
                        new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) }
                    },
                    ColumnSpacing = 20.0,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center

                };
                int col = 0;
                //Even rows
                int toAdd = 2;
                if (row % 2 == 0)
                {
                    toAdd = 3;
                }
                int min = Math.Min(i + toAdd, importantPeople.Count);
                //Debug.WriteLine("min = " + min);
                while (i < min){
                    tempGrid.ColumnDefinitions.Add(new ColumnDefinition
                    { Width = new GridLength(smallCircleHW, GridUnitType.Absolute) });
                    tempGrid.Children.Add(new Frame{
                        BindingContext = importantPeople[i],
                        CornerRadius = smallCircleRadius,
                        HeightRequest = smallCircleHW,
                        WidthRequest = smallCircleHW,
                        Padding = 0,
                        IsClippedToBounds = true,
                        //Padding = new Thickness(10.0,0.0,10.0,0.0),
                        Content =  new Image{
                            Source = importantPeople[i].PicUrl,
                            HeightRequest = smallCircleHW*2,
                            WidthRequest = smallCircleHW*2,
                            Aspect = Aspect.AspectFill
                        },
                        GestureRecognizers = { tapGestureRecognizer }
                    }, col, 0);
                    tempGrid.Children.Add(new Label
                    {
                        Text = importantPeople[i].Name,
                        TextColor = Color.Black
                    }, col, 1);
                    col++;
                    i++;
                }
                //Debug.WriteLine("i = " + i);
                userAdvisors.Children.Add(tempGrid);
                row++;
            }
            Debug.WriteLine("Num elements in stack: " + userAdvisors.Children.Count);
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            // Application.Current.Properties.Remove("mobile_auth_token");
            // Debug.WriteLine(Application.Current.Properties["userID"]);
            // Debug.WriteLine(Application.Current.Properties["platform"]);
            // Debug.WriteLine(Application.Current.Properties["time_stamp"]);
            // Application.Current.Properties.Remove("userID");
            // Application.Current.Properties.Remove("platform");
            // Application.Current.Properties.Remove("time_stamp");
            Application.Current.MainPage = new LogInPage();
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
