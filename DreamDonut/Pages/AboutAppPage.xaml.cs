using System;
using System.Collections.Generic;

using Xamarin.Forms;

using DreamDonut.Abstractions;

namespace DreamDonut.Pages
{
    public partial class AboutAppPage : BackgroundImagePage
    {


        string bodyLabelText = @"
This app was developed for Dream Week 2018 by Ricardo Amaya as part of a social experiment to expose the injustices of judging the value and worth of a human being through the initial conditions of their birth.

It is the developer's hope that this app can emphatically open the imagination of the user and allow them to experience what DACA recipents, TPS recipients, and other undocumented people experience every day.
";


        public AboutAppPage()
        {
            InitializeComponent();

            VersionLabel.Text = "Version: " + DependencyService.Get<INativeOperations>().AppVersion;
            BodyLabel.Text = bodyLabelText;
        }


    }
}
