using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Manifest.Models;

using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class EventsPage : ContentPage
    {
        public ObservableCollection<Attendee> datagrid = new ObservableCollection<Attendee>();

        public EventsPage(Event newEvent)
        {
            InitializeComponent();
            eventName.Text = newEvent.Title;
            eventDescription.Text = newEvent.Description;
            if (eventDescription.Text == "" || eventDescription.Text == null)
            {
                eventDescription.Text = "No description provided";
            }
            eventInfo.ItemsSource = datagrid;
            //datagrid.Add(newEvent);
            initialiseAttendees(newEvent.Attendees);

        }

        private void initialiseAttendees(List<Attendee> attendees)
        {
            foreach (Attendee attendee in attendees)
            {
                if (attendee.HavePic == false)
                {
                    attendee.PicUrl = "aboutme.png";
                }
                if (attendee.Name == "" || attendee.Name == null)
                {
                    attendee.Name = "Anonymous";
                }
                datagrid.Add(attendee);
            }

        }

        private void goToTodaysList(object sender, EventArgs args)
        {
            Application.Current.MainPage = new TodaysListPage();
        }
    }
}
