using LetsGo.Model;
using LetsGo.Model.Authentication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class ViewCommunityEventsController
    {
        private CommunityProfile community { get; set; }
        private FirebaseDB fb = new FirebaseDB();
        private string Eventname { get; set; }
        private string Eventlocation { get; set; }
        private string Eventdescription { get; set; }
        private Image EventImg { get; set; }


        public ObservableCollection<EventProfile> CommEvents { get; set; }
        public ViewCommunityEventsController()
        {
            var auth = DependencyService.Get<IFirebaseAuthenticator>();
            community = auth.GetCurrentCommunity();
            SetValues(community);
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
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
        public async void SetValues(CommunityProfile community)
        {
            Events = await fb.GetCommunityEvents(community);
            
            if (Events.Count == 0)
            {
                Events = new List<EventProfile>();
                Events.Add(new EventProfile("This community has no events.", "No description available", DateTime.Today,
                        "00:00:00", "00:00:00", "No location", "No owner", "no interests,", true));
                CommEvents = new ObservableCollection<EventProfile>(Events);
                ListEvents.IsVisible = false;
                noEvents.IsVisible = true;

                
            }
            CommEvents = new ObservableCollection<EventProfile>(Events);
            ListEvents.ItemsSource = CommEvents;
        }

        public async void Event_Tapped(object sender, ItemTappedEventArgs e)
        {
            EventProfile selectedEvent = e.Item as EventProfile;
            var auth = DependencyService.Get<IFirebaseAuthenticator>();
            auth.SetCurrentEvent(selectedEvent);
            await Navigation.PushAsync(new EventMemberViewController(selectedEvent));
            

        }
    }
}
