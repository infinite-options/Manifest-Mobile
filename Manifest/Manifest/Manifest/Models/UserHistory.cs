using System;
namespace Manifest.Models
{

    public class UserHistory
    {
        public string message { get; set; }
        public Categories result { get; set; }
    }

    public class Categories
    {
        public object motivation { get; set; }
        public object feelings { get; set; }
        public object happy { get; set; }
        public object important { get; set; }
    }
}
