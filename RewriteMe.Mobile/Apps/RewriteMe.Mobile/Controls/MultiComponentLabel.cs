using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Controls
{
    public sealed class MultiComponentLabel : Label
    {
        public static readonly BindableProperty ComponentsProperty =
            BindableProperty.Create(
                nameof(Components),
                typeof(IEnumerable<WordComponent>),
                typeof(MultiComponentLabel),
                propertyChanged: OnWordsPropertyChanged);

        public IEnumerable<WordComponent> Components
        {
            get => (IEnumerable<WordComponent>)GetValue(ComponentsProperty);
            set => SetValue(ComponentsProperty, value);
        }

        private static void OnWordsPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var multiComponentLabel = bindable as MultiComponentLabel;
            if (multiComponentLabel == null)
                return;

            if (multiComponentLabel.Components == null || !multiComponentLabel.Components.Any())
            {
                multiComponentLabel.FormattedText = default;
            }
            else
            {
                var formattedString = new FormattedString();
                foreach (var component in multiComponentLabel.Components)
                {
                    var span = new HighlightedSpan
                    {
                        Text = $"{component.Text} ",
                        BindingContext = component
                    };

                    span.SetBinding(HighlightedSpan.IsHighlightedProperty, nameof(WordComponent.IsHighlighted));

                    formattedString.Spans.Add(span);
                }

                multiComponentLabel.FormattedText = formattedString;
            }
        }
    }
}
