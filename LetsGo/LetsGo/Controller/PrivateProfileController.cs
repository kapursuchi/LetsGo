using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using LetsGo.Model;
using System.Globalization;

namespace LetsGo.Controller
{
   public partial  class PrivateProfileController
    {
        readonly FirebaseDB fb = new FirebaseDB();
        private string _name { get; set; }
        private UserProfile profile { get; set; }
        private Image _img { get; set; }
        public Image ProfilePicture
        {
            get
            {
                return _img;
            }
            set
            {
                _img = value;
                OnPropertyChanged(nameof(ProfilePicture));
            }
        }

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
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#51aec2");
            name.BindingContext = this;
        }

        public async void SetValues(UserProfile user)
        {
            Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.Name);
            string profilePictureStr = await fb.GetProfilePicture(user.Email);
            if (profilePictureStr != null)
            {
                profilePicture.Source = ImageSource.FromUri(new Uri(profilePictureStr));
            }
            else
            {
                profilePicture.Source = ImageSource.FromFile("defaultProfilePic.jpg");
            }

        }

        public async void Request_Clicked(object sender, EventArgs e)
        {
            fb.AddFriend(profile.Email);
            await DisplayAlert("Request Sent", "You have requested this user to be your friend. If this user accepts your request, you will be able to view their profile.", "OK");
        }

        
    }
}
