using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class TodaysListTest : ContentPage
    {
        public TodaysListTest()
        {
            InitializeComponent();
        }

        void Button_Clicked_2(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine("Button 2 pressed");
        }

        void Button_Clicked_1(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine("Button 1 pressed");
        }
    }
}
