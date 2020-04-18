using LetsGo.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class PublicEventController
    {
        readonly FirebaseDB fb = new FirebaseDB();
        public EventProfile thisEvent { get; set; }
        public List<string> Interests { get; set; }
        private string _name { get; set; }
        private string _location { get; set; }
        private string _description { get; set; }
        private string _evtowner { get; set; }
        private string EventID { get; set; }
        private string _ownername { get; set; }
        private Image _img { get; set; }

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

        public string OwnerName
        {
            get
            {
                return _ownername;
            }
            set
            {
                _ownername = value;
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
        public PublicEventController(EventProfile evt)
        {
            thisEvent = evt;
            SetValues(thisEvent);
            InitializeComponent();
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#80b3d1");
            name.BindingContext = this;
            location.BindingContext = this;
            description.BindingContext = this;
            owner.BindingContext = this;
        }

        public async void SetValues(EventProfile evt)
        {
            Name = evt.Name;
            Interests = new List<string>();
            Location = evt.Location;
            Description = evt.Description;
            EventID = evt.EventID;
            EventOwner = evt.EventOwner;
            if (Location == null)
            {
                Location = "No Location Yet...";
            }
            //Interests = await fb.GetCommunityInterests(id);
            if (Interests.Count == 0)
            {
                Interests.Add("No interests listed yet...");
            }
            string ln = await fb.GetUsersName(EventOwner);
            OwnerName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ln);

            string eventPictureStr = await fb.GetEventPicture(EventID);
            if (eventPictureStr != null)
            {
                imgChosen.Source = ImageSource.FromUri(new Uri(eventPictureStr));
            }
            else
            {
                imgChosen.Source = ImageSource.FromFile("eventimage.png");
            }
            EventImage = imgChosen;
        }

        public async void Join_Clicked(object sender, EventArgs e)
        {
            
            bool added = false;
            if (!thisEvent.PublicEvent)
            {
                added = await fb.JoinEvent(thisEvent);
                await DisplayAlert("Request Sent", "This event is private." +
                    "The event owner will have to invite you for you to become a member.", "OK");
            }
            else
            {
                added = await fb.JoinEvent(thisEvent);
            }
            if (added)
            {
                await DisplayAlert("Success", "You are now a member of this event", "OK");
                Navigation.RemovePage(this);
                await Navigation.PushAsync(new EventMemberViewController(thisEvent));
            }
            
        }
    }
}
