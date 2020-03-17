using System;
using System.Collections.Generic;
using System.Text;
using LetsGo.Model;
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


            string token = await loginPage.LoginUser(emailAddress, pass);
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

        private async void Navigate_CreateAccount_Page(object sender, EventArgs e)
        {
            //Navigate to the Account Creation Page
            await Navigation.PushAsync(new CreateAccountController());

        }

    }
}
