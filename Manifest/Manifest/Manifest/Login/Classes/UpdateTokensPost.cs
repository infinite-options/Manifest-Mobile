﻿using System;
namespace Manifest.Login.Classes
{
    public class UpdateTokensPost
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string uid { get; set; }
        public string social_timestamp { get; set; }
    }
}
