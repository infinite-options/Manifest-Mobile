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

        }

        private async void GetRelations()
        {
            List<string> emails = new List<string>();
            if(attendees.Count != 0)
            {
                foreach (Attendee att in attendees)
                {
                    emails.Add(att.Email);
                }
                var header = new Dictionary<string, List<string>> { { "emails", emails } };
                string jsonObject = JsonConvert.SerializeObject(header);
                string jsonObject2 = JsonConvert.SerializeObject(emails);
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("email", jsonObject2);
                Debug.WriteLine("Writing headers");
                Debug.WriteLine(client.DefaultRequestHeaders);
                string url = AppConstants.BaseUrl + AppConstants.getRelations;
                var res = await client.GetStringAsync(url);
                //Debug.WriteLine(res.Content);
                var info = JsonConvert.DeserializeObject<RelationResponse>(res);
                List<RelationDto> peopleInfo = info.result;
                Debug.WriteLine(peopleInfo.ToString());
                for (int i = 0; i < attendees.Count; i++)
                {
                    RelationDto person = peopleInfo[i];
                    if (person.first_name != null && person.first_name != "")
                    {
                        if (person.last_name != null && person.last_name != "")
                        {
                            attendees[i].Name = person.first_name + " " + person.last_name;
                        }
                        else
                        {
                            attendees[i].Name = person.first_name;
                        }
                    }
                    if (person.role != null && person.role != "")
                    {
                        attendees[i].Relation = person.role;
                    }
                    if (person.picture != null && person.picture != "")
                    {
                        attendees[i].PicUrl = person.picture;
                        attendees[i].HavePic = true;
                    }
                }
                initialiseAttendees(attendees);
            }
            else
            {
                await DisplayAlert("Message","There are no attendees","OK");
            }
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
