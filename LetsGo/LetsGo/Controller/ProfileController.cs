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

        private ProfilePage profile = new ProfilePage();
        private FirebaseDB fb = new FirebaseDB();
        public async void Logout_Clicked(object sender, EventArgs e)
        {
            fb.SignOutUser();
            await Navigation.PushAsync(new LoginController());
        }

        public async void UpdateProfile_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UpdateProfileController());
        }
    }
}
