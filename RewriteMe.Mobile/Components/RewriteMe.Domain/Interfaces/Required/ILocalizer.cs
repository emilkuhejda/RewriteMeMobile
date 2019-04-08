using System;
using System.Globalization;
using RewriteMe.Domain.Events;

namespace RewriteMe.Domain.Interfaces.Required
{
    public interface ILocalizer
    {
        event EventHandler<CultureInfoChangedEventArgs> CultureInfoChanged;

        CultureInfo GetCurrentCulture();

        void SetCultureInfo(CultureInfo cultureInfo);
    }
}
