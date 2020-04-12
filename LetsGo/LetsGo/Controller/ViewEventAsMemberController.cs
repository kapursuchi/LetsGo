using LetsGo.Model;
using LetsGo.Model.Authentication;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class ViewEventAsMemberController
    {
        public ViewEventAsMemberController(EventProfile evt)
        {
            thisEvent = evt;
            SetValues(thisEvent);
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            name.BindingContext = this;
            location.BindingContext = this;
            description.BindingContext = this;
            owner.BindingContext = this;
        }
        public ViewEventAsMemberController()
        {
            var auth = DependencyService.Get<IFirebaseAuthenticator>();
            thisEvent = auth.GetCurrentEvent();
            SetValues(thisEvent);
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            name.BindingContext = this;
            location.BindingContext = this;
            description.BindingContext = this;
            owner.BindingContext = this;
        }

        readonly FirebaseDB fb = new FirebaseDB();
        private EventProfile thisEvent { get; set; }
        public List<string> Interests { get; set; }
        public List<string> Members { get; set; }
        private string _name { get; set; }
        private string _location { get; set; }
        private string _description { get; set; }
        private string _evtowner { get; set; }
        private string EventID { get; set; }
        private string _ownerName { get; set; }
        private Image _img { get; set; }

        public Image EventImage
        {
            get
            {
                return _img;
            }
            set
            {
                _img = value;
                OnPropertyChanged(nameof(EventImage));
            }
        }
        public string OwnerName
        {
            get
            {
                return _ownerName;
            }
            set
            {
                _ownerName = value;
                OnPropertyChanged(nameof(OwnerName));
            }
        }
        public string EventOwner
        {
            get
            {
                return _evtowner;
            }
            set
            {
                _evtowner = value;
                OnPropertyChanged(nameof(EventOwner));
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


        public async void SetValues(EventProfile evt)
        {
            List<string> interestList = evt.Interests;
            Interests = new List<string>();
            Name = evt.Name;
            Location = evt.Location;
            Description = evt.Description;

            EventID = evt.EventID;
            EventOwner = evt.EventOwner;
            //Members = evt.Members;
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
            string ln = await fb.GetUsersName(EventOwner);
            OwnerName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ln);

            interests.ItemsSource = Interests;

            string eventPictureStr = await fb.GetEventPicture(EventID);
            if (eventPictureStr != null)
            {
                imgChosen.Source = ImageSource.FromUri(new Uri(eventPictureStr));
            }
            else
            {
                imgChosen.Source = ImageSource.FromFile("eventimage.png");
            }
        }

        public async void OnOwnerName_Clicked(object sender, EventArgs e)
        {
            bool publicOwner = await fb.IsPublicUser(EventOwner);
            string userEmail = fb.GetCurrentUser();
            bool isFriendOfOwner = await fb.isFriend(EventOwner);
            // leader is the current user
            if (userEmail == EventOwner)
            {
                await Navigation.PushAsync(new ProfileController());
            }
            // leader is a friend of current user
            else if (isFriendOfOwner)
            {
                UserProfile friend = await fb.GetUserObject(EventOwner);
                await Navigation.PushAsync(new FriendProfileController(friend));
            }
            // leader is not a friend of current user and has a public account
            else if (!isFriendOfOwner && publicOwner)
            {
                UserProfile userToVisit = await fb.GetUserObject(EventOwner);
                await Navigation.PushAsync(new PublicProfileController(userToVisit));
            }
            // leader is not a friend of current user and has a private account
            else
            {
                UserProfile userToVisit = await fb.GetUserObject(EventOwner);
                await Navigation.PushAsync(new PrivateProfileController(userToVisit));
            }
        }
    }
}
