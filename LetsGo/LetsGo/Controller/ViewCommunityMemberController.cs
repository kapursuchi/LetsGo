using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using LetsGo.Model;
using LetsGo.Model.Authentication;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class ViewCommunityMemberController
    {
        readonly FirebaseDB fb = new FirebaseDB();

        public CommunityProfile comm { get; set; }
        public List<string> Interests { get; set; }
        public List<string> Members { get; set; }
        private string _name { get; set; }
        private string _location { get; set; }
        private string _description { get; set; }
        private string _leader { get; set; }
        private string CommunityID { get; set; }
        private string _leaderName { get; set; }
        private Image _img { get; set; }
        public Image CommunityImage
        {
            get
            {
                return _img;
            }
            set
            {
                _img = value;
                OnPropertyChanged(nameof(CommunityImage));
            }
        }
        public string LeaderName
        {
            get
            {
                return _leaderName;
            }
            set
            {
                _leaderName = value;
                OnPropertyChanged(nameof(LeaderName));
            }
        }
        public string Leader
        {
            get
            {
                return _leader;
            }
            set
            {
                _leader = value;
                OnPropertyChanged(nameof(Leader));
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


        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        public ViewCommunityMemberController(CommunityProfile c)
        {
            comm = c;
            SetValues(comm);
            InitializeComponent();
            name.BindingContext = this;
            location.BindingContext = this;
            description.BindingContext = this;
            leader.BindingContext = this;
        }

        public ViewCommunityMemberController()
        {
            var auth = DependencyService.Get<IFirebaseAuthenticator>();
            comm = auth.GetCurrentCommunity();
            SetValues(comm);
            InitializeComponent();
            name.BindingContext = this;
            location.BindingContext = this;
            description.BindingContext = this;
            leader.BindingContext = this;
        }

        public async void SetValues(CommunityProfile community)
        {
            List<string> interestList = community.Interests;
            Interests = new List<string>();
            Name = community.Name;
            Location = community.Location;
            Description = community.Description;
            CommunityID = community.CommunityID;
            Leader = community.Leader;
            Members = community.Members;
            if (Location == null)
            {
                Location = "No Location Yet...";
            }
            //Interests = await fb.GetCommunityInterests(community);
            if (interestList.Count == 0)
            {
                Interests.Add("No interests listed yet...");
            }
            else
            {
                foreach (string interest in interestList)
                {
                    CultureInfo.CurrentCulture.TextInfo.ToTitleCase(interest);
                    Interests.Add(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(interest));
                }

            }
            string ln = await fb.GetUsersName(Leader);
            LeaderName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ln);

            interests.ItemsSource = Interests;

            string communityPictureStr = await fb.GetCommunityPicture(CommunityID);
            if (communityPictureStr != null)
            {
                imgChosen.Source = ImageSource.FromUri(new Uri(communityPictureStr));
            }
            else
            {
                imgChosen.Source = ImageSource.FromFile("communityimage.jpg");
            }
        }

        public async void OnLeaderName_Clicked(object sender, EventArgs e)
        {
            bool publicLeader = await fb.IsPublicUser(Leader);
            string userEmail = fb.GetCurrentUser();
            bool isFriendOfLeader = await fb.isFriend(Leader);
            // leader is the current user
            if (userEmail == Leader)
            {
                await Navigation.PushAsync(new ProfileController());
            }
            // leader is a friend of current user
            else if (isFriendOfLeader)
            {
                UserProfile friend = await fb.GetUserObject(Leader);
                await Navigation.PushAsync(new FriendProfileController(friend));
            }
            // leader is not a friend of current user and has a public account
            else if (!isFriendOfLeader && publicLeader)
            {
                UserProfile userToVisit = await fb.GetUserObject(Leader);
                await Navigation.PushAsync(new PublicProfileController(userToVisit));
            }
            // leader is not a friend of current user and has a private account
            else
            {
                UserProfile userToVisit = await fb.GetUserObject(Leader);
                await Navigation.PushAsync(new PrivateProfileController(userToVisit));
            }
        }

        public async void OnLeave_Clicked(object sender, EventArgs e)
        {
            string current = fb.GetCurrentUser();
            bool choice = await DisplayAlert("Confirm", "Are you sure you want to leave this community?", "YES", "NO");
            if (choice)
            {
                fb.RemoveCommunityMember(comm, current);
                await DisplayAlert("Success", "You are no longer a member of this community. You will now be redirected to your communities page.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                return;
            }
               
        }

    }
}
