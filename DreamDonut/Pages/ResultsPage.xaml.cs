using System;
using System.Collections.Generic;

using Xamarin.Forms;

using DreamDonut.Components;

namespace DreamDonut.Pages
{
    public partial class ResultsPage : BackgroundImagePage
    {


        public ResultsPage()
        {
            InitializeComponent();
        }

        public async void OnGoBackClicked(object sender, EventArgs args) {
            await Navigation.PopAsync();
        }

        public static ResultsPage GetResultsPageFromDate(DateTime birthDate) {

            ResultsPage resultPage;
            if(GetsPizza(birthDate)) 
                resultPage = new DonutResultsPage();
            else
                resultPage = new IceResultsPage();

            resultPage.BackgroundImageSource = "butterfly.jpg";
            resultPage.BirthDate = birthDate;

            return resultPage;
        }

        protected virtual RateExperienceView RateExpView { get; set; }


        protected virtual void UpdateReasonText() {}

        static bool GetsPizza(DateTime birthDate) {

            if(birthDate == DateTime.MinValue)
                return false;

            switch(birthDate.DayOfWeek) {
                case DayOfWeek.Monday:
                case DayOfWeek.Wednesday:
                case DayOfWeek.Friday:
                    return true;
                default:
                    return false;
            }
        }

        protected string GetDayOfWeekText(DateTime birthDate) {

            switch(birthDate.DayOfWeek) {
                case DayOfWeek.Monday:
                    return "Monday";
                case DayOfWeek.Tuesday:
                    return "Tuesday";
                case DayOfWeek.Wednesday:
                    return "Wednesday";
                case DayOfWeek.Thursday:
                    return "Thursday";
                case DayOfWeek.Friday:
                    return "Friday";
                case DayOfWeek.Saturday:
                    return "Saturday";
                case DayOfWeek.Sunday:
                    return "Sunday";

                default: return string.Empty;
            }
        }


        public string DayText
        {
            get => (string)GetValue(DayTextProperty);
            set => SetValue(DayTextProperty, value);
        }
        public readonly BindableProperty DayTextProperty =
            BindableProperty.Create("DayText", typeof(string), typeof(ResultsPage), null);

        public string ReasonText
        {
            get => (string)GetValue(ReasonTextProperty);
            set => SetValue(ReasonTextProperty, value);
        }
        public readonly BindableProperty ReasonTextProperty =
            BindableProperty.Create("ReasonText", typeof(string), typeof(ResultsPage), null);


        public DateTime BirthDate { 
            get => (DateTime)GetValue(BirthDateProperty);
            set {
                SetValue(BirthDateProperty, value);
            } 
        }
        public readonly BindableProperty BirthDateProperty =
            BindableProperty.Create("BirthDate", typeof(DateTime), typeof(ResultsPage), DateTime.MinValue, propertyChanged: OnBirthDateChanged);

        private static void OnBirthDateChanged(BindableObject bindable, object oldValue, object newValue)
        {

            if(newValue != null) 
            {

                ((ResultsPage)bindable).UpdateReasonText();
                ((ResultsPage)bindable).RateExpView.BirthDate = (DateTime)newValue;
                
            }
        }
    }
}
