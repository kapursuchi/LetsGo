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

        private ObservableCollection<object> _requests { get; set; }
        public ObservableCollection<object> RequestNotifications
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
            RequestNotifications =  new ObservableCollection<object>();
            List<UserProfile> profiles = await fb.GetFriendRequests();
            List<UserProfile> commRequests = await fb.GetCommunityRequests();
            List<CommunityProfile> commInvites = await fb.GetCommunityInvites();

            if (profiles != null || profiles.Count != 0)
            {
                for (int i = 0; i < profiles.Count; i++)
                {
                    profiles.ElementAt(i).Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(profiles.ElementAt(i).Name);
                    RequestNotifications.Add(profiles.ElementAt(i));
                }
            }
            if (commRequests != null || commRequests.Count != 0)
            {
                for (int i = 0; i < commRequests.Count; i++)
                {
                    commRequests.ElementAt(i).Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(commRequests.ElementAt(i).Name);
                    RequestNotifications.Add(commRequests.ElementAt(i));
                }
            }

            if (commInvites != null || commInvites.Count != 0)
            {
                for (int i = 0; i < commInvites.Count; i++)
                {
                    commInvites.ElementAt(i).Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(commInvites.ElementAt(i).Name);
                    RequestNotifications.Add(commInvites.ElementAt(i));
                }
            }


            if (RequestNotifications.Count == 0)
            {

                RequestNotifications.Add(new UserProfile() { Name = "No notifications!" });

            }

            notifications.ItemsSource = RequestNotifications;
        }

        public async void OnAccept(object sender, EventArgs e)
        {
            
            var type = (MenuItem)sender;
            if (type.CommandParameter.ToString() == "LetsGo.Model.UserProfile")
            {
                UserProfile profile = (UserProfile)type.CommandParameter;
                fb.AcceptRequest(null, null, profile);
                RequestNotifications.Remove(type.CommandParameter);
            }
            else if (type.CommandParameter.ToString() == "LetsGo.Model.CommunityProfile")
            {
                CommunityProfile profile = (CommunityProfile)type.CommandParameter;
                string current =  fb.GetCurrentUser();
                UserProfile currentUser = await fb.GetUserObject(current);
                fb.AcceptRequest(null, profile, currentUser);
                RequestNotifications.Remove(type.CommandParameter);

            }
            else if (type.CommandParameter.ToString() == "LetsGo.Model.EventProfile")
            {
                EventProfile profile = (EventProfile)type.CommandParameter;
                string current = fb.GetCurrentUser();
                UserProfile currentUser = await fb.GetUserObject(current);
                //fb.AcceptRequest(profile, null, currentUser);
                //RequestNotifications.Remove(type.CommandParameter);
            }

            if (RequestNotifications.Count == 0)
            {

                RequestNotifications.Add(new UserProfile() { Name = "No notifications!" });

            }
            
        }

        public void OnDecline(object sender, EventArgs e)
        {/*
            var type = (MenuItem)sender;
            
            UserProfile profile = (UserProfile)type.CommandParameter;
            fb.RemoveRequest(profile);
            UserProfile listitem = (from pro in RequestNotifications
                                    where pro == type.CommandParameter
                                    select pro)
                            .FirstOrDefault<UserProfile>();
            RequestNotifications.Remove(listitem);
            */

            var type = (MenuItem)sender;
            if (type.CommandParameter.ToString() == "LetsGo.Model.UserProfile")
            {
                UserProfile profile = (UserProfile)type.CommandParameter;
                fb.RemoveRequest(profile);
                int index = RequestNotifications.IndexOf(profile);
                RequestNotifications.RemoveAt(index);
            }
            else if (type.CommandParameter.ToString() == "LetsGo.Model.CommunityProfile")
            {
                CommunityProfile profile = (CommunityProfile)type.CommandParameter;
                string current = fb.GetCurrentUser();
                fb.RemoveInvite(null, profile.CommunityID, current);
                int index = RequestNotifications.IndexOf(profile);
                RequestNotifications.RemoveAt(index);
            }
            else if (type.CommandParameter.ToString() == "LetsGo.Model.EventProfile")
            {
                EventProfile profile = (EventProfile)type.CommandParameter;
                string current = fb.GetCurrentUser();
                fb.RemoveInvite(profile.EventID, null, current);
                int index = RequestNotifications.IndexOf(profile);
                RequestNotifications.RemoveAt(index);
            }

            if (RequestNotifications.Count == 0)
            {

                RequestNotifications.Add(new UserProfile() { Name = "No notifications!" });

            }
        }

    }

}
