using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using LetsGo.Model;
using System.Threading.Tasks;
using System.ComponentModel;
using Firebase.Database;
using Firebase.Database.Query;
using System.Linq;

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
                            "00:00:00", "00:00:00", "No location", "No owner", "no interests,", true ));
                
                
            }
            FeedView.ItemsSource = FeedEvents;
            
            
        }
        
        public void OnAdd(object sender, EventArgs e)
        {

        }

        public void OnView(object sender, EventArgs e)
        {

        }

    }


}
