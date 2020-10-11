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
            BindingContext = new ItemDetailViewModel();
        }
    }
}