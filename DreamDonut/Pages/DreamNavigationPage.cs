using System;

using Xamarin.Forms;

using DreamDonut.Helpers;

namespace DreamDonut.Pages
{
    public class DreamNavigationPage : NavigationPage
    {
        public DreamNavigationPage(Page root) : base(root){
            BarBackgroundColor = ColorPalette.PrimaryColor;
            BarTextColor = ColorPalette.SecondaryColor;
        }
    }
}

