using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RewriteMe.Mobile.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageActivityIndicator
    {
        public static readonly BindableProperty CaptionProperty = BindableProperty.Create(
                nameof(Caption),
                typeof(string),
                typeof(PageActivityIndicator),
                null);

        public static readonly BindableProperty ColorProperty = BindableProperty.Create(
            nameof(Color),
            typeof(Color),
            typeof(PageActivityIndicator),
            Color.Black);

        public static readonly BindableProperty BackgroundOpacityProperty = BindableProperty.Create(
            nameof(BackgroundOpacity),
            typeof(double),
            typeof(PageActivityIndicator),
            0.8);

        public PageActivityIndicator()
        {
            InitializeComponent();
        }

        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public double BackgroundOpacity
        {
            get => (double)GetValue(BackgroundOpacityProperty);
            set => SetValue(BackgroundOpacityProperty, value);
        }
    }
}