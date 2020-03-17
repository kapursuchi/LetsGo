using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetsGo.Model;
using LetsGo.Model.Authentication;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LetsGo.Model
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class CreateAccountPage : ContentPage
    {
        FirebaseDB fb = new FirebaseDB();
        public async Task<string> CreateUserAccount(string email, string pass, string name, DateTime dob, bool publicAcct)
        {
            var auth = DependencyService.Get<IFirebaseAuthenticator>();

            string token = await auth.RegisterWithEmailPassword(email, pass);
            await fb.InitializeUser(name, dob, email, publicAcct);
            return token;
        
        }
    }
}