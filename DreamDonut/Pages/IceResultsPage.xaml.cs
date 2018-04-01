using System;
using System.Collections.Generic;

using Xamarin.Forms;

using DreamDonut.Components;

namespace DreamDonut.Pages
{
    public partial class IceResultsPage : ResultsPage
    {
        public IceResultsPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override RateExperienceView RateExpView { get => RTView; }

		protected override void UpdateReasonText()
		{
            base.UpdateReasonText();
            DayText = GetDayOfWeekText(BirthDate);

            ReasonText = $"You were not born on the correct day of the week to grant you the privilege to a donut.";
		}
	}
}
