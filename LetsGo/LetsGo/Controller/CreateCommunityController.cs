using System;
using System.Collections.Generic;
using System.Text;
using LetsGo.Model;
using Xamarin.Forms;
using System.ComponentModel;
using Plugin.Media;
using Plugin.Media.Abstractions;


namespace LetsGo.Controller
{
    public partial class CreateCommunityController
    {
        private bool isPublic;
        private bool isInviteOnly;
        // move image stuff to other page
        private Image _img { get; set; }
        readonly private FirebaseDB fb = new FirebaseDB();

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

     
        public CreateCommunityController()
        {
            InitializeComponent();
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.LightSteelBlue;
        }
        readonly private CreateCommunityPage _createCommunity = new CreateCommunityPage();
        public async void CreateCommunity_Clicked(object sender, EventArgs e)
        {
            string leaderEmail = fb.GetCurrentUser();
            string description = eDesc.Text;
            string location = eLoc.Text;
            string interestTags = interests.Text;
            string name = eName.Text;
            List<string> mems = new List<string>();
            mems.Add(leaderEmail);
            Guid id = Guid.NewGuid();

            bool token = await _createCommunity.CreateCommunity(leaderEmail, description, location, interestTags, name, isPublic, isInviteOnly, mems, id);
            if (token == true)
            {
                await DisplayAlert("Success", "Your community has breen created.", "OK");
                await Navigation.PopAsync();
                await Navigation.PushAsync(new CommunityPageController());
            }
            else
            {
                await DisplayAlert("Community creation failed", "Failed to create community.", "OK");
            }
        }
        public void On_Toggled_Public(object sender, ToggledEventArgs e)
        {
            isPublic = e.Value;
        }
        public void On_Toggled_Invite(object sender, ToggledEventArgs e)
        {
            isInviteOnly = e.Value;
        }

        // move this stuff into ViewCommunity page
        MediaFile file;
        public async void Upload_Picture_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });
                if (file == null)
                    return;
                imgChosen.Source = ImageSource.FromStream(() =>
                {
                    var imageStram = file.GetStream();
                    return imageStram;
                });
                //string photo = await fb.UploadProfilePhoto(file.GetStream());
                CommunityImage = imgChosen;

            }
            catch (Exception ex)
            {
                await DisplayAlert("Upload Failed", ex.Message, "OK");
            }

        }

    }
}
