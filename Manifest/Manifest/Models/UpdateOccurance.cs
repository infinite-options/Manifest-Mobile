using System;
using Newtonsoft.Json;

namespace Manifest.Models
{
    public class UpdateOccurance
    {
        public string id { get; set; }
        public DateTime datetime_completed { get; set; }
        public DateTime datetime_started { get; set; }
        public bool is_in_progress { get; set; }
        public bool is_complete { get; set; }

        public string updateOccurance()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
