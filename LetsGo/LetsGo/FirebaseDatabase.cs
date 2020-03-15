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
    public class FirebaseDatabase
    {
        FirebaseClient firebase = new FirebaseClient("https://letsgo-f4d0d.firebaseio.com/");

        //method that gets called once the user presses the "add" button
        //adds the user's name they've provided to the database
        public async Task AddUser(string name, string pass)
        {
            await firebase
              .Child("Users")
              .PostAsync(new User() { Name = name, Password = pass });

        }

    }
}
