using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LetsGo.Model;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class CommunityPageController
    {
        readonly FirebaseDB fb = new FirebaseDB();
        public List<CommunityProfile> CommunityList { get; set; }

        public CommunityPageController()
        {
            CommunityList = new List<CommunityProfile>();
            InitializeComponent();
            DisplayCommunities();
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.LightSteelBlue;
        }
        public async void StartCommunity_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateCommunityController());
        }

        public async void DisplayCommunities()
        {
            CommunityList = await fb.GetMyCommunities();
            viewComms.ItemsSource = CommunityList;
        }

        public async void Community_Tapped(object sender, ItemTappedEventArgs e)
        {
            CommunityProfile selectedCommunity = e.Item as CommunityProfile;
            string userEmail = fb.GetCurrentUser();
            // Community Leader taps on community
            if (selectedCommunity.Leader == userEmail)
            {
                await Navigation.PushAsync(new ViewCommunityLeaderController(selectedCommunity));
            }
            // Regular member taps on community
            else
            {
                await Navigation.PushAsync(new ViewCommunityMemberController(selectedCommunity));
            }

        }
        /*
        public async void ViewCommunity_Clicked(object sender, EventArgs e)
        {

        } */
    }
}
