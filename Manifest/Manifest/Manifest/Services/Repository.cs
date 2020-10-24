using Manifest.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
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
        private Dictionary<string, Event> Events = new Dictionary<string, Event>();
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

        public async Task<User> RefreshUser(string userId)
        {
            var userTask =  dataClient.GetUser(userId);
            user = await userTask;
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

        public async Task<List<Occurance>> GetAllFreshOccurances(string userId)
        {
            var occurancesTask = dataClient.GetOccurances(userId);
            occurancesTask.Wait();
            OccuranceIdOccurancePair.Clear();
            foreach (Occurance occur in occurancesTask.Result)
            {
                OccuranceIdOccurancePair.Add(occur.Id, occur);
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

        public List<SubOccurance> GetFreshSubOccurances(string occuranceId)
        {
            var task = dataClient.GetSubOccurances(occuranceId);
            task.Wait();
            OccuranceSubOccurancePair.Clear();
            OccuranceSubOccurancePair.Add(occuranceId, task.Result);
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
            SecureStorage.RemoveAll();
            Preferences.Clear();
            user = null;
            OccuranceIdOccurancePair.Clear();
            OccuranceSubOccurancePair.Clear();
            Session = null;
            Events.Clear();
            Application.Current.Properties.Clear();
        }

        public void SaveSession(Session session)
        { 
            SaveSession(session.result[0]);
        }

        private void SaveSession(Result session)
        {
            Session = session;
            string SessionString = JsonConvert.SerializeObject(Session);
            Application.Current.Properties["session"] = SessionString;
        }

        public bool LoadSession()
        {
            if (Application.Current.Properties.ContainsKey("session"))
            {
                string SessionString = (string) Application.Current.Properties["session"];
                Session = JsonConvert.DeserializeObject<Result>(SessionString);
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

        public User RefreshUserData()
        {
            try
            {
                var task = RefreshUser(Session.user_unique_id);
                task.Wait();
                return task.Result;
            }
            catch (Exception e)
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
                if (Events.Count == 0)
                {
                    var eventList = await calendarClient.GetEventsList(DateTimeOffset.Now);
                    foreach(Event @event in eventList)
                    {

                        Events.Add(@event.Id, @event);
                    }  
                }
                return Events.Values.ToList<Event>();
            }catch(Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        public async Task<List<Event>> GetFreshEvents()
        {
            if (calendarClient == null)
            {
                calendarClient = calendarFactory.GetClient(Session.user_social_media as string);
            }
            try
            {
                var eventList = await calendarClient.GetEventsList(DateTimeOffset.Now);
                Events.Clear();
                foreach (Event @event in eventList)
                {
                    Events.Add(@event.Id, @event);
                }
                return Events.Values.ToList<Event>();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        public async Task<Event> GetEventById(string id)
        {
            return Events[id];
        }

        public void UpdateAccessToken(string accessToken)
        {
            Session.mobile_auth_token = accessToken;
            SaveSession(Session);
        }

        public string GetAccessToken()
        {
            return Session.mobile_auth_token;
        }

        public string GetRefreshToken()
        {
            return Session.mobile_refresh_token;
        }

    }
}
