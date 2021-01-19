using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Manifest.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Manifest.Views
{
    public partial class AccelerometerPage : ContentPage
    {
        string uri = "https://gyn3vgy3fb.execute-api.us-west-1.amazonaws.com/dev/api/v2/addCoordinates";
        //string uri = "https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev/api/v2/addCoordinates";
        long counter = 0;
        public AccelerometerPage()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("IN ACCLEROMETER PAGE INTIALIZER");
        }

        void ButtonStart_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                    return;
                Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
                Accelerometer.Start(SensorSpeed.UI);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Console.WriteLine("Feature not supported");
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        private async void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            counter += 1;
            //Console.WriteLine("XXXXXXXXX");
            //Console.WriteLine(Math.Round(e.Reading.Acceleration.X, 3).ToString());
            //Console.WriteLine("XXXXXXXXX");

            //string interX = e.Reading.Acceleration.X.ToString();
            string interX = Math.Round(e.Reading.Acceleration.X, 3).ToString();
            if (interX == null || interX.Equals("")) { interX = "NaN"; }

            //string interY = e.Reading.Acceleration.Y.ToString();
            string interY = Math.Round(e.Reading.Acceleration.Y, 3).ToString();
            if (interY == null || interX.Equals("")) { interY = "NaN"; }

            //string interZ = e.Reading.Acceleration.Z.ToString();
            string interZ = Math.Round(e.Reading.Acceleration.Z, 3).ToString();
            if (interZ == null || interX.Equals("")) { interZ = "NaN"; }

            LabelX.Text = interX;
            LabelY.Text = interY;
            LabelZ.Text = interZ;

            AccelerometerValues accelerometerValue = new AccelerometerValues()
            {
                x = interX,
                y = interY,
                z = interZ,
                timestamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")

            };

            //Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            //Console.WriteLine(accelerometerValue.x + " " + accelerometerValue.y + " " + accelerometerValue.z + " " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            //Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXX");

            var updatePostSerilizedObject = JsonConvert.SerializeObject(accelerometerValue);
            System.Diagnostics.Debug.WriteLine(updatePostSerilizedObject);
            var updatePostContent = new StringContent(updatePostSerilizedObject, Encoding.UTF8, "application/json");
            var client = new HttpClient();
            if (counter == 5)
            {
                Console.WriteLine("Counter: " + counter);
                var RDSrespose = await client.PostAsync(uri, updatePostContent);
                var message = await RDSrespose.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(message);
                System.Diagnostics.Debug.WriteLine(RDSrespose.IsSuccessStatusCode);
                counter = 0;
            }

        }

        void ButtonStop_Clicked(System.Object sender, System.EventArgs e)
        {
            if (!Accelerometer.IsMonitoring)
                return;
            Accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
            Accelerometer.Stop();
        }

    }
}
