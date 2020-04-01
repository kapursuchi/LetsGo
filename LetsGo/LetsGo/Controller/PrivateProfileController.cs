using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using LetsGo.Model;

namespace LetsGo.Controller
{
   public partial  class PrivateProfileController
    {
        readonly FirebaseDB fb = new FirebaseDB();
        private string _name { get; set; }
        private UserProfile profile { get; set; }

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
        public PrivateProfileController(UserProfile user)
        {
            profile = user;
            SetValues(user);
            InitializeComponent();
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.LightSteelBlue;
            name.BindingContext = this;
        }

        public async void SetValues(UserProfile user)
        {
            Name = user.Name;

        }

        public async void Request_Clicked(object sender, EventArgs e)
        {
            fb.AddFriend(profile.Email);
            await DisplayAlert("Request Sent", "You have requested this user to be your friend. If this user accepts your request, you will be able to view their profile.", "OK");
        }

        
    }
}
