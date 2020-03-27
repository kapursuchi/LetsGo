using System;
using System.Collections.Generic;
using System.Text;

namespace LetsGo.Model
{
    public class CommunityProfile
    {
        public bool PublicCommunity { get; set; }
        public List<string> Interests { get; set; }
        public string Identity { get; set; }
        public string Name { get; set; }

        public string CommunityLeader { get; set; }
        public CommunityProfile()
        {

        }
    }
}
