using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Text;
using Firebase.Database;
using Firebase.Database.Query;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LetsGo.Model;

namespace LetsGo.Model
{
    public class FirebaseDB
    {
        FirebaseClient firebase = new FirebaseClient("https://letsgo-f4d0d.firebaseio.com/");


        //method that gets called once the user presses the "add" button
        //adds the user's name they've provided to the database
        public async Task InitializeUser(string uName, DateTime uDOB, string uEmail, bool publicAcct)
        {
            UserProfile newUser = new UserProfile(uName, uDOB, uEmail, publicAcct);
            await firebase
              .Child("UserProfiles")
              .PostAsync(newUser);

        }

    }
}
