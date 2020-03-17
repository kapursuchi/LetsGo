using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetsGo.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LetsGo.Model.Authentication;
using System.ComponentModel;



namespace LetsGo.Model
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public async Task<string> LoginUser(string email, string pass)
        {
            var auth = DependencyService.Get<IFirebaseAuthenticator>();

            string token = await auth.LoginWithEmailPassword(email, pass);
            return token;
               
        }

       
 
    }
}