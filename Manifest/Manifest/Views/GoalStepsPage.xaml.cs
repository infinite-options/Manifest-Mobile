using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manifest.Models;
using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class GoalStepsPage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        List<OrigSteps> instructions;
        string passedTitle;
        string passedPhoto;
        string passedColor;


        public GoalStepsPage(string goalTitle, SubOccuranceDto subOccur, string subtaskColor)
        {
            passedTitle = goalTitle;
            passedPhoto = subOccur.photo;
            passedColor = subtaskColor;
            Debug.WriteLine("photo url: " + passedPhoto);
            Debug.WriteLine("subtask color: " + subtaskColor);
            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;
            lastRowHeight = barStackLayoutRow.Height;
            instructions = subOccur.instructions_steps;

            frameColor.BackgroundColor = Color.FromHex("#9DB2CB");
            //title.Text = subOccur.Title;
            title.Text = "Goals";
            subTitle.Text = goalTitle;
            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();

            foreach (OrigSteps step in instructions)
            {
                Debug.WriteLine("a step: " + step.title);
            }

            NavigationPage.SetHasNavigationBar(this, false);
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void completedClicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Completed(passedTitle, passedPhoto, passedColor));
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Navigation.PopAsync(false);
        }
    }
}
