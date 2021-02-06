using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manifest.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class Goals : ContentPage
    {
        double deviceHeight = DeviceDisplay.MainDisplayInfo.Height;
        double deviceWidth = DeviceDisplay.MainDisplayInfo.Width;
        List<Occurance> currentOccurances;
        Dictionary<string, Occurance> occuranceDict;

        public Goals(List<Occurance> occuranceList)
        {
            occuranceDict = new Dictionary<string, Occurance>();
            InitializeComponent();

            int counter = 0;
            foreach (Occurance occurance in occuranceList)
            {
                occuranceDict.Add(occurance.Title, occurance);
                string name = "button" + counter.ToString();
                Button button1 = new Button { Text = occurance.Title, FontAttributes = FontAttributes.Bold, FontSize = 25, HorizontalOptions = LayoutOptions.Center };
                button1.Clicked += navigatetoActions;
                tempStack.Children.Add(button1);
                Debug.WriteLine("titles: " + occurance.Title);
                counter++;
            }
            //loadGoals(occurance.StartDayAndTime.ToString("t"), occurance.EndDayAndTime.ToString("t"));
            //first.CornerRadius = (int)(deviceHeight * 0.3);
            //second.CornerRadius = (int)(deviceHeight * 0.2);
            first.HeightRequest = deviceWidth * 0.1;
            first.WidthRequest = deviceWidth * 0.1;
            first.CornerRadius = (int)((deviceWidth * 0.1) / 2);
            second.HeightRequest = deviceWidth * 0.25;
            second.WidthRequest = deviceWidth * 0.25;
            second.CornerRadius = (int)((deviceWidth * 0.25) / 2);
        }



        //temporary for testing
        void navigatetoTodaysList(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine(Application.Current.Properties["userID"]);
            Application.Current.MainPage = new TodaysListTest((String)Application.Current.Properties["userID"]);
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
