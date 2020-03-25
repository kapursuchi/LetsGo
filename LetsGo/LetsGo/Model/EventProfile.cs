using System;
using System.Collections.Generic;
using System.Text;

namespace LetsGo.Model
{
    public class EventProfile
    {
        public string Name { get; set; }

        //public ImageSource ProfilePicture { get; set; }
        public DateTime DateOfEvent { get; set; }

        public TimeSpan StartOfEvent { get; set; }

        public TimeSpan EndOfEvent { get; set; }

        public string Detail { get; set; }

        public string Email { get; set; }

        public string Location { get; set; }

        public bool PublicEvent { get; set; }

        //public List<string> Interests { get; set; }

        public EventProfile(string eName, string eDetails, DateTime eDate, TimeSpan eStart, TimeSpan eEnd, string eMail, bool publicAccount)
        {
            Name = eName;
            Detail = eDetails;
            DateOfEvent = eDate;
            StartOfEvent = eStart;
            EndOfEvent = eEnd;
            Email = eMail;
            PublicEvent = publicAccount;
            //ProfilePicture = ImageSource.FromFile("defaultProfilePic.jpg");
        }

        public EventProfile()
        {

        }
    }
}
