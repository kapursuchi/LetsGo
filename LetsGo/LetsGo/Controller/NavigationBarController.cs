using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace LetsGo.Controller
{
    public partial class NavigationBarController
    {
        public NavigationBarController()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            On<Android>().SetToolbarPlacement(value: ToolbarPlacement.Bottom);
            UpdateChildrenLayout();
        }
    }
}
