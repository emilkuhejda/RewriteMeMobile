using System.Globalization;

namespace RewriteMe.Domain.Events
{
    public class CultureInfoChangedEventArgs : System.EventArgs
    {
        public CultureInfoChangedEventArgs(CultureInfo cultureInfo)
        {
            CultureInfo = cultureInfo;
        }

        public CultureInfo CultureInfo { get; }
    }
}
