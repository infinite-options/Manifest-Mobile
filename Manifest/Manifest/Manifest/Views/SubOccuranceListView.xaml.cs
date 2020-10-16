using Manifest.Models;
using Manifest.Services;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Manifest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubOccuranceListView : ContentPage
    {
        public SubOccuranceListView()
        {
            InitializeComponent();
            List<SubOccurance> subOccurances = Repository.Instance.GetSubOccurances("300-000049");
            SubOccuranceCollectionView.ItemsSource = subOccurances;
        }
    }
}