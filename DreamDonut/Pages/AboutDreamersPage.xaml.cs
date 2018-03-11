using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace DreamDonut.Pages
{
    public partial class AboutDreamersPage : BackgroundImagePage
    {
        public AboutDreamersPage()
        {
            InitializeComponent();
            BodyLabel.Text = AboutDescription;

        }


        string AboutDescription = @"Since our establishment in the fall of 2011, Mason DREAMers has grown to be one of the most active and influential student organizations at George Mason University. We educate, inspire, and take action to break barriers created by the current broken immigration system in the United States.

In the past five years, we have created educational initiatives, events and resources to advocate for undocumented students within and outside of George Mason University. These have included:​

    •Student Panels
    •High school outreach initiatives
    •Immigration Monologues
    •DREAM weeks
    •UndocuAlly trainigs

All have been open to the general public and our largest event to date has had over 150 attendees. We have also worked tirelessly to generate additional funds for the Mason Dream Scholarship and have paved the way for the Stay Mason Student Support Fund

Ultimately, we do not seek to only benefit one segment of the student population, but create a fair ground for all.

We are proud supporters of similar student organizations at: Virginia Tech, Virginia Commonwealth University, Northern Virginia Community College, the University of Virginia, and Georgetown. We are grateful that they have adopted Mason DREAMers as a model organization to follow.";
    }
}
