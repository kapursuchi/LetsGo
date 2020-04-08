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

        readonly private FirebaseDB fb = new FirebaseDB();
     
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
            string id = Guid.NewGuid().ToString();

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



    }
}
