
using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using DreamDonut.iOS.Renderers;

[assembly: ExportRenderer(typeof(DatePicker), typeof(CenterTextDatePickerRenderer))]
namespace DreamDonut.iOS.Renderers
{
    public class CenterTextDatePickerRenderer : DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.TextAlignment = UITextAlignment.Center;
            }
        }
    }
}