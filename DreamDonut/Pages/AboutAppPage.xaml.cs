using System;
using System.Collections.Generic;

using Xamarin.Forms;

using DreamDonut.Abstractions;

namespace DreamDonut.Pages
{
    public partial class AboutAppPage : BackgroundImagePage
    {


        string bodyLabelText = @"
This app was developed for Dream Week 2018 by Ricardo Amaya as part of a social experiment designed to expose the user to the injustices of judging the value and worth of a human being by the initial conditions of their birth.

It is the developer's hope that this app can open the imagination of the user and allow them to experience the systematic discrimination that DACA recipents, TPS recipients, and other undocumented people experience every day.
";


        public AboutAppPage()
        {
            InitializeComponent();

            VersionLabel.Text = "Version: " + DependencyService.Get<INativeOperations>().AppVersion;
            BodyLabel.Text = bodyLabelText;
        }


    }
}
