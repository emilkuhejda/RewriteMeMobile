using System;
using System.Globalization;
using Syncfusion.XForms.BadgeView;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Converters
{
    public class BoolToBadgeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? BadgeIcon.Dot : BadgeIcon.None;
            }

            return BadgeIcon.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
