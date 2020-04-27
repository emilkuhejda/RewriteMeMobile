using System.Collections.Generic;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Controls
{
    public class MultiComponentLabel : Label
    {
        public static readonly BindableProperty ComponentsProperty =
            BindableProperty.Create(
                nameof(Components),
                typeof(IEnumerable<LabelComponent>),
                typeof(MultiComponentLabel),
                propertyChanged: OnWordsPropertyChanged);

        public IEnumerable<LabelComponent> Components
        {
            get => (IEnumerable<LabelComponent>)GetValue(ComponentsProperty);
            set => SetValue(ComponentsProperty, value);
        }

        private static void OnWordsPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var multiComponentLabel = bindable as MultiComponentLabel;
            if (multiComponentLabel?.Components == null)
                return;

            var formattedString = new FormattedString();
            foreach (var component in multiComponentLabel.Components)
            {
                var span = new HighlightedSpan
                {
                    Text = $"{component.Text} ",
                    BindingContext = component
                };

                span.SetBinding(HighlightedSpan.IsHighlightedProperty, nameof(LabelComponent.IsHighlighted));

                formattedString.Spans.Add(span);
            }

            multiComponentLabel.FormattedText = formattedString;
        }
    }
}
