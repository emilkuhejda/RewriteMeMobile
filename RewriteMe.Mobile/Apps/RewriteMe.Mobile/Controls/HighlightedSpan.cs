using Prism;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Controls
{
    public class HighlightedSpan : Span
    {
        public static readonly BindableProperty IsHighlightedProperty =
            BindableProperty.Create(
                nameof(IsHighlighted),
                typeof(bool),
                typeof(HighlightedSpan),
                propertyChanged: OnIsHighlightedPropertyChanged);

        public bool IsHighlighted
        {
            get => (bool)GetValue(IsHighlightedProperty);
            set => SetValue(IsHighlightedProperty, value);
        }

        private static void OnIsHighlightedPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var highlightedSpan = bindable as HighlightedSpan;
            if (highlightedSpan == null)
                return;

            if (highlightedSpan.IsHighlighted)
            {
                highlightedSpan.BackgroundColor = (Color)PrismApplicationBase.Current.Resources["HighlightedBackgroundColor"];
            }
            else
            {
                highlightedSpan.BackgroundColor = Color.Transparent;
            }
        }
    }
}
