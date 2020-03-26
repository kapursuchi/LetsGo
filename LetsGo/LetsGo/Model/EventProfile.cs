using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LetsGo.Model
{
    public class EventProfile
    {
        public string Name { get; set; }

        //public ImageSource ProfilePicture { get; set; }
        public DateTime DateOfEvent { get; set; }

        public string StartOfEvent { get; set; }

        public string EndOfEvent { get; set; }

        public string Description { get; set; }

        public string EventOwner { get; set; }

        public string Location { get; set; }

        public bool PublicEvent { get; set; }

        public List<string> Interests { get; set; }

        public EventProfile(string eName, string eDetails, DateTime eDate, string eStart, string eEnd, string location, string eMail, string interestTags, bool publicAccount)
        {
            Name = eName;
            Description = eDetails;
            DateOfEvent = eDate;
            StartOfEvent = eStart;
            EndOfEvent = eEnd;
            EventOwner = eMail;
            PublicEvent = publicAccount;
            Location = location;
            Interests = new List<string>();
            List<string> preInterests = interestTags.Split(',').ToList();
            for (int i = 0; i < preInterests.Count; i++)
            {
                Interests.Add(preInterests.ElementAt(i).ToLower().Trim());
            }
            //ProfilePicture = ImageSource.FromFile("defaultProfilePic.jpg");
        }

        public EventProfile()
        {

        }
    }
}
