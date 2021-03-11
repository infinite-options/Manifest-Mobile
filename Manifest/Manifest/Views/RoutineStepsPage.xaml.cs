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

        ObservableCollection<Instruction> items;

        public RoutineStepsPage(SubOccurance subTask, Occurance routine)
        {
            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;
            //lastRowHeight = barStackLayoutRow.Height;

            mainGridLayout.BackgroundColor = Color.FromHex((string)Application.Current.Properties["background"]);
            frameColor.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);
            routinesStepsFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["routine"]);

            buttonFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["routine"]);
            routinesStepsFrame.BackgroundColor = Color.FromHex((string)Application.Current.Properties["routine"]);
            doneButton.BackgroundColor = Color.FromHex((string)Application.Current.Properties["header"]);

            //barStackLayoutProperties.BackgroundColor = Color.FromHex((string)Application.Current.Properties["navBar"]);

            title.Text = routine.Title;
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
                }
                step.stepIndex = stepsCount;
                step.color = (string)Application.Current.Properties["routine"];
                items.Add(step);
                stepsCount++;
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
                //(true, currInstruction);
                //string url = AppConstants.BaseUrl + AppConstants.updateInstruction;
                //currInstruction.updateIsComplete(true);
                //UpdateInstruction updateInstruction = new UpdateInstruction() {
                //    id = currInstruction.unique_id,
                //    is_complete = currInstruction.IsComplete,
                //    is_in_progress = currInstruction.IsInProgress
                //};
                //string toSend = updateInstruction.updateInstruction();
                //var content = new StringContent(toSend);
                //var res = await client.PostAsync(url, content);
                //if (res.IsSuccessStatusCode)
                //{
                //    Debug.WriteLine("Wrote to the datebase");
                //}
                //else
                //{
                //    Debug.WriteLine("Some error");
                //    Debug.WriteLine(toSend);
                //    Debug.WriteLine(res.ToString());
                //}
                if (numTasks == numComplete)
                {
                    parentIsComplete();
                }
                else if (parent.IsInProgress == false && parent.IsComplete == false)
                {
                    parentIsInProgress();
                }
            }
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
            if(processedSteps.Count != 0)
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

            await Navigation.PopAsync(false);
        }

        async void userDone(System.Object sender, System.EventArgs e)
        {
            // cases if it all steps are done, if none are done, if some are done
            if(parent.IsComplete != true)
            {
                foreach (Instruction steps in items)
                {
                    await RdsConnect.updateInstruction(true, steps);
                }
                parentIsComplete();
            }
           
            //parentIsComplete();

            //if (processedSteps.Count != 0)
            //{
            //    if (processedSteps.Count == parent.instructions.Count)
            //    {
            //        parentIsComplete();
            //    }
            //    else
            //    {
            //        parentIsInProgress();
            //    }
            //}

            //foreach (int i in processedSteps)
            //{
            //    await RdsConnect.updateInstruction(true, items[i]);
            //}


            //if (processedSteps.Count == parent.instructions.Count)
            //{
            //    foreach (Instruction i in parent.instructions)
            //    {
            //        await RdsConnect.updateInstruction(true, i);
            //    }
            //    parentIsComplete();
            //}
            //else
            //{
            //    foreach (Instruction i in parent.instructions)
            //    {
            //        await RdsConnect.updateInstruction(true, i);
            //    }
            //    parentIsInProgress();
            //}

            await Navigation.PopAsync(false);
        }

        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new SettingsPage(), false);
        }

        async void TapGestureRecognizer_Tapped_1(System.Object sender, System.EventArgs e)
        {
            var frame = (Frame)sender;
            if(frame.ClassId != "" && frame.ClassId != null)
            {
                var i = Int16.Parse(frame.ClassId);
                
                if (items[i].IsComplete == false)
                {
                    if(processedSteps.Count == 0)
                    {
                        if(i == 0)
                        {
                            RegistedStep(i, true, "greencheckmark.png");
                            processedSteps.Add(i);
                        }
                        else
                        {
                            await DisplayAlert("Oops","You must follow the steps in order","OK");
                        }
                    }
                    else
                    {
                        var previousStep = processedSteps[processedSteps.Count - 1];
                        
                        if(previousStep + 1 == i)
                        {
                            RegistedStep(i, true, "greencheckmark.png");
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
                        RegistedStep(i, false, parent.PicUrl);
                        processedSteps.RemoveAt(i);
                    }
                    else
                    {
                        await DisplayAlert("Oops", "You must undo the steps in order", "OK");
                    }
                }
            }
        }

        void RegistedStep(int i, bool status, string newImage)
        {
            items[i].Photo = newImage;
            items[i].IsComplete = status;
            items[i].updateImage();
            
            //updateInstruction(status, Instruction currInstruction);
        }
    }
}
