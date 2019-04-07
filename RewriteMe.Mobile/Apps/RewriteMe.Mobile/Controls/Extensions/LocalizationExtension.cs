using System;
using RewriteMe.Resources.Localization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RewriteMe.Mobile.Controls.Extensions
{
    [ContentProperty("Text")]
    public class LocalizationExtension : IMarkupExtension
    {
        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return Loc.Text(Text);
        }
    }
}
