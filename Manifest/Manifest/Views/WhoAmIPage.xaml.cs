using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manifest.Config;
using Manifest.LogIn.Classes;
using Manifest.Models;
using Manifest.RDS;
using Newtonsoft.Json;
using Xamarin.Auth;
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
        public WhoAmIPage()
        {
            InitializeComponent();

            setting = false;
            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            scheduleFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            lobbyFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            supportFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            title.Text = "My Story";

            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();
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
                }
                Debug.WriteLine("ABOUT ME: " + res);
                UserResponse userResponse = JsonConvert.DeserializeObject<UserResponse>(res);
                ToUser(userResponse);
                //userID.Text = (String)Application.Current.Properties["userID"];

                userName.Text = user.FirstName + " " + user.LastName;
                //userImage.Source = user.PicUrl;
                if (user.user_birth_date == null)
                {
                    userBirthDate.Text = "Date of birth: N/A";
                }
                else
                {
                    try
                    {
                        var date = DateTime.Parse(user.user_birth_date);
                        Debug.WriteLine("DATE OF BIRTH: " + date.ToString("dd MMMM yyyy"));
                        userBirthDate.Text = "Date of birth: " + date.ToString("dd MMMM yyyy");
                    }
                    catch (Exception birthDate)
                    {
                        await DisplayAlert("Oops", birthDate.Message, "OK");
                        userBirthDate.Text = "Date of birth: N/A";
                    }
                }

                if(user.MessageCard != null)
                {
                    messageCard.Text = user.MessageCard;
                }

                if (user.MessageDay != null)
                {
                    messageCard.Text = user.MessageDay;
                }

                if (user.PicUrl != null && user.PicUrl != "")
                {
                    userPic.Source = user.PicUrl;
                }

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
                            TimeSettings = ToTimeSettings(dto),
                            user_birth_date = dto.user_birth_date
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

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
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
    }
}
