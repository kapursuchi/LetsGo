using LetsGo.Model;
using LetsGo.Model.Authentication;
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


        public async void OnView(object sender, ItemTappedEventArgs e)
        {
            var type = e.ItemIndex;
            if (SearchResults[type].ToString() == "LetsGo.Model.UserProfile")
            {
                UserProfile profile = (UserProfile)SearchResults[type];
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
            else if (SearchResults[type].ToString() == "LetsGo.Model.CommunityProfile")
            {
                CommunityProfile selectedCommunity = e.Item as CommunityProfile;
                var auth = DependencyService.Get<IFirebaseAuthenticator>();
                auth.SetCurrentCommunity(selectedCommunity);
                bool member = await fb.isCommunityMember(selectedCommunity);
                string userEmail = fb.GetCurrentUser();
                // Community Leader taps on community
                if (selectedCommunity.Leader == userEmail)
                {
                    await Navigation.PushAsync(new CommunityLeaderViewController(selectedCommunity));
                }
                // Regular member taps on community
                else if (member)
                {
                    await Navigation.PushAsync(new CommunityMemberViewController(selectedCommunity));
                    //await Navigation.PushAsync(new ViewCommunityMemberController(selectedCommunity));
                }
                else
                {
                    await Navigation.PushAsync(new PublicCommunityController(selectedCommunity));
                }

            }/*
            else if (SearchResults[type].ToString() == "LetsGo.Model.EventProfile")
            {
                EventProfile evt = (EventProfile)SearchResults[type];
                var auth = DependencyService.Get<IFirebaseAuthenticator>();
                auth.SetCurrentEvent(evt);
            }*/
            this.ClearValue(ListView.SelectedItemProperty);

        }
    }
}
