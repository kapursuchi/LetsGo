using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetsGo.Model.Authentication;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LetsGo.Model
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public void logoutUser()
        {
            var auth = DependencyService.Get<IFirebaseAuthenticator>();
            auth.SignoutUser();
        }
    }
}