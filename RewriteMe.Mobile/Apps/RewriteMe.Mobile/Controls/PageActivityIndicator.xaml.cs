using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RewriteMe.Mobile.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageActivityIndicator
    {
        public static readonly BindableProperty CaptionProperty =
            BindableProperty.Create(
                nameof(Caption),
                typeof(string),
                typeof(PageActivityIndicator),
                null);

        public PageActivityIndicator()
        {
            InitializeComponent();
        }

        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }
    }
}