using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Manifest.Config;
using Manifest.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;
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
            NavigationPage.SetHasNavigationBar(this, false);
            eventName.Text = newEvent.Title;
            eventDescription.Text = newEvent.Description;
            if (eventDescription.Text == "" || eventDescription.Text == null)
            {
                eventDescription.Text = "No description provided";
            }
            eventInfo.ItemsSource = datagrid;
            //datagrid.Add(newEvent);
            attendees = newEvent.Attendees;
            initializeEvent();

        }

        private async void initializeEvent()
        {
            try
            {
                await GetRelations();
                initializeAttendees(attendees);
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", "Error in EventsPage in intializeEvent function: " + e.ToString(), "OK");
            } 
        }

        private async Task GetRelations()
        {
            try
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
                client.DefaultRequestHeaders.Add("email", jsonObject2);
                Debug.WriteLine("Writing headers");
                Debug.WriteLine(client.DefaultRequestHeaders);
                string url = RdsConfig.BaseUrl + RdsConfig.getRelations;
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
                    else
                    {
                        attendees[i].Relation = "Unknown";
                    }
                    if (person.picture != null && person.picture != "")
                    {
                        attendees[i].PicUrl = person.picture;
                        attendees[i].HavePic = true;
                    }
                    if (person.phone_number != null && person.phone_number != "")
                    {
                        attendees[i].PhoneNumber = person.phone_number;
                    }
                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", "Error in EventsPage in GetRelations function: " + e.ToString(), "OK");
            }
        }


        private void initializeAttendees(List<Attendee> attendees)
        {
            try
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
            catch (Exception e)
            {
                DisplayAlert("Error", "Error in EventsPage in initializeAttendees function: " + e.ToString(), "OK");
            }
        }



        private async void callPerson(object sender, EventArgs args)
        {
            Image myvar = (Image)sender;
            Person person = myvar.BindingContext as Person;
            string phoneNumber = person.PhoneNumber;
            if (phoneNumber == "" || phoneNumber == null)
            {
                await DisplayAlert("Sorry!", $"Hmmm... We don't have a phone number on file", "OK");
            }
            else
            {
                //Console.WriteLine("ZZZZZZZZZZZZZZZ");
                Debug.WriteLine("Manifest.ViewModels.AboutViewModel: Dialing Number:" + phoneNumber);
                //Console.WriteLine("ZZZZZZZZZZZZZZZ");
                try
                {
                    PhoneDialer.Open(phoneNumber);
                    Debug.WriteLine("IN ABOUTVIEWMODEL. LAUNCHING PHONE");
                }
                catch (Exception e)
                {
                    await DisplayAlert("Error", "Unable to perform a phone call", "OK");
                }
                //await Launcher.OpenAsync(new Uri("tel:" + phoneNumber));
            }
        }


        private void goToTodaysList(object sender, EventArgs args)
        {
            Navigation.PopAsync();
        }
    }
}
