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

            var gamePage = new DreamNavigationPage (new HomePage{
                BackgroundImageSource = "butterfly.jpg",
                Title="Home"
            });
            gamePage.Title = "Home";
            gamePage.Icon = "donutTabIcon30";
            Children.Add (gamePage);

            var aboutDreamersPage = new DreamNavigationPage (new AboutDreamersPage{
                BackgroundImageSource = "butterfly.jpg"
            });
            aboutDreamersPage.Title = "About Mason DREAMers";
            aboutDreamersPage.Icon = "Juan32.png";
            Children.Add (aboutDreamersPage);

            var aboutAppPage = new DreamNavigationPage (new AboutAppPage{
                BackgroundImageSource = "butterfly.jpg"
            });
            aboutAppPage.Title = "About App";
            aboutAppPage.Icon = "help-white30.png";
            Children.Add (aboutAppPage);

            BarBackgroundColor = ColorPalette.PrimaryColor;
            BarTextColor = ColorPalette.SecondaryColor;
        }
    }
}
