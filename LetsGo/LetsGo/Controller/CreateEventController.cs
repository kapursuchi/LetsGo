using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using LetsGo.Model;

namespace LetsGo.Controller
{
    public partial class CreateEventController
    {
        private bool eventPublic;
        DateTime dobChosen;
        TimeSpan Start;
        TimeSpan End;
        readonly FirebaseDB fb = new FirebaseDB();

        public CreateEventController()
        {
            InitializeComponent();
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#80b3d1");
        }
        readonly private EventsPage _createEvent = new EventsPage();
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
            bool token = await _createEvent.CreateUserEvent(EventName, eventDetails, EventDate, eStart, eEnd, location, eMail, likes, eventPublic);
            if (token == true)
            {
                await DisplayAlert("Success", "Event has been created.", "OK");
                await Navigation.PushAsync(new EventsPageController());
            }
            else
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
        /*Start and end assigned to are here
        public void OnStartTimeSelected(object sender, TimeEventArgs e)
        {
            Start = e.;
        }
        public void OnEndTimeSelected(object sender, DateChangedEventArgs e)
        {
            End = e.NewTime;
        }*/
    }
}
