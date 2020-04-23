using LetsGo.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class CommunityChatController
    {
        private readonly FirebaseDB fb = new FirebaseDB();
        List<CommunityProfile> ConversationsWith { get; set; }

        List<Conversation> conversations { get; set; }
        public CommunityChatController()
        {
            SetValues();
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public async void SetValues()
        {
            ConversationsWith = new List<CommunityProfile>();
            conversations = await fb.GetConversationsWithCommunities();
            if (conversations != null && conversations.Count != 0)
            {
                foreach (Conversation convo in conversations)
                {
                    CommunityProfile community = await fb.GetCommunity(convo.ConversationID);
                    community.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(community.Name);
                    ConversationsWith.Add(community);
                }
                double height = 40;
                chatWithCommunities.HeightRequest = ConversationsWith.Count * height;
                chatWithCommunities.ItemsSource = ConversationsWith;
                activeChats.IsVisible = true;
                noChats.IsVisible = false;
                chatWithCommunities.IsVisible = true;
            }
            else
            {
                chatWithCommunities.IsVisible = false;
                activeChats.IsVisible = false;
                noChats.IsVisible = true;
            }

        }

        public async void OnChat_Clicked(object sender, ItemTappedEventArgs e)
        {
            var type = e.ItemIndex;
            Conversation conversation = await fb.GetConversationWith(ConversationsWith[type]);
            await Navigation.PushAsync(new ViewCommunityChatController(conversation));
        }

        protected override void OnAppearing()
        {
            SetValues();
            base.OnAppearing();
        }
    }
}
