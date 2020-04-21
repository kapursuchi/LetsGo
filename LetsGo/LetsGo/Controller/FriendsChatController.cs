using LetsGo.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class FriendsChatController
    {
        private readonly FirebaseDB fb = new FirebaseDB();
        public string SearchStr { get; set; }
        public List<UserProfile> SearchResults { get; set; }
        List<UserProfile> ConversationsWith { get; set; }
        public FriendsChatController()
        {
            SetValues();
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public async void SetValues()
        {
            string current = fb.GetCurrentUser();
            string email = "";
            ConversationsWith = new List<UserProfile>();
            List<Conversation> conversations = await fb.GetMyConversationsWithFriends();
            if (conversations != null && conversations.Count != 0)
            {
                foreach (Conversation convo in conversations)
                {

                    foreach (string recipient in convo.ConversationBetween)
                    {
                        if (recipient != current)
                            email = recipient;
                    }
                    UserProfile userprofile = await fb.GetUserObject(email);
                    userprofile.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(userprofile.Name);
                    ConversationsWith.Add(userprofile);
                }
                double height = 40;
                chatWithFriends.HeightRequest = ConversationsWith.Count * height;
                chatWithFriends.ItemsSource = ConversationsWith;
                activeChats.IsVisible = true;
                noChats.IsVisible = false;
                search.IsVisible = false;
                noMatchingSearchResultsLbl.IsVisible = false;
            }
            else
            {
                chatWithFriends.IsVisible = false;
                activeChats.IsVisible = false;
                noChats.IsVisible = true;
                search.IsVisible = false;
                noMatchingSearchResultsLbl.IsVisible = false;
            }

            //UserProfile user = await fb.GetUserObject("khali009@cougars.csusm.edu");
            //fb.StartConversationWithFriend(user);
            //fb.SendMessageToFriend(user.Email, "hello");
            
        }

        public async void Search_Text(object sender, EventArgs e)
        {
            string current = fb.GetCurrentUser();
            SearchStr = searchBar.Text.ToLower();
            List<string> myFriends = await fb.GetAllFriends(current);
            List<UserProfile> friendsProfiles = new List<UserProfile>();
            SearchResults = new List<UserProfile>();
            for (int i = 0; i < myFriends.Count; i++)
            {
                UserProfile friend = await fb.GetUserObject(myFriends[i]);
                friendsProfiles.Add(friend);
            }
            for (int i = 0; i < friendsProfiles.Count; i++)
            {
                if (friendsProfiles[i].Name.ToLower().Contains(SearchStr))
                {
                    bool alreadyHaveConversation = await fb.HaveConversationWith(friendsProfiles[i].Email);
                    if (!alreadyHaveConversation)
                    {
                        friendsProfiles[i].Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(friendsProfiles[i].Name);
                        SearchResults.Add(friendsProfiles[i]);
                    }

                }
            }
            if (SearchResults.Count != 0)
            {
                double height = 40;
                search.HeightRequest = SearchResults.Count * height;
                search.IsVisible = true;
                search.ItemsSource = SearchResults;
                noMatchingSearchResultsLbl.IsVisible = false;
            }
            else if (SearchResults.Count == 0 )
            {
                search.IsVisible = false;
                noMatchingSearchResultsLbl.IsVisible = true;
            }
            else if (searchBar.Text == string.Empty || searchBar.Text == null)
            {
                search.IsVisible = false;
                noMatchingSearchResultsLbl.IsVisible = false;
            }


        }

        public async void OnSearch_Tapped(object sender, ItemTappedEventArgs e)
        {
            var type = e.ItemIndex;
            
            bool choice = await DisplayAlert("Start Chat", "Do you want to start a chat with this user?", "Yes", "No");
            if (choice)
            {
                fb.StartConversationWithFriend(SearchResults[type]);

                //navigate user to chat page
            }
            searchBar.Text= string.Empty;
            SearchResults = new List<UserProfile>();
            search.ItemsSource = SearchResults;
            search.IsVisible = false;
        }

        public async void OnChat_Clicked(object sender, ItemTappedEventArgs e)
        {
            var type = e.ItemIndex;
            Conversation conversation = await fb.GetConversationWith(ConversationsWith[type].Email);
            await Navigation.PushAsync(new ViewChatBetweenFriendsController(conversation));
        }

       
    }
}
