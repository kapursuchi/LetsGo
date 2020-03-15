using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetsGo.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LetsGo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class CreateAccount : ContentPage
    {
        
        readonly FirebaseDB firebaseDB = new FirebaseDB();
        public CreateAccount()
        {
            InitializeComponent();
        }

        private async void createAccount_Clicked(object sender, EventArgs e)
        {
            //Create user and add to database
            //await firebaseDB.CreateUser(email.Text, password.Text, dob.Text, name.Text);

            //Clear all fields
            email.Text = string.Empty;
            password.Text = string.Empty;
            dob.Text = string.Empty;
            name.Text = string.Empty;

            //Success Message
            //await DisplayAlert("Success", "You have successfully created an account!", "OK");

        }
    }
}