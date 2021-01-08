namespace NotificationDispatcher
{
    public static class DispatcherConstants
    {

        /// <summary>
        /// These are the tags the app will send to. These could be subjects like
        /// news, sports, and weather, or they can be custom tags that identify a user
        /// across devices.
        /// </summary>
        public static string[] SubscriptionTags { get; set; } = { "default" };


        /// <summary>
        /// This is the name of your Azure Notification Hub, found in your Azure portal.
        /// </summary>
        public static string NotificationHubName { get; set; } = "Manifest-MyLife-Notification-Hub";

        //public static string NotificationHubName { get; set; } = "Manifest-Notification-Hub";

        /// <summary>
        /// This is the "DefaultFullSharedAccessSignature" connection string, which is
        /// found in your Azure Notification Hub portal under "Access Policies".
        /// </summary>
        public static string FullAccessConnectionString { get; set; } = "Endpoint=sb://manifest-mylife-notification-namespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=5qjUCYi5MhG8mwB9XmNGpxLxOeDcUJCtCuwodeDEsoc=";

        //public static string FullAccessConnectionString { get; set; } = "Endpoint=sb://manifest-notifications-namespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=UWW7o7LFe8Oz6FZUQQ/gaNgqSfdN4Ckp6FCVCm3xuVg=";

    }
}
