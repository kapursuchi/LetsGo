using LetsGo.Model.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LetsGo.Model
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForgotPasswordPage : ContentPage
    {
        public  void emailPasswordRecovery(string email)
        {
            var auth = DependencyService.Get<IFirebaseAuthenticator>();

            auth.SendPasswordRecoveryEmail(email);
            
        }
    }
}