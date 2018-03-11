using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace DreamDonut.Pages
{
    public partial class ResultsPage : BackgroundImagePage
    {

        DateTime SubmittedBirthDate { get; set; }

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
            resultPage.SubmittedBirthDate = birthDate;

            return resultPage;
        }


        private static bool GetsPizza(DateTime birthDate) {

            if(birthDate == null)
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
    }
}
