using LetsGo.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class PrivateEventController
    {
        private EventProfile privateEvent { get; set; }
        readonly FirebaseDB fb = new FirebaseDB();
        private string _name { get; set; }
        private Image _img { get; set; }
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
        public PrivateEventController(EventProfile evt)
        {
            privateEvent = evt;
            SetValues(privateEvent);
            InitializeComponent();
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#80b3d1");
            name.BindingContext = this;
        }

        private async void SetValues(EventProfile evt)
        {
            Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(evt.Name);
            string evtimg = await fb.GetEventPicture(evt.EventID);
            if (evtimg != null)
            {
                eventImage.Source = ImageSource.FromUri(new Uri(evtimg));
            }
            else
            {
                eventImage.Source = ImageSource.FromFile("eventimage.png");
            }

        }

        public async void Request_Clicked(object sender, EventArgs e)
        {
            //add code here to request to join event
        }
    }
}
