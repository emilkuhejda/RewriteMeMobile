using System;
using System.Globalization;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Converters
{
    public class BoolToGridLengthConverter : BindableObject, IValueConverter
    {
        public static readonly BindableProperty TrueValueProperty = BindableProperty.Create(nameof(TrueValue), typeof(GridLength), typeof(BoolToGridLengthConverter), default(GridLength));
        public static readonly BindableProperty FalseValueProperty = BindableProperty.Create(nameof(FalseValue), typeof(GridLength), typeof(BoolToGridLengthConverter), default(GridLength));

        public GridLength TrueValue
        {
            get => (GridLength)GetValue(TrueValueProperty);
            set => SetValue(TrueValueProperty, value);
        }

        public GridLength FalseValue
        {
            get => (GridLength)GetValue(FalseValueProperty);
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
