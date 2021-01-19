using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Manifest.Models;
using Manifest.ViewModels;

namespace Manifest.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("IN NEW ITEM PAGE INITIALIZER");
            BindingContext = new NewItemViewModel();
        }
    }
}