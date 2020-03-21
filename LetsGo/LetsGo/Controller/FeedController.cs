using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace LetsGo.Controller
{
    public partial class FeedController
    {
        public FeedController()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            
        }
    }
}
