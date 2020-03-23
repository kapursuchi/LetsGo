using System;
using System.Collections.Generic;
using System.Text;
using LetsGo.Model;
using LetsGo.Model.Authentication;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class LoginController
    {
        public LoginController()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private LoginPage loginPage = new LoginPage();

        public async void Login_Clicked(object sender, EventArgs e)
        {
            string emailAddress = email.Text;
            string pass = password.Text;

            bool LoggedIn = await loginPage.LoginUserWithEmailPass(emailAddress, pass);
            if (LoggedIn)
            {
                var auth = DependencyService.Get<IFirebaseAuthenticator>();
                auth.SetCurrentUser(emailAddress);
                await Navigation.PushAsync(new NavigationBarController());
            }
            else
            {
                await DisplayAlert("Authentication Failed", "Email or password are incorrect. Please try again.", "OK");
            }

        }

        private async void Navigate_CreateAccount_Page(object sender, EventArgs e)
        {
            //Navigate to the Account Creation Page
            await Navigation.PushAsync(new CreateAccountController());

        }

        private async void Navigate_ForgotPasswordPage(object sender, EventArgs e)
        {
            //Navigate to Forgot Password Page
            await Navigation.PushAsync(new ForgotPasswordController());
        }

    }
}
