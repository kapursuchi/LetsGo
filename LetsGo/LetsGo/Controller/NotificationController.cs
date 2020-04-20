using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using LetsGo.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Collections;
using System.Globalization;

namespace LetsGo.Controller
{
    public partial class NotificationController : INotifyPropertyChanged
    {

        private ObservableCollection<object> _friendrequests { get; set; }

        private ObservableCollection<object> _commRequests { get; set; }
        public ObservableCollection<object> CommunityRequests
        {
            get
            {
                return _commRequests;
            }
            set
            {
                _commRequests = value;
                OnPropertyChanged(nameof(CommunityRequests));
            }
        }

        private ObservableCollection<object> _commInvites { get; set; }
        public ObservableCollection<object> CommunityInvites
        {
            get
            {
                return _commInvites;
            }
            set
            {
                _commInvites = value;
                OnPropertyChanged(nameof(CommunityInvites));
            }
        }

        private ObservableCollection<object> _eventRequests { get; set; }
        public ObservableCollection<object> EventRequests
        {
            get
            {
                return _eventRequests;
            }
            set
            {
                _eventRequests = value;
                OnPropertyChanged(nameof(EventRequests));
            }
        }

        private ObservableCollection<object> _eventInvites { get; set; }
        public ObservableCollection<object> EventInvites
        {
            get
            {
                return _eventInvites;
            }
            set
            {
                _eventInvites = value;
                OnPropertyChanged(nameof(EventInvites));
            }
        }
        public ObservableCollection<object> FriendRequests
        {
            get
            {
                return _friendrequests;
            }
            set
            {
                _friendrequests = value;
                OnPropertyChanged(nameof(FriendRequests));
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
            List<UserProfile> profiles = await fb.GetFriendRequests();
            List<UserProfile> commRequests = await fb.GetCommunityRequests();
            List<CommunityProfile> commInvites = await fb.GetCommunityInvites();
            List<EventProfile> eventInvites = await fb.GetEventInvites();
            List<UserProfile> eventRequests = await fb.GetEventRequests();
            FriendRequests = new ObservableCollection<object>();
            EventRequests = new ObservableCollection<object>();
            CommunityRequests = new ObservableCollection<object>();
            CommunityInvites = new ObservableCollection<object>();
            EventInvites = new ObservableCollection<object>();

            if (profiles.Count != 0)
            {
                for (int i = 0; i < profiles.Count; i++)
                {
                    profiles.ElementAt(i).Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(profiles.ElementAt(i).Name);
                    FriendRequests.Add(profiles.ElementAt(i));
                }
            }
            else
            {
                friendRequests.IsVisible = false;
                friendRequestsLbl.IsVisible = false;
            }
            if (commRequests.Count != 0)
            {
                for (int i = 0; i < commRequests.Count; i++)
                {
                    commRequests.ElementAt(i).Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(commRequests.ElementAt(i).Name);
                    CommunityRequests.Add(commRequests.ElementAt(i));
                }

            }
            else
            {
                commRequestsList.IsVisible = false;
                commRequestsLbl.IsVisible = false;
            }

            if (commInvites.Count != 0)
            {
                for (int i = 0; i < commInvites.Count; i++)
                {
                    commInvites.ElementAt(i).Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(commInvites.ElementAt(i).Name);

                    CommunityInvites.Add(commInvites.ElementAt(i));
                }
            }
            else
            {
                commInvitesList.IsVisible = false;
                commInivtesLbl.IsVisible = false;
            }

            if (eventInvites.Count != 0)
            {
                for (int i = 0; i < eventInvites.Count; i++)
                {
                    eventInvites.ElementAt(i).Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(eventInvites.ElementAt(i).Name);
                    EventInvites.Add(eventInvites.ElementAt(i));
                }
            }
            else
            {
                eventInvitesList.IsVisible = false;
                eventInvitesLbl.IsVisible = false;
            }

            if (eventRequests.Count != 0)
            {
                for (int i = 0; i < eventRequests.Count; i++)
                {
                    eventRequests.ElementAt(i).Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(eventRequests.ElementAt(i).Name);
                    EventRequests.Add(eventRequests.ElementAt(i));
                }
            }
            else
            {
                eventRequestsList.IsVisible = false;
                eventRequestsLbl.IsVisible = false;
            }


            NoNotifications();


            double height = 40;
            friendRequests.ItemsSource = FriendRequests;
            friendRequests.HeightRequest = FriendRequests.Count * height;
            commRequestsList.ItemsSource = CommunityRequests;
            commRequestsList.HeightRequest = CommunityRequests.Count * height;
            eventRequestsList.ItemsSource = EventRequests;
            eventRequestsList.HeightRequest = EventRequests.Count * height;
            commInvitesList.ItemsSource = CommunityInvites;
            commInvitesList.HeightRequest = CommunityInvites.Count * height;
            eventInvitesList.ItemsSource = EventInvites;
            eventInvitesList.HeightRequest = EventInvites.Count * height;
        }

        private void NoNotifications()
        {
            if (FriendRequests.Count == 0 && CommunityRequests.Count == 0 && EventRequests.Count == 0 && CommunityInvites.Count == 0 && EventInvites.Count == 0)
            {
                NoNotificationsLbl.IsVisible = true;
                friendRequestsLbl.IsVisible = false;
                commRequestsLbl.IsVisible = false;
                eventRequestsLbl.IsVisible = false;
                commInivtesLbl.IsVisible = false;
                eventInvitesLbl.IsVisible = false;
            }
            else
            {
                NoNotificationsLbl.IsVisible = false;
            }


        }

        public void OnAcceptERequest(object sender, EventArgs e)
        {

            var type = (MenuItem)sender;
            if (type.CommandParameter.ToString() == "LetsGo.Model.UserProfile")
            {
                UserProfile profile = (UserProfile)type.CommandParameter;
                fb.AcceptRequest(null, null, profile);
                EventRequests.Remove(type.CommandParameter);
            }


            NoNotifications();

        }
        public void OnAcceptFRequest(object sender, EventArgs e)
        {

            var type = (MenuItem)sender;
            if (type.CommandParameter.ToString() == "LetsGo.Model.UserProfile")
            {
                UserProfile profile = (UserProfile)type.CommandParameter;
                fb.AcceptRequest(null, null, profile);
                FriendRequests.Remove(type.CommandParameter);
            }


            NoNotifications();

        }

        public void OnAcceptCRequest(object sender, EventArgs e)
        {

            var type = (MenuItem)sender;
            if (type.CommandParameter.ToString() == "LetsGo.Model.UserProfile")
            {
                UserProfile profile = (UserProfile)type.CommandParameter;
                fb.AcceptRequest(null, null, profile);
                CommunityRequests.Remove(type.CommandParameter);
            }


            NoNotifications();

        }
        public async void OnAcceptEInvite(object sender, EventArgs e)
        {

            var type = (MenuItem)sender;
            if (type.CommandParameter.ToString() == "LetsGo.Model.EventProfile")
            {
                EventProfile profile = (EventProfile)type.CommandParameter;
                string current = fb.GetCurrentUser();
                UserProfile currentUser = await fb.GetUserObject(current);
                fb.AcceptRequest(profile, null, currentUser);
                EventInvites.Remove(type.CommandParameter);
            }
            NoNotifications();
        }

        public async void OnAcceptCInvite(object sender, EventArgs e)
        {

            var type = (MenuItem)sender;
            if (type.CommandParameter.ToString() == "LetsGo.Model.CommunityProfile")
            {
                CommunityProfile profile = (CommunityProfile)type.CommandParameter;
                string current = fb.GetCurrentUser();
                UserProfile currentUser = await fb.GetUserObject(current);
                fb.AcceptRequest(null, profile, currentUser);
                CommunityInvites.Remove(type.CommandParameter);
            }
            NoNotifications();

        }

        public void OnDeclineFRequest(object sender, EventArgs e)
        {
            var type = (MenuItem)sender;

            UserProfile profile = (UserProfile)type.CommandParameter;
            fb.RemoveRequest(profile);
            FriendRequests.Remove(type.CommandParameter);
            NoNotifications();
        }

        public void OnDeclineERequest(object sender, EventArgs e)
        {
            var type = (MenuItem)sender;

            UserProfile profile = (UserProfile)type.CommandParameter;
            fb.RemoveRequest(profile);
            EventRequests.Remove(type.CommandParameter);
            NoNotifications();
        }

        public void OnDeclineCRequest(object sender, EventArgs e)
        {
            var type = (MenuItem)sender;

            UserProfile profile = (UserProfile)type.CommandParameter;
            fb.RemoveRequest(profile);
            CommunityRequests.Remove(type.CommandParameter);
            NoNotifications();
        }

        public void OnDeclineCInvite(object sender, EventArgs e)
        {
            var type = (MenuItem)sender;
            CommunityProfile profile = (CommunityProfile)type.CommandParameter;
            string current = fb.GetCurrentUser();
            fb.RemoveInvite(null, profile.CommunityID, current);
            int index = CommunityInvites.IndexOf(profile);
            CommunityInvites.RemoveAt(index);
            NoNotifications();
        }

        public void OnDeclineEInvite(object sender, EventArgs e)
        {
            var type = (MenuItem)sender;
            EventProfile profile = (EventProfile)type.CommandParameter;
            string current = fb.GetCurrentUser();
            fb.RemoveInvite(profile.EventID, null, current);
            int index = EventInvites.IndexOf(profile);
            EventInvites.RemoveAt(index);
            NoNotifications();
        }

        protected override void OnAppearing()
        {
            SetValues();
            base.OnAppearing();
        }

    }
}


