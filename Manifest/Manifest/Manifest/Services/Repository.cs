using Manifest.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Manifest.Services
{
    public class Repository
    {
        private static readonly Lazy<Repository> lazy = new Lazy<Repository>(() => new Repository());
        public static Repository Instance => lazy.Value;

        readonly private DataFactory facotry;
        readonly private IDataClient dataClient;
        private User user = null;
        private List<Occurance> occurances = null;

        private Repository()
        {
            facotry = DataFactory.Instance;
            dataClient = facotry.GetDataClient();
        }
        public User GetUser(string userId)
        {
            if (user == null)
            {
                var userTask = dataClient.GetUser(userId);
                userTask.Wait();
                user = userTask.Result;
            }
            return user;
        }

        public List<Occurance> GetOccurance(string userId)
        {
            if (occurances == null)
            {
                var occurancesTask =  dataClient.GetOccurances(userId);
                occurancesTask.Wait();
                occurances = occurancesTask.Result;
            }
            return occurances;
        }
    }
}
