using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using LetsGo.Model;
using System.ComponentModel;

namespace LetsGo.Controller
{
    public partial class CreateEventController
    {
        private bool eventPublic;
        DateTime dobChosen;
        TimeSpan Start;
        TimeSpan End;
        readonly FirebaseDB fb = new FirebaseDB();
        private CommunityProfile community { get; set; }

        public CreateEventController()
        {
            community = null;
            InitializeComponent();
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#80b3d1");
        }

        public CreateEventController(CommunityProfile c)
        {
            community = c;
            InitializeComponent();
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#80b3d1");
        }
        readonly private CreateEvent _createEvent = new CreateEvent();
        public async void CreateEvent_Clicked(object sender, EventArgs e)
        {
            string eventDetails = Edetails.Text;
            string EventName = Ename.Text;
            string eMail = fb.GetCurrentUser();
            string location = city.Text;
            string likes = interests.Text;
            DateTime EventDate = dobChosen;
            string eStart = Start.ToString();
            string eEnd = End.ToString();
            bool token = false;
            if (community == null)
            {

                 token = await _createEvent.CreateUserEvent(EventName, eventDetails, EventDate, eStart, eEnd, location, eMail, likes, eventPublic);

            }
            else
            {
                token = await fb.InitializeEvent(community, EventName, eventDetails, EventDate, eStart, eEnd, location, eMail, likes, eventPublic);
            }
            if (token == true && community == null)
            {
                await DisplayAlert("Success", "Event has been created.", "OK");
                await Navigation.PushAsync(new EventsPageController());
            }
            else if (token == true)
            {
                await DisplayAlert("Success", "Event has been created.", "OK");
                await Navigation.PopAsync();

            }
            else if (token == false)
            {
                await DisplayAlert("Failed to Create Event", "?", "OK");
            }

        }
        public void On_Toggled(object sender, ToggledEventArgs e)
        {
            eventPublic = e.Value;
        }
        public void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            dobChosen = e.NewDate;
        }
        public void OnStartTimeSelected(object sender, PropertyChangedEventArgs e)
        {
            Start =  _startTime.Time;
        }
        public void OnEndTimeSelected(object sender, PropertyChangedEventArgs e)
        {
            End = _endTime.Time;
        }
    }
}
