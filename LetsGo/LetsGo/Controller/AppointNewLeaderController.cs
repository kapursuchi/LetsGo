using LetsGo.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class AppointNewLeaderController
    {
        private CommunityProfile community { get; set; }
        private readonly FirebaseDB fb = new FirebaseDB();
        public List<UserProfile> SearchResults { get; set; }
        public UserProfile userToAppoint { get; set; }

        public string SearchStr { get; set; }
        public AppointNewLeaderController(CommunityProfile c)
        {
            community = c;
            SearchResults = new List<UserProfile>();
            userToAppoint = new UserProfile();
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public async void Search_Text(object sender, EventArgs e)
        {

            SearchStr = searchBar.Text.ToLower();
            List<string> members = community.Members;
            List<UserProfile> membersProfiles = new List<UserProfile>();
            for (int i = 0; i < members.Count; i++)
            {
                UserProfile user = await fb.GetUserObject(members[i]);
                membersProfiles.Add(user);
            }
            for (int i = 0; i < membersProfiles.Count; i++)
            {
                if (membersProfiles[i].Name.ToLower().Contains(SearchStr))
                {
                    membersProfiles[i].Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(membersProfiles[i].Name);
                    SearchResults.Add(membersProfiles[i]);
                }
            }
            search.ItemsSource = SearchResults;

        }

        public async void OnAppoint_Clicked(object sender, EventArgs e)
        {
            var item = (Xamarin.Forms.Button)sender;
            item.Text = "Selected";
            item.TextColor = Color.Green;
            item.FontAttributes = FontAttributes.Italic;
            bool choice = await DisplayAlert("Selection", "Are you sure you want to appoint this member as the new community leader?", "Yes", "No");
            if (choice)
            {
                userToAppoint = (UserProfile)item.CommandParameter;
                fb.AppointNewCommunityLeader(community, userToAppoint.Email);
                await DisplayAlert("Success", "You have appointed the new community leader and have been successfully removed from this community!", "OK");
                Navigation.RemovePage(this);
                await Navigation.PushAsync(new NavigationBarController());


            }
            else
            {
                item.Text = "Select";
                item.TextColor = Color.Navy;
                item.FontAttributes = FontAttributes.None;
                return;
            }


        }
 


    }
}
