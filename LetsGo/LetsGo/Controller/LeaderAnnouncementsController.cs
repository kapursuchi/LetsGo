using LetsGo.Model;
using LetsGo.Model.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class LeaderAnnouncementsController
    {
        readonly FirebaseDB fb = new FirebaseDB();
        private CommunityProfile thisCommunity { get; set; }

        public List<Announcement> AnnouncementList { get; set; }

        public LeaderAnnouncementsController()
        {
            var auth = DependencyService.Get<IFirebaseAuthenticator>();
            thisCommunity = auth.GetCurrentCommunity();
            AnnouncementList = new List<Announcement>();
            SetValues();

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public async void SetValues()
        {

            AnnouncementList = await fb.GetCommunityAnnouncements(thisCommunity.CommunityID);
            if (AnnouncementList == null || AnnouncementList.Count == 0)
            {
                AnnouncementList = new List<Announcement>();
                AnnouncementList.Add(new Announcement(thisCommunity.CommunityID, "No Announcements"));
            }
            announcements.ItemsSource = AnnouncementList;
        }

        public void OnPost_Clicked(object sender, EventArgs e)
        {
            string announcementStr = newAnnouncement.Text;
            fb.CreateAnnouncement(thisCommunity.CommunityID, announcementStr);
            newAnnouncement.Text = string.Empty;
            newAnnouncement.Placeholder = "Create a new announcement...";
        }
    }
}
