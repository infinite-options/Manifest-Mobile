using System;
namespace Manifest.Config
{
    public class AppConstants
    {
        // ====================================================================
        // BASE URL AND ENDPOINTS - START
        // ====================================================================

        // KEY CONSTANTS FOR MANIFEST MY SPACE

        // BASE URL, GOOGLE INTERECEPTOR ANDROID (ONLY), NOTIFICATION HUB NAME,
        // AND NOTIFICATION LISTENER
       
        public const string BaseUrl = "https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev";
        public const string GoogleInterceptorUrlAndroid = "com.googleusercontent.apps.1009120542229-55an5om5ecl3it6quigclsnj0035oiap";
        public static string NotificationHubName { get; set; } = "Manifest-Notification-Hub";
        public static string ListenConnectionString { get; set; } = "Endpoint=sb://manifest-notifications-namespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=UWW7o7LFe8Oz6FZUQQ/gaNgqSfdN4Ckp6FCVCm3xuVg=";

        // KEY CONSTANTS FOR MANIFEST MY LIFE

        // BASE URL, GOOGLE INTERECEPTOR ANDROID (ONLY), NOTIFICATION HUB NAME,
        // AND NOTIFICATION LISTENER

        //public const string BaseUrl = "https://gyn3vgy3fb.execute-api.us-west-1.amazonaws.com/dev";
        //public const string GoogleInterceptorUrlAndroid = "com.googleusercontent.apps.1009120542229-55an5om5ecl3it6quigclsnj0035oiap";
        //public static string NotificationHubName { get; set; } = "Manifest-MyLife-Notification-Namespace";
        //public static string ListenConnectionString { get; set; } = "Endpoint=sb://manifest-mylife-notification-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=mTelt5YCK6pDrS8FDlj4K/+AMRFR0ScZXfKtDvFGZps=";

        // ENDPOINTS

        public const string listAllOtherTA = "/api/v2/listAllTA";
        public const string aboutMeUrl = "/api/v2/aboutme";
        public const string goalsAndRoutinesUrl = "/api/v2/getgoalsandroutines";
        public const string UserIdFromEmailUrl = "/api/v2/userLogin";
        public const string actionAndTaskUrl = "/api/v2/actionsTasks";
        public const string updateGoalAndRoutine = "/api/v2/udpateGRWatchMobile";
        public const string updateActionAndTask = "/api/v2/updateATWatchMobile";
        public const string timeSettingsUrl = "/api/v2/timeSettings"; //plus userId
        public const string goalsActInstrUrl = "/api/v2/gai";
        public const string addPulse = "/api/v2/changeAboutMeHistory";
        public const string getOptions = "/api/v2/";
        public const string login = "/api/v2/login";

        //Accelerometer
        public const string addCoordinates = "/api/v2/addCoordinates";

        //Endpoints for guid update and add
        public const string updateGuid = "/api/v2/updateGuid/update";
        public const string addGuid = "/api/v2/updateGuid/add";

        //Endpoints for routines
        //https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev/api/v2/rts/<string:user_id>
        public const string getRoutines = "/api/v2/rts";

        //Endpoint to update instruction
        public const string updateInstruction = "/api/v2/updateISWatchMobile";

        //Endpoint used to get relations. Takes in a list of emails
        public const string getRelations = "/api/v2/userTADetails";

        // ====================================================================
        // URL BASE AND ENDPOINTS - END
        // ====================================================================

        // ====================================================================
        // LOGIN CREDENTIALS - START
        // ====================================================================

        // FACEBOOK CONSTANTS
        public const string FacebookScope = "email";
        public const string FacebookAuthorizeUrl = "https://www.facebook.com/dialog/oauth/";
        public const string FacebookAccessTokenUrl = "https://www.facebook.com/connect/login_success.html";
        public const string FacebookUserInfoUrl = "https://graph.facebook.com/me?fields=email,name,picture&access_token=";

        // FACEBOOK ID Manifest Myspace
        public const string FacebookAndroidClientID = "343871123534886";
        public const string FacebookiOSClientID = "343871123534886";

        // FACEBOOK REDIRECT Manifest Myspace
        public const string FacebookiOSRedirectUrl = "https://www.facebook.com/connect/login_success.html:/oauth2redirect";
        public const string FacebookAndroidRedirectUrl = "https://www.facebook.com/connect/login_success.html";

