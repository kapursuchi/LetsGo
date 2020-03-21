using System;
using System.Collections.Generic;
using System.Text;
using LetsGo.Model;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class FriendsPageController
    {
        public FriendsPageController()
        {
            InitializeComponent();
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.LightSteelBlue;
        }
    }
}
