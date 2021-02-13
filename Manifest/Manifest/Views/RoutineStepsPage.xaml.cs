using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Manifest.Models;

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

        List<Instruction> instruction_steps;
        public RoutineStepsPage(SubOccurance subTask)
        {
            InitializeComponent();
            setting = false;
            height = mainStackLayoutRow.Height;
            //lastRowHeight = barStackLayoutRow.Height;

            frameColor.BackgroundColor = Color.FromHex("#9DB2CB");
            title.Text = "Routine Steps";
            subTitle.Text = "";
            var helperObject = new MainPage();
            locationTitle.Text = (string)Application.Current.Properties["location"];
            dateTitle.Text = helperObject.GetCurrentTime();

            circleHW = (float)(deviceWidth * 0.13);
            circleRadius = (float)(circleHW * 0.5);
            routineFrame.HeightRequest = circleHW;
            routineFrame.WidthRequest = circleHW;
            routineFrame.CornerRadius = circleRadius;
            arrow.HeightRequest = circleHW;

            rowHeight = (float)Math.Min(80, 0.02 * deviceHeight);

            routineName.Text = subTask.Title;
            instruction_steps = subTask.instructions;
            NavigationPage.SetHasNavigationBar(this, false);
            CreateList();

        }

        private void CreateList()
        {
            TapGestureRecognizer doneRecognizer = new TapGestureRecognizer();
            doneRecognizer.NumberOfTapsRequired = 1;
            doneRecognizer.Tapped += stepComplete;

            foreach (Instruction step in instruction_steps)
            {
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
                    },0,0);
                newGrid.Children.Add(routineComplete,0,0);

                float corner_radius = (float)(rowHeight * 0.3);

                Frame gridFrame = new Frame
                {
                    BackgroundColor = Color.White,
                    CornerRadius = corner_radius,
                    Content = newGrid,
                    Padding = 10,
                    GestureRecognizers = { doneRecognizer }
                };

                instructions.Children.Add(gridFrame);
            }
        }

        void stepComplete(System.Object sender, System.EventArgs e)
        {
            Image myvar = (Image)sender;
            Instruction currOccurance = myvar.BindingContext as Instruction;
            //Write to database here
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Navigation.PopAsync(false);
        }
    }
}
