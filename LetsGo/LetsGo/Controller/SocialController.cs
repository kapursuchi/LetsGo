using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class SocialController
    {
        public SocialController()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public async void Communities_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CommunityPageController());
        }
        public async void Events_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EventsPageController());
        }
        public async void  Friends_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FriendsPageController());
        }
    }
}
