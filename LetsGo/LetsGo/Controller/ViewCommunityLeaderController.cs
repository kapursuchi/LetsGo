using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LetsGo.Model;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class ViewCommunityLeaderController
    {
        readonly FirebaseDB fb = new FirebaseDB();

        public ViewCommunityLeaderController(CommunityProfile c)
        {
            InitializeComponent();
        }

        public ViewCommunityLeaderController()
        {

        }
    }
}
