using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace DreamDonut.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            Icon = "logoWhite";
        }


        async void OnSubmitClicked(object sender, System.EventArgs e)
        {
            DateTime birthDate = MainDatePicker.Date;
            Page resultPage;
            if(GetsPizza(birthDate)) 
                resultPage = new DonutResultsPage();
            else
                resultPage = new IceResultsPage();

            await Navigation.PushAsync(resultPage);
        }


        bool GetsPizza(DateTime birthDate) {

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
