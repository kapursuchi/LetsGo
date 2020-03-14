using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using LetsGo.Model;

namespace LetsGo
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        FirebaseDatabase firebaseDatabase = new FirebaseDatabase();
        public MainPage()
        {
            InitializeComponent();
        }

        //This is just a test function that adds the user to the database
        private async void BtnAdd_Clicked(object sender, EventArgs e)
        {
            await firebaseDatabase.AddUser(txtName.Text);
            txtName.Text = string.Empty;
            await DisplayAlert("Success", "User Added Successfully", "OK");
        }
    }
}
