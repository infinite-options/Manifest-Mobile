using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Manifest.Services;
using Manifest.Views;

namespace Manifest
{
    public partial class App : Application
    {
        Repository repo = Repository.Instance;

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
