using System;
using System.Collections.Generic;

using Xamarin.Forms;

using DreamDonut.Components;

namespace DreamDonut.Pages
{
    public partial class DonutResultsPage : ResultsPage
    {
        public DonutResultsPage()
        {
            InitializeComponent();
            BindingContext = this;
        }


        protected override RateExperienceView RateExpView { get => RTView; }

        protected override void UpdateReasonText()
        {
            base.UpdateReasonText();
            DayText = GetDayOfWeekText(BirthDate);

            ReasonText = $"The day in which you were born grants you the privilege to a donut! Enjoy.";
        }
    }
}
