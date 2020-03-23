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
    //[DesignTimeVisible(true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private FirebaseDB fb = new FirebaseDB();
        public async Task<bool> LoginUserWithEmailPass(string email, string pass)
        {
            bool token = await fb.LoginUser(email, pass);
            return token;
               
        }

       
 
    }
}