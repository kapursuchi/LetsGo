using Firebase.Auth;
using LetsGo.Model.Authentication;
using Xamarin.Forms;
using System.Threading.Tasks;
using System;
using LetsGo.Droid;

[assembly: Dependency(typeof(FirebaseAuthenticator))]
namespace LetsGo.Droid
{
    public class FirebaseAuthenticator : IFirebaseAuthenticator
    {
        public async Task<string> LoginWithEmailPassword(string email, string password)
        {
            try
            {
                var user = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
                var token = await user.User.GetIdTokenAsync(false);
                return token.Token;
            }
            catch (FirebaseAuthInvalidUserException notFound)
            {
                return notFound.Message;
            }
            catch(Exception err)
            {
                return "";
            }

        }

        public async Task<string> RegisterWithEmailPassword(string email, string password)
        {
            try
            {
                var user = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
                var token = await user.User.GetIdTokenAsync(false);
                return token.Token;
            }
            catch (Exception err)
            {
                return "";
            }
        }


        public async void SendPasswordRecoveryEmail(string email)
        {
            await FirebaseAuth.Instance.SendPasswordResetEmailAsync(email);
          
        }

        public void SignoutUser()
        {
            FirebaseAuth.Instance.SignOut();
        }

    }
}