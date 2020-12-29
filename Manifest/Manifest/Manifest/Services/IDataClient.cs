using Manifest.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Manifest.Services
{
    public interface IDataClient
    {
        Task<User> GetUser(string userId);

        Task<List<Occurance>> GetOccurances(string userId);

        Task<List<SubOccurance>> GetSubOccurances(string occuranceId);

        Task UpdateOccurance(Occurance occurance);

        Task UpdateSubOccurance(SubOccurance subOccurance);

        //Task storeGUID(string guid, string uid);
    }
}
