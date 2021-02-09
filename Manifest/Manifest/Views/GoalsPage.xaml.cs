using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manifest.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class GoalsPage : ContentPage
    {
        double deviceHeight = DeviceDisplay.MainDisplayInfo.Height;
        double deviceWidth = DeviceDisplay.MainDisplayInfo.Width;
        List<Occurance> currentOccurances;
        Dictionary<string, Occurance> occuranceDict;

        public GoalsPage(List<Occurance> occuranceList)
        {
            occuranceDict = new Dictionary<string, Occurance>();
            InitializeComponent();

            //int counter = 0;
            foreach (Occurance occurance in occuranceList)
            {
                occuranceDict.Add(occurance.Title, occurance);
                //string name = "button" + counter.ToString();
                //Button button1 = new Button { Text = occurance.Title, FontAttributes = FontAttributes.Bold, FontSize = 25, HorizontalOptions = LayoutOptions.Center };
                //button1.Clicked += navigatetoActions;
                //tempStack.Children.Add(button1);
                //Debug.WriteLine("titles: " + occurance.Title);
                //counter++;
            }
            //loadGoals(occurance.StartDayAndTime.ToString("t"), occurance.EndDayAndTime.ToString("t"));
            //first.CornerRadius = (int)(deviceHeight * 0.3);
            //second.CornerRadius = (int)(deviceHeight * 0.2);

            setProperties3();
            if (occuranceList.Count == 3)
            {
                first7.Text = occuranceList[0].Title;
                second7.Text = occuranceList[1].Title;
                third7.Text = occuranceList[2].Title;
            }
        }

        void setProperties3()
        {
            AbsoluteLayout.SetLayoutBounds(first7, new Rectangle(0.10, 0.07, deviceWidth * 0.23, deviceWidth * 0.23));
            first7.HeightRequest = deviceWidth * 0.23;
            first7.WidthRequest = deviceWidth * 0.23;
            first7.CornerRadius = (int)((deviceWidth * 0.23) / 2);
            first7.Text = "Goal 1";
            first7.FontSize = deviceWidth / 28;

            AbsoluteLayout.SetLayoutBounds(second7, new Rectangle(0.85, 0.42, deviceWidth * 0.30, deviceWidth * 0.30));
            second7.HeightRequest = deviceWidth * 0.30;
            second7.WidthRequest = deviceWidth * 0.30;
            second7.CornerRadius = (int)((deviceWidth * 0.30) / 2);
            second7.Text = "Goal 2";
            second7.FontSize = deviceWidth / 24;

            AbsoluteLayout.SetLayoutBounds(third7, new Rectangle(0.14, 0.77, deviceWidth * 0.22, deviceWidth * 0.22));
            third7.HeightRequest = deviceWidth * 0.22;
            third7.WidthRequest = deviceWidth * 0.22;
            third7.CornerRadius = (int)((deviceWidth * 0.22) / 2);
            third7.Text = "Goal 3";
            third7.FontSize = deviceWidth / 28;

            AbsoluteLayout.SetLayoutBounds(fourth7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(fifth7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(sixth7, new Rectangle(0, 0, 0, 0));
            AbsoluteLayout.SetLayoutBounds(seventh7, new Rectangle(0, 0, 0, 0));
            fourth7.HeightRequest = 0;
            fifth7.HeightRequest = 0;
            sixth7.HeightRequest = 0;
            seventh7.HeightRequest = 0;

            next7.HeightRequest = deviceHeight * 0.025;
            next7.WidthRequest = deviceWidth * 0.14;
            next7.CornerRadius = (int)((deviceHeight * 0.025) / 2);
        }

        void setProperties4()
        {
            first4.HeightRequest = deviceWidth * 0.23;
            first4.WidthRequest = deviceWidth * 0.23;
            first4.CornerRadius = (int)((deviceWidth * 0.23) / 2);
            first4.Text = "Goal 1";
            first4.FontSize = deviceWidth / 25;
            second4.HeightRequest = deviceWidth * 0.30;
            second4.WidthRequest = deviceWidth * 0.30;
            second4.CornerRadius = (int)((deviceWidth * 0.30) / 2);
            second4.Text = "Goal 2";
            second4.FontSize = deviceWidth / 22;
            third4.HeightRequest = deviceWidth * 0.22;
            third4.WidthRequest = deviceWidth * 0.22;
            third4.CornerRadius = (int)((deviceWidth * 0.22) / 2);
            third4.Text = "Goal 3";
            third4.FontSize = deviceWidth / 26;
            fourth4.HeightRequest = deviceWidth * 0.16;
            fourth4.WidthRequest = deviceWidth * 0.16;
            fourth4.CornerRadius = (int)((deviceWidth * 0.16) / 2);
            fourth4.Text = "Goal 4";
            fourth4.FontSize = deviceWidth / 33;

            next4.HeightRequest = deviceHeight * 0.025;
            next4.WidthRequest = deviceWidth * 0.14;
            next4.CornerRadius = (int)((deviceHeight * 0.025) / 2);
        }

        void setProperties5()
        {
            first5.HeightRequest = deviceWidth * 0.21;
            first5.WidthRequest = deviceWidth * 0.21;
            first5.CornerRadius = (int)((deviceWidth * 0.21) / 2);
            first5.Text = "Goal 1";
            first5.FontSize = deviceWidth / 26;
            second5.HeightRequest = deviceWidth * 0.26;
            second5.WidthRequest = deviceWidth * 0.26;
            second5.CornerRadius = (int)((deviceWidth * 0.26) / 2);
            second5.Text = "Goal 2";
            second5.FontSize = deviceWidth / 24;
            third5.HeightRequest = deviceWidth * 0.21;
            third5.WidthRequest = deviceWidth * 0.21;
            third5.CornerRadius = (int)((deviceWidth * 0.21) / 2);
            third5.Text = "Goal 3";
            third5.FontSize = deviceWidth / 27;
            fourth5.HeightRequest = deviceWidth * 0.16;
            fourth5.WidthRequest = deviceWidth * 0.16;
            fourth5.CornerRadius = (int)((deviceWidth * 0.16) / 2);
            fourth5.Text = "Goal 4";
            fourth5.FontSize = deviceWidth / 33;
            fifth5.HeightRequest = deviceWidth * 0.17;
            fifth5.WidthRequest = deviceWidth * 0.17;
            fifth5.CornerRadius = (int)((deviceWidth * 0.17) / 2);
            fifth5.Text = "Goal 5";
            fifth5.FontSize = deviceWidth / 32;

            next5.HeightRequest = deviceHeight * 0.025;
            next5.WidthRequest = deviceWidth * 0.14;
            next5.CornerRadius = (int)((deviceHeight * 0.025) / 2);
        }

        void setProperties6()
        {
            first6.HeightRequest = deviceWidth * 0.20;
            first6.WidthRequest = deviceWidth * 0.20;
            first6.CornerRadius = (int)((deviceWidth * 0.20) / 2);
            first6.Text = "Goal 1";
            first6.FontSize = deviceWidth / 26;
            second6.HeightRequest = deviceWidth * 0.26;
            second6.WidthRequest = deviceWidth * 0.26;
            second6.CornerRadius = (int)((deviceWidth * 0.26) / 2);
            second6.Text = "Goal 2";
            second6.FontSize = deviceWidth / 25;
            third6.HeightRequest = deviceWidth * 0.22;
            third6.WidthRequest = deviceWidth * 0.22;
            third6.CornerRadius = (int)((deviceWidth * 0.22) / 2);
            third6.Text = "Goal 3";
            third6.FontSize = deviceWidth / 26;
            fourth6.HeightRequest = deviceWidth * 0.17;
            fourth6.WidthRequest = deviceWidth * 0.17;
            fourth6.CornerRadius = (int)((deviceWidth * 0.17) / 2);
            fourth6.Text = "Goal 4";
            fourth6.FontSize = deviceWidth / 31;
            fifth6.HeightRequest = deviceWidth * 0.16;
            fifth6.WidthRequest = deviceWidth * 0.16;
            fifth6.CornerRadius = (int)((deviceWidth * 0.16) / 2);
            fifth6.Text = "Goal 5";
            fifth6.FontSize = deviceWidth / 34;
            sixth6.HeightRequest = deviceWidth * 0.17;
            sixth6.WidthRequest = deviceWidth * 0.17;
            sixth6.CornerRadius = (int)((deviceWidth * 0.17) / 2);
            sixth6.Text = "Goal 6";
            sixth6.FontSize = deviceWidth / 31;

            next6.HeightRequest = deviceHeight * 0.025;
            next6.WidthRequest = deviceWidth * 0.14;
            next6.CornerRadius = (int)((deviceHeight * 0.025) / 2);
        }

        void setProperties7()
        {
            AbsoluteLayout.SetLayoutBounds(first7, new Rectangle(0.10, 0.05, deviceWidth * 0.20, deviceWidth * 0.20));
            first7.HeightRequest = deviceWidth * 0.20;
            first7.WidthRequest = deviceWidth * 0.20;
            first7.CornerRadius = (int)((deviceWidth * 0.20) / 2);
            first7.Text = "Goal 1";
            first7.FontSize = deviceWidth / 27;

            AbsoluteLayout.SetLayoutBounds(second7, new Rectangle(0.90, 0.23, deviceWidth * 0.26, deviceWidth * 0.26));
            second7.HeightRequest = deviceWidth * 0.26;
            second7.WidthRequest = deviceWidth * 0.26;
            second7.CornerRadius = (int)((deviceWidth * 0.26) / 2);
            second7.Text = "Goal 2";
            second7.FontSize = deviceWidth / 25;

            AbsoluteLayout.SetLayoutBounds(third7, new Rectangle(0.09, 0.39, deviceWidth * 0.17, deviceWidth * 0.17));
            third7.HeightRequest = deviceWidth * 0.17;
            third7.WidthRequest = deviceWidth * 0.17;
            third7.CornerRadius = (int)((deviceWidth * 0.17) / 2);
            third7.Text = "Goal 3";
            third7.FontSize = deviceWidth / 31;

            AbsoluteLayout.SetLayoutBounds(fourth7, new Rectangle(0.78, 0.60, deviceWidth * 0.17, deviceWidth * 0.17));
            fourth7.HeightRequest = deviceWidth * 0.17;
            fourth7.WidthRequest = deviceWidth * 0.17;
            fourth7.CornerRadius = (int)((deviceWidth * 0.17) / 2);
            fourth7.Text = "Goal 4";
            fourth7.FontSize = deviceWidth / 30;

            AbsoluteLayout.SetLayoutBounds(fifth7, new Rectangle(0.10, 0.61, deviceWidth * 0.13, deviceWidth * 0.13));
            fifth7.HeightRequest = deviceWidth * 0.13;
            fifth7.WidthRequest = deviceWidth * 0.13;
            fifth7.CornerRadius = (int)((deviceWidth * 0.13) / 2);
            fifth7.Text = "Goal 5";
            fifth7.FontSize = deviceWidth / 39;

            AbsoluteLayout.SetLayoutBounds(sixth7, new Rectangle(0.93, 0.86, deviceWidth * 0.17, deviceWidth * 0.17));
            sixth7.HeightRequest = deviceWidth * 0.17;
            sixth7.WidthRequest = deviceWidth * 0.17;
            sixth7.CornerRadius = (int)((deviceWidth * 0.17) / 2);
            sixth7.Text = "Goal 6";
            sixth7.FontSize = deviceWidth / 31;

            AbsoluteLayout.SetLayoutBounds(seventh7, new Rectangle(0.31, 0.86, deviceWidth * 0.19, deviceWidth * 0.19));
            seventh7.HeightRequest = deviceWidth * 0.19;
            seventh7.WidthRequest = deviceWidth * 0.19;
            seventh7.CornerRadius = (int)((deviceWidth * 0.19) / 2);
            seventh7.Text = "Goal 7";
            seventh7.FontSize = deviceWidth / 30;

            next7.HeightRequest = deviceHeight * 0.025;
            next7.WidthRequest = deviceWidth * 0.14;
            next7.CornerRadius = (int)((deviceHeight * 0.025) / 2);
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
            show6();
        }

        void clickedShow5(System.Object sender, System.EventArgs e)
        {
            //show5();
            setProperties5();
            show5();
        }

        void clickedShow4(System.Object sender, System.EventArgs e)
        {
            //show4();
            setProperties4();
            show4();
        }

        void clickedShow3(System.Object sender, System.EventArgs e)
        {
            //show3();
            setProperties3();
            show3();
        }

        //temporary for testing
        void navigatetoTodaysList(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine(Application.Current.Properties["userID"]);
            Application.Current.MainPage = new TodaysListPage((String)Application.Current.Properties["userID"]);
        }

        void navigatetoActions(System.Object sender, System.EventArgs e)
        {
            Button receiving = (Button)sender;

            if (occuranceDict[receiving.Text].IsSublistAvailable == true)
                Application.Current.MainPage = new Actions(occuranceDict[receiving.Text]);
            else DisplayAlert("Error", "this goal doesn't have subtasks", "OK");
        }
    }
}
