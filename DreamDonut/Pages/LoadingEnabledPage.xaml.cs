using System;
using System.Threading.Tasks;

using Xamarin.Forms;

using Rg.Plugins.Popup.Extensions;

using DreamDonut.Abstractions;
using DreamDonut.Utilities;


namespace DreamDonut.Pages
{
    public partial class LoadingEnabledPage : ContentPage
    {
        public LoadingEnabledPage()
        {

            InitializeComponent();

            if(Shared.IsDebug)
                ShowDebugLayout();
            
        }

        /// <summary>
        /// Sets the visibility of the loading indicator asynchronously
        /// </summary>
        protected async Task SetIsLoading(bool value) {
            _isLoading = value;
            IsBusy = value;
            DependencyService.Get<INativeOperations>().NetworkActivityIndicator = value;
            IsLoadingLayoutVisible = value;

            if(_isLoading) {
                if(_currentPopUp == null) {
                    _currentPopUp = new LoadingPopUpPage();
                    await Navigation.PushPopupAsync(_currentPopUp);
                }
            } else {
                if(_currentPopUp != null) {
                    await _currentPopUp.DismissAsync();
                    _currentPopUp = null;
                }
            }
            OnLoadingChange();
        }


        private LoadingPopUpPage _currentPopUp;
        private bool _isLoading;

        /// <summary>
        /// Sets the visibility of the loading indicator
        /// </summary>
        protected bool IsLoading {
            set {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    IsBusy = value;
                    DependencyService.Get<INativeOperations>().NetworkActivityIndicator = value;
                    IsLoadingLayoutVisible = value;

                    Device.BeginInvokeOnMainThread(async () => {
                        if(_isLoading) {
                            if(_currentPopUp == null) {
                                _currentPopUp = new LoadingPopUpPage();
                                await Navigation.PushPopupAsync(_currentPopUp);
                            }
                        } else {
                            if(_currentPopUp != null) {
                                await _currentPopUp.DismissAsync();
                                _currentPopUp = null;
                            }
                        }
                    });

                    OnLoadingChange();
                }
            }
            get {
                return _isLoading;
            }
        }

        private void ShowDebugLayout() {
            IsDebugLayoutVisible = true;
            DebugLayoutBounds = new Rectangle(1,1,90,20);
            DebugLayoutText = "Debug Build";
        }

        protected virtual void OnLoadingChange(){}


        public bool IsLoadingLayoutVisible
        {
            get
            {
                return (bool)GetValue(IsLoadingLayoutVisibleProperty);
            }
            set
            {
                SetValue(IsLoadingLayoutVisibleProperty, value);
            }
        }
        public static readonly BindableProperty IsLoadingLayoutVisibleProperty =
            BindableProperty.Create("IsLoadingLayoutVisible", typeof(bool), typeof(LoadingEnabledPage), false);

        public bool IsDebugLayoutVisible
        {
            get
            {
                return (bool)GetValue(IsDebugLayoutVisibleProperty);
            }
            set
            {
                SetValue(IsDebugLayoutVisibleProperty, value);
            }
        }
        public static readonly BindableProperty IsDebugLayoutVisibleProperty =
            BindableProperty.Create("IsDebugLayoutVisible", typeof(bool), typeof(LoadingEnabledPage), Shared.IsDebug);


        public string DebugLayoutText
        {
            get
            {
                return (string)GetValue(DebugLayoutTextProperty);
            }
            set
            {
                SetValue(DebugLayoutTextProperty, value);
            }
        }
        public static readonly BindableProperty DebugLayoutTextProperty =
            BindableProperty.Create("DebugLayoutText", typeof(string), typeof(LoadingEnabledPage), String.Empty);


        public Rectangle DebugLayoutBounds
        {
            get
            {
                return (Rectangle)GetValue(DebugLayoutBoundsProperty);
            }
            set
            {
                SetValue(DebugLayoutBoundsProperty, value);
            }
        }
        public static readonly BindableProperty DebugLayoutBoundsProperty =
            BindableProperty.Create("DebugLayoutBounds", typeof(Rectangle), typeof(LoadingEnabledPage), new Rectangle(0,0,0,0));
        
    }
}
