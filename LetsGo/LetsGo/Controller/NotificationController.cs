using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using LetsGo.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace LetsGo.Controller
{
    public partial class NotificationController : INotifyPropertyChanged
    {

        private ObservableCollection<UserProfile> _requests { get; set; }
        public ObservableCollection<UserProfile> FriendRequestNotifications 
        {
            get
            {
               return _requests ;
            }
            set
            {
                _requests = value;
                OnPropertyChanged(nameof(FriendRequestNotifications));
            }
        }
        readonly FirebaseDB fb = new FirebaseDB();
        public NotificationController()
        {
            SetValues();
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        

        public async void SetValues()
        {
            FriendRequestNotifications = new ObservableCollection<UserProfile>();
            List<UserProfile> profiles = await fb.GetFriendRequests();
            FriendRequestNotifications = new ObservableCollection<UserProfile>(profiles);

            if (FriendRequestNotifications.Count == 0)
            {

                FriendRequestNotifications.Add(new UserProfile() { Name = "No notifications to display, you're caught up!" });

            }

            notifications.ItemsSource = FriendRequestNotifications;
        }

        public void OnAccept(object sender, EventArgs e)
        {
            var type = (MenuItem)sender;
            UserProfile profile = (UserProfile)type.CommandParameter;
            fb.AcceptRequest(profile);
            UserProfile listitem = (from pro in FriendRequestNotifications
                               where pro == type.CommandParameter
                               select pro)
                            .FirstOrDefault<UserProfile>();
            FriendRequestNotifications.Remove(listitem);
        }

        public void OnDecline(object sender, EventArgs e)
        {
            var type = (MenuItem)sender;
            UserProfile profile = (UserProfile)type.CommandParameter;
            fb.RemoveRequest(profile);
            UserProfile listitem = (from pro in FriendRequestNotifications
                                    where pro == type.CommandParameter
                                    select pro)
                            .FirstOrDefault<UserProfile>();
            FriendRequestNotifications.Remove(listitem);

        }

    }

}
