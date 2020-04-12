using LetsGo.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class ViewEventAsOwnerController
    {
        public ViewEventAsOwnerController(EventProfile evt)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        public ViewEventAsOwnerController()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
