using LetsGo.Model;
using LetsGo.Model.Authentication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class LeaderMembersListController
    {
        readonly FirebaseDB fb = new FirebaseDB();
        private ObservableCollection<UserProfile> _members { get; set; }

        private CommunityProfile thisCommunity { get; set; }

        private List<string> usersToRemove { get; set; }
        private List<UserProfile> removeList { get; set; }

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

        public LeaderMembersListController()
        {
            usersToRemove = new List<string>();
            removeList = new List<UserProfile>();
            SetValues();
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

        }

        public async void SetValues()
        {

            string current = fb.GetCurrentUser();
            MembersList = new ObservableCollection<UserProfile>();
            var auth = DependencyService.Get<IFirebaseAuthenticator>();
            thisCommunity = auth.GetCurrentCommunity();
            List<UserProfile> profiles = await fb.GetCommunityMembers(thisCommunity);
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

        public async void OnRemove_Clicked(object sender, EventArgs e)
        {
            var item = (Xamarin.Forms.Button)sender;
            if (item.Text=="Remove")
            {
                
                item.FontAttributes = FontAttributes.Italic;
                item.TextColor = Color.FromHex("#962c3e");
                UserProfile userToRemove = (UserProfile)item.CommandParameter;

                if (!usersToRemove.Contains(userToRemove.Email))
                {
                    usersToRemove.Add(userToRemove.Email);
                    int index = MembersList.IndexOf(userToRemove);
                    removeList.Add(MembersList[index]);
                }
                    
                item.Text = "Removed";
            }
            else
            {
                item.Text = "Remove";
                item.FontAttributes = FontAttributes.Bold;
                item.TextColor = Color.FromHex("#2d4ca1");
                UserProfile userToRemove = (UserProfile)item.CommandParameter;
                if (usersToRemove.Contains(userToRemove.Email))
                    usersToRemove.Remove(userToRemove.Email);
            }

        }

        public async void OnRemoveFinished_Clicked(object sender, EventArgs e)
        {
            UserProfile leader = await fb.GetProfileOfUser(thisCommunity.Leader);
            if (usersToRemove.Contains(thisCommunity.Leader))
            {
                bool yes = await DisplayAlert("Community Leader", "You are the community leader. Do you wish to leave or delete the community?", "LEAVE", "DELETE");
                if (yes)
                {
                    if (thisCommunity.Members.Count == 1 && thisCommunity.Members.ElementAt(0) == leader.Email)
                    {
                        await DisplayAlert("Delete Community", "You are the only member of this community, so it will be deleted.", "OK");
                        bool deleted = await fb.DeleteCommunity(thisCommunity);
                        if (deleted)
                        {
                            await Navigation.PopAsync();
                        }
                    }
                    bool proceed = await DisplayAlert("Community Leader", "Before leaving this community, you will need to appoint a new leader. Do you still wish to continue?", "YES", "NO");
                    if (proceed)
                    {
                        
                        await Navigation.PushAsync(new AppointNewLeaderController(thisCommunity));
                    }
                }
                else
                {
                    bool deleted = await fb.DeleteCommunity(thisCommunity);
                    if (deleted)
                    {
                        await DisplayAlert("Success", "You have deleted this community", "OK");
                        await Navigation.PopAsync();
                    }
                }
            }
            else
            {
                bool YesRemove = await DisplayAlert("Removing Members", "You are about to permanently remove members from your community. Please press OK to continue or CANCEL to cancel.", "OK", "CANCEL");
                if (YesRemove)
                {
                    for (int i = 0; i < usersToRemove.Count; i++)
                    {
                        fb.RemoveCommunityMember(thisCommunity, usersToRemove.ElementAt(i));
                        MembersList.Remove(removeList.ElementAt(i));
                    }
                }
                
                
                
            }
            

            
            

        }
    }
}
