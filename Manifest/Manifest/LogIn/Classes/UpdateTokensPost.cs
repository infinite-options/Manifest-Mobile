using System;
namespace Manifest.LogIn.Classes
{
    public class UpdateTokensPost
    {
        public string uid { get; set; }
        public string mobile_access_token { get; set; }
        public string mobile_refresh_token { get; set; }
    }
}
