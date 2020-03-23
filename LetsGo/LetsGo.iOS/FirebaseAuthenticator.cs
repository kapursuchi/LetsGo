using LetsGo.Model.Authentication;
using LetsGo.iOS;
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