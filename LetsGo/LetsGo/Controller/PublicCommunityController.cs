using System;
using System.Collections.Generic;
using System.Text;
using LetsGo.Model;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class PublicCommunityController
    {
        readonly FirebaseDB fb = new FirebaseDB();
        private CommunityProfile community { get; set; }
        //private Guid CommunityID { get; set; }
        private Image _img { get; set; }
        public Image ProfileImage
        {
            get
            {
                return _img;
            }
            set
            {
                _img = value;
                OnPropertyChanged(nameof(ProfileImage));
            }
        }

        private string _name { get; set; }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public PublicCommunityController(CommunityProfile comm)
        {
            community = comm;
            SetValues(community);
            InitializeComponent();
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.LightSteelBlue;
            name.BindingContext = this;
        }

        public void SetValues(CommunityProfile comm)
        {
            Name = comm.Name;
            //CommunityID = comm.CommunityID;
        }

        public async void Join_Clicked(object sender, EventArgs e)
        {
            bool added = false;
            if (community.InviteOnly && !community.Members.Contains(fb.GetCurrentUser()))
            {
                await DisplayAlert("Message", "This community is invite only. You have requested to join. " +
                    "The community leader will have to accept your request for you to become a member.", "OK");
            }
            else
            {
                //added = await fb.JoinCommunity(community);
            }
            if (added)
            {
                await DisplayAlert("Message", "You are now a member of this community", "OK");
            }
        }
    }
}
