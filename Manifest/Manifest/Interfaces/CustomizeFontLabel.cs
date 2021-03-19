using System;
using Xamarin.Forms;

namespace Manifest.Interfaces
{
    public class CustomizeFontLabel: Label
    {
        public CustomizeFontLabel()
        {
            if(Device.RuntimePlatform != Device.iOS) { Style = Application.Current.Resources["FontLabelStyle"] as Style; }
        }
    }
}
