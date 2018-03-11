using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace DreamDonut.Pages
{
    public partial class HomePage : BackgroundImagePage
    {
        public HomePage()
        {
            InitializeComponent();
            Icon = "logoWhite";
        }


        async void OnSubmitClicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(ResultsPage.GetResultsPageFromDate(MainDatePicker.Date));

        }


    }
}
