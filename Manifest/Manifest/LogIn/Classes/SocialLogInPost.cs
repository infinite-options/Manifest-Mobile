using System;
namespace Manifest.LogIn.Classes
{
    public class SocialLogInPost
    {
        public string email { get; set; }
        public string password { get; set; }
        public string social_id { get; set; }
        public string signup_platform { get; set; }
        public string mobile_access_token { get; set; }
        public string mobile_refresh_token { get; set; }
    }
}
