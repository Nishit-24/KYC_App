using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XFUploadFile.Views;

namespace XFUploadFile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new MainPage();
            //MainPage = new NavigationPage(new MainPage());

            MainPage = new NavigationPage(new CustomerDetailsPage());
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
