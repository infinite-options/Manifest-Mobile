using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manifest.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Manifest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TodaysList : ContentPage
    {
        public TodaysList()
        {
            DataFactory.Instance.GetDataClient().GetSubOccurances("300-000049");
            InitializeComponent();
        }
    }
}