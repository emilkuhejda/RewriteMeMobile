﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Converters
{
    public class NullToTrueBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
