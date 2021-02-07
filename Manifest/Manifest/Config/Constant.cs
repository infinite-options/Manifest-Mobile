using System;
namespace Manifest.Config
{
    public class Constant
    {
        // FACEBOOK CONSTANTS
        public static string FacebookScope = "email";
        public static string FacebookAuthorizeUrl = "https://www.facebook.com/dialog/oauth/";
        public static string FacebookAccessTokenUrl = "https://www.facebook.com/connect/login_success.html";
        public static string FacebookUserInfoUrl = "https://graph.facebook.com/me?fields=email,name,picture&access_token=";

        // FACEBOOK ID Manifest Myspace
        public static string FacebookAndroidClientID = "343871123534886";
        public static string FacebookiOSClientID = "343871123534886";

        // FACEBOOK REDIRECT Manifest Myspace
        public static string FacebookiOSRedirectUrl = "https://www.facebook.com/connect/login_success.html:/oauth2redirect";
        public static string FacebookAndroidRedirectUrl = "https://www.facebook.com/connect/login_success.html";

        // GOOGLE CONSTANTS
        public static string GoogleScope = "https://www.googleapis.com/auth/photoslibrary  https://www.googleapis.com/auth/calendar  https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email";
        public static string GoogleAuthorizeUrl = "https://accounts.google.com/o/oauth2/v2/auth";
        public static string GoogleAccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        public static string GoogleUserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";
        public const string GoogleCalendarUrl = "https://www.googleapis.com/calendar/v3/calendars/primary/events";

        // GOOGLE ID Manifest Myspace
        public static string GoogleiOSClientID = "287117315224-68vb8vfam5gj4epoghhkm6oh8fhgabeb.apps.googleusercontent.com";
        public static string GoogleAndroidClientID = "287117315224-m3v1urhm5ii73chqfj1a0hlfid8ivimg.apps.googleusercontent.com";

        // GOOGLE REDIRECT Manifest Myspace
        public static string GoogleRedirectUrliOS = "com.googleusercontent.apps.287117315224-68vb8vfam5gj4epoghhkm6oh8fhgabeb:/oauth2redirect";
        public static string GoogleRedirectUrlAndroid = "com.googleusercontent.apps.287117315224-m3v1urhm5ii73chqfj1a0hlfid8ivimg:/oauth2redirect";

        // ENDPOINTS Manifest Myspace
        public static string AccountSaltUrl = "https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev/api/v2/accountsalt";
        public static string LogInUrl = "https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev/api/v2/login";
        public static string SignUpUrl = "https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev/api/v2/signup";
        public static string UpdateTokensUrl = "https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev/api/v2/access_refresh_update";

        //Session

        // RDS CODES
        public static string EmailNotFound = "404";
        public static string ErrorPlatform = "411";
        public static string ErrorUserDirectLogIn = "406";
        public static string UseSocialMediaLogin = "401";
        public static string AutheticatedSuccesful = "200";

        // PLATFORM
        public static string Google = "GOOGLE";
        public static string Facebook = "FACEBOOK";
        public static string Apple = "Apple";

        // EXTENDED TIME
        public static double days = 14;
    }
}
