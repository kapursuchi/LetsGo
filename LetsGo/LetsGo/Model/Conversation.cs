using System;
using System.Collections.Generic;
using System.Text;

namespace LetsGo.Model
{
    public class Conversation
    {
        public List<string> ConversationBetween { get; set; }

        public string ConversationID { get; set; }

        public List<ChatMessage> Messages { get; set; }
        public Conversation(CommunityProfile community)
        {
            List<string> usersInConversation = community.Members;
            ConversationID = community.CommunityID;
            ConversationBetween = usersInConversation;
            Messages = new List<ChatMessage>();
        }

        public Conversation(EventProfile eventProfile)
        {
            List<string> usersInConversation = eventProfile.Members;
            ConversationID = eventProfile.EventID;
            ConversationBetween = usersInConversation;
            Messages = new List<ChatMessage>();
        }

        public Conversation(string currentUser, UserProfile userToChatWith)
        {
            List<string> usersInConversation = new List<string>();
            usersInConversation.Add(currentUser);
            usersInConversation.Add(userToChatWith.Email);
            ConversationID = Guid.NewGuid().ToString();
            ConversationBetween = usersInConversation;
            Messages = new List<ChatMessage>();
        }

        public Conversation()
        {

        }
    }
}
