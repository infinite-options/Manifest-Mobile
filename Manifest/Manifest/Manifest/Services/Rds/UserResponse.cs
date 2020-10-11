using Manifest.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manifest.Services.Rds
{
    public class UserResponse
    {
        public string message { get; set; }
        public List<UserDto> result { get; set; }

        internal User ToUser(string userId)
        {
            User user = ToUser();
            user.Id = userId;
            return user;
        }

        private User ToUser()
        {
            User user = null;
            List<Person> importantPeople = new List<Person>();
            if (result != null)
            {
                foreach (UserDto dto in result)
                {
                    User usr = dto.ToUser();
                    if (usr != null)
                    {
                        user = usr;
                    }
                    else
                    {
                        Person p = dto.ToPerson();
                        if (p != null)
                        {
                            importantPeople.Add(p);
                        }
                    }
                }
            }
            if (user != null && importantPeople.Count > 0)
            {
                user.ImportantPeople = importantPeople;
            }
            return user;
        }
    }
}
