using System;
using System.Collections.Generic;

using Xamarin.Forms;

using DreamDonut.Helpers;

namespace DreamDonut.Pages
{
    public partial class MainTabbedPage : TabbedPage
    {
        public MainTabbedPage()
        {
            InitializeComponent();

            var gamePage = new DreamNavigationPage (new HomePage());
            gamePage.Title = "Pizza";
            gamePage.Icon = "pizza-30.png";
            Children.Add (gamePage);

            var aboutDreamersPage = new DreamNavigationPage (new AboutDreamersPage());
            aboutDreamersPage.Title = "About Mason DREAMers";
            aboutDreamersPage.Icon = "Juan32.png";
            Children.Add (aboutDreamersPage);

            var aboutAppPage = new DreamNavigationPage (new AboutAppPage());
            aboutAppPage.Title = "About App";
            aboutAppPage.Icon = "help-white30.png";
            Children.Add (aboutAppPage);

            BarBackgroundColor = ColorPalette.PrimaryColor;
            BarTextColor = ColorPalette.SecondaryColor;
        }
    }
}
