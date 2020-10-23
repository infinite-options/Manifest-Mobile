using Manifest.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Manifest.Services
{
    public class Repository
    {
        private static readonly Lazy<Repository> lazy = new Lazy<Repository>(() => new Repository());
        public static Repository Instance => lazy.Value;

        readonly private DataFactory factory;
        readonly private CalendarFactory calendarFactory;

        readonly private IDataClient dataClient;
        private ICalendarClient calendarClient;

        private User user = null;
        private readonly Dictionary<String, Occurance> OccuranceIdOccurancePair = new Dictionary<String, Occurance>();
        private readonly Dictionary<String, List<SubOccurance>> OccuranceSubOccurancePair = new Dictionary<string, List<SubOccurance>>();
        private List<Event> Events = null;
        private Result Session = null;

        private Repository()
        {
            factory = DataFactory.Instance;
            dataClient = factory.GetDataClient();

            calendarFactory = CalendarFactory.Instance;
        }
        public async Task<User> GetUser(string userId)
        {
            if (user == null)
            {
                user = await dataClient.GetUser(userId);
            }
            return user;
        }

        public async Task<List<Occurance>> GetAllOccurances(string userId)
        {
            if (OccuranceIdOccurancePair.Count == 0)
            {
                var occurancesTask =  dataClient.GetOccurances(userId);
                occurancesTask.Wait();
                foreach (Occurance occur in occurancesTask.Result)
                {
                    OccuranceIdOccurancePair.Add(occur.Id, occur);
                }
            }
            return OccuranceIdOccurancePair.Values.ToList();
        }

        public Occurance GetOccuranceById(string occuranceId)
        {
            return OccuranceIdOccurancePair[occuranceId];
        }

        public List<SubOccurance> GetSubOccurances(string occuranceId)
        {
            if (!OccuranceSubOccurancePair.ContainsKey(occuranceId))
            {
                var task = dataClient.GetSubOccurances(occuranceId);
                task.Wait();
                OccuranceSubOccurancePair.Add(occuranceId, task.Result);
            }
            return OccuranceSubOccurancePair[occuranceId];
        }

        public async Task<bool> UpdateOccurance(Occurance occurance)
        {
            try
            {
                _ = dataClient.UpdateOccurance(occurance);
                return true;
            }catch(Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> UpdateSubOccurance(SubOccurance subOccurance)
        {
            try
            {
                _ = dataClient.UpdateSubOccurance(subOccurance);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public void ClearSession()
        {
            user = null;
            OccuranceIdOccurancePair.Clear();
            OccuranceSubOccurancePair.Clear();
            Session = null;
            Events = null;
            Application.Current.Properties.Clear();
        }

        public void SaveSession(Session session)
        {
            Session = session.result[0];
            string SessionString = JsonConvert.SerializeObject(Session);
            Application.Current.Properties["session"] = SessionString;
            //Application.Current.Properties["user_id"] = session.result[0].user_unique_id;
            //Application.Current.Properties["google_access_token"] = session.result[0].google_auth_token;
            //Application.Current.Properties["google_refresh_token"] = session.result[0].google_refresh_token;
        }

        public bool LoadSession()
        {
            if (Application.Current.Properties.ContainsKey("session"))
            {
                string SessionString = (string) Application.Current.Properties["session"];
                Session = JsonConvert.DeserializeObject<Result>(SessionString);
                //Session = new Result
                //{
                //    user_unique_id = (string)Application.Current.Properties["user_id"],
                //Session.google_auth_token = (string)Application.Current.Properties["google_access_token"];
                //Session.google_refresh_token = (string)Application.Current.Properties["google_refresh_token"];
                //};
                return true;
            }
            return false;
        }

        public User LoadUserData()
        {
            try
            {
                var task = GetUser(Session.user_unique_id);
                task.Wait();
                return task.Result;
            }catch(Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        public async Task<List<Event>> GetEvents()
        {
            if (calendarClient==null)
            {
                calendarClient = calendarFactory.GetClient(Session.user_social_media as string);
            }
            try
            {
                if (Events == null)
                {
                    var task = calendarClient.GetEventsList(Session.mobile_auth_token, DateTimeOffset.Now);
                    Events = await task;
                }
                return Events;
            }catch(Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }
    }
}
