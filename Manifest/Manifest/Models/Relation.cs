using System;
using System.Collections.Generic;

namespace Manifest.Models
{
    public class Relation
    {

    }

    public class RelationResponse
    {
        public string message { get; set; }
        public List<RelationDto> result { get; set; }
    }

    public class RelationDto
    {
        public string email_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone_number { get; set; }
        public string picture { get; set; }
        public string role { get; set; }
        //public List<Json> actions_tasks { get; set; }
    }
}
