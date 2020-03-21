using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Xamarin.Forms;
using LetsGo.Model;

namespace LetsGo.Controller
{
    public partial class ProfileController
    {
        public ProfileController()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public ProfilePage profile = new ProfilePage();
        public async void Logout_Clicked(object sender, EventArgs e)
        {
            profile.logoutUser();
            await Navigation.PushAsync(new LoginController());
        }
    }
}
