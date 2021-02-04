using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class Goals : ContentPage
    {
        public Goals()
        {
            InitializeComponent();
        }

        //temporary for testing
        void navigatetoTodaysList(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine(Application.Current.Properties["userID"]);
            Application.Current.MainPage = new TodaysListTest((String)Application.Current.Properties["userID"]);
        }
    }
}
