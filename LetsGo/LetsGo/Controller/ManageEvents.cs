using System;
using LetsGo.Model;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.ComponentModel;
using System.Globalization;

namespace LetsGo.Controller
{
    public partial class ManageEvents : ContentPage, INotifyPropertyChanged
    {
        //private ManageEvents profile = new ManageEvents();

        private FirebaseDB fb = new FirebaseDB();
        private string Eventname { get; set; }
        private string Eventlocation { get; set; }
        private string Eventdescription { get; set; }
        //private List<string> EventInterests { get; set; }

        private Image EventImg { get; set; }


        TextInfo Text_Info = new CultureInfo("en-US", false).TextInfo;

        public ManageEvents()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            SetValues();
            EventName.BindingContext = this;
            EventLocation.BindingContext = this;
            EventDescription.BindingContext = this;

        }
        public async void NavigateToCreateEvent(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EventsPageController());
        }

        public string Name
        {
            get
            {
                return Eventname;
            }
            set
            {
                Eventname = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public Image ProfilePicture
        {
            get
            {
                return EventImg;
            }
            set
            {
                EventImg = value;
                OnPropertyChanged(nameof(ProfilePicture));
            }
        }

        public string Location
        {
            get
            {
                return Eventlocation;
            }
            set
            {
                Eventlocation = value;
                OnPropertyChanged(nameof(Location));
            }
        }

        public string Description
        {
            get
            {
                return Eventdescription;
            }
            set
            {
                Eventdescription = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        private async void SetValues()
        {
            Name = await fb.GetEventName();
            Name = Text_Info.ToTitleCase(Name);
            Location = await fb.GetEventLocation();
            if (Location != null)
                Location = Text_Info.ToTitleCase(Location);
            else
            {
                Location = "No Location Yet...";
            }
            Description = await fb.GetEventDescription();
            /*string profilePictureStr = await fb.GetEventPicture();
            if (profilePictureStr != null)
            {
                EventImg.Source = ImageSource.FromUri(new Uri(profilePictureStr));
            }
            else
            {
                EventImg.Source = ImageSource.FromFile("LetsGoLogo.PNG");
            }
            ProfilePicture = EventImg;*/
        }
    }
}
