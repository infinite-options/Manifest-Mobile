using Manifest.Config;
using Manifest.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Manifest.Services.Rds
{
    public class RdsClient : IDataClient
    {
        private readonly string BaseUrl;

        public RdsClient(string BaseUrl)
        {
            this.BaseUrl = BaseUrl;
        }

        public async Task<User> GetUser(string userId)
        {
            string url = BaseUrl + RdsConfig.aboutMeUrl + "/" + userId;
            //Debug.WriteLine("Making call to : "+url);
            try
            {
                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(url);
                response.Wait();
                //Debug.WriteLine(response);
                UserResponse userResponse = JsonConvert.DeserializeObject<UserResponse>(response.Result);
                //foreach(UserDto dto in userResponse.result)
                //{
                //    Debug.WriteLine(dto.relation_type);
                //}
                return userResponse.ToUser(userId);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                throw;
            }
        }

        public async Task<List<Occurance>> GetOccurances(string userId)
        {
            string url = BaseUrl + RdsConfig.goalsAndRoutinesUrl + "/" + userId;
            try
            {
                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(url);
                response.Wait();
                OccuranceResponse occuranceResponse = JsonConvert.DeserializeObject<OccuranceResponse>(response.Result);
                return occuranceResponse.ToOccurances();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                throw;
            }
        }

        public async Task<List<SubOccurance>> GetSubOccurances(string occuranceId)
        {
            string url = BaseUrl + RdsConfig.actionAndTaskUrl + "/" + occuranceId;
            try
            {
                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(url);
                response.Wait();
                SubOccuranceResponse SubOccuranceResponse = JsonConvert.DeserializeObject<SubOccuranceResponse>(response.Result);
                return SubOccuranceResponse.ToSubOccurances();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                throw;
            }
           
        }
    }
}
