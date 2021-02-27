using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Manifest.Models;
using Manifest.Config;
using System.Net.Http;
using System.Diagnostics;
using Manifest.RDS;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Manifest.Views
{
    public class InstructionItem: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public int stepIndex { get; set; }
        public string image { get; set; }
        public string title { get; set; }
        public string color { get; set; }
        public int time { get; set; }
        public bool isCompleted { get; set; }
        public double opacity { get; set; }
        public void updateImage()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("image"));
        }
        public void updateOpacity()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("opacity"));
        }
        //public I
    }

    public partial class GoalStepsPage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;
        string passedTitle;
        string passedPhoto;
        string passedColor;

        ObservableCollection<Instruction> items;
        List<int> processedInstructions;
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
            items = new ObservableCollection<Instruction>();
            processedInstructions = new List<int>();
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
            var instructionStep = 0;
            items.Clear();

            foreach (Instruction step in instruction_steps)
            {
                if(step.IsComplete == true)
                {
                    step.opacity = 0.5;
                    processedInstructions.Add(instructionStep);
                }
                else
                {
                    step.opacity = 1;
                }
                step.time = instructionStep + 1;
                step.color = "#F26D4B";
                step.stepIndex = instructionStep;
                items.Add(step);
                instructionStep++;
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

        async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            foreach (int i in processedInstructions)
            {
                await RdsConnect.updateInstruction(true, items[i]);
            }

            if (processedInstructions.Count == parent.instructions.Count)
            {
                parentIsComplete();
                await Navigation.PopAsync(false);
            }
            else
            {
                await Navigation.PopAsync(false);
                // Navigation.PushAsync(new Completed(passedTitle, passedPhoto, passedColor));
            }
            //Navigation.PushAsync(new Completed(passedTitle, passedPhoto, passedColor));
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

        async void doneClicked(System.Object sender, System.EventArgs e)
        {
            foreach (int i in processedInstructions)
            {
                await RdsConnect.updateInstruction(true, items[i]);
            }

            if (processedInstructions.Count == parent.instructions.Count)
            {
                parentIsComplete();
                await Navigation.PopAsync(false);
            }
            else
            {
                await Navigation.PopAsync(false);
               // Navigation.PushAsync(new Completed(passedTitle, passedPhoto, passedColor));
            }
            //Navigation.PushAsync(new Completed(passedTitle, passedPhoto, passedColor));
        }

        void RegistedStep(int i, bool status, double newOpacity)
        {
            items[i].opacity = newOpacity;
            items[i].IsComplete = status;
            items[i].updateOpacity();
        }

        async void TapGestureRecognizer_Tapped_1(System.Object sender, System.EventArgs e)
        {
            var frame = (Frame)sender;
            if (frame.ClassId != "" && frame.ClassId != null)
            {
                var i = Int16.Parse(frame.ClassId);

                if (items[i].IsComplete == false)
                {
                    if (processedInstructions.Count == 0)
                    {
                        if (i == 0)
                        {
                            RegistedStep(i, true, 0.5);
                            processedInstructions.Add(i);
                        }
                        else
                        {
                            await DisplayAlert("Oops", "You must follow the steps in order", "OK");
                        }
                    }
                    else
                    {
                        var previousStep = processedInstructions[processedInstructions.Count - 1];

                        if (previousStep + 1 == i)
                        {
                            RegistedStep(i, true, 0.5);
                            processedInstructions.Add(i);
                        }
                        else
                        {
                            await DisplayAlert("Oops", "You must follow the steps in order", "OK");
                        }
                    }
                }
                else
                {
                    var previousStep = processedInstructions[processedInstructions.Count - 1];
                    if (previousStep == i)
                    {
                        RegistedStep(i, false, 1);
                        processedInstructions.RemoveAt(i);
                    }
                    else
                    {
                        await DisplayAlert("Oops", "You must undo the steps in order", "OK");
                    }
                }
            }
        }
    }
}