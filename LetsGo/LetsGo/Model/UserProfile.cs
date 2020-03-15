using System;
using System.Collections.Generic;
using System.Text;

namespace LetsGo.Model
{
    //User that will be stored in database
    public class UserProfile
    {
        public string name { get; set; }

        public string dateOfBirth { get; set; }

        public string email { get; set; }

        public string password { get; set; }
        public UserProfile(string uName, string uDOB, string uEmail, string uPass)
        {
            name = uName;
            dateOfBirth = uDOB;
            email = uEmail;
            password = uPass;
        }
    }
        
}
