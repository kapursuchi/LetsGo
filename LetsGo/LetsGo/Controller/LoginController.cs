﻿using System;
using System.Collections.Generic;
using System.Text;
using LetsGo.Model;

namespace LetsGo.Controller
{
    public partial class LoginController
    {
        public LoginController()
        {
            InitializeComponent();
        }

        private LoginPage loginPage = new LoginPage();
        public async void Login_Clicked(object sender, EventArgs e)
        {
            string emailAddress = email.Text;
            string pass = password.Text;


            string token = await loginPage.LoginUser(emailAddress, pass);
<<<<<<< Updated upstream
            if (token.Contains("There is no user record") || token == "")
=======
            if (token != string.Empty && !token.Contains("There is no user record"))
            {
                await Navigation.PushAsync(new NavigationBarController());
            }
            else
            {
                await DisplayAlert("Authentication Failed", "Email or password are incorrect. Please try again.", "OK");
            }
            /*
            if (token.Contains("There is no user record") || token == null || token == "")
>>>>>>> Stashed changes
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
            */

        }

        public async void CreateAccount_Clicked(object sender, EventArgs e)
        {
            string emailAddress = email.Text;
            string pass = password.Text;

            string token = await loginPage.CreateAccount(emailAddress, pass);
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