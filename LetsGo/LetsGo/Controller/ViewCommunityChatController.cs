using LetsGo.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Text;
using System.Globalization;

namespace LetsGo.Controller
{

    public partial class ViewCommunityChatController
    {
        private readonly FirebaseDB fb = new FirebaseDB();
        private Conversation conversation { get; set; }
        public string ChatWith { get; set; }

        public List<string> recipient { get; set; }

        public ObservableCollection<object> messageViews { get; set; }

        public ObservableCollection<object> chats { get; set; }

        public CommunityProfile community { get; set; }
        public ViewCommunityChatController(Conversation c)
        {
            conversation = c;
            SetValues();
            InitializeComponent();
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#51aec2");
            Messages.BindingContext = this;
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarTextColor = Color.White;
        }

        public async void SetValues()
        {
            community = await fb.GetCommunity(conversation.ConversationID);
            string current = fb.GetCurrentUser();
            SetTitle();
            //sendMessage.IsEnabled = false;
            List<ChatMessage> msgs = await fb.GetMessagesFromCommunityConversation(conversation.ConversationID);
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
            this.Title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(community.Name);
        }

        public void OnEditorChanged(object sender, TextChangedEventArgs e)
        {
            if (messageToSend.Text != string.Empty || messageToSend.Text != null)
                sendMessage.BackgroundColor = Color.FromHex("#bee3db");
            else
                sendMessage.BackgroundColor = Color.LightGray;
        }

        public async void OnSend_Clicked(object sender, EventArgs e)
        {
            string current = fb.GetCurrentUser();
            UserProfile currentUser = await fb.GetUserObject(current);
            currentUser.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(currentUser.Name);
            if (messageToSend.Text != string.Empty || messageToSend.Text != null)
            {
                string message = messageToSend.Text;
                ChatMessage sentMessage = await fb.SendMessageToCommunity(conversation.ConversationID, community, message);
                chats.Add(sentMessage);
                messageViews.Add(new MessageView(currentUser.Name, true, sentMessage.Message, sentMessage.TimeSent));
                //Messages.HeightRequest = chats.Count * (double)40;
                messageToSend.Text = string.Empty;
                sendMessage.BackgroundColor = Color.LightGray;
            }

        }

    }

    public class GroupMessageView
    {
        public string Sender { get; set; }
        public bool IsSender { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public Color MsgColor { get; set; }

        public string SetWidth { get; set; }

        public LayoutOptions SetLayout { get; set; }

        public Image MessageImage { get; set; }

        public Color MessageTextColor { get; set; }

        public GroupMessageView(string sender, bool currentIsSender, string message, DateTime timeSent)
        {
            Sender = sender;
            IsSender = currentIsSender;
            Message = message;
            Time = timeSent;
            if (IsSender)
            {
                MsgColor = Color.FromHex("#63c6db");
                SetLayout = LayoutOptions.End;
                MessageTextColor = Color.FromHex("#043240");
                //MessageImage.Source = ImageSource.FromFile("");
                if (message.Length < 10 && (message.Length + 4 <= 10))
                {
                    double width = (double)((int)message.Length + 4);
                    SetWidth = width.ToString();
                }
                    
                else
                    SetWidth = "20"; //idk if this actually works, but the view seems fine
            }
            else
            {
                MessageTextColor = Color.Black;
                MsgColor = Color.FromHex("#dce0e0");
                SetLayout = LayoutOptions.Start;
            }
        }
    }
}
