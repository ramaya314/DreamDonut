using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Xamarin.Forms;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using DreamDonut.Services;

namespace DreamDonut.Components
{
    public partial class RateExperienceView : ContentView
    {


        public event EventHandler OnRateSendError;
        public event EventHandler OnRateSendSuccess;

        public string RateSendErrorMessage {
            get;
            private set;
        }

        public DateTime BirthDate { 
            get => (DateTime)GetValue(BirthDateProperty);
            set => SetValue(BirthDateProperty, value);
        }

        public static readonly BindableProperty BirthDateProperty =
            BindableProperty.Create("BirthDate", typeof(DateTime), typeof(RateExperienceView), DateTime.MinValue);
        
        public bool GotDonut { get; set; }

        const string BaseRateRequestUrl = "http://masondreamers.org/api/v1/SendDreamDonutFeedback";

        public RateExperienceView()
        {
            InitializeComponent();
            BindingContext = this;


            var clickGestureRecognizer = new TapGestureRecognizer();
            clickGestureRecognizer.Tapped += OnRate1Click;
            RateButton1.GestureRecognizers.Add(clickGestureRecognizer);

            clickGestureRecognizer = new TapGestureRecognizer();
            clickGestureRecognizer.Tapped += OnRate2Click;
            RateButton2.GestureRecognizers.Add(clickGestureRecognizer);

            clickGestureRecognizer = new TapGestureRecognizer();
            clickGestureRecognizer.Tapped += OnRate3Click;
            RateButton3.GestureRecognizers.Add(clickGestureRecognizer);

            clickGestureRecognizer = new TapGestureRecognizer();
            clickGestureRecognizer.Tapped += OnRate4Click;
            RateButton4.GestureRecognizers.Add(clickGestureRecognizer);

            clickGestureRecognizer = new TapGestureRecognizer();
            clickGestureRecognizer.Tapped += OnRate5Click;
            RateButton5.GestureRecognizers.Add(clickGestureRecognizer);

        }


        protected async void OnRate1Click(object sender, EventArgs args)
        {
            await SendFeedbackRatingResponse(1);
        }

        protected async void OnRate2Click(object sender, EventArgs args)
        {
            await SendFeedbackRatingResponse(2);
        }

        protected async void OnRate3Click(object sender, EventArgs args)
        {
            await SendFeedbackRatingResponse(3);
        }

        protected async void OnRate4Click(object sender, EventArgs args)
        {
            await SendFeedbackRatingResponse(4);
        }

        protected async void OnRate5Click(object sender, EventArgs args)
        {
            await SendFeedbackRatingResponse(5);
        }

        async Task SendFeedbackRatingResponse(int rating) {
            Uri uri = GetFeedbackUriForRating(rating);

            ButtonsVisible = false;
            ActivityIndicatorVisible = true;
            try{
                var response = await RESTService.ExecuteGetService(uri);
                OnRateSendSuccess?.Invoke(this, new EventArgs());
                SuccessMessageLabel.IsVisible = true;
            } catch (RESTException e) {
                RateSendErrorMessage = e.Message;
                OnRateSendError?.Invoke(this, new EventArgs());
                ErrorMessageLabel.IsVisible = true;
            } finally {
                ActivityIndicatorVisible = false;
            }
        }

        Uri GetFeedbackUriForRating(int rating) {
            Uri uri = new Uri($"{BaseRateRequestUrl}?birthDate={BirthDate.ToString("MM/dd/yyyy")}&birthWeekDay={BirthDate.ToString("dddd")}&gotDonut={GotDonut}&satisfactionRating={rating}");
            return uri;
        }


        public bool ButtonsVisible { 
            get => (bool)GetValue(ButtonsVisibleProperty);
            set => SetValue(ButtonsVisibleProperty, value);
        }
        public static readonly BindableProperty ButtonsVisibleProperty =
            BindableProperty.Create("ButtonsVisible", typeof(bool), typeof(RateExperienceView), true);


        public bool ActivityIndicatorVisible { 
            get => (bool)GetValue(ActivityIndicatorVisibleProperty);
            set => SetValue(ActivityIndicatorVisibleProperty, value);
        }
        public static readonly BindableProperty ActivityIndicatorVisibleProperty =
            BindableProperty.Create("ActivityIndicatorVisible", typeof(bool), typeof(RateExperienceView), false);
        
    }
}
