using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RewriteMe.Mobile.Controls
{
    [ContentProperty("ContainerContent")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrillDownButton
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(
                nameof(Text),
                typeof(string),
                typeof(DrillDownButton),
                string.Empty);

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(
                nameof(Command),
                typeof(ICommand),
                typeof(DrillDownButton));

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(
                nameof(CommandParameter),
                typeof(object),
                typeof(DrillDownButton));

        public static new readonly BindableProperty IsEnabledProperty =
            BindableProperty.Create(
                nameof(IsEnabled),
                typeof(bool),
                typeof(DrillDownButton),
                true);

        public static readonly BindableProperty IsBusyProperty =
            BindableProperty.Create(
                nameof(IsBusy),
                typeof(bool),
                typeof(DrillDownButton),
                false);

        public static readonly BindableProperty OutlineColorProperty =
            BindableProperty.Create(
                nameof(OutlineColor),
                typeof(Color),
                typeof(DrillDownButton),
                Color.Default);

        public static readonly BindableProperty RowHeightProperty =
            BindableProperty.Create(
                nameof(RowHeight),
                typeof(double),
                typeof(DrillDownButton),
                -1.0d);

        public DrillDownButton()
        {
            InitializeComponent();
        }

        public View ContainerContent
        {
            get => ContentPresenter?.Content;
            set
            {
                if (ContentPresenter != null)
                {
                    ContentPresenter.Content = value;
                }
            }
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public new bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        public bool IsBusy
        {
            get => (bool)GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }

        public Color OutlineColor
        {
            get => (Color)GetValue(OutlineColorProperty);
            set => SetValue(OutlineColorProperty, value);
        }

        public double RowHeight
        {
            get => (double)GetValue(RowHeightProperty);
            set => SetValue(RowHeightProperty, value);
        }
    }
}