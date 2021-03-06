﻿using System;
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
        string uri = "https://3s3sftsr90.execute-api.us-west-1.amazonaws.com/dev/api/v2/addCoordinates";

        public AccelerometerPage()
        {
            InitializeComponent();
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
            string interX = e.Reading.Acceleration.X.ToString();
            if (interX == null || interX.Equals("")) { interX = "NaN"; }

            string interY = e.Reading.Acceleration.Y.ToString();
            if (interY == null || interX.Equals("")) { interY = "NaN"; }

            string interZ = e.Reading.Acceleration.Z.ToString();
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
            var RDSrespose = await client.PostAsync(uri, updatePostContent);
            var message = await RDSrespose.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(RDSrespose.IsSuccessStatusCode);

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
