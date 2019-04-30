using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RewriteMe.Mobile.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TileCellGrid : Grid
    {
        public static readonly BindableProperty HoverBackgroundColorProperty =
            BindableProperty.Create(
                nameof(HoverBackgroundColor),
                typeof(Color),
                typeof(TileCellGrid),
                Color.Default);

        public static readonly BindableProperty TouchBackgroundColorProperty =
            BindableProperty.Create(
                nameof(TouchBackgroundColor),
                typeof(Color),
                typeof(TileCellGrid),
                Color.Default);

        public TileCellGrid()
        {
            InitializeComponent();
        }

        public Color HoverBackgroundColor
        {
            get => (Color)GetValue(HoverBackgroundColorProperty);
            set => SetValue(HoverBackgroundColorProperty, value);
        }

        public Color TouchBackgroundColor
        {
            get => (Color)GetValue(TouchBackgroundColorProperty);
            set => SetValue(TouchBackgroundColorProperty, value);
        }
    }
}