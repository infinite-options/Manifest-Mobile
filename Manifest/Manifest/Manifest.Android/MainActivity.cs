using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common;
using Android.Util;


namespace Manifest.Droid
{
    [Activity(Label = "Manifest", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        public const string TAG = "MainActivity";
        internal static readonly string CHANNEL_ID = "my_notification_channel";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            //Below lines are required for notifications
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            //LoadApplication(new App());

            //if (!IsPlayServiceAvailable())
            //{
            //    throw new Exception("This device does not have Google Play Services and cannot receive push notifications.");
            //}

            //CreateNotificationChannel();

            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    if (key != null)
                    {
                        var value = Intent.Extras.GetString(key);
                        Log.Debug(TAG, "Key: {0} Value: {1}", key, value);
                    }
                }
            }

            IsPlayServicesAvailable();
            CreateNotificationChannel();




            //End of lines needed for notifications

            global::Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, savedInstanceState);
            global::Xamarin.Auth.CustomTabsConfiguration.CustomTabsClosingMessage = null;
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            //testing with forms9patch
            Forms9Patch.Droid.Settings.Initialize(this);

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        //This function checks if GooglePlayService is available
        //bool IsPlayServiceAvailable()
        //{
        //    int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
        //    if (resultCode != ConnectionResult.Success)
        //    {
        //        if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
        //            Log.Debug(AppConstants.DebugTag, GoogleApiAvailability.Instance.GetErrorString(resultCode));
        //        else
        //        {
        //            Log.Debug(AppConstants.DebugTag, "This device is not supported");
        //        }
        //        return false;
        //    }
        //    Console.WriteLine("GooglePlay service is available");
        //    return true;
        //}

        // Step 8 of Set up notification hubs in your project
        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    Log.Debug(TAG, GoogleApiAvailability.Instance.GetErrorString(resultCode));
                else
                {
                    Log.Debug(TAG, "This device is not supported");
                    Finish();
                }
                return false;
            }

            Log.Debug(TAG, "Google Play Services is available.");
            return true;
        }



        ////Creates the notification channel
        //void CreateNotificationChannel()
        //{
        //    // Notification channels are new as of "Oreo".
        //    // There is no need to create a notification channel on older versions of Android.
        //    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        //    {
        //        var channelName = Manifest.AppConstants.NotificationChannelName;
        //        var channelDescription = String.Empty;
        //        var channel = new NotificationChannel(channelName, channelName, NotificationImportance.Max)
        //        {
        //            Description = channelDescription
        //        };

        //        var notificationManager = (NotificationManager)GetSystemService(NotificationService);
        //        notificationManager.CreateNotificationChannel(channel);
        //    }
        //    Console.WriteLine("Notification channel created");
        //}

        // Step 9 of Set up notification hubs in your project
        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channelName = CHANNEL_ID;
            var channelDescription = string.Empty;
            var channel = new NotificationChannel(CHANNEL_ID, channelName, NotificationImportance.Default)
            {
                Description = channelDescription
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}