using System;
using System.Collections.Generic;
using System.Text;
using LetsGo.Model;
using Xamarin.Forms;
using LetsGo.Model.Authentication;
using System.Linq;
using System.ComponentModel;
using System.Globalization;

namespace LetsGo.Controller
{
    public partial class UpdateProfileController : ContentPage, INotifyPropertyChanged
    {
        private ProfilePage profile = new ProfilePage();
        readonly FirebaseDB fb = new FirebaseDB();
        private bool isPublic;
        private bool toggled = false;

        public event PropertyChangedEventHandler PropertyChanged;

        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _name;

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

        private string _location;

        public string Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
                OnPropertyChanged(nameof(Location));
            }
        }


        private string _interests;

        public string Interests
        {
            get
            {
                return _interests;
            }
            set
            {
                _interests = value;
                OnPropertyChanged(nameof(Interests));
            }
        }

        public UpdateProfileController()
        {
            
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            SetValues();
            name.BindingContext = this;
            interests.BindingContext = this;
            location.BindingContext = this;
            

        }

        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        private async void SetValues()
        {
            Name= await fb.GetUsersName();
            Name = textInfo.ToTitleCase(Name);
            Location = await fb.GetUsersLocation();
            if (Location != null)
                Location = textInfo.ToTitleCase(Location);
            else
            {
                Location = "Location";
            }
            List<string> interestList = await fb.GetUsersInterests();
            if (interestList != null)
            { 
                for (int i = 0; i < interestList.Count; i++)
                {
                    Interests += textInfo.ToTitleCase(interestList.ElementAt(i)) + ", ";
                }
                Interests = Interests.Substring(0, Interests.Length-2);
            }
            else
            {
                Interests = "List your interests here, comma separated.";
                
            }
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
            await Navigation.PushAsync(new ProfileController());
            
        }


        public void On_Toggled(object sender, ToggledEventArgs e)
        {
            toggled = true;
            isPublic = e.Value;
        }

        public async void Delete_Clicked(object sender, EventArgs e)
        {
            bool deleted = await profile.DeleteUser();
            if (deleted)
            {
                await DisplayAlert("Success", "Your account has been deleted", "OK");
                await Navigation.PushAsync(new LoginController());
            }    
        }
        
        public void Change_Picture_Clicked(object sender, EventArgs e)
        {

        }


    }
}
