using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Extensions;

namespace DreamDonut.Pages
{
    public partial class LoadingPopUpPage : PopupPage
    {
        public LoadingPopUpPage()
        {
            InitializeComponent();

            CloseWhenBackgroundIsClicked = false;
        }

        public async Task DismissAsync() {
            try {
                await Navigation.PopPopupAsync();
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
            }
        }

        public void Dismiss() {
            try {
                Navigation.PopPopupAsync();
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
