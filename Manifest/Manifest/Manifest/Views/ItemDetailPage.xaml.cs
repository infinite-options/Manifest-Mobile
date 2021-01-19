using System.ComponentModel;
using Xamarin.Forms;
using Manifest.ViewModels;

namespace Manifest.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("IN ITEM DETAIL PAGE INITIALIZER");
            BindingContext = new ItemDetailViewModel();
        }
    }
}