using System.Windows.Input;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Controls
{
    public class NavigationToolbarItem : BindableObject
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(
                nameof(Text),
                typeof(string),
                typeof(NavigationToolbarItem));

        public static readonly BindableProperty ImageSourceProperty =
            BindableProperty.Create(
                nameof(Source),
                typeof(string),
                typeof(NavigationToolbarItem));

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(
                nameof(Command),
                typeof(ICommand),
                typeof(NavigationToolbarItem));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string Source
        {
            get => (string)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
    }
}
