using LetsGo.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class ViewChatBetweenFriendsController
    {
        private readonly FirebaseDB fb = new FirebaseDB();
        private Conversation conversation { get; set; }
        public string ChatWith { get; set; }

        public Color SendReceiveColor { get; set; }

        public bool SenderOrReceiver { get; set; }

        public string recipient { get; set; }

        public ObservableCollection<object> messageViews { get; set; }

        public ObservableCollection<object> chats { get; set; }
        public ViewChatBetweenFriendsController(Conversation c)
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
            string current = fb.GetCurrentUser();
            SetTitle();
            //sendMessage.IsEnabled = false;
            List<ChatMessage> msgs = await fb.GetMessagesFromFriendsConversation(conversation.ConversationID);
            chats = new ObservableCollection<object>(msgs);

            if (chats != null)
            {
                messageViews = new ObservableCollection<object>();
                foreach (ChatMessage chat in chats)
                {
                    bool yes = (chat.Sender == current) ? true : false;
                    messageViews.Add(new MessageView(chat.Sender, yes, chat.Message, chat.TimeSent));
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

        public async void SetTitle()
        {
            string current = fb.GetCurrentUser();
            for (int i = 0; i < conversation.ConversationBetween.Count; i++)
            {
                if (conversation.ConversationBetween[i] != current)
                {
                    UserProfile chattingWith = await fb.GetUserObject(conversation.ConversationBetween[i]);
                    string Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(chattingWith.Name);
                    ChatWith = Name;
                    recipient = chattingWith.Email;
                    this.Title = ChatWith;
                }
            }
        }

        public void OnEditorChanged(object sender, TextChangedEventArgs e)
        {
            if (messageToSend.Text != string.Empty || messageToSend.Text != null)
                sendMessage.BackgroundColor = Color.LightSteelBlue;
            else
                sendMessage.BackgroundColor = Color.Gray;
        }

        public async void OnSend_Clicked(object sender, EventArgs e)
        {
            string current = fb.GetCurrentUser();
            if (messageToSend.Text != string.Empty || messageToSend.Text != null)
            {
                string message = messageToSend.Text;
                ChatMessage sentMessage = await fb.SendMessageToFriend(conversation.ConversationID, recipient, message);
                chats.Add(sentMessage);
                messageViews.Add(new MessageView(current, true, sentMessage.Message, sentMessage.TimeSent));
                //Messages.HeightRequest = chats.Count * (double)40;
                messageToSend.Text = string.Empty;
                sendMessage.BackgroundColor = Color.Gray;
            }

        }
    }

    public class MessageView
    {
        public string Sender { get; set; }
        public bool IsSender { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public Color MsgColor { get; set; }

        public double SetWidth { get; set; }

        public LayoutOptions SetLayout { get; set; }
        public MessageView(string sender, bool currentIsSender, string message, DateTime timeSent)
        {
            Sender = sender;
            IsSender = currentIsSender;
            Message = message;
            Time = timeSent;
            if (IsSender)
            {
                MsgColor = Color.FromHex("#c7dcff");
                SetLayout = LayoutOptions.End;
                if (message.Length <= 12)
                    SetWidth = (double)((int)message.Length + 4);
                else
                    SetWidth = (double)15.00; //idk if this actually works, but the view seems fine
            }
            else
            {
                MsgColor = Color.FromHex("#e9ebf0");
                SetLayout = LayoutOptions.Start;
            }
            
        }
    }
}
