using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using LetsGo.Model;

namespace LetsGo.Controller
{
    public partial class ChangePasswordController
    {

        public ChangePasswordController()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private FirebaseDB fb = new FirebaseDB();
        public async void SavePassword_Clicked(object sender, EventArgs e)
        {
            if (newpass.Text == confirmpass.Text)
            {
                bool changed = await fb.ChangePassword(oldpass.Text, newpass.Text);
                if (changed)
                {
                    await DisplayAlert("Success", "Your password has been changed!", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Password Change Failed", "Incorrect password in \"Old Password\" field.", "OK");
                    

                }

            }
            else
            {
                await DisplayAlert("Password Change Failed", "The passwords must match.", "OK");

            }
        }

        public async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
