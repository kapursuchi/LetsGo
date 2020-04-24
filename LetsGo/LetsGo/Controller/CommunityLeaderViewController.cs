using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Core;
using LetsGo.Model;

namespace LetsGo.Controller
{
    public partial class CommunityLeaderViewController
    {
        public CommunityLeaderViewController(CommunityProfile c)
        {
            InitializeComponent();
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#51aec2");
            On<Android>().SetToolbarPlacement(value: ToolbarPlacement.Top);
            UpdateChildrenLayout();
        }
    }
}