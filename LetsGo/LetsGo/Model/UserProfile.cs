using System;
using System.Collections.Generic;
using System.Text;

namespace LetsGo.Model
{
    //User that will be stored in database
    public class UserProfile
    {
        public string name { get; set; }

        public DateTime dateOfBirth { get; set; }

        public string email { get; set; }

        //public string password { get; set; }

        public bool publicAcct;

        public List<string> interests;

        public UserProfile(string uName, DateTime uDOB, string uEmail, bool publicAccount)
        {
            name = uName;
            dateOfBirth = uDOB;
            email = uEmail;
            publicAcct = publicAccount;
            interests = new List<string>();
        }
    }
        
}
