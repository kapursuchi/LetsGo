using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using LetsGo.Model;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class FriendsPageController
    {
        
        private ObservableCollection<UserProfile> _friends { get; set; }
        public new ObservableCollection<UserProfile> FriendsList
        {
            get
            {
                return _friends;
            }
            set
            {
                _friends = value;
                OnPropertyChanged(nameof(FriendsList));
            }
        }
        readonly FirebaseDB fb = new FirebaseDB();
        public FriendsPageController()
        {
            SetValues();
            InitializeComponent();
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#80b3d1");
        }

        public async void SetValues()
        {
            FriendsList = new ObservableCollection<UserProfile>();
            string current = fb.GetCurrentUser();
            List<UserProfile> profiles = await fb.GetFriends(current);
            FriendsList = new ObservableCollection<UserProfile>(profiles);

            if (FriendsList.Count == 0)
            {

                FriendsList.Add(new UserProfile() { Name = "You have no friends..." });

            }

            friends.ItemsSource = FriendsList;
        }

        public async void OnView(object sender, EventArgs e)
        {
            var type = (MenuItem)sender;
            UserProfile profile = (UserProfile)type.CommandParameter;
            string email = profile.Email;
            await Navigation.PushAsync(new FriendProfileController(profile));
        }

        public void OnRemove(object sender, EventArgs e)
        {
            var type = (MenuItem)sender;
            UserProfile profile = (UserProfile)type.CommandParameter;
            string email = profile.Email;
            fb.DeleteFriend(email);
            FriendsList.Remove(profile);
            friends.ItemsSource = FriendsList;
        }
    }
}