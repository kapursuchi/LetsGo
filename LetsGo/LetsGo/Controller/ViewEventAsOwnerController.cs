using LetsGo.Model;
using LetsGo.Model.Authentication;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class ViewEventAsOwnerController
    {
        public ViewEventAsOwnerController(EventProfile evt)
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
        public ViewEventAsOwnerController()
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
            StartTime = evt.StartOfEvent;
            EndTime = evt.EndOfEvent;
            EventTime = StartTime + " to " + EndTime;

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

        MediaFile file;
        public async void Upload_Picture_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });
                if (file == null)
                    return;
                imgChosen.Source = ImageSource.FromStream(() =>
                {
                    var imageStram = file.GetStream();
                    return imageStram;
                });
                string id = EventID.ToString();
                string photo = await fb.UploadEventPhoto(file.GetStream(), id);
                EventImage = imgChosen;

            }
            catch (Exception ex)
            {
                await DisplayAlert("Upload Failed", ex.Message, "OK");
            }

        }

        public async void OnDeleteEvent_Pressed(object sender, EventArgs e)
        {
            bool choice = await DisplayAlert("Delete Event", "Are you sure you want to delete this event? This action cannot be undone.", "OK", "Cancel");
            // user selects cancel on prompt
            if (choice == false)
            {
                return;
            }
            // user selects OK, delete community
            else
            {
                await fb.DeleteEvent(thisEvent);
                await Navigation.PopToRootAsync();
            }
        }
        public async void OnInviteUsers_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InviteEventMembersController(thisEvent));
        }
        public async void OnUpdate_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UpdateEventController());
        }
    }
}
