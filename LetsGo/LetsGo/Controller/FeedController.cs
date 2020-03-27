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

        private List<EventProfile> _events { get; set; }


        public FeedController()
        {
            //FeedEvents = new List<EventProfile>() { new EventProfile("Event 1", "Description 1",  Convert.ToDateTime("3/12/2020"), "4:30", "5:30", "Oceanside", "kapursuchi@gmail.com", "Running,health", true),
            //                                        new EventProfile("Event 2", "Description 2",  Convert.ToDateTime("3/12/2020"), "4:30", "5:30", "Oceanside", "kapursuchi@gmail.com", "Running,health", true)};
            FeedEvents = new List<EventProfile>();
            
            SetFeed();

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            
        }


        public List<EventProfile> FeedEvents { get; set; }


        public async void SetFeed()
        {
            FeedEvents = await fb.GetFeed();
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
