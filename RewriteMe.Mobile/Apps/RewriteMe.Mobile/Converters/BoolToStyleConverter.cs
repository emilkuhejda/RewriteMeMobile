using System;
using System.Globalization;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Converters
{
    public class BoolToStyleConverter : BindableObject, IValueConverter
    {
        public static readonly BindableProperty TrueValueProperty = BindableProperty.Create(nameof(TrueValue), typeof(Style), typeof(BoolToStyleConverter), default(Style));
        public static readonly BindableProperty FalseValueProperty = BindableProperty.Create(nameof(FalseValue), typeof(Style), typeof(BoolToStyleConverter), default(Style));

        public Style TrueValue
        {
            get => (Style)GetValue(TrueValueProperty);
            set => SetValue(TrueValueProperty, value);
        }

        public Style FalseValue
        {
            get => (Style)GetValue(FalseValueProperty);
            set => SetValue(FalseValueProperty, value);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var obj = FalseValue;
            if (value is bool b)
                obj = b ? TrueValue : FalseValue;

            return obj;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
