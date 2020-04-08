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

            Events = new List<EventProfile>();
            SetValues();

            InitializeComponent();
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.LightSteelBlue;
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
        public List<EventProfile> Events { get; set; }
        public async void SetValues()
        {
            Events = await fb.GetUserEvents();
            if(Events.Count == 0)
            {
                Events = new List<EventProfile>();

                Events.Add(new EventProfile("There are no events you are a part of!", "No description available", DateTime.Today,
                            "00:00:00", "00:00:00", "No location", "No owner", "no interests,", true));
            }
            ListEvents.ItemsSource = Events;
        }
    }
}
