using Manifest.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Manifest.Services
{
    public class Repository
    {
        private static readonly Lazy<Repository> lazy = new Lazy<Repository>(() => new Repository());
        public static Repository Instance => lazy.Value;

        readonly private DataFactory factory;
        readonly private IDataClient dataClient;
        private User user = null;
        private readonly Dictionary<String, Occurance> OccuranceIdOccurancePair = new Dictionary<String, Occurance>();
        private readonly Dictionary<String, List<SubOccurance>> OccuranceSubOccurancePair = new Dictionary<string, List<SubOccurance>>();

        private Repository()
        {
            factory = DataFactory.Instance;
            dataClient = factory.GetDataClient();
        }
        public async Task<User> GetUser(string userId)
        {
            if (user == null)
            {
                user = await dataClient.GetUser(userId);
            }
            return user;
        }

        public List<Occurance> GetAllOccurances(string userId)
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
        }
    }
}
