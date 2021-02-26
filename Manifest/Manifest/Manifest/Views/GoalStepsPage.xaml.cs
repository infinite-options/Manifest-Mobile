﻿using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Manifest.Models;
using Manifest.Config;
using System.Net.Http;
using System.Diagnostics;
using Manifest.RDS;
using System.Collections.ObjectModel;

namespace Manifest.Views
{
    public class InstructionItem
    {
        public string title { get; set; }
        public string color { get; set; }
        public int time { get; set; }
    }

    public partial class GoalStepsPage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        string passedTitle;
        string passedPhoto;
        string passedColor;

        ObservableCollection<InstructionItem> items;

        double deviceHeight = DeviceDisplay.MainDisplayInfo.Height;
        double deviceWidth = DeviceDisplay.MainDisplayInfo.Width;

        float circleHW;
        float circleRadius;
        float rowHeight;

        HttpClient client = new HttpClient();

        List<Instruction> instruction_steps;

        SubOccurance parent;

        int numComplete;
        int numTasks;
        Occurance passedOccurance;

        public GoalStepsPage(Occurance occurance, SubOccurance subTask, string color)
        {
            passedTitle = occurance.Title;
            passedPhoto = subTask.PicUrl;
            passedColor = color;
            passedOccurance = occurance;

            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;
            items = new ObservableCollection<InstructionItem>();
            //lastRowHeight = barStackLayoutRow.Height;

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            //barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);
            
            //title.Text = subOccur.Title;
            title.Text = "Goals";
            subTitle.Text = occurance.Title;
            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();

            circleHW = (float)(deviceWidth * 0.13);
            circleRadius = (float)(circleHW * 0.5);
            //routineFrame.HeightRequest = circleHW;
            //routineFrame.WidthRequest = circleHW;
            //routineFrame.CornerRadius = circleRadius;
            arrow.HeightRequest = circleHW;

            rowHeight = (float)Math.Min(80, 0.02 * deviceHeight);

            parent = subTask;
            routineName.Text = subTask.Title;
            instruction_steps = subTask.instructions;
            NavigationPage.SetHasNavigationBar(this, false);
            CreateList();

        }

        private async void CreateList()
        {
            TapGestureRecognizer doneRecognizer = new TapGestureRecognizer();
            doneRecognizer.NumberOfTapsRequired = 1;
            doneRecognizer.Tapped += stepComplete;
            items.Clear();
            foreach (Instruction step in instruction_steps)
            {
                numTasks++;
                if (step.IsComplete == true) numComplete++;
                Image routineComplete = new Image();
                Binding completeVisible = new Binding("IsComplete");
                completeVisible.Source = step;
                routineComplete.BindingContext = step;
                routineComplete.Source = "greencheckmark.png";
                routineComplete.SetBinding(Image.IsVisibleProperty, completeVisible);
                routineComplete.HorizontalOptions = LayoutOptions.End;
                //We want to get every subtask for that routine
                Grid newGrid = new Grid
                {
                    RowDefinitions = {
                        new RowDefinition { Height = new GridLength(rowHeight, GridUnitType.Absolute)}
                    },
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    RowSpacing = 10
                };
                newGrid.Children.Add(
                    new Label
                    {
                        Text = step.title,
                        TextColor = Color.Black,
                        FontSize = 30,
                        LineBreakMode = LineBreakMode.WordWrap
                    }, 0, 0);
                newGrid.Children.Add(routineComplete, 0, 0);

                float corner_radius = (float)(rowHeight * 0.3);

                Frame gridFrame = new Frame
                {
                    BackgroundColor = Color.White,
                    CornerRadius = corner_radius,
                    Content = newGrid,
                    Padding = 10,
                    GestureRecognizers = { doneRecognizer }
                };

                gridFrame.BindingContext = step;


                instructions.Children.Add(gridFrame);

                items.Add(new InstructionItem() { time = numTasks, title = step.title, color = "#F26D4B" });

            }
            if (numTasks == numComplete)
            {
                parentIsComplete();
            }
            InstructionsList.ItemsSource = items;
        }

        async void stepComplete(System.Object sender, System.EventArgs e)
        {
            Frame myvar = (Frame)sender;
            Instruction currInstruction = myvar.BindingContext as Instruction;
            //Write to database here
            if (currInstruction.IsComplete == false)
            {
                numComplete++;

                // use global function
                string url = AppConstants.BaseUrl + AppConstants.updateInstruction;
                currInstruction.updateIsComplete(true);
                UpdateInstruction updateInstruction = new UpdateInstruction()
                {
                    id = currInstruction.unique_id,
                    is_complete = currInstruction.IsComplete,
                    is_in_progress = currInstruction.IsInProgress
                };
                string toSend = updateInstruction.updateInstruction();
                var content = new StringContent(toSend);
                var res = await client.PostAsync(url, content);
                if (res.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Wrote to the datebase");
                }
                else
                {
                    Debug.WriteLine("Some error");
                    Debug.WriteLine(toSend);
                    Debug.WriteLine(res.ToString());
                }
                string urlSub = AppConstants.BaseUrl + AppConstants.updateActionAndTask;
                string urlOccur = AppConstants.BaseUrl + AppConstants.updateGoalAndRoutine;
                if (numTasks == numComplete)
                {
                    parentIsComplete();
                }
                else
                {
                    await RdsConnect.updateOccurance(parent, true, false, urlSub);
                    await RdsConnect.updateOccurance(passedOccurance, true, false, urlOccur);
                }
                    
            }
        }

        async void parentIsComplete()
        {
            string url = AppConstants.BaseUrl + AppConstants.updateActionAndTask;
            await RdsConnect.updateOccurance(parent, false, true, url);
            url = AppConstants.BaseUrl + AppConstants.updateGoalAndRoutine;
            await RdsConnect.updateOccurance(passedOccurance, false, true, url);
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Navigation.PopAsync(false);
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SettingsPage(), false);
        }
        private DateTime ToDateTime(string dateString)
        {
            try
            {
                return DateTime.Parse(dateString);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in ToDateTime function in TodaysList class");
            }
            return new DateTime();
        }

        void doneClicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Completed(passedTitle, passedPhoto, passedColor));
        }
    }
}