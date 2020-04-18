using LetsGo.Model;
using LetsGo.Model.Authentication;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class CommunityMembersListController
    {
        readonly FirebaseDB fb = new FirebaseDB();
        private ObservableCollection<UserProfile> _members { get; set; }

        public ObservableCollection<UserProfile> MembersList
        {
            get
            {
                return _members;
            }
            set
            {
                _members = value;
                OnPropertyChanged(nameof(MembersList));
            }
        }

        public CommunityMembersListController()
        {
            SetValues();
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

        }

        public async void SetValues()
        {

            string current = fb.GetCurrentUser();
            MembersList = new ObservableCollection<UserProfile>();
            var auth = DependencyService.Get<IFirebaseAuthenticator>();
            CommunityProfile community = auth.GetCurrentCommunity();
            List<UserProfile> profiles = await fb.GetCommunityMembers(community);
            MembersList = new ObservableCollection<UserProfile>(profiles);

            if (MembersList.Count == 0)
            {

                MembersList.Add(new UserProfile() { Name = "This community has no members yet..." });

            }
            members.ItemsSource = MembersList;

        }

        public async void User_Tapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.ItemIndex;

            UserProfile profile = (UserProfile)MembersList[item];
            bool friend = await fb.isFriend(profile.Email);
            string current = fb.GetCurrentUser();
            if (profile.Email == current)
            {
                await Navigation.PushAsync(new ProfileController(profile));
            }
            else if (friend)
            {
                await Navigation.PushAsync(new FriendProfileController(profile));
            }
            else if (profile.PublicAcct)
            {
                await Navigation.PushAsync(new PublicProfileController(profile));
            }
            else if (!profile.PublicAcct)
            {
                await Navigation.PushAsync(new PrivateProfileController(profile));
            }


        }


    }


}
