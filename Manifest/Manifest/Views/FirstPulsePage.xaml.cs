using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Manifest.Config;
using Manifest.Models;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class FirstPulsePage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        string city;
        string time;
        string option;

        public FirstPulsePage()
        {
            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;
            

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            submitButton.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            title.Text = "How do I feel?";
            subTitle.Text = "Choose 1";
            NavigationPage.SetHasNavigationBar(this, false);
            //calendarSwitch.IsToggled = (bool)Application.Current.Properties["showCalendar"];
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            if(option != null && option != "")
            {
                var response =  await SendRequest("feelings", option);

                if (!response)
                {
                    await DisplayAlert("Oops", "We were not able to fulfill this request. Please check 'changeAboutMeHistory' endpoint.", "OK");
                }
                _ = Navigation.PushAsync(new SecondPulsePage(), false);
            }
            else
            {
                await DisplayAlert("Select one option", "Please select one of the following options", "OK");
            }
        }

         void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            var selection = (StackLayout)sender;
            option = selection.ClassId;

            if(option == "1")
            {
                mood_one.Stroke = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["header"]));
                mood_one.StrokeThickness = 5;

                mood_two.StrokeThickness = 0;
                mood_three.StrokeThickness = 0;
                mood_four.StrokeThickness = 0;
                mood_five.StrokeThickness = 0;
                mood_six.StrokeThickness = 0;
                mood_seven.StrokeThickness = 0;
            }
            else if(option == "2")
            {
                mood_two.Stroke = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["header"]));
                mood_two.StrokeThickness = 5;

                mood_one.StrokeThickness = 0;
                mood_three.StrokeThickness = 0;
                mood_four.StrokeThickness = 0;
                mood_five.StrokeThickness = 0;
                mood_six.StrokeThickness = 0;
                mood_seven.StrokeThickness = 0;
            }
            else if (option == "3")
            {
                mood_three.Stroke = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["header"]));
                mood_three.StrokeThickness = 5;

                mood_one.StrokeThickness = 0;
                mood_two.StrokeThickness = 0;
                mood_four.StrokeThickness = 0;
                mood_five.StrokeThickness = 0;
                mood_six.StrokeThickness = 0;
                mood_seven.StrokeThickness = 0;
            }
            else if (option == "4")
            {
                mood_four.Stroke = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["header"]));
                mood_four.StrokeThickness = 5;

                mood_one.StrokeThickness = 0;
                mood_two.StrokeThickness = 0;
                mood_three.StrokeThickness = 0;
                mood_five.StrokeThickness = 0;
                mood_six.StrokeThickness = 0;
                mood_seven.StrokeThickness = 0;
            }
            else if (option == "5")
            {
                mood_five.Stroke = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["header"]));
                mood_five.StrokeThickness = 5;

                mood_one.StrokeThickness = 0;
                mood_two.StrokeThickness = 0;
                mood_three.StrokeThickness = 0;
                mood_four.StrokeThickness = 0;
                mood_six.StrokeThickness = 0;
                mood_seven.StrokeThickness = 0;
            }
            else if (option == "6")
            {
                mood_six.Stroke = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["header"]));
                mood_six.StrokeThickness = 5;

                mood_one.StrokeThickness = 0;
                mood_two.StrokeThickness = 0;
                mood_three.StrokeThickness = 0;
                mood_five.StrokeThickness = 0;
                mood_four.StrokeThickness = 0;
                mood_seven.StrokeThickness = 0;
            }
            else if (option == "7")
            {
                mood_seven.Stroke = new SolidColorBrush(Color.FromHex((string)Application.Current.Properties["header"]));
                mood_seven.StrokeThickness = 5;
  
                mood_one.StrokeThickness = 0;
                mood_two.StrokeThickness = 0;
                mood_three.StrokeThickness = 0;
                mood_five.StrokeThickness = 0;
                mood_six.StrokeThickness = 0;
                mood_four.StrokeThickness = 0;
            }
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new SettingsPage("FirstPulsePage");
            Navigation.PushAsync(new SettingsPage(), false);
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
    }
}
