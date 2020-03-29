using LetsGo.Model.Authentication;
using Xamarin.Forms;
using LetsGo.Droid;

[assembly: Dependency(typeof(FirebaseAuthenticator))]
namespace LetsGo.Droid
{
    public class FirebaseAuthenticator : IFirebaseAuthenticator
    {
        private string CurrentUser;
   
        public void SetCurrentUser(string email)
        {
            CurrentUser = email.ToLower();
        }

        public string GetCurrentUser()
        {
            return CurrentUser;
        }


    }
}