using System;
using System.Collections.Generic;
using System.Text;

namespace LetsGo.Model
{
    public class ChatMessage
    {
        public string Sender { get; set; }

        public List<string> Recipients { get; set; }

        public bool IsRead { get; set; }

        public string Message { get; set; }

        public DateTime TimeSent { get; set; }
        public ChatMessage(string sender, List<string> recipients, string message, DateTime dateTimeSent)
        {
            Sender = sender;
            Recipients = recipients;
            Message = message;
            TimeSent = dateTimeSent;
            IsRead = false;
        }
    }
}
