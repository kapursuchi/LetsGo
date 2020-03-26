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
    public partial class ProfileController : ContentPage, INotifyPropertyChanged
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
        private string _location { get; set; }
        private string _interests { get; set; }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
                OnPropertyChanged(nameof(Location));
            }
        }

        

        public string Interests
        {
            get
            {
                return _interests;
            }
            set
            {
                _interests = value;
                OnPropertyChanged(nameof(Interests));
            }
        }
        
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        private async void SetValues()
        {
            Name = await fb.GetUsersName();
            Name = textInfo.ToTitleCase(Name);
            Location = await fb.GetUsersLocation();
            if (Location != null)
                Location = textInfo.ToTitleCase(Location);
            else
            {
                Location = "No Location Yet...";
            }
            List<string> interestList = await fb.GetUsersInterests();
            if (interestList != null)
            {
                for (int i = 0; i < interestList.Count; i++)
                {
                    Interests += textInfo.ToTitleCase(interestList.ElementAt(i)) + ", ";
                }
                Interests = Interests.Substring(0, Interests.Length - 2);
            }
            else
            {
                Interests = "No Interests Yet...";

            }
        }

        public async void Logout_Clicked(object sender, EventArgs e)
        {
            bool done = profile.LogoutUser();
            if (done)
                await Navigation.PushAsync(new LoginController());

        }

        public async void ChangePass_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ChangePasswordController());
        }

        public async void UpdateProfile_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UpdateProfileController());
        }
    }
}
