using System;
namespace Manifest.Login.Classes
{
    public class SocialLogInPost
    {
        public string email { get; set; }
        public string social_id { get; set; }
        public string mobile_access_token { get; set; }
        public string mobile_refresh_token { get; set; }
        public string signup_platform { get; set; }
    }
}
