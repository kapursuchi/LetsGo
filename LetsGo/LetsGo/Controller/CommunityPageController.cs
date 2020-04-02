using System;
using System.Collections.Generic;
using System.Text;
using LetsGo.Model;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class CommunityPageController
    {
        readonly FirebaseDB fb = new FirebaseDB();
        public CommunityPageController()
        {
            InitializeComponent();
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.LightSteelBlue;
        }
        public async void StartCommunity_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateCommunityController());
        }
    }
}
