using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetsGo.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LetsGo.Authentication;
using System.ComponentModel;

namespace LetsGo
{
    [DesignTimeVisible(true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            string eMail = email.Text;
            string pass = password.Text;

            var auth = DependencyService.Get<IFirebaseAuthenticator>();

            string token = await auth.LoginWithEmailPassword(eMail, pass);
            if (token.Contains("There is no user record") || token == "")
            {
                if (token != "")
                    await DisplayAlert("Login Unsuccessful", token, "OK");
                else
                    await DisplayAlert("Login Unsuccessful", "Invalid login credentials", "OK");

            }
            else
            {
                await DisplayAlert("Login Successful", "User has logged in", "OK");
            }

               
        }

        private async void CreateAccount_Clicked(object sender, EventArgs e)
        {
            string eMail = email.Text;
            string pass = password.Text;

            var auth = DependencyService.Get<IFirebaseAuthenticator>();

            string token = await auth.RegisterWithEmailPassword(eMail, pass);
            if (token != "")
            {
                await DisplayAlert("Success", "User account created.", "OK");
            }
            else
            {
                await DisplayAlert("Failure", "User account could not be created", "OK");
            }
            
        }
    }
}