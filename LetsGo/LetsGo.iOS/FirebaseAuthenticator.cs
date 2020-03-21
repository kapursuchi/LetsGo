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
        public async Task<string> LoginWithEmailPassword(string email, string password)
        {
            try
            {
                var user = await Auth.DefaultInstance.SignInWithPasswordAsync(email, password);
                return await user.User.GetIdTokenAsync();
            }
            catch (Exception err)
            {
                return "";
            }

        }

       
        public async Task<string> RegisterWithEmailPassword(string email, string password)
        {
            try
            {
                var user =  await Auth.DefaultInstance.CreateUserAsync(email, password);
                return user.ToString();
            }
            catch (Exception err)
            {
                return "";
            }
        }

        public void SendPasswordRecoveryEmail(string email)
        {
            Auth.DefaultInstance.SendPasswordResetAsync(email);
        }

        public void SignoutUser()
        {
            Auth.DefaultInstance.SignOut(out NSError error);
        }
    }
}