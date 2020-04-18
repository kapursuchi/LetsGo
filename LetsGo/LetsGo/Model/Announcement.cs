using System;
using System.Collections.Generic;
using System.Text;

namespace LetsGo.Model
{
    public class Announcement
    {
        public string AnnouncementID { get; set; }
        public string CommunityID { get; set; }

        public string Description { get; set; }
        public Announcement(string communityID, string  description)
        {
            AnnouncementID = Guid.NewGuid().ToString();
            CommunityID = communityID;
            Description = description;

        }

        public Announcement()
        {

        }
    }
}
