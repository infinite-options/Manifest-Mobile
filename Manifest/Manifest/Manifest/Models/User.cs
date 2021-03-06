﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Manifest.Models
{
    public class User
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public bool HavePic { get; set; }
        public string Id { get; set; }
        public string LastName { get; set; }
        public string MessageCard { get; set; }
        public string MessageDay { get; set; }
        public string PicUrl { get; set; }
        public List<Person> ImportantPeople { get; set; }
        public TimeSettings TimeSettings { get; set; }
    }
}
