using System;
using System.Globalization;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Converters
{
    public class BoolToColorConverter : BindableObject, IValueConverter
    {
        public static readonly BindableProperty TrueValueProperty = BindableProperty.Create(nameof(TrueValue), typeof(Color), typeof(BoolToColorConverter), default(Color));
        public static readonly BindableProperty FalseValueProperty = BindableProperty.Create(nameof(FalseValue), typeof(Color), typeof(BoolToColorConverter), default(Color));

        public Color TrueValue
        {
            get => (Color)GetValue(TrueValueProperty);
            set => SetValue(TrueValueProperty, value);
        }

        public Color FalseValue
        {
            get => (Color)GetValue(FalseValueProperty);
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
