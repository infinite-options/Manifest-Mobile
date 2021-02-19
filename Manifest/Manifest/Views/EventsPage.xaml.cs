using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using Manifest.Config;
using Manifest.Models;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class EventsPage : ContentPage
    {
        public ObservableCollection<Attendee> datagrid = new ObservableCollection<Attendee>();
        List<Attendee> attendees = new List<Attendee>();
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
            attendees = newEvent.Attendees;
            GetRelations();
            initialiseAttendees(newEvent.Attendees);

        }

        private async void GetRelations()
        {
            List<string> emails = new List<string>();
            foreach (Attendee att in attendees)
            {
                emails.Add(att.Email);
            }
            var header = new Dictionary<string, List<string>> { { "emails", emails } };
            string jsonObject = JsonConvert.SerializeObject(header);
            string jsonObject2 = JsonConvert.SerializeObject(emails);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("emails", jsonObject);
            Debug.WriteLine("Writing headers");
            Debug.WriteLine(client.DefaultRequestHeaders);
            string url = RdsConfig.BaseUrl + RdsConfig.getRelations;
            var res = await client.GetAsync(url);
            Debug.WriteLine(res);


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
                    if (attendee.Email != "" && attendee.Email != null)
                    {
                        attendee.Name = attendee.Email;
                    }
                    else
                    {
                        attendee.Name = "Anonymous";
                    } 
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
