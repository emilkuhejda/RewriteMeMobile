using Xamarin.Forms;

namespace RewriteMe.Mobile.Controls
{
    public sealed class CustomButton : Button
    {
        public static readonly BindableProperty BackgroundColorPressedProperty =
            BindableProperty.Create(
                nameof(BackgroundColorPressed),
                typeof(Color),
                typeof(CustomButton),
                Color.White);

        public Color BackgroundColorPressed
        {
            get => (Color)GetValue(BackgroundColorPressedProperty);
            set => SetValue(BackgroundColorPressedProperty, value);
        }

        public static readonly BindableProperty BorderColorPressedProperty =
            BindableProperty.Create(
                nameof(BorderColorPressed),
                typeof(Color),
                typeof(CustomButton),
                Color.White);

        public Color BorderColorPressed
        {
            get => (Color)GetValue(BorderColorPressedProperty);
            set => SetValue(BorderColorPressedProperty, value);
        }

        public static readonly BindableProperty VerticalContentAlignmentProperty =
            BindableProperty.Create(
                nameof(VerticalContentAlignment),
                typeof(TextAlignment),
                typeof(CustomButton),
                TextAlignment.Center);

        public TextAlignment VerticalContentAlignment
        {
            get => (TextAlignment)GetValue(VerticalContentAlignmentProperty);
            set => SetValue(VerticalContentAlignmentProperty, value);
        }

        public static readonly BindableProperty HorizontalContentAlignmentProperty =
            BindableProperty.Create(
                nameof(HorizontalContentAlignment),
                typeof(TextAlignment),
                typeof(CustomButton),
                TextAlignment.Center);

        public TextAlignment HorizontalContentAlignment
        {
            get => (TextAlignment)GetValue(HorizontalContentAlignmentProperty);
            set => SetValue(HorizontalContentAlignmentProperty, value);
        }
    }
}
