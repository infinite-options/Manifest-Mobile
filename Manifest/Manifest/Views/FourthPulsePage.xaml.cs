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
    public partial class FourthPulsePage : ContentPage
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
        public FourthPulsePage()
        {
            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;

            
            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            submitButton.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            //barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
            title.Text = "What motivates me?";
            subTitle.Text = "Choose 1";
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            var userId = (string)Application.Current.Properties["userId"];
            options = new List<string>();
            _ = SetAboutMeMotivatesOptionsAsync("motivation", userId);
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            if (option != null && option != "")
            {
                var helper = new FirstPulsePage();
                var response = await helper.SendRequest("motivation", option);

                if (!response)
                {
                    await DisplayAlert("Oops", "We were not able to fulfill this request. Please check 'changeAboutMeHistory' endpoint.", "OK");
                }
                Application.Current.MainPage = new MainPage();
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
            Navigation.PushAsync(new SettingsPage(),false);
        }


        public async Task SetAboutMeMotivatesOptionsAsync(string category, string userId)
        {
            try
            {
                var helper = new SecondPulsePage();
                options = await helper.GetOptions(category, userId);
                if (options.Count != 0 && options != null)
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

                if (options.Count == 0)
                {
                    DisplayAlert("Oops", "no goals available at this time", "OK");
                }
                else if (options.Count == 1)
                {
                    setProperties1();
                    show7();
                    //first7.Text = goalsInRange[0].gr_title;
                    text1.Text = options[0];
                    //occuranceDict.Add(text1, options[0]);
                    //occuranceDict.Add(first7, goalsInRange[0]);
                }
                else if (options.Count == 2)
                {
                    setProperties2();
                    show7();
                    //first7.Text = goalsInRange[0].gr_title;
                    //second7.Text = goalsInRange[1].gr_title;
                    text1.Text = options[0];
                    //occuranceDict.Add(text1, options[0]);
                    //occuranceDict.Add(first7, goalsInRange[0]);

                    text2.Text = options[1];
                    //occuranceDict.Add(text2, options[1]);
                    //occuranceDict.Add(second7, goalsInRange[1]);
                }
                else if (options.Count == 3)
                {
                    setProperties3();
                    show3();
                    //first7.Text = goalsInRange[0].gr_title;
                    //second7.Text = goalsInRange[1].gr_title;
                    //third7.Text = goalsInRange[2].gr_title;
                    text1.Text = options[0];
                    //occuranceDict.Add(text1, options[0]);
                    //occuranceDict.Add(first7, goalsInRange[0]);

                    text2.Text = options[1];
                    //occuranceDict.Add(text2, options[1]);
                    //occuranceDict.Add(second7, goalsInRange[1]);

                    text3.Text = options[2];
                    //occuranceDict.Add(text3, options[2]);
                    //occuranceDict.Add(third7, goalsInRange[2]);

                }
                else if (options.Count == 4)
                {
                    setProperties4();
                    show7();
                    text1.Text = options[0];
                    //occuranceDict.Add(text1, options[0]);
                    //occuranceDict.Add(first7, goalsInRange[0]);

                    text2.Text = options[1];
                    //occuranceDict.Add(text2, options[1]);
                    //occuranceDict.Add(second7, goalsInRange[1]);

                    text3.Text = options[2];
                    //occuranceDict.Add(text3, options[2]);
                    //occuranceDict.Add(third7, goalsInRange[2]);

                    text4.Text = options[3];
                    //occuranceDict.Add(text4, options[3]);
                    //occuranceDict.Add(fourth7, goalsInRange[3]);

                }
                else if (options.Count == 5)
                {
                    setProperties5();
                    show7();
                    //first5.Text = goalsInRange[0].gr_title;
                    text1.Text = options[0];
                    //occuranceDict.Add(text1, options[0]);
                    //occuranceDict.Add(first7, goalsInRange[0]);

                    text2.Text = options[1];
                    //occuranceDict.Add(text2, options[1]);
                    //occuranceDict.Add(second7, goalsInRange[1]);

                    text3.Text = options[2];
                    //occuranceDict.Add(text3, options[2]);
                    //occuranceDict.Add(third7, goalsInRange[2]);

                    text4.Text = options[3];
                    //occuranceDict.Add(text4, options[3]);
                    //occuranceDict.Add(fourth7, goalsInRange[3]);

                    text5.Text = options[4];
                    //occuranceDict.Add(text5, options[4]);
                    //occuranceDict.Add(fifth7, goalsInRange[4]);

                }
                else if (options.Count == 6)
                {
                    setProperties6();
                    show7();
                    text1.Text = options[0];
                    //occuranceDict.Add(text1, options[0]);
                    //occuranceDict.Add(first7, goalsInRange[0]);

                    text2.Text = options[1];
                    //occuranceDict.Add(text2, options[1]);
                    //occuranceDict.Add(second7, goalsInRange[1]);

                    text3.Text = options[2];
                    //occuranceDict.Add(text3, options[2]);
                    //occuranceDict.Add(third7, goalsInRange[2]);

                    text4.Text = options[3];
                    //occuranceDict.Add(text4, options[3]);
                    //occuranceDict.Add(fourth7, goalsInRange[3]);

                    text5.Text = options[4];
                    //occuranceDict.Add(text5, options[4]);
                    //occuranceDict.Add(fifth7, goalsInRange[4]);

                    text6.Text = options[5];
                    //occuranceDict.Add(text6, options[5]);
                    //occuranceDict.Add(sixth7, goalsInRange[5]);

                }
                else
                {
                    setProperties7();
                    show7();
                    text1.Text = options[0];
                    //occuranceDict.Add(text1, options[0]);
                    //occuranceDict.Add(first7, goalsInRange[0]);

                    text2.Text = options[1];
                    //occuranceDict.Add(text2, options[1]);
                    //occuranceDict.Add(second7, goalsInRange[1]);

                    text3.Text = options[2];
                    //occuranceDict.Add(text3, options[2]);
                    //occuranceDict.Add(third7, goalsInRange[2]);

                    text4.Text = options[3];
                    //occuranceDict.Add(text4, options[3]);
                    //occuranceDict.Add(fourth7, goalsInRange[3]);

                    text5.Text = options[4];
                    //occuranceDict.Add(text5, options[4]);
                    //occuranceDict.Add(fifth7, goalsInRange[4]);

                    text6.Text = options[5];
                    //occuranceDict.Add(text6, options[5]);
                    //occuranceDict.Add(sixth7, goalsInRange[5]);

                    text7.Text = options[6];
                    //occuranceDict.Add(text7, options[6]);
                    //occuranceDict.Add(seventh7, goalsInRange[6]);

                }


                return;
            }
            catch (Exception e)
            {
                DisplayAlert("Alert", "Error in TodaysListTest ToOccurances(). Error: " + e.ToString(), "OK");
            }
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

        void navigatetoActions(System.Object sender, System.EventArgs e)
        {


            //chosenOccurance = occuranceDict[receiving];

            //if (receiving == text1)
            //{
            //    first7.BackgroundColor = Color.FromHex("#FFBD27");
            //    second7.BackgroundColor = Color.Transparent;
            //    third7.BackgroundColor = Color.Transparent;
            //    fourth7.BackgroundColor = Color.Transparent;
            //    fifth7.BackgroundColor = Color.Transparent;
            //    sixth7.BackgroundColor = Color.Transparent;
            //    seventh7.BackgroundColor = Color.Transparent;
            //}
            //else if (receiving == text2)
            //{
            //    first7.BackgroundColor = Color.Transparent;
            //    second7.BackgroundColor = Color.FromHex("#FFBD27");
            //    third7.BackgroundColor = Color.Transparent;
            //    fourth7.BackgroundColor = Color.Transparent;
            //    fifth7.BackgroundColor = Color.Transparent;
            //    sixth7.BackgroundColor = Color.Transparent;
            //    seventh7.BackgroundColor = Color.Transparent;
            //}
            //else if (receiving == text3)
            //{
            //    first7.BackgroundColor = Color.Transparent;
            //    second7.BackgroundColor = Color.Transparent;
            //    third7.BackgroundColor = Color.FromHex("#FFBD27");
            //    fourth7.BackgroundColor = Color.Transparent;
            //    fifth7.BackgroundColor = Color.Transparent;
            //    sixth7.BackgroundColor = Color.Transparent;
            //    seventh7.BackgroundColor = Color.Transparent;
            //}
            //else if (receiving == text4)
            //{
            //    first7.BackgroundColor = Color.Transparent;
            //    second7.BackgroundColor = Color.Transparent;
            //    third7.BackgroundColor = Color.Transparent;
            //    fourth7.BackgroundColor = Color.FromHex("#FFBD27");
            //    fifth7.BackgroundColor = Color.Transparent;
            //    sixth7.BackgroundColor = Color.Transparent;
            //    seventh7.BackgroundColor = Color.Transparent;
            //}
            //else if (receiving == text5)
            //{
            //    first7.BackgroundColor = Color.Transparent;
            //    second7.BackgroundColor = Color.Transparent;
            //    third7.BackgroundColor = Color.Transparent;
            //    fourth7.BackgroundColor = Color.Transparent;
            //    fifth7.BackgroundColor = Color.FromHex("#FFBD27");
            //    sixth7.BackgroundColor = Color.Transparent;
            //    seventh7.BackgroundColor = Color.Transparent;
            //}
            //else if (receiving == text6)
            //{
            //    first7.BackgroundColor = Color.Transparent;
            //    second7.BackgroundColor = Color.Transparent;
            //    third7.BackgroundColor = Color.Transparent;
            //    fourth7.BackgroundColor = Color.Transparent;
            //    fifth7.BackgroundColor = Color.Transparent;
            //    sixth7.BackgroundColor = Color.FromHex("#FFBD27");
            //    seventh7.BackgroundColor = Color.Transparent;
            //}
            //else
            //{
            //    first7.BackgroundColor = Color.Transparent;
            //    second7.BackgroundColor = Color.Transparent;
            //    third7.BackgroundColor = Color.Transparent;
            //    fourth7.BackgroundColor = Color.Transparent;
            //    fifth7.BackgroundColor = Color.Transparent;
            //    sixth7.BackgroundColor = Color.Transparent;
            //    seventh7.BackgroundColor = Color.FromHex("#FFBD27");
            //}
        }

        //private void setGoals()
        //{
        //    try
        //    {
        //        if (startTime == endTime)
        //        {
        //            foreach (Occurance dto in currentOccurances)
        //            {
        //                if (ToDateTime(dto.StartDayAndTime.ToString("t")).TimeOfDay <= ToDateTime(startTime).TimeOfDay && ToDateTime(dto.EndDayAndTime.ToString("t")).TimeOfDay >= ToDateTime(endTime).TimeOfDay)
        //                {
        //                    Debug.WriteLine(dto.Title + " passes");
        //                    goalsInRange.Add(dto);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (Occurance dto in currentOccurances)
        //            {
        //                if (ToDateTime(dto.StartDayAndTime.ToString("t")).TimeOfDay >= ToDateTime(startTime).TimeOfDay && ToDateTime(dto.EndDayAndTime.ToString("t")).TimeOfDay <= ToDateTime(endTime).TimeOfDay)
        //                {
        //                    Debug.WriteLine(dto.Title + " status: in progress: " + dto.IsInProgress + " completed: " + dto.IsComplete);
        //                    goalsInRange.Add(dto);
        //                }
        //            }
        //        }

        //        Debug.WriteLine("number of today goals: " + todayOccurs.Count.ToString());
        //        Debug.WriteLine("number of ranged goals: " + goalsInRange.Count.ToString());

        //        if (goalsInRange.Count == 0)
        //        {
        //            DisplayAlert("Oops", "no goals available at this time", "OK");
        //        }
        //        else if (goalsInRange.Count == 1)
        //        {
        //            setProperties1();
        //            show7();
        //            //first7.Text = goalsInRange[0].gr_title;
        //            text1.Text = goalsInRange[0].Title;
        //            occuranceDict.Add(text1, goalsInRange[0]);
        //            //occuranceDict.Add(first7, goalsInRange[0]);
        //        }
        //        else if (goalsInRange.Count == 2)
        //        {
        //            setProperties2();
        //            show7();
        //            //first7.Text = goalsInRange[0].gr_title;
        //            //second7.Text = goalsInRange[1].gr_title;
        //            text1.Text = goalsInRange[0].Title;
        //            occuranceDict.Add(text1, goalsInRange[0]);
        //            //occuranceDict.Add(first7, goalsInRange[0]);

        //            text2.Text = goalsInRange[1].Title;
        //            occuranceDict.Add(text2, goalsInRange[1]);
        //            //occuranceDict.Add(second7, goalsInRange[1]);
        //        }
        //        else if (goalsInRange.Count == 3)
        //        {
        //            setProperties3();
        //            show3();
        //            //first7.Text = goalsInRange[0].gr_title;
        //            //second7.Text = goalsInRange[1].gr_title;
        //            //third7.Text = goalsInRange[2].gr_title;
        //            text1.Text = goalsInRange[0].Title;
        //            occuranceDict.Add(text1, goalsInRange[0]);
        //            //occuranceDict.Add(first7, goalsInRange[0]);

        //            text2.Text = goalsInRange[1].Title;
        //            occuranceDict.Add(text2, goalsInRange[1]);
        //            //occuranceDict.Add(second7, goalsInRange[1]);

        //            text3.Text = goalsInRange[2].Title;
        //            occuranceDict.Add(text3, goalsInRange[2]);
        //            //occuranceDict.Add(third7, goalsInRange[2]);

        //        }
        //        else if (goalsInRange.Count == 4)
        //        {
        //            setProperties4();
        //            show7();
        //            text1.Text = goalsInRange[0].Title;
        //            occuranceDict.Add(text1, goalsInRange[0]);
        //            //occuranceDict.Add(first7, goalsInRange[0]);

        //            text2.Text = goalsInRange[1].Title;
        //            occuranceDict.Add(text2, goalsInRange[1]);
        //            //occuranceDict.Add(second7, goalsInRange[1]);

        //            text3.Text = goalsInRange[2].Title;
        //            occuranceDict.Add(text3, goalsInRange[2]);
        //            //occuranceDict.Add(third7, goalsInRange[2]);

        //            text4.Text = goalsInRange[3].Title;
        //            occuranceDict.Add(text4, goalsInRange[3]);
        //            //occuranceDict.Add(fourth7, goalsInRange[3]);

        //        }
        //        else if (goalsInRange.Count == 5)
        //        {
        //            setProperties5();
        //            show7();
        //            //first5.Text = goalsInRange[0].gr_title;
        //            text1.Text = goalsInRange[0].Title;
        //            occuranceDict.Add(text1, goalsInRange[0]);
        //            //occuranceDict.Add(first7, goalsInRange[0]);

        //            text2.Text = goalsInRange[1].Title;
        //            occuranceDict.Add(text2, goalsInRange[1]);
        //            //occuranceDict.Add(second7, goalsInRange[1]);

        //            text3.Text = goalsInRange[2].Title;
        //            occuranceDict.Add(text3, goalsInRange[2]);
        //            //occuranceDict.Add(third7, goalsInRange[2]);

        //            text4.Text = goalsInRange[3].Title;
        //            occuranceDict.Add(text4, goalsInRange[3]);
        //            //occuranceDict.Add(fourth7, goalsInRange[3]);

        //            text5.Text = goalsInRange[4].Title;
        //            occuranceDict.Add(text5, goalsInRange[4]);
        //            //occuranceDict.Add(fifth7, goalsInRange[4]);

        //        }
        //        else if (goalsInRange.Count == 6)
        //        {
        //            setProperties6();
        //            show7();
        //            text1.Text = goalsInRange[0].Title;
        //            occuranceDict.Add(text1, goalsInRange[0]);
        //            //occuranceDict.Add(first7, goalsInRange[0]);

        //            text2.Text = goalsInRange[1].Title;
        //            occuranceDict.Add(text2, goalsInRange[1]);
        //            //occuranceDict.Add(second7, goalsInRange[1]);

        //            text3.Text = goalsInRange[2].Title;
        //            occuranceDict.Add(text3, goalsInRange[2]);
        //            //occuranceDict.Add(third7, goalsInRange[2]);

        //            text4.Text = goalsInRange[3].Title;
        //            occuranceDict.Add(text4, goalsInRange[3]);
        //            //occuranceDict.Add(fourth7, goalsInRange[3]);

        //            text5.Text = goalsInRange[4].Title;
        //            occuranceDict.Add(text5, goalsInRange[4]);
        //            //occuranceDict.Add(fifth7, goalsInRange[4]);

        //            text6.Text = goalsInRange[5].Title;
        //            occuranceDict.Add(text6, goalsInRange[5]);
        //            //occuranceDict.Add(sixth7, goalsInRange[5]);

        //        }
        //        else
        //        {
        //            setProperties7();
        //            show7();
        //            text1.Text = goalsInRange[0].Title;
        //            occuranceDict.Add(text1, goalsInRange[0]);
        //            //occuranceDict.Add(first7, goalsInRange[0]);

        //            text2.Text = goalsInRange[1].Title;
        //            occuranceDict.Add(text2, goalsInRange[1]);
        //            //occuranceDict.Add(second7, goalsInRange[1]);

        //            text3.Text = goalsInRange[2].Title;
        //            occuranceDict.Add(text3, goalsInRange[2]);
        //            //occuranceDict.Add(third7, goalsInRange[2]);

        //            text4.Text = goalsInRange[3].Title;
        //            occuranceDict.Add(text4, goalsInRange[3]);
        //            //occuranceDict.Add(fourth7, goalsInRange[3]);

        //            text5.Text = goalsInRange[4].Title;
        //            occuranceDict.Add(text5, goalsInRange[4]);
        //            //occuranceDict.Add(fifth7, goalsInRange[4]);

        //            text6.Text = goalsInRange[5].Title;
        //            occuranceDict.Add(text6, goalsInRange[5]);
        //            //occuranceDict.Add(sixth7, goalsInRange[5]);

        //            text7.Text = goalsInRange[6].Title;
        //            occuranceDict.Add(text7, goalsInRange[6]);
        //            //occuranceDict.Add(seventh7, goalsInRange[6]);

        //        }


        //        return;
        //    }
        //    catch (Exception e)
        //    {
        //        DisplayAlert("Alert", "Error in TodaysListTest ToOccurances(). Error: " + e.ToString(), "OK");
        //    }
        //}


        void navigatetoActionsFrame(System.Object sender, System.EventArgs e)
        {
            //int navigationStackCount = Application.Current.MainPage.Navigation.NavigationStack.Count;
            //if (navigationStackCount > 2)
            //    Navigation.PopAsync();

            Frame receiving = (Frame)sender;
            int index = Int16.Parse(receiving.ClassId);

            if (receiving == first7)
            {
                option = options[index];
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
                option = options[index];
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
                option = options[index];
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
                option = options[index];
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
                option = options[index];
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
                option = options[index];
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
                option = options[index];
                first7.BackgroundColor = Color.Transparent;
                second7.BackgroundColor = Color.Transparent;
                third7.BackgroundColor = Color.Transparent;
                fourth7.BackgroundColor = Color.Transparent;
                fifth7.BackgroundColor = Color.Transparent;
                sixth7.BackgroundColor = Color.Transparent;
                seventh7.BackgroundColor = Color.FromHex("#FFBD27");
            }
            //else DisplayAlert("Error", "this goal doesn't have subtasks", "OK");

        }



    }
}
