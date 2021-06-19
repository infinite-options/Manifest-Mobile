using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Manifest.Config;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class ThirdPulsePage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        string city;
        string time;
        string option;
        List<string> options;

        double deviceHeight = DeviceDisplay.MainDisplayInfo.Height;
        double deviceWidth = DeviceDisplay.MainDisplayInfo.Width;

        public ThirdPulsePage()
        {
            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;


            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            submitButton.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            //barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
            title.Text = "What is important to me?";
            subTitle.Text = "Choose 1";
            NavigationPage.SetHasNavigationBar(this, false);
            var userId = (string)Application.Current.Properties["userId"];
            options = new List<string>();
            _ = SetAboutMeImportantOptionsAsync("important", userId);
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            if (option != null && option != "")
            { 
                var helper = new FirstPulsePage();
                var response = await helper.SendRequest("important", option);

                if (!response)
                {
                    await DisplayAlert("Oops", "We were not able to fulfill this request. Please check 'changeAboutMeHistory' endpoint.", "OK");
                }
                _ = Navigation.PushAsync(new FourthPulsePage(), false);
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
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            //Application.Current.MainPage = new SettingsPage("FirstPulsePage");
            Navigation.PushAsync(new SettingsPage(), false);
        }

        public async Task SetAboutMeImportantOptionsAsync(string category, string userId)
        {
            try
            {
                var helper = new SecondPulsePage();
                options = await helper.GetOptions(category, userId);
                if (options.Count != 0)
                {
                    setGoals();
                }
                else
                {
                    Application.Current.MainPage = new MainPage();
                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Oops", e.Message, "OK");
            }
        }

        private void setGoals()
        {
            try
            {
                if (options.Count == 1)
                {
                    text1.Text = options[0];

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
                else if (options.Count == 2)
                {
                    text1.Text = options[0];

                    text2.Text = options[1];

                    stack2.Margin = new Thickness(50, 0, 0, 0);

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
                else if (options.Count == 3)
                {
                    text1.Text = options[0];

                    text2.Text = options[1];

                    text3.Text = options[2];

                    stack2.Margin = new Thickness(50, 0, 0, 0);

                    stack4.IsEnabled = false;
                    stack4.IsVisible = false;

                    stack5.IsEnabled = false;
                    stack5.IsVisible = false;

                    stack6.IsEnabled = false;
                    stack6.IsVisible = false;

                    stack7.IsEnabled = false;
                    stack7.IsVisible = false;
                }
                else if (options.Count == 4)
                {
                    text1.Text = options[0];

                    text2.Text = options[1];

                    text3.Text = options[2];

                    text4.Text = options[3];

                    stack2.Margin = new Thickness(70, 0, 0, 0);

                    stack5.IsEnabled = false;
                    stack5.IsVisible = false;

                    stack6.IsEnabled = false;
                    stack6.IsVisible = false;

                    stack7.IsEnabled = false;
                    stack7.IsVisible = false;
                }
                else if (options.Count == 5)
                {
                    text1.Text = options[0];

                    text2.Text = options[1];

                    text3.Text = options[2];

                    text4.Text = options[3];

                    text5.Text = options[4];

                    stack2.Margin = new Thickness(0, 0, 0, 0);
                    stack5.Margin = new Thickness(15, 5, 0, 0);

                    stack6.IsEnabled = false;
                    stack6.IsVisible = false;

                    stack7.IsEnabled = false;
                    stack7.IsVisible = false;
                }
                else if (options.Count == 6)
                {
                    text1.Text = options[0];

                    text2.Text = options[1];

                    text3.Text = options[2];

                    text4.Text = options[3];

                    text5.Text = options[4];

                    text6.Text = options[5];

                    stack7.IsEnabled = false;
                    stack7.IsVisible = false;
                }
                else
                {
                    text1.Text = options[0];

                    text2.Text = options[1];

                    text3.Text = options[2];

                    text4.Text = options[3];

                    text5.Text = options[4];

                    text6.Text = options[5];

                    text7.Text = options[6];
                }
                gridDisplay.IsVisible = true;
                return;
            }
            catch (Exception getGoalsIssue)
            {
                Debug.WriteLine(getGoalsIssue.Message);
            }
        }

        void navigatetoActionsFrame(System.Object sender, System.EventArgs e)
        {
            var myStack = (StackLayout)sender;
            int index = Int16.Parse(myStack.ClassId) - 1;

            if (myStack.ClassId == "1")
            {
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
                option = options[index];
            }
            catch (Exception indexIssue)
            {
                Debug.WriteLine(indexIssue.Message);
            }
        }
    }
}
