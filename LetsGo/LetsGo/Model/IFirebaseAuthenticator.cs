namespace LetsGo.Model.Authentication
{
    public interface IFirebaseAuthenticator
    {

        void SetCurrentUser(string email);

        string GetCurrentUser();

    }
}
