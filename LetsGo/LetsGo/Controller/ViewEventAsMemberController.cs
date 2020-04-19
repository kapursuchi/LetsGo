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
            eventTime.BindingContext = this;
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
            eventTime.BindingContext = this;
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

        private string _startTime { get; set; }
        private string _endTime { get; set; }
        private string _eventTime { get; set; }

        public string StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
                OnPropertyChanged(nameof(StartTime));
            }
        }

        public string EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                _endTime = value;
                OnPropertyChanged(nameof(EndTime));
            }
        }

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

        public string EventTime
        {
            get
            {
                return _eventTime;
            }
            set
            {
                _eventTime = value;
                OnPropertyChanged(nameof(EventTime));
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
            StartTime = evt.StartOfEvent;
            EndTime = evt.EndOfEvent;
            EventTime = StartTime + " to " + EndTime;

            EventID = evt.EventID;
            EventOwner = evt.EventOwner;
            bool isUser = await fb.IsUser(evt.EventOwner);
            EventOwner = evt.EventOwner;
            if (isUser)
            {
                string ln = await fb.GetUsersName(EventOwner);
                OwnerName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ln);
            }
            else
            {
                CommunityProfile ln = await fb.GetCommunity(evt.EventOwner);
                OwnerName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ln.Name);
            }
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
            bool isUser = await fb.IsUser(EventOwner);
            string userEmail = fb.GetCurrentUser();
            if (isUser)
            {
                bool publicOwner = await fb.IsPublicUser(EventOwner);
                
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
            else
            {
                CommunityProfile selectedCommunity = await fb.GetCommunity(EventOwner);
                bool member = await fb.isCommunityMember(selectedCommunity);
                // Community Leader taps on community
                if (selectedCommunity.Leader == userEmail)
                {
                    await Navigation.PushAsync(new CommunityLeaderViewController(selectedCommunity));
                }
                // Regular member taps on community
                else if (member)
                {
                    await Navigation.PushAsync(new CommunityMemberViewController(selectedCommunity));
                }
                else if (selectedCommunity.PublicCommunity)
                {
                    await Navigation.PushAsync(new PublicCommunityController(selectedCommunity));
                }
                else if (!selectedCommunity.PublicCommunity)
                {
                    await Navigation.PushAsync(new PrivateCommunityController(selectedCommunity));
                }
            }

        }
    }
}
