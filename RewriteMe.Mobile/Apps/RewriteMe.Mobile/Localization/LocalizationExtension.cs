using System;
using System.Threading;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Resources.Localization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RewriteMe.Mobile.Localization
{
    [ContentProperty("Text")]
    public class LocalizationExtension : BindableObject, IMarkupExtension<BindingBase>
    {
        private static Lazy<ILocalizer> _localizer;

        public static void Init(Func<ILocalizer> localizerFunction)
        {
            _localizer = new Lazy<ILocalizer>(localizerFunction, LazyThreadSafetyMode.PublicationOnly);
        }

        public string Text { get; set; }

        public BindingBase ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding(nameof(TranslationData.Value))
            {
                Source = new TranslationData(Text, _localizer.Value, key => Loc.Text(key))
            };

            return binding;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }
    }
}
