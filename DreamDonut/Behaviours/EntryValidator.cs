using System;
using System.Text.RegularExpressions;

using Xamarin.Forms;

namespace DreamDonut.Behaviors
{
	public class EntryValidator : Behavior<Entry>
	{

		public int MinLength { get; set; }
		public bool IsRequired { get; set; }
        public bool IsNumeric { get; set; }
        public bool IsEmail { get; set; }
		public string RegexMatch { get; set; }
		public string FieldName { get; set; }
        public string DynamicErrorStyle { get; set; }

		static readonly BindablePropertyKey ErrorTextPropertyKey = BindableProperty.CreateReadOnly("ErrorText", typeof(string), typeof(EntryValidator), string.Empty);
		public static readonly BindableProperty ErrorTextProperty = ErrorTextPropertyKey.BindableProperty;
		public string ErrorText
		{
			get { return (string)GetValue(ErrorTextProperty); }
			private set { SetValue(ErrorTextPropertyKey, value); }
		}

		static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(EntryValidator), true);
		public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;
		public bool IsValid
		{
			get { return (bool) GetValue(IsValidProperty); }
			private set { SetValue(IsValidPropertyKey, value); }
		}


		Entry bindable;
        Style originalStyle = null;
		protected override void OnAttachedTo(Entry bindable)
		{
			this.bindable = bindable;
			this.bindable.TextChanged += HandleTextChanged;
            originalStyle = bindable.Style;
		}

		public bool TriggerChangeAndGetIsValid()
		{
			ExecuteHandleTextChanged(bindable, bindable.Text);
			return IsValid;
		}

		void HandleTextChanged(object sender, TextChangedEventArgs e)
		{
			ExecuteHandleTextChanged((Entry)sender, e.NewTextValue);
		}
		void ExecuteHandleTextChanged(Entry entry, string newTextValue)
		{
			ErrorText = string.Empty;


			if (IsRequired && string.IsNullOrEmpty(newTextValue))
			{
				ErrorText += FieldName + " is required.";
                IsValid = false;
                if(!string.IsNullOrEmpty(DynamicErrorStyle))
                    entry.SetDynamicResource (VisualElement.StyleProperty, DynamicErrorStyle);
				return;
			}

			bool correctLength = MinLength <= 0 || !string.IsNullOrEmpty(newTextValue) && newTextValue.Length >= MinLength;
			if (!correctLength)
			{
				ErrorText += FieldName + " must be at least " + MinLength + " " + (IsNumeric ? "digits" : "characters") + ".";
                IsValid = false;
                if(!string.IsNullOrEmpty(DynamicErrorStyle))
                    entry.SetDynamicResource (VisualElement.StyleProperty, DynamicErrorStyle);
				return;
			}


			if (IsNumeric)
			{
				Regex rgx = new Regex("[^0-9]");
				string matchedValue = rgx.Replace(newTextValue, string.Empty);
				newTextValue = rgx.Replace(newTextValue, string.Empty);
			}


			if (!string.IsNullOrEmpty(RegexMatch) && !Regex.IsMatch(newTextValue, RegexMatch, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
			{
				ErrorText += FieldName + " is in the wrong format.";
                IsValid = false;
                if(!string.IsNullOrEmpty(DynamicErrorStyle))
                    entry.SetDynamicResource (VisualElement.StyleProperty, DynamicErrorStyle);
				return;
			}


            if(IsEmail)
            {
                string emailRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
                if(!Regex.IsMatch(newTextValue, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))){
                    ErrorText += "Please enter a valid email address.";
                    IsValid = false;

                    if(!string.IsNullOrEmpty(DynamicErrorStyle))
                        entry.SetDynamicResource (VisualElement.StyleProperty, DynamicErrorStyle);
                    return;
                }
            }

			//if we are here then there are no errors.
			entry.Text = newTextValue;
			ErrorText = string.Empty;
			IsValid = true;

            entry.Style = originalStyle;

		}

		protected override void OnDetachingFrom(Entry bindable)
		{
			bindable.TextChanged -= HandleTextChanged;

		}
	}

}

