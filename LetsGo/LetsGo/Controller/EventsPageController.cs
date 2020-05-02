using System;
using LetsGo.Model;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.ComponentModel;
using System.Globalization;
using LetsGo.Model.Authentication;

namespace LetsGo.Controller
{
    public partial class EventsPageController : ContentPage, INotifyPropertyChanged
    {
        //private ManageEvents profile = new ManageEvents();

        private FirebaseDB fb = new FirebaseDB();
        private string Eventname { get; set; }
        private string Eventlocation { get; set; }
        private string Eventdescription { get; set; }
        //private List<string> EventInterests { get; set; }
        public List<EventProfile> EventList { get; set; }

        private Image EventImg { get; set; }


        TextInfo Text_Info = new CultureInfo("en-US", false).TextInfo;

        public EventsPageController()
        {

            Events = new List<EventProfile>();
            GrabEvents();
            SetValues();

            InitializeComponent();
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#51aec2");
        }
        public async void NavigateToCreateEvent(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateEventController());
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
            if(Events == null || Events.Count == 0)
            {
                viewEvents.IsVisible = false;
                noEvents.IsVisible = true;
            }
            else
            {
                viewEvents.ItemsSource = Events;
                noEvents.IsVisible = false;
            }
            
        }

        public async void Event_Tapped(object sender, ItemTappedEventArgs e)
        {
            EventProfile selectedEvent = e.Item as EventProfile;
            var auth = DependencyService.Get<IFirebaseAuthenticator>();
            auth.SetCurrentEvent(selectedEvent);
            string userEmail = fb.GetCurrentUser();
            // Event Owner taps on event
            if (selectedEvent.EventOwner == userEmail)
            {
                await Navigation.PushAsync(new EventOwnerViewController(selectedEvent));
            }
            // Regular member taps on event
            else
            {
                await Navigation.PushAsync(new EventMemberViewController(selectedEvent));
            }

        }
        public async void GrabEvents()
        {
            EventList = await fb.GetMyEvents();
            viewEvents.ItemsSource = EventList;
        }
        protected override void OnAppearing()
        {
            GrabEvents();
            SetValues();
            base.OnAppearing();
        }
        protected override void OnDisappearing()
        {
            GrabEvents();
            SetValues();
            base.OnDisappearing();
        }
    }
}
