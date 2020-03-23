using System;
using System.Collections.Generic;
using System.Text;
using LetsGo.Model;
using Xamarin.Forms;
using LetsGo.Model.Authentication;
using System.Linq;
using System.ComponentModel;

namespace LetsGo.Controller
{
    public partial class UpdateProfileController : INotifyPropertyChanged
    {
        
        public UpdateProfileController()
        {
           // SetValues();
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = this;
            
        }
        

        private ProfilePage profile = new ProfilePage();
        readonly FirebaseDB fb = new FirebaseDB();
        private bool isPublic;
        private bool toggled = false;

        private async void SetValues()
        {
            name.Text = await fb.GetUsersName();
            location.Text = await fb.GetUsersLocation();
            interests.Text =  fb.GetUsersInterests().ToString();
        }

        public async void Save_Update_Clicked(object sender, EventArgs e)
        {
            if (toggled == false)
            {
                isPublic = await fb.HasPublicAccount();
            }
            string uName = name.Text;
            string city = location.Text;
            List<string> interestSplit = interests.Text.Split(',').ToList();
            List<string> interestList = new List<string>();
            for (int i = 0; i < interestSplit.Count; i++)
            {
                interestList.Add(interestSplit.ElementAt(i).Trim());
            }
            bool updated = await profile.UpdateProfile(uName, city, isPublic, interestList);
            if (!updated)
            {
                await DisplayAlert("Update Unsuccessful", "Your profile update was unsuccessful.", "OK");
            }
            
        }


        public void On_Toggled(object sender, ToggledEventArgs e)
        {
            toggled = true;
            isPublic = e.Value;
        }

        public void Delete_Clicked(object sender, EventArgs e)
        {
        }


    }
}
