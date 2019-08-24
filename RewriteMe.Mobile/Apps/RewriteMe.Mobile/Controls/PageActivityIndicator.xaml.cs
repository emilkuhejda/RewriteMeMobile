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
                typeof(PageActivityIndicator));

        public static readonly BindableProperty CaptionColorProperty = BindableProperty.Create(
            nameof(CaptionColor),
            typeof(Color),
            typeof(PageActivityIndicator),
            Color.White);

        public static readonly BindableProperty IndicatorBackgroundColorProperty = BindableProperty.Create(
            nameof(IndicatorBackgroundColor),
            typeof(Color),
            typeof(PageActivityIndicator),
            Color.Black);

        public static readonly BindableProperty BackgroundOpacityProperty = BindableProperty.Create(
            nameof(BackgroundOpacity),
            typeof(double),
            typeof(PageActivityIndicator),
            0.6);

        public PageActivityIndicator()
        {
            InitializeComponent();
        }

        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        public Color CaptionColor
        {
            get => (Color)GetValue(CaptionColorProperty);
            set => SetValue(CaptionColorProperty, value);
        }

        public Color IndicatorBackgroundColor
        {
            get => (Color)GetValue(IndicatorBackgroundColorProperty);
            set => SetValue(IndicatorBackgroundColorProperty, value);
        }

        public double BackgroundOpacity
        {
            get => (double)GetValue(BackgroundOpacityProperty);
            set => SetValue(BackgroundOpacityProperty, value);
        }
    }
}