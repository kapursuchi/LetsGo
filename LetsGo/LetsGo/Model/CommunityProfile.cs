using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LetsGo.Model
{
    public class CommunityProfile
    {
        public string Leader { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public List<string> Interests { get; set; }
        public string Name { get; set; }
        public bool PublicCommunity { get; set; }
        public bool InviteOnly { get; set; }
        public List<string> Members { get; set; }
        public string CommunityImage { get; set; }
        public string CommunityID { get; set; }

        public List<string> CommunityRequests { get; set; }
        
        public CommunityProfile(string eMail, string eDesc, string location, string interestTags, string eName, bool isPublic, bool invOnly, List<string> members, string id)
        {
            Leader = eMail;
            Description = eDesc;
            Location = location;
            Name = eName;
            PublicCommunity = isPublic;
            Interests = new List<string>();
            InviteOnly = invOnly;
            Members = members;
            CommunityID = id;
            CommunityRequests = new List<string>();
            List<string> preInterests = interestTags.Split(',').ToList();
            for (int i = 0; i < preInterests.Count; i++)
            {
                Interests.Add(preInterests.ElementAt(i).ToLower().Trim());
            }
            CommunityImage = "communityimage.jpg";
        }

        public CommunityProfile()
        {
        }
    }
}
