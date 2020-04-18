using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using LetsGo.Model;
using System.ComponentModel;

namespace LetsGo.Controller
{
    public partial class FriendProfileController : ContentPage, INotifyPropertyChanged
    {
        readonly FirebaseDB fb = new FirebaseDB();
        private UserProfile friendProfile { get; set; }

        public List<string> Interests { get; set; }
        private string _name { get; set; }
        private string _location { get; set; }
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

        public FriendProfileController(UserProfile friend)
        {
            friendProfile = friend;
            Interests = new List<string>();
            SetValues(friend);
            InitializeComponent();
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#80b3d1");
            name.BindingContext = this;
            location.BindingContext = this;
        }



        public async void SetValues(UserProfile friend)
        {
            Interests = new List<string>();
            Name = friend.Name;
            Location = friend.Location;
            if (Location == null)
            {
                Location = "No Location Yet...";
            }
            Interests = new List<string>();
            Interests = await fb.GetUsersInterests(friend.Email);
            

            if (Interests.Count == 0)
            {

                Interests.Add("No interests listed yet...");

            }
            interests.ItemsSource = Interests;

            string profilePictureStr = await fb.GetProfilePicture(friend.Email);
            if (profilePictureStr != null)
            {
                profilePicture.Source = ImageSource.FromUri(new Uri(profilePictureStr));
            }
            else
            {
                profilePicture.Source = ImageSource.FromFile("defaultProfilePic.jpg");
            }
        }


    }
}
