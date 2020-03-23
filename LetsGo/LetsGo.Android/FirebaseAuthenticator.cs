using Firebase.Auth;
using LetsGo.Model.Authentication;
using Xamarin.Forms;
using System.Threading.Tasks;
using System;
using LetsGo.Droid;
using Firebase;

[assembly: Dependency(typeof(FirebaseAuthenticator))]
namespace LetsGo.Droid
{
    public class FirebaseAuthenticator : IFirebaseAuthenticator
    {
        private string CurrentUser;
   
        public void SetCurrentUser(string email)
        {
            CurrentUser = email;
        }

        public string GetCurrentUser()
        {
            return CurrentUser;
        }


    }
}