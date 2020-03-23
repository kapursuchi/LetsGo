using System;
using System.Collections.Generic;
using System.Text;
using LetsGo.Model;
using LetsGo.Model.Authentication;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class ForgotPasswordController
    {
        public ForgotPasswordController()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private ForgotPasswordPage ForgotPassword = new ForgotPasswordPage();
        public async void Forgot_Password_Clicked(object sender, EventArgs e)
        {
            string emailAddress = email.Text;

            bool sent = await ForgotPassword.emailPasswordRecovery(emailAddress);
            if (sent)
                await Navigation.PushAsync(new LoginController());
            

        }

        public async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginController());
        }
    }
}
