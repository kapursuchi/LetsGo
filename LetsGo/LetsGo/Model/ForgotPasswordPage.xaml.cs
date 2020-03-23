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
        private FirebaseDB fb = new FirebaseDB();
        public async Task<bool> emailPasswordRecovery(string email)
        {
            bool sent = await fb.SendPasswordRecoverEmail(email);
            return sent;
            
        }
    }
}