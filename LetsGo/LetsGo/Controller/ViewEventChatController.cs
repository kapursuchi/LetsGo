using LetsGo.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class ViewEventChatController
    {

        private readonly FirebaseDB fb = new FirebaseDB();
        private Conversation conversation { get; set; }
        public string ChatWith { get; set; }

        public List<string> recipient { get; set; }

        public ObservableCollection<object> messageViews { get; set; }

        public ObservableCollection<object> chats { get; set; }

        public EventProfile eventProfile { get; set; }

        public ViewEventChatController(Conversation c)
        {
            conversation = c;
            SetValues();
            InitializeComponent();
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#80b3d1");
            Messages.BindingContext = this;
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarTextColor = Color.White;
        }

        public async void SetValues()
        {
            eventProfile = await fb.GetEvent(conversation.ConversationID);
            string current = fb.GetCurrentUser();
            SetTitle();
            //sendMessage.IsEnabled = false;
            List<ChatMessage> msgs = await fb.GetMessagesFromEventConversation(conversation.ConversationID);
            chats = new ObservableCollection<object>(msgs);

            if (chats != null)
            {
                messageViews = new ObservableCollection<object>();
                foreach (ChatMessage chat in chats)
                {
                    bool yes = (chat.Sender == current) ? true : false;
                    UserProfile sender = await fb.GetUserObject(chat.Sender);
                    sender.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sender.Name);
                    messageViews.Add(new MessageView(sender.Name, yes, chat.Message, chat.TimeSent));
                }
                Messages.ItemsSource = messageViews;
                //Messages.HeightRequest = chats.Count * (double)45;
                Messages.IsVisible = true;
            }
            else
            {
                Messages.IsVisible = false;
            }

        }

        public void SetTitle()
        {
            this.Title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(eventProfile.Name);
        }

        public void OnEditorChanged(object sender, TextChangedEventArgs e)
        {
            if (messageToSend.Text != string.Empty || messageToSend.Text != null)
                sendMessage.BackgroundColor = Color.FromHex("#80b3d1");
            else
                sendMessage.BackgroundColor = Color.Gray;
        }

        public async void OnSend_Clicked(object sender, EventArgs e)
        {
            string current = fb.GetCurrentUser();
            UserProfile currentUser = await fb.GetUserObject(current);
            currentUser.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(currentUser.Name);
            if (messageToSend.Text != string.Empty || messageToSend.Text != null)
            {
                string message = messageToSend.Text;
                ChatMessage sentMessage = await fb.SendMessageToEvent(conversation.ConversationID, eventProfile, message);
                chats.Add(sentMessage);
                messageViews.Add(new MessageView(currentUser.Name, true, sentMessage.Message, sentMessage.TimeSent));
                //Messages.HeightRequest = chats.Count * (double)40;
                messageToSend.Text = string.Empty;
                sendMessage.BackgroundColor = Color.Gray;
            }

        }

    }
}
