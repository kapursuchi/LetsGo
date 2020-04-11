namespace LetsGo.Model.Authentication
{
    public interface IFirebaseAuthenticator
    {

        void SetCurrentUser(string email);

        string GetCurrentUser();

        void SetCurrentCommunity(CommunityProfile comm);

        CommunityProfile GetCurrentCommunity();

        void SetCurrentEvent(EventProfile evt);

        EventProfile GetCurrentEvent();


    }
}
