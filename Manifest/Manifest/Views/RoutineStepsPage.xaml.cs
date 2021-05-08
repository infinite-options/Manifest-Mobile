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
using Manifest.RDS;

namespace Manifest.Views
{
    public partial class RoutineStepsPage : ContentPage
    {
        bool setting;
        GridLength height;
        GridLength lastRowHeight;

        public bool registeredChange = false;

        double deviceHeight = DeviceDisplay.MainDisplayInfo.Height;
        double deviceWidth = DeviceDisplay.MainDisplayInfo.Width;

        float circleHW;
        float circleRadius;
        float rowHeight;

        HttpClient client = new HttpClient();

        List<Instruction> instruction_steps;
        List<int> processedSteps;
        SubOccurance parent;
        Occurance currRoutine;

        int numComplete;
        int numTasks;

        public ObservableCollection<Instruction> items;

        public RoutineStepsPage(SubOccurance subTask, Occurance routine)
        {
            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;
            //lastRowHeight = barStackLayoutRow.Height;

            //mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            routinesStepsFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["routine"]);
            routineTitle.Text = subTask.Title;
            routineTime.Text = "This takes: " + routine.ExpectedCompletionTime.ToString(@"%h") + "hrs " + routine.ExpectedCompletionTime.ToString(@"mm") + "min";
            //buttonFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["routine"]);
            routinesStepsFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["routine"]);
            //doneButton.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            //barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);

            title.Text = routineTitle.Text;
            if (Device.RuntimePlatform == Device.iOS)
            {
                titleGrid.Margin = new Thickness(0, 10, 0, 0);
            }
            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();

            circleHW = (float)(deviceWidth * 0.13);
            circleRadius = (float)(circleHW * 0.5);
            //routineFrame.HeightRequest = circleHW;
            //routineFrame.WidthRequest = circleHW;
            //routineFrame.CornerRadius = circleRadius;
            //arrow.HeightRequest = circleHW;

            //var helperObject = new MainPage();
            //locationTitle.Text = (string)Application.Current.Properties["location"];
            //dateTitle.Text = helperObject.GetCurrentTime();

            rowHeight = (float)Math.Min(80, 0.02 * deviceHeight);


            //stepsScroll.HeightRequest = (deviceHeight / 2) - 500;

            parent = subTask;
            currRoutine = routine;
            subTitle.Text =  subTask.Title;
            instruction_steps = subTask.instructions;
            items = new ObservableCollection<Instruction>();
            processedSteps = new List<int>();
            NavigationPage.SetHasNavigationBar(this, false);
            CreateList();
        }

        private void CreateList()
        {
            var stepsCount = 0;
            items.Clear();

            foreach (Instruction step in instruction_steps)
            {
                if(step.IsComplete == true)
                {
                    step.Photo = "greencheckmark.png";
                    processedSteps.Add(stepsCount);
                }
                else
                {
                    step.Photo = parent.PicUrl;
                    step.Photo = "whiteCheckMarkIcon";
                }
                step.stepIndex = stepsCount;
                step.color = (string)Application.Current.Properties["routine"];
                items.Add(step);
                stepsCount++;
            }

            subTaskList.ItemsSource = items;
            
        }

        async void AllDone(System.Object sender, System.EventArgs e)
        {
            // user gets a complete check mark at the parent level in all cases
            string url = AppConstants.BaseUrl + AppConstants.updateActionAndTask;

            parent.IsComplete = true;
            parent.IsInProgress = false;
            string res = await RdsConnect.updateOccurance(parent, false, true, url);

            Application.Current.MainPage = new RoutinePage();
        }

        async void NotDone(System.Object sender, System.EventArgs e)
        {
            var totalNumberOfCompleteInstructions = 0;
            foreach (Instruction instruction in items)
            {
                if (instruction.IsComplete == true)
                {
                    totalNumberOfCompleteInstructions++;
                }
            }

            if(totalNumberOfCompleteInstructions == 0)
            {
                string url = AppConstants.BaseUrl + AppConstants.updateActionAndTask;
              
                parent.IsComplete = false;
                parent.IsInProgress = false;
                string res = await RdsConnect.updateOccurance(parent, false, false, url);

            }
            else
            {
                string url = AppConstants.BaseUrl + AppConstants.updateActionAndTask;

                parent.IsComplete = false;
                parent.IsInProgress = true;
                string res = await RdsConnect.updateOccurance(parent, true, false, url);
            }

            Application.Current.MainPage = new RoutinePage();
        }

