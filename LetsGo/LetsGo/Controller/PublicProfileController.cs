using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using LetsGo.Model;

namespace LetsGo.Controller
{
    public partial class PublicProfileController
    {
        readonly FirebaseDB fb = new FirebaseDB();
        private UserProfile profile { get; set; }

        public List<string> Interests { get; set; }
        
        private string _location { get; set; }
        private string _name { get; set; }
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


        public PublicProfileController(UserProfile user)
        {
            profile = user;
            SetValues(user);
            InitializeComponent();
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.LightSteelBlue;
            name.BindingContext = this;
            location.BindingContext = this;
        }

        public async void SetValues(UserProfile user)
        {
            Interests = new List<string>();
            Name = user.Name;
            Location = user.Location;
            if (Location == null)
            {
                Location = "No Location Yet...";
            }
            Interests = new List<string>();
            Interests = await fb.GetUsersInterests(user.Email);


            if (Interests.Count == 0)
            {

                Interests.Add("No interests listed yet...");

            }
            interests.ItemsSource = Interests;

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

        public async void OnAdd(object sender, EventArgs e)
        {
            fb.AddFriend(profile.Email);
            await DisplayAlert("Success", "You've added this user as a friend!", "OK");
            Navigation.RemovePage(this);
            await Navigation.PushAsync(new FriendProfileController(profile));
        }
    }
}
