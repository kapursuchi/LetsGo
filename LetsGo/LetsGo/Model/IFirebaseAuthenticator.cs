using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace LetsGo.Model.Authentication
{
    public interface IFirebaseAuthenticator
    {
        Task<string> LoginWithEmailPassword(string email, string password);

        Task<string> RegisterWithEmailPassword(string email, string password);

        void SendPasswordRecoveryEmail(string email);

        void SignoutUser();

    }
}
