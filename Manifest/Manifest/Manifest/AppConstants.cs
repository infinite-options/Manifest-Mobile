namespace Manifest
{
    public static class AppConstants
    {
        /// <summary>
        /// Notification channels are used on Android devices starting with "Oreo"
        /// </summary>
        public static string NotificationChannelName { get; set; } = "MyLifeNotifyChannel";

        /// <summary>
        /// This is the name of your Azure Notification Hub, found in your Azure portal.
        /// </summary>
        public static string NotificationHubName { get; set; } = "Manifest-MyLife-Notification-Hub";

        //public static string NotificationHubName { get; set; } = "Manifest-Notification-Hub";
        //Manifest MySpace Hub Name: "Manifest-Notification-Hub";

        /// <summary>
        /// This is the "DefaultListenSharedAccessSignature" connection string, which is
        /// found in your Azure Notification Hub portal under "Access Policies".
        /// 
        /// You should always use the ListenShared connection string. Do not use the
        /// FullShared connection string in a client application.
        /// </summary>
        public static string ListenConnectionString { get; set; } = "Endpoint=sb://manifest-mylife-notification-namespace.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=V3sSgMRx09GcTtk/8spc70YsA2uJ1tJmwWd6x6NZhW4=";

        //public static string ListenConnectionString { get; set; } = "Endpoint=sb://manifest-notifications-namespace.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=AQuiUeN4QL+TJJVwwWyPvRtQM6U/hTo2Q4YO8kJzC8E=";



        /// <summary>
        /// Tag used in log messages to easily filter the device log
        /// during development.
        /// </summary>
        public static string DebugTag { get; set; } = "XamarinNotify";

        /// <summary>
        /// The tags the device will subscribe to. These could be subjects like
        /// news, sports, and weather. Or they can be tags that identify a user
        /// across devices.
        /// </summary>
        public static string[] SubscriptionTags { get; set; } = { "default" };

        /// <summary>
        /// This is the template json that Android devices will use. Templates
        /// are defined by the device and can include multiple parameters.
        /// </summary>
        public static string FCMTemplateBody { get; set; } = "{\"data\":{\"message\":\"$(messageParam)\"}}";

        /// <summary>
        /// This is the template json that Apple devices will use. Templates
        /// are defined by the device and can include multiple parameters.
        /// </summary>
        public static string APNTemplateBody { get; set; } = "{\"aps\":{\"alert\":\"$(messageParam)\"}}";
    }
}
