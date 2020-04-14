using System;
using System.Collections.Generic;
using Xamarin.Forms;
using LetsGo.Model;
using System.ComponentModel;
using LetsGo.Model.Authentication;

namespace LetsGo.Controller
{
    public partial class FeedController : INotifyPropertyChanged
    {
        readonly private FirebaseDB fb = new FirebaseDB();

        public FeedController()
        {
            FeedEvents = new List<EventProfile>();

            SetFeed();

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

        }


        public List<EventProfile> FeedEvents { get; set; }


        public async void SetFeed()
        {
            FeedEvents = await fb.GetFeed();
            if (FeedEvents.Count == 0)
            {
                FeedEvents = new List<EventProfile>();

                FeedEvents.Add(new EventProfile("There are no public events in your location!", "No description available", DateTime.Today,
                            "00:00:00", "00:00:00", "No location", "No owner", "no interests,", true));


            }
            FeedView.ItemsSource = FeedEvents;


        }


        public async void OnView(object sender, ItemTappedEventArgs e)
        {
            var type = e.ItemIndex;

            EventProfile selectedEvent = (EventProfile)FeedEvents[type];
            var auth = DependencyService.Get<IFirebaseAuthenticator>();
            auth.SetCurrentEvent(selectedEvent);
            bool member = await fb.isEventMember(selectedEvent);
            string userEmail = fb.GetCurrentUser();
            // EventOwner  taps on event
            if (selectedEvent.EventOwner == userEmail)
            {
                await Navigation.PushAsync(new EventOwnerViewController(selectedEvent));
            }
            // Regular member taps on event
            else if (member)
            {
                await Navigation.PushAsync(new EventMemberViewController(selectedEvent));
            }
            else
            {
                await Navigation.PushAsync(new PublicEventController(selectedEvent));
            }

            this.ClearValue(Xamarin.Forms.ListView.SelectedItemProperty);

        }

    }


}
