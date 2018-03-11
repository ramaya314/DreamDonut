using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;

using DreamDonut.Utilities;

namespace DreamDonut.Pages
{
    public partial class BackgroundImagePage : LoadingEnabledPage
    {


        public BackgroundImagePage()
        {
            InitializeComponent();
            IsDebugLayoutVisible = Shared.IsDebug;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected virtual void SetUpNavBar(bool enableBackButton)
        {

            NavigationPage.SetHasBackButton(this, enableBackButton);
            NavigationPage.SetHasNavigationBar(this, true);

        }

        protected virtual void SetUpNavBar() {
            SetUpNavBar(true);
        }



        public FileImageSource BackgroundImageSource
        {
            get => (FileImageSource)GetValue(BackgroundImageSourceProperty);
            set => SetValue(BackgroundImageSourceProperty, value);
        }
        public readonly BindableProperty BackgroundImageSourceProperty =
            BindableProperty.Create("BackgroundImageSource", typeof(FileImageSource), typeof(BackgroundImagePage), null);


        public FileImageSource BackNavigationButtonImageSource
        {
            get => (FileImageSource)GetValue(BackNavigationButtonImageSourceProperty);
            set => SetValue(BackNavigationButtonImageSourceProperty, value);
        }
        public readonly BindableProperty BackNavigationButtonImageSourceProperty =
            BindableProperty.Create("BackNavigationButtonImageSource", typeof(FileImageSource), typeof(BackgroundImagePage), null);


        public string BackNavigationButtonTitle
        {
            get => (string)GetValue(BackNavigationButtonTitleProperty);
            set => SetValue(BackNavigationButtonTitleProperty, value);
        }
        public readonly BindableProperty BackNavigationButtonTitleProperty =
            BindableProperty.Create("BackNavigationButtonTitle", typeof(string), typeof(BackgroundImagePage), string.Empty);
        

    }
}
