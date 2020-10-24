using System;
using System.Collections.Generic;
using System.IO;
using Manifest.Models;
using Manifest.Services;
using Manifest.ViewModels;
using Xamarin.Forms;

namespace Manifest.Views
{

    public partial class EventsPage : ContentPage
    {
        Event Event;
        public EventsPage(string id)
        {
            InitializeComponent();
            var task = Repository.Instance.GetEventById(id);
            Event = task.Result;
            LoadUI();
        }

        private async void LoadUI()
        {
            Title.Text = "Title: " + Event.Title;
            Description.Text = $"Description:\n{Event.Description}";
            Timing.Text = $"Time: {Event.StartTime.LocalDateTime.ToString("h:mm tt")} - {Event.EndTime.LocalDateTime.ToString("h:mm tt")}";
            //Attendees.ItemsSource = Event.Attendees;
            foreach (Attendee attendee in Event.Attendees)
            {
                if(attendee.Organizer.HasValue && attendee.Organizer.Value==true)
                {
                    string createdby = (attendee.Name != null) ? attendee.Name : attendee.Email;
                    CreatedBy.Text = $"Created by: {createdby}";
                }
                AttendeesStack.Children.Add(new Label()
                {
                    Text = (attendee.Name != null) ? attendee.Name : attendee.Email,
                    FontSize = 20,
                    TextColor = Color.Black,
                    Padding = new Thickness(40, 0, 0, 0)
                });
            }
        }
    }
}
