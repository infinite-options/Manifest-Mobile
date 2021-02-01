using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Firebase.Messaging;
using System;
using System.Linq;
using WindowsAzure.Messaging;
using Xamarin.Essentials;   // Added so that Preferences would work
namespace Manifest.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class FirebaseService : FirebaseMessagingService
    {
        public override void OnNewToken(string token)
        {
            Console.WriteLine("Entered OnNewToken");
            // TODO: save token instance locally, or log if desired
            Console.WriteLine("New Token:" + token);
            SendRegistrationToServer(token);
        }

        // serving fresh code
        void SendRegistrationToServer(string token)
        {
            //Console.WriteLine("SendRegistrationToServer called");
            if (Preferences.Get("guid", null) != null)
            {
                var tag = Preferences.Get("guid", null);
                GlobalVars.user_guid = tag;
                Console.WriteLine("guid:" + tag);
                Console.WriteLine("token:" + token);
                return;
            }
            try
            {
                NotificationHub hub = new NotificationHub(AppConstants.NotificationHubName, AppConstants.ListenConnectionString, this);
                var guid = Guid.NewGuid();
                var tag = "guid_" + guid.ToString();
                //Send guid to endpoint using following fornat
                //{     "user_unique_id": "100-000045",     "guid": "ndbfndbfnbn",     "notification": "FALSE" }
                //Endpoint  =  https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev/api/v2/updateGuid/add or replace add with update
                GlobalVars.user_guid = guid.ToString();
                Console.WriteLine("guid:" + tag);
                Console.WriteLine("token:" + token);
                Preferences.Set("guid", tag);
                string[] tags = new string[2] { "default", tag };

                // register device with Azure Notification Hub using the token from FCM
                Registration registration = hub.Register(token, tags);

                // subscribe to the SubscriptionTags list with a simple template.
                string pnsHandle = registration.PNSHandle;
                TemplateRegistration templateReg = hub.RegisterTemplate(pnsHandle, "defaultTemplate", AppConstants.FCMTemplateBody, tags);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error registering device: {e.Message}");
                Log.Error(AppConstants.DebugTag, $"Error registering device: {e.Message}");
            }
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            Console.WriteLine("Message Received!");
            base.OnMessageReceived(message);
            string messageBody = string.Empty;

            if (message.GetNotification() != null)
            {
                messageBody = message.GetNotification().Body;
            }

            // NOTE: test messages sent via the Azure portal will be received here
            else
            {
                messageBody = message.Data.Values.First();
            }

            // convert the incoming message to a local notification
            SendLocalNotification(messageBody);

            // send the incoming message directly to the MainPage
            //SendMessageToMainPage(messageBody);
        }

        void SendLocalNotification(string body)
        {
            var intent = new Intent(this, typeof(Manifest.Droid.MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.PutExtra("message", body);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(this, AppConstants.NotificationChannelName)
                .SetContentTitle("XamarinNotify Message")
                .SetSmallIcon(Manifest.Droid.Resource.Drawable.ic_launcher)
                .SetContentText(body)
                .SetAutoCancel(true)
                .SetShowWhen(false)
                .SetContentIntent(pendingIntent);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                notificationBuilder.SetChannelId(AppConstants.NotificationChannelName);
            }

            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());
        }

    }
}
