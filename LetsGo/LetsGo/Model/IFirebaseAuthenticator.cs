using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace LetsGo.Model.Authentication
{
    public interface IFirebaseAuthenticator
    {

        void SetCurrentUser(string email);

        string GetCurrentUser();

    }
}
