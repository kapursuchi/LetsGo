using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace LetsGo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new LoginPage(); //making login page as the main page
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
