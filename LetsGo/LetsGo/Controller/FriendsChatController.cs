using LetsGo.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class FriendsChatController
    {
        private readonly FirebaseDB fb = new FirebaseDB();
        public string SearchStr { get; set; }

        public string current { get; set; }
        public List<UserProfile> SearchResults { get; set; }
        public ObservableCollection<object> ConversationsWith { get; set; }
        public FriendsChatController()
        {
            SetValues();
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public async void SetValues()
        {
            current = fb.GetCurrentUser();
            string email = "";
            ConversationsWith = new ObservableCollection<object>();
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
                    
                    if (convo.Messages != null)
                    {
                        ConversationsWith.Add(new ViewConversations(userprofile, userprofile.Name, current, convo.Messages[convo.Messages.Count - 1].IsRead));
                    }
                    else
                    {
                        ConversationsWith.Add(new ViewConversations(userprofile, userprofile.Name, current, true));
                    }
                        
                    
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
                SearchResults[type].Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(SearchResults[type].Name);
                
                ConversationsWith.Add(new ViewConversations(SearchResults[type], SearchResults[type].Name, current, true));
                //navigate user to chat page
            }
            searchBar.Text= string.Empty;
            SearchResults = new List<UserProfile>();
            search.ItemsSource = SearchResults;
            search.IsVisible = false;
            noChats.IsVisible = false;
            chatWithFriends.IsVisible = true;
            chatWithFriends.HeightRequest = ConversationsWith.Count * (double)40.0;
        }

        public async void OnChat_Clicked(object sender, ItemTappedEventArgs e)
        {
            var type = e.ItemIndex;
            ViewConversations user = (ViewConversations)ConversationsWith[type];
            Conversation conversation = await fb.GetConversationWith(user.sender.Email);
            fb.ReadMessages("FriendsMessages", conversation.ConversationID);
            await Navigation.PushAsync(new ViewChatBetweenFriendsController(conversation));
        }

        protected override void OnAppearing()
        {
            SetValues();
            base.OnAppearing();
        }


    }
    public class ViewConversations 
    {
        private readonly FirebaseDB fb = new FirebaseDB();


        public ImageSource UnReadMessage { get; set; }
        public string Recipient { get; set; }

        public UserProfile sender { get; set; }

        public ViewConversations(UserProfile recipient, string name, string current, bool read)
        {
            Recipient = name;
            sender = recipient;

            if (!read && recipient.Email != current)
            {
                UnReadMessage = ImageSource.FromFile("newmessage.png");

            }
            else
            {
                if (recipient.ProfileImage != "defaultProfilePic.jpg")
                    UnReadMessage = ImageSource.FromUri(new Uri(recipient.ProfileImage));
                else
                    UnReadMessage = ImageSource.FromFile("defaultProfilePic.jpg");
            }
            
        }




    }
}
