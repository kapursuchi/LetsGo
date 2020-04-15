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
        public ObservableCollection<UserProfile> RequestNotifications
        {
            get
            {
               return _requests ;
            }
            set
            {
                _requests = value;
                OnPropertyChanged(nameof(RequestNotifications));
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
            RequestNotifications = new ObservableCollection<UserProfile>();
            List<UserProfile> profiles = await fb.GetFriendRequests();
            List<UserProfile> commRequests = await fb.GetCommunityRequests();
            if (profiles != null)
            {
                RequestNotifications = new ObservableCollection<UserProfile>(profiles);
            }
            else if (commRequests != null)
            {
                RequestNotifications = new ObservableCollection<UserProfile>(commRequests);
            }
            
            for (int i = 0; i < commRequests.Count; i++)
            {
                RequestNotifications.Add(commRequests.ElementAt(i));
            }

            if (RequestNotifications.Count == 0)
            {

                RequestNotifications.Add(new UserProfile() { Name = "No notifications!" });

            }

            notifications.ItemsSource = RequestNotifications;
        }

        public void OnAccept(object sender, EventArgs e)
        {
            var type = (MenuItem)sender;
            UserProfile profile = (UserProfile)type.CommandParameter;
            fb.AcceptRequest(profile);
            UserProfile listitem = (from pro in RequestNotifications
                                    where pro == type.CommandParameter
                               select pro)
                            .FirstOrDefault<UserProfile>();
            RequestNotifications.Remove(listitem);
        }

        public void OnDecline(object sender, EventArgs e)
        {
            var type = (MenuItem)sender;
            
            UserProfile profile = (UserProfile)type.CommandParameter;
            fb.RemoveRequest(profile);
            UserProfile listitem = (from pro in RequestNotifications
                                    where pro == type.CommandParameter
                                    select pro)
                            .FirstOrDefault<UserProfile>();
            RequestNotifications.Remove(listitem);

        }

    }

}
