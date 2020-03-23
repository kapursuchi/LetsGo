using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetsGo.Controller;
using LetsGo.Model.Authentication;
using Xamarin.Forms;
using LetsGo.Model;

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
            else
            {
                string emailAddress = email.Text;
                string pass = password.Text;
                string userName = name.Text;
                DateTime userdob = dobChosen;

                bool token = await createAcct.CreateUserAccount(emailAddress, pass, userName, userdob, userPublicAcct);
                if (token == true)
                {
                    await DisplayAlert("Success", "User account created.", "OK");
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

    }
}
