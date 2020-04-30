using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetsGo.Controller;
using LetsGo.Model.Authentication;
using System.Linq;
using Xamarin.Forms;
using LetsGo.Model;
using System.Text.RegularExpressions;

namespace LetsGo.Controller
{
    public partial class CreateAccountController
    {
        private bool userPublicAcct;
        DateTime dobChosen;
        public CreateAccountController()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        readonly private CreateAccountPage createAcct = new CreateAccountPage();
        
        
        public async void CreateAccount_Clicked(object sender, EventArgs e)
        {
            if (confirmpassword.Text != password.Text)
            {
                await DisplayAlert("Alert", "Passwords do not match!", "OK");
            }
            else if (PasswordCheck(password.Text) == false)
            {
                await DisplayAlert("Alert", "Password must be at least 8 characters, and contain an uppercase letter, a number, and a special character.", "OK");
            }
            else
            {
                string emailAddress = email.Text;
                string pass = password.Text;
                string userName = name.Text;
                DateTime userdob = dobChosen;

                bool token = await createAcct.CreateUserAccount(emailAddress, pass, userName, userdob, userPublicAcct);
                if (token == true)
                {
                    await DisplayAlert("Success", "User account created. You will be redirected to the login screen now, and may update your profile once logged in.", "OK");
                    await Navigation.PushAsync(new LoginController());
                }
                else
                {
                    await DisplayAlert("Failed to Create Account", "A user with this email address already exists.", "OK");
                }
            }

            

        }

        public async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginController());
        }

        public void On_Toggled(object sender, ToggledEventArgs e)
        {
            userPublicAcct = e.Value;
        }

        public void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            dobChosen = e.NewDate;
        }

        public bool PasswordCheck(string pass)
        {
            bool cleared = false;
            if (pass.Length >= 8)
            {
                if (pass.Any(char.IsUpper))
                {
                    if (pass.Any(char.IsDigit))
                    {
                        if (HasSpecialChar(pass))
                        {
                            cleared = true;
                        }
                    }
                }
            }

            return cleared;
        }

        public static bool HasSpecialChar(string input)
        {
            string specialChar = " !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            foreach (var item in specialChar)
            {
                if (input.Contains(item)) return true;
            }

            return false;
        }
    }
}
