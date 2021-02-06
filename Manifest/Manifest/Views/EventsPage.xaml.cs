using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Manifest.Models;

using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class EventsPage : ContentPage
    {
        public ObservableCollection<Event> datagrid = new ObservableCollection<Event>();

        public EventsPage(Event newEvent)
        {
            InitializeComponent();
            eventName.Text = newEvent.Title;
            eventInfo.ItemsSource = datagrid;
            datagrid.Add(newEvent);

        }

        private void goToTodaysList(object sender, EventArgs args)
        {
            Application.Current.MainPage = new TodaysListTest((String)Application.Current.Properties["userID"]);
        }
    }
}
