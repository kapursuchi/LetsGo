using System;
using System.Collections.Generic;
using System.Text;
using LetsGo.Model;
using Xamarin.Forms;
using LetsGo.Model.Authentication;
using System.Linq;
using System.ComponentModel;
using System.Globalization;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;

namespace LetsGo.Controller
{
    public partial class UpdateProfileController : ContentPage, INotifyPropertyChanged
    {
        private readonly ProfilePage profile = new ProfilePage();
        readonly FirebaseDB fb = new FirebaseDB();
        private bool isPublic;
        private bool toggled = false;
        private bool _istoggled;


        
        public bool istoggled
        {
            get
            {
                return _istoggled;
            }
            set
            {
                _istoggled = value;
                OnPropertyChanged(nameof(istoggled));
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



        public UpdateProfileController()
        {
            
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            SetValues();
            name.BindingContext = this;
            interests.BindingContext = this;
            location.BindingContext = this;
            publicAccountSwitch.BindingContext = this;
            
        }

        readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        private async void SetValues()
        {
            istoggled = await fb.HasPublicAccount();
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
            InterestList = new ObservableCollection<string>(interestList);

            if (InterestList == null)
            {

                InterestList.Add("No interests listed yet...");

            }
            interests.ItemsSource = InterestList;
            
        }

        public async void Save_Update_Clicked(object sender, EventArgs e)
        {
            if (toggled == false)
            {
                isPublic = await fb.HasPublicAccount();
            }
            string uName = name.Text;
            string city = location.Text;
            List<string> interestList;
            if (intToAdd.Text != null)
            { 
                List<string> interestSplit = intToAdd.Text.Split(',').ToList();
                interestList= InterestList.ToList();

                for (int i = 0; i < interestSplit.Count; i++)
                {
                    interestList.Add(interestSplit.ElementAt(i).Trim());
                }
            }
            else
            {
                interestList = InterestList.ToList();
            }
            bool updated = await profile.UpdateProfile(uName, city, isPublic, interestList);
            if (!updated)
            {
                await DisplayAlert("Update Unsuccessful", "Your profile update was unsuccessful.", "OK");
            }
            await Navigation.PushAsync(new NavigationBarController());
            
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

        public async void Upload_Picture_Clicked(object sender, EventArgs e)
        {/*
            await CrossMedia.Current.Initialize();
            try
            {
                MediaFile file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });
                if (file != null)
                {
                    ProfileImage.Source = ImageSource.FromStream(() =>
                    {
                        var imageStram = file.GetStream();
                        //string file = await fb.UploadFile(file.GetStream(), "userprofiles", Path.GetFileName(file.Path));
                        return imageStram;

                    });
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Failed", ex.Message, "OK");
            }
            */
        }

        public void OnRemove(object sender, EventArgs e)
        {
            var item = (Xamarin.Forms.Button)sender;
            string listitem = (from itm in InterestList
                             where itm == item.CommandParameter.ToString()
                             select itm)
                            .FirstOrDefault<string>();
            InterestList.Remove(listitem);
        }
        

    }


}

