using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace LetsGo.Model
{
    //User that will be stored in database
    public class UserProfile
    {
        public string Name { get; set; }

        public ImageSource ProfilePicture { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; }
        
        public string Location { get; set; }

        public string Password { get; set; }

        public bool PublicAcct { get; set; }

        public List<string> Friends { get; set; }
        public List<string> Interests { get; set; }

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
            //ProfilePicture = ImageSource.FromFile("defaultProfilePic.jpg");
        }

        public UserProfile()
        {

        }


    }
        
}
