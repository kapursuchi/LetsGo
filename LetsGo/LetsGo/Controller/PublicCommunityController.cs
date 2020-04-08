using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using LetsGo.Model;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class PublicCommunityController
    {
        readonly FirebaseDB fb = new FirebaseDB();
        private CommunityProfile community { get; set; }
        public List<string> Interests { get; set; }
        private string _name { get; set; }
        private string _location { get; set; }
        private string _description { get; set; }
        private string _leader { get; set; }
        private string CommunityID { get; set; }
        private string _leaderName { get; set; }
        private Image _img { get; set; }
        public Image ProfileImage
        {
            get
            {
                return _img;
            }
            set
            {
                _img = value;
                OnPropertyChanged(nameof(ProfileImage));
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

        public PublicCommunityController(CommunityProfile comm)
        {
            community = comm;
            SetValues(community);
            InitializeComponent();
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.LightSteelBlue;
            name.BindingContext = this;
            location.BindingContext = this;
            description.BindingContext = this;
            leader.BindingContext = this;
        }

        public async void SetValues(CommunityProfile comm)
        {
            Name = comm.Name;
            Interests = new List<string>();
            Location = comm.Location;
            Description = comm.Description;
            CommunityID = comm.CommunityID;
            Leader = comm.Leader;
            if (Location == null)
            {
                Location = "No Location Yet...";
            }
            //Interests = await fb.GetCommunityInterests(id);
            if (Interests.Count == 0)
            {
                Interests.Add("No interests listed yet...");
            }
            string ln = await fb.GetUsersName(Leader);
            LeaderName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ln);

            string communityPictureStr = null; // await fb.GetCommunityPicture(id);
            if (communityPictureStr != null)
            {
                imgChosen.Source = ImageSource.FromUri(new Uri(communityPictureStr));
            }
            else
            {
                imgChosen.Source = ImageSource.FromFile("communityimage.jpg");
            }
            CommunityImage = imgChosen;
        }

        public async void Join_Clicked(object sender, EventArgs e)
        {
            bool added = false;
            if (community.InviteOnly && !community.Members.Contains(fb.GetCurrentUser()))
            {
                await DisplayAlert("Message", "This community is invite only. You have requested to join. " +
                    "The community leader will have to accept your request for you to become a member.", "OK");
            }
            else
            {
                added = await fb.JoinCommunity(community);
            }
            if (added)
            {
                await DisplayAlert("Message", "You are now a member of this community", "OK");
            }
        }
    }
}
