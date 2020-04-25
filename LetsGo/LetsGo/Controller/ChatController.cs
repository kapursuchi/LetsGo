using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class ChatController
    {
        public ChatController()
        {
            InitializeComponent();
            ((Xamarin.Forms.NavigationPage)Xamarin.Forms.Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#51aec2");
            UpdateChildrenLayout();

        }


    }
}
