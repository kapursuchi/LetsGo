using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetsGo.Model.Authentication;
using Firebase.Auth;
using LetsGo.iOS;
using Foundation;
using LetsGo.Model;
using UIKit;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirebaseAuthenticator))]
namespace LetsGo.iOS
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