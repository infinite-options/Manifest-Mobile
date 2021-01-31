using System;
using System.Collections.Generic;
using System.Net.Http;
using Manifest.Config;
using Newtonsoft.Json;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Manifest.RDS
{
    public class RdsConnect
    {
        static HttpClient client = new HttpClient();

        //Use this function to send the guid of a user to the database
        public static async void storeGUID(string guid, string uid)
        {
            string url = RdsConfig.BaseUrl + RdsConfig.addGuid;
            Console.WriteLine("guid = " + guid + ", uid = " + uid);
            if (guid == null || guid == "")
            {
                return;
            }
            try
            {
                IDictionary<string, string> userDict = new Dictionary<string, string>();
                userDict.Add("user_unique_id", uid);
                userDict.Add("guid", guid);
                userDict.Add("notification", "TRUE");
                string json_data = JsonConvert.SerializeObject(userDict);
                //Now, send a PUT request to the end point
                var httpContent = new StringContent(json_data, Encoding.UTF8);
                var response = await client.PostAsync(url, httpContent);
                Debug.WriteLine("IN GUID TRY BLOCK");
                //response.Wait();
            }
            catch (Exception e)
            {
                Debug.WriteLine("IN GUID CATCH BLOCK");
                Debug.WriteLine(e);
                throw;
            }
            //Below is format to post
            //{     "user_unique_id": "100-000045",     "guid": "ndbfndbfnbn",     "notification": "FALSE" }
        }

        //
        public static async Task<string> getUser(string uid)
        {
            string url = RdsConfig.BaseUrl + RdsConfig.aboutMeUrl + "/" + uid;
            var res = await client.GetStringAsync(url);
            return res;
        }
    }
}
