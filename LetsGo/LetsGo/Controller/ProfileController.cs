using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Xamarin.Forms;
using LetsGo.Model;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace LetsGo.Controller
{
    public partial class ProfileController : ContentPage
    {
        public ProfileController()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            SetValues();
            name.BindingContext = this;
            location.BindingContext = this;
            interests.BindingContext = this;
        }

        private ProfilePage profile = new ProfilePage();
        private FirebaseDB fb = new FirebaseDB();


        private string _name { get; set; }
        public string Name
        {
            get
            {
                return _name;
            }
        }

        private string _location { get; set; }

        public string Location
        {
            get
            {
                return _location;
            }
        }

        private string _interests { get; set; }

        public string Interests
        {
            get
            {
                return _interests;
            }
        }
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        private async void SetValues()
        {
            _name = await fb.GetUsersName();
            _name = textInfo.ToTitleCase(_name);
            _location = await fb.GetUsersLocation();
            if (_location != null)
                _location = textInfo.ToTitleCase(_location);
            else
            {
                _location = "No Location Yet...";
            }
            List<string> interestList = await fb.GetUsersInterests();
            if (interestList != null)
            {
                for (int i = 0; i < interestList.Count; i++)
                {
                    _interests += textInfo.ToTitleCase(interestList.ElementAt(i)) + ", ";
                }
                _interests = _interests.Substring(0, Interests.Length - 2);
            }
            else
            {
                _interests = "No Interests Yet...";

            }
        }

        public async void Logout_Clicked(object sender, EventArgs e)
        {
            bool done = profile.LogoutUser();
            if (done)
                await Navigation.PushAsync(new LoginController());

        }

        public async void UpdateProfile_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UpdateProfileController());
        }
    }
}
