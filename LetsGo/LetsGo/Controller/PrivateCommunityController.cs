using LetsGo.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class PrivateCommunityController
    {
        private CommunityProfile community { get; set; }

        readonly FirebaseDB fb = new FirebaseDB();
        private string _name { get; set; }
        private Image _img { get; set; }
        public Image CommunityImage
        {
            get
            {
                return _img;
            }
            set
            {
                _img = value;
                OnPropertyChanged(nameof(CommunityImage));
            }
        }
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
        public PrivateCommunityController(CommunityProfile comm)
        {
            community = comm;
            InitializeComponent();
            SetValues(community);
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#51aec2");
            name.BindingContext = this;
        }

        private async void SetValues(CommunityProfile comm)
        {
            Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(comm.Name);
            string commimg = await fb.GetCommunityPicture(comm.CommunityID);
            if (commimg != null)
            {
                commImage.Source = ImageSource.FromUri(new Uri(commimg));
            }
            else
            {
                commImage.Source = ImageSource.FromFile("communityimage.jpg");
            }
            CommunityImage = commImage;
        }

        public async void Request_Clicked(object sender, EventArgs e)
        {
            bool added = false;
            if (!community.PublicCommunity && !community.Members.Contains(fb.GetCurrentUser()))
            {
                added = await fb.JoinCommunity(community);
                await DisplayAlert("Request Sent", "This community is private. " +
                    "The community leader will have to accept your request for you to become a member.", "OK");
            }
        }
    }
}
