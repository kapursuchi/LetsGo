using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class ChatController
    {
        public ChatController()
        {
            InitializeComponent();
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#80b3d1");


        }

        public async void OnFriendsChat_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FriendsChatController());
        }
        public async void OnCommunitiesChat_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CommunityChatController());
        }
        public async void OnEventsChat_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EventsChatController());
        }





    }
}
