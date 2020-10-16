using System;
using System.Collections.Generic;
using Manifest.ViewModels;
using Manifest.Views;
using Xamarin.Forms;

namespace Manifest
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(SubOccuranceCarousalView), typeof(SubOccuranceCarousalView));
            Routing.RegisterRoute(nameof(SubOccuranceListView), typeof(SubOccuranceListView));
            Shell.SetNavBarIsVisible(this, false);
        }

    }
}
