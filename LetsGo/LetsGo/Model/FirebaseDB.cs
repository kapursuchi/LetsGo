using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Text;
using Firebase.Database;
using Firebase.Database.Query;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LetsGo.Model
{
    public class FirebaseDB
    {
        FirebaseClient firebase = new FirebaseClient("https://letsgo-f4d0d.firebaseio.com/");

        //method that gets called once the user presses the "add" button
        //adds the user's name they've provided to the database
        public async Task CreateUser(string email, string password, string dob, string name)
        {
            await firebase
              .Child("Users")
              .PostAsync(new UserProfile(name, dob, email, password));
        }

        public async Task AddUser(string email, string password)
        {
            await firebase
                .Child("Users")
                .PostAsync(new UserProfile("no name", "no dob", email, password));
        }

    }
}
