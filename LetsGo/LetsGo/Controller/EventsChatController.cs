using LetsGo.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class EventsChatController
    {
        private readonly FirebaseDB fb = new FirebaseDB();
        List<EventProfile> ConversationsWith { get; set; }

        List<Conversation> conversations { get; set; }

        public EventsChatController()
        {
            SetValues();
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public async void SetValues()
        {
            ConversationsWith = new List<EventProfile>();
            conversations = await fb.GetConversationsWithEvents();
            if (conversations != null && conversations.Count != 0)
            {
                foreach (Conversation convo in conversations)
                {
                    EventProfile eventProfile = await fb.GetEvent(convo.ConversationID);
                    eventProfile.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(eventProfile.Name);
                    ConversationsWith.Add(eventProfile);
                }
                double height = 40;
                chatWithEvents.HeightRequest = ConversationsWith.Count * height;
                chatWithEvents.ItemsSource = ConversationsWith;
                activeChats.IsVisible = true;
                noChats.IsVisible = false;
                chatWithEvents.IsVisible = true;
            }
            else
            {
                chatWithEvents.IsVisible = false;
                activeChats.IsVisible = false;
                noChats.IsVisible = true;
            }

        }

        public async void OnChat_Clicked(object sender, ItemTappedEventArgs e)
        {
            var type = e.ItemIndex;
            Conversation conversation = await fb.GetConversationWith(ConversationsWith[type]);
            await Navigation.PushAsync(new ViewEventChatController(conversation));
        }

        protected override void OnAppearing()
        {
            SetValues();
            base.OnAppearing();
        }
    }
}