        // GOOGLE CONSTANTS
        public const string GoogleScope = "https://www.googleapis.com/auth/photoslibrary  https://www.googleapis.com/auth/calendar  https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email";
        public const string GoogleAuthorizeUrl = "https://accounts.google.com/o/oauth2/v2/auth";
        public const string GoogleAccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        public const string GoogleUserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";
        public const string GoogleCalendarUrl = "https://www.googleapis.com/calendar/v3/calendars/primary/events";

        // GOOGLE ID Manifest Myspace (Credentials from Manifest Web Hosting)
        public const string GoogleiOSClientID = "1009120542229-m56ni4pvg8pj4tffuc2i6pthevrken6m.apps.googleusercontent.com";
        public const string GoogleAndroidClientID = "1009120542229-55an5om5ecl3it6quigclsnj0035oiap.apps.googleusercontent.com";

        // GOOGLE REDIRECT Manifest Myspace (Credentials from Manifest Web Hosting)
        public const string GoogleRedirectUrliOS = "com.googleusercontent.apps.1009120542229-m56ni4pvg8pj4tffuc2i6pthevrken6m:/oauth2redirect";
        public const string GoogleRedirectUrlAndroid = "com.googleusercontent.apps.1009120542229-55an5om5ecl3it6quigclsnj0035oiap:/oauth2redirect";
        // public const string GoogleInterceptorUrlAndroid = "com.googleusercontent.apps.1009120542229-55an5om5ecl3it6quigclsnj0035oiap";

        // ENDPOINTS Manifest Myspace
        public const string AccountSaltUrl = "https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev/api/v2/accountsalt";
        public const string LogInUrl = "https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev/api/v2/login";
        public const string SignUpUrl = "https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev/api/v2/signup";
        public const string UpdateTokensUrl = "https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev/api/v2/access_refresh_update";

        // RDS CODES
        public const string EmailNotFound = "404";
        public const string ErrorPlatform = "411";
        public const string ErrorUserDirectLogIn = "406";
        public const string UseSocialMediaLogin = "401";
        public const string AutheticatedSuccesful = "200";

        // PLATFORM
        public const string Google = "GOOGLE";
        public const string Facebook = "FACEBOOK";
        public const string Apple = "Apple";

        // EXTENDED TIME
        public const double days = 14;

        // ====================================================================
        // LOGIN CREDENTIALS - END
        // ====================================================================

        // ====================================================================
        // NOTIFICATION CREDENTIALS - START
        // ====================================================================

        /// Notification channels are used on Android devices starting with "Oreo"
        public static string NotificationChannelName { get; set; } = "XamarinNotifyChannel";

        /// This is the name of your Azure Notification Hub, found in your Azure portal.
        // public static string NotificationHubName { get; set; } = "Manifest-Notification-Hub";

        /// This is the "DefaultListenSharedAccessSignature" connection string, which is
        /// found in your Azure Notification Hub portal under "Access Policies".
        /// You should always use the ListenShared connection string. Do not use the
        /// FullShared connection string in a client application.

        // USE THE LISTEN, MANAGE, AND SEND
        //public static string ListenConnectionString { get; set; } = "Endpoint=sb://manifest-notifications-namespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=UWW7o7LFe8Oz6FZUQQ/gaNgqSfdN4Ckp6FCVCm3xuVg=";

        /// Tag used in log messages to easily filter the device log
        /// during development.
        public static string DebugTag { get; set; } = "XamarinNotify";

        /// The tags the device will subscribe to. These could be subjects like
        /// news, sports, and weather. Or they can be tags that identify a user
        /// across devices.
        public static string[] SubscriptionTags { get; set; } = { "default" };

        /// This is the template json that Android devices will use. Templates
        /// are defined by the device and can include multiple parameters.
        public static string FCMTemplateBody { get; set; } = "{\"data\":{\"message\":\"$(messageParam)\"}}";

        /// This is the template json that Apple devices will use. Templates
        /// are defined by the device and can include multiple parameters.
        public static string APNTemplateBody { get; set; } = "{\"aps\":{\"alert\":\"$(messageParam)\"}}";

        // ====================================================================
        // NOTIFICATION CREDENTIALS - END
        // ====================================================================
    }
}
