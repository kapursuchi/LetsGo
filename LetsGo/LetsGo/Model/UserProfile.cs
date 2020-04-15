using System;
using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;

namespace LetsGo.Model
{
    //User that will be stored in database
    public class UserProfile
    {
        public string Name { get; set; }

        public string ProfileImage { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; }
        
        public string Location { get; set; }

        public string Password { get; set; }

        public bool PublicAcct { get; set; }

        public List<string> Friends { get; set; }
        public List<string> Interests { get; set; }

        public List<string> FriendRequests { get; set; }

        public List<string> EventRequests { get; set; }

        public List<string> CommunityRequests { get; set; }

        public List<string> CommunityInvites { get; set; }

        

        public UserProfile(string uName, DateTime uDOB, string uEmail, string uPass, bool publicAccount)
        {
            Name = uName;
            DateOfBirth = uDOB;
            Email = uEmail;
            Password = uPass;
            Location = null;
            PublicAcct = publicAccount;
            Interests = new List<string>();
            Friends = new List<string>();
            FriendRequests = new List<string>();
            EventRequests = new List<string>();
            CommunityRequests = new List<string>();
            CommunityInvites = new List<string>();
            ProfileImage = "defaultProfilePic.jpg";
        }

        public UserProfile()
        {

        }


    }
        
}
