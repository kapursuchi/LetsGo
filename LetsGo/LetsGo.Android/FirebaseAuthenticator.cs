using LetsGo.Model.Authentication;
using Xamarin.Forms;
using LetsGo.Droid;
using LetsGo.Model;

[assembly: Dependency(typeof(FirebaseAuthenticator))]
namespace LetsGo.Droid
{
    public class FirebaseAuthenticator : IFirebaseAuthenticator
    {
        private string CurrentUser;
        private CommunityProfile CurrentCommunity;
        private EventProfile CurrentEvent;
   
        public void SetCurrentUser(string email)
        {
            CurrentUser = email.ToLower();
        }

        public string GetCurrentUser()
        {
            return CurrentUser;
        }

        public void SetCurrentCommunity(CommunityProfile comm)
        {
            CurrentCommunity = comm;
        }

        public CommunityProfile GetCurrentCommunity()
        {
            return CurrentCommunity;
        }

        public void SetCurrentEvent(EventProfile evt)
        {
            CurrentEvent = evt;
        }

        public EventProfile GetCurrentEvent()
        {
            return CurrentEvent;
        }
    }
}