using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manifest.Services
{
    public interface IDataStore<T>
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);

        //Task<User> GetUser(string userId);
        //Task<List<user>> GetAllOtherTAs(string userId);
        //Task<List<GratisObject>> GetGoalsAndRoutines(string userId);
        //Task<string> GetUserId(string emailId);
        //Task<List<atObject>> GetActionsAndTasks(string grId);
        //Task<List<atObject>> GetActionsAndTasks(string grId, string type);
    }
}
