﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LetsGo.Model;
using Xamarin.Forms;

namespace LetsGo.Controller
{
    public partial class ViewCommunityMemberController
    {
        readonly FirebaseDB fb = new FirebaseDB();

        public ViewCommunityMemberController(CommunityProfile c)
        {
            InitializeComponent();
        }

        public ViewCommunityMemberController()
        {
            
        }
    }
}
