using LetsGo.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class SocialController
    {
        readonly private FirebaseDB fb = new FirebaseDB();
        public string searchTag { get; set; }
        public ArrayList SearchResults { get; set; }
        public SocialController()
        {
            SearchResults = new ArrayList();
            
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

       

        public async void Search_Text(object sender, TextChangedEventArgs e)
        {
            searchTag = e.NewTextValue;
            SearchResults = await fb.Search(searchTag);
            search.ItemsSource = SearchResults;
        }
        public async void Communities_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CommunityPageController());
        }
        public async void Events_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ManageEvents());
        }
        public async void Friends_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FriendsPageController());
        }
        public async void OnAdd(object sender, EventArgs e)
        {
            var type = (MenuItem)sender;
            if (type.CommandParameter.ToString() == "LetsGo.Model.UserProfile")
            {
                UserProfile profile = (UserProfile)type.CommandParameter;
                string email = profile.Email;
                fb.AddFriend(email);
            }

        }

        public async void OnView(object sender, EventArgs e)
        {
            var type = (MenuItem)sender;
            if (type.CommandParameter.ToString() == "LetsGo.Model.UserProfile")
            {
                UserProfile profile = (UserProfile)type.CommandParameter;
                bool friend = await fb.isFriend(profile.Email);
                if (friend)
                {
                    await Navigation.PushAsync(new FriendProfileController(profile));
                }
                else if (profile.PublicAcct)
                {
                    await Navigation.PushAsync(new PublicProfileController(profile));
                }
                else if (!profile.PublicAcct)
                {
                    await Navigation.PushAsync(new PrivateProfileController(profile));
                }
                
            }
        }
    }
}
