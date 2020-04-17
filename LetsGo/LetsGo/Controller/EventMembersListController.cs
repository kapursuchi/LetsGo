using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.ComponentModel;
using System.Globalization;
using LetsGo.Model.Authentication;
using LetsGo.Model;

namespace LetsGo.Controller
{
    public partial class EventMembersListController : ContentPage, INotifyPropertyChanged
    {
        private FirebaseDB fb = new FirebaseDB();
        private EventProfile thisEvent { get; set; }
        public List<EventProfile> Events { get; set; }
        private List<string> MemberList { get; set; }
        public List<string> Members
        {
            get
            {
                return MemberList;
            }
            set
            {
                MemberList = value;
                OnPropertyChanged(nameof(Members));
            }
        }
        public EventMembersListController(EventProfile evt)
        {
            thisEvent = evt;
            SetValues(evt);
            //Members.BindingContext = this;
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        public EventMembersListController()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
            public async void SetValues(EventProfile evt)
        {
            Members = evt.Members;
            
            if (Members.Count == 0)
            {
                Members.Add("No Members");
            }
            //ListEvents.ItemsSource = Members;
        }
    }
}