        async void CompleteInstructionsStep(System.Object sender, System.EventArgs e)
        {
            var image = (ImageButton)sender;
            var item = (Instruction)image.CommandParameter;

            registeredChange = true;

            var i = item.stepIndex;

            if (items[i].IsComplete == false)
            {
                if (processedSteps.Count == 0)
                {
                    if (i == 0)
                    {
                        RegistedStep(i, true, "greenCheckMarkIcon",item);
                        processedSteps.Add(i);
                    }
                    else
                    {
                        await DisplayAlert("Oops", "You must follow the steps in order", "OK");
                    }
                }
                else
                {
                    var previousStep = processedSteps[processedSteps.Count - 1];

                    if (previousStep + 1 == i)
                    {
                        RegistedStep(i, true, "greenCheckMarkIcon", item);
                        processedSteps.Add(i);
                    }
                    else
                    {
                        await DisplayAlert("Oops", "You must follow the steps in order", "OK");
                    }
                }
            }
            else
            {
                var previousStep = processedSteps[processedSteps.Count - 1];
                if (previousStep == i)
                {
                    RegistedStep(i, false, "whiteCheckMarkIcon", item);
                processedSteps.RemoveAt(i);
                }
                else
                {
                    await DisplayAlert("Oops", "You must undo the steps in order", "OK");
                }
            }
        }

        async void NavigateToPreviousPage(System.Object sender, System.EventArgs e)
        {

            if (registeredChange)
            {
                var totalNumberOfCompleteInstructions = 0;
                foreach (Instruction instruction in items)
                {
                    if (instruction.IsComplete == true)
                    {
                        totalNumberOfCompleteInstructions++;
                    }
                }

                if (totalNumberOfCompleteInstructions == 0)
                {
                    string url = AppConstants.BaseUrl + AppConstants.updateActionAndTask;

                    parent.IsComplete = false;
                    parent.IsInProgress = false;
                    string res = await RdsConnect.updateOccurance(parent, false, false, url);

                }
                else if (totalNumberOfCompleteInstructions != 0 && totalNumberOfCompleteInstructions == items.Count)
                {
                    string url = AppConstants.BaseUrl + AppConstants.updateActionAndTask;

                    parent.IsComplete = true;
                    parent.IsInProgress = false;
                    string res = await RdsConnect.updateOccurance(parent, false, true, url);
                }
                else
                {
                    string url = AppConstants.BaseUrl + AppConstants.updateActionAndTask;

                    parent.IsComplete = false;
                    parent.IsInProgress = true;
                    string res = await RdsConnect.updateOccurance(parent, true, false, url);
                }
            }
            Application.Current.MainPage = new RoutinePage();
        }


        async void RegistedStep(int i, bool status, string newImage, Instruction step)
        {
            items[i].PhotoUpdate = newImage;
            await RdsConnect.updateInstruction(status, step);
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new WhoAmIPage(), false);
        }

        void TapGestureRecognizer_Tapped_1(System.Object sender, System.EventArgs e)
        {

        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {

            foreach (int i in processedSteps)
            {
                await RdsConnect.updateInstruction(true, items[i]);
            }

            //foreach (Instruction i in parent.instructions)
            //{
            //    await RdsConnect.updateInstruction(true, i);
            //}
            if (processedSteps.Count != 0)
            {
                if (processedSteps.Count == parent.instructions.Count)
                {
                    parentIsComplete();
                }
                else
                {
                    if (parent.IsComplete != true)
                    {
                        parentIsComplete();
                    }
                }
            }

            await Navigation.PopModalAsync(false);
        }

        async void stepComplete(System.Object sender, System.EventArgs e)
        {
            Frame myvar = (Frame)sender;
            Instruction currInstruction = myvar.BindingContext as Instruction;

        }

        async void parentIsComplete()
        {
            string url = AppConstants.BaseUrl + AppConstants.updateActionAndTask;
            string res = await RdsConnect.updateOccurance(parent, false, true, url);
            if (res == "Failure")
            {
                await DisplayAlert("Error", "There was an error writing to the database.", "OK");
            }
            currRoutine.SubOccurancesCompleted++;
            updateRoutine();
        }

        async void parentIsInProgress()
        {
            string url = AppConstants.BaseUrl + AppConstants.updateActionAndTask;
            string res = await RdsConnect.updateOccurance(parent, true, false, url);
            if (res == "Failure")
            {
                await DisplayAlert("Error", "There was an error writing to the database.", "OK");
            }
            //currRoutine.SubOccurancesCompleted++;
            updateRoutine();
        }

        async void updateRoutine()
        {
            string url = AppConstants.BaseUrl + AppConstants.updateGoalAndRoutine;
            if (currRoutine.IsInProgress == false && currRoutine.IsComplete == false)
            {
                string res = await RdsConnect.updateOccurance(currRoutine, true, false, url);
                if (res == "Failure")
                {
                    await DisplayAlert("Error", "There was an error writing to the database.", "OK");
                }
            }
            else if (currRoutine.IsInProgress == true && currRoutine.IsComplete == false && currRoutine.NumSubOccurances == currRoutine.SubOccurancesCompleted)
            {
                string res = await RdsConnect.updateOccurance(currRoutine, false, true, url);
                if (res == "Failure")
                {
                    await DisplayAlert("Error", "There was an error writing to the database.", "OK");
                }
            }
        }
    }
}
