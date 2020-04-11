using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LetsGo.Model;
using LetsGo.Model.Authentication;
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
            GrabCommunities();
            InitializeComponent();
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#80b3d1");
        }
        public async void StartCommunity_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateCommunityController());
        }

        public async void GrabCommunities()
        {
            CommunityList = await fb.GetMyCommunities();
            viewComms.ItemsSource = CommunityList;
        }

        public async void Community_Tapped(object sender, ItemTappedEventArgs e)
        {
            CommunityProfile selectedCommunity = e.Item as CommunityProfile;
            var auth = DependencyService.Get<IFirebaseAuthenticator>();
            auth.SetCurrentCommunity(selectedCommunity);
            string userEmail = fb.GetCurrentUser();
            // Community Leader taps on community
            if (selectedCommunity.Leader == userEmail)
            {
                await Navigation.PushAsync(new CommunityLeaderViewController(selectedCommunity));
            }
            // Regular member taps on community
            else
            {
                await Navigation.PushAsync(new CommunityMemberViewController(selectedCommunity));
                //await Navigation.PushAsync(new ViewCommunityMemberController(selectedCommunity));
            }

        }

        protected override void OnAppearing()
        {
            GrabCommunities();
            base.OnAppearing();
        }

    }
} 
