using System;
using LetsGo.Model;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class ManageEvents
    {
        public ManageEvents()
        {
           InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        public async void NavigateToCreateEvent(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EventsPageController());
        }
    }
}
