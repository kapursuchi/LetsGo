﻿using LetsGo.Model;
using LetsGo.Model.Authentication;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class UpdateCommunityController
    {
        private CommunityProfile community { get; set; }
        readonly FirebaseDB fb = new FirebaseDB();
        private bool isPublic;
        private bool inviteOnly;
        private bool toggledPublic = false;
        private bool toggledInvite = false;
        private bool _istoggled;
        private bool _inviteonly;
        public bool istoggledPublic
        {
            get
            {
                return _istoggled;
            }
            set
            {
                _istoggled = value;
                OnPropertyChanged(nameof(istoggledPublic));
            }
        }
        public bool istoggledInvite
        {
            get
            {
                return _inviteonly;
            }
            set
            {
                _inviteonly = value;
                OnPropertyChanged(nameof(istoggledInvite));
            }
        }

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
        private string _description;

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
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


        private ObservableCollection<string> _interests;

        public ObservableCollection<string> InterestList
        {
            get
            {
                return _interests;
            }
            set
            {
                _interests = value;
                OnPropertyChanged(nameof(InterestList));
            }
        }

        private Image _img;
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
        public UpdateCommunityController()
        {
            var auth = DependencyService.Get<IFirebaseAuthenticator>();
            community = auth.GetCurrentCommunity();
            SetValues();
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            name.BindingContext = this;
            interests.BindingContext = this;
            location.BindingContext = this;
            desc.BindingContext = this;
            publicCommunitySwitch.BindingContext = this;
            inviteOnlySwitch.BindingContext = this;
        }

        readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        MediaFile file;

        public async void SetValues()
        {
            istoggledPublic = community.PublicCommunity;
            istoggledInvite = community.InviteOnly;
            Name = community.Name;
            Name = textInfo.ToTitleCase(Name);
            Location = community.Location;
            if (Location != null)
                Location = textInfo.ToTitleCase(Location);
            else
            {
                Location = "Location";
            }
            Description = textInfo.ToTitleCase(community.Description);
            List<string> interestList = await fb.GetCommunityInterests(community);
            
            InterestList = new ObservableCollection<string>(interestList);

            if (InterestList == null || InterestList.Count == 0)
            {

                InterestList.Add("No interests listed yet...");

            }
            double height = 40;
            interests.HeightRequest = InterestList.Count * height;
            interests.ItemsSource = InterestList;

            string communityImageStr = await fb.GetCommunityPicture(community.CommunityID);
            if (communityImageStr != null)
            {
                imgChosen.Source = ImageSource.FromUri(new Uri(communityImageStr));
            }
            else
            {
                imgChosen.Source = ImageSource.FromFile("communityimage.jpg");
            }
        }

        public async void Save_Update_Clicked(object sender, EventArgs e)
        {
            if (toggledPublic == false)
            {
                isPublic = community.PublicCommunity;
            }
            if (toggledInvite == false)
            {
                inviteOnly = community.InviteOnly;
            }
            string uName = name.Text;
            string city = location.Text;
            string description = desc.Text;
            List<string> interestList;
            if (intToAdd.Text != null)
            {
                List<string> interestSplit = intToAdd.Text.Split(',').ToList();
                interestList = InterestList.ToList();

                for (int i = 0; i < interestSplit.Count; i++)
                {
                    interestList.Add(interestSplit.ElementAt(i).Trim());
                }
            }
            else
            {
                interestList = InterestList.ToList();
            }
            bool updated = await fb.UpdateCommunity(community, uName, description, city, isPublic, inviteOnly, interestList);
            if (!updated)
            {
                await DisplayAlert("Update Unsuccessful", "Your community update was unsuccessful.", "OK");
            }
            await Navigation.PopAsync();

        }


        public void On_Toggled_Public(object sender, ToggledEventArgs e)
        {
            toggledPublic = true;
            isPublic = e.Value;
        }


        public void On_Toggled_Invites(object sender, ToggledEventArgs e)
        {
            toggledInvite = true;
            inviteOnly = e.Value;
        }

        public async void Delete_Clicked(object sender, EventArgs e)
        {
            bool choice = await DisplayAlert("Delete Community", "Are you sure you want to delete this community? This action cannot be undone.", "OK", "Cancel");
            // user selects cancel on prompt
            if (choice == false)
            {
                return;
            }
            // user selects OK, delete community
            else
            {
                await fb.DeleteCommunity(community);
                await Navigation.PopToRootAsync();
            }
        }

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
                string photo = await fb.UploadCommunityPhoto(file.GetStream(), community.CommunityID);
                CommunityImage = imgChosen;

            }
            catch (Exception ex)
            {
                await DisplayAlert("Upload Failed", ex.Message, "OK");
            }

        }

        public void OnRemove(object sender, EventArgs e)
        {
            var item = (Xamarin.Forms.Button)sender;
            item.Text = "Removed";
            item.TextColor = Color.Crimson;
            string listitem = (from itm in InterestList
                               where itm == item.CommandParameter.ToString()
                               select itm)
                            .FirstOrDefault<string>();
            InterestList.Remove(listitem);
        }
    }
}
