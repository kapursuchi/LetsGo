using LetsGo.Model;
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
    public partial class UpdateEventController
    {
        private EventProfile _event { get; set; }
        readonly FirebaseDB fb = new FirebaseDB();
        private bool isPublic;
        private bool toggledPublic = false;
        private bool _istoggled;
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
        public Image EventImage
        {
            get
            {
                return _img;
            }
            set
            {
                _img = value;
                OnPropertyChanged(nameof(EventImage));
            }
        }
        public UpdateEventController()
        {
            var auth = DependencyService.Get<IFirebaseAuthenticator>();
            _event = auth.GetCurrentEvent();
            SetValues();
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            name.BindingContext = this;
            interests.BindingContext = this;
            location.BindingContext = this;
            desc.BindingContext = this;
            publicEventSwitch.BindingContext = this;
            
        }

        readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        MediaFile file;

        public async void SetValues()
        {
            istoggledPublic = _event.PublicEvent;
            Name = _event.Name;
            Name = textInfo.ToTitleCase(Name);
            Location = _event.Location;
            if (Location != null)
                Location = textInfo.ToTitleCase(Location);
            else
            {
                Location = "Location";
            }
            Description = textInfo.ToTitleCase(_event.Description);
            List<string> interestList = await fb.GetEventInterests(_event);
            
            InterestList = new ObservableCollection<string>(interestList);

            if (InterestList == null || InterestList.Count == 0)
            {

                InterestList.Add("No interests listed yet...");

            }
            double height = 40;
            interests.HeightRequest = InterestList.Count * height;
            interests.ItemsSource = InterestList;

            string eventImageStr = await fb.GetEventPicture(_event.EventID);
            if (eventImageStr != null)
            {
                imgChosen.Source = ImageSource.FromUri(new Uri(eventImageStr));
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
                isPublic = _event.PublicEvent;
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
            bool updated = await fb.UpdateEventProfile(_event, uName, description, city, isPublic, interestList);
            if (!updated)
            {
                await DisplayAlert("Update Unsuccessful", "Your event update was unsuccessful.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Update successful", "Your event update was successful.", "OK");
                await Navigation.PopAsync();
            }
            

        }


        public void On_Toggled_Public(object sender, ToggledEventArgs e)
        {
            toggledPublic = true;
            isPublic = e.Value;
        }

        public async void Delete_Clicked(object sender, EventArgs e)
        {
            bool choice = await DisplayAlert("Delete Event", "Are you sure you want to delete this Event? This action cannot be undone.", "OK", "Cancel");
            // user selects cancel on prompt
            if (choice == false)
            {
                return;
            }
            // user selects OK, delete community
            else
            {
                await fb.DeleteEvent(_event);
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
                string photo = await fb.UploadCommunityPhoto(file.GetStream(), _event.EventID);
                EventImage = imgChosen;

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
