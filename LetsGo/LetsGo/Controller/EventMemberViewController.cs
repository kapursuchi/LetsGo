using LetsGo.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace LetsGo.Controller
{
    public partial class EventMemberViewController
    {
        public EventMemberViewController(EventProfile evt)
        {
            InitializeComponent();
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#51aec2");
            On<Android>().SetToolbarPlacement(value: ToolbarPlacement.Top);
            UpdateChildrenLayout();
        }
    }
}
