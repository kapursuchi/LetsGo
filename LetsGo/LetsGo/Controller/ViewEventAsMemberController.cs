using LetsGo.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class ViewEventAsMemberController
    {
        public ViewEventAsMemberController(EventProfile evt)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        public ViewEventAsMemberController()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
