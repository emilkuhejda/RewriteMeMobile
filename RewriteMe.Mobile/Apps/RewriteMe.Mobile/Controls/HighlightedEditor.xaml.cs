using System.Collections.Generic;
using System.Windows.Input;
using Prism;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RewriteMe.Mobile.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HighlightedEditor
    {
        public static readonly BindableProperty EditorUnFocusedCommandProperty =
            BindableProperty.Create(
                nameof(EditorUnFocusedCommand),
                typeof(ICommand),
                typeof(HighlightedEditor));

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(
                nameof(Text),
                typeof(string),
                typeof(HighlightedEditor),
                default(string),
                BindingMode.TwoWay);

        public static readonly BindableProperty WordsProperty =
            BindableProperty.Create(
                nameof(Words),
                typeof(IEnumerable<WordComponent>),
                typeof(HighlightedEditor));

        public static readonly BindableProperty IsHighlightEnabledProperty =
            BindableProperty.Create(
                nameof(IsHighlightEnabled),
                typeof(bool),
                typeof(HighlightedEditor),
                false,
                BindingMode.TwoWay,
                propertyChanged: OnIsHighlightEnabledPropertyChanged);

        public HighlightedEditor()
        {
            InitializeComponent();

            SetEditorTextColor();
        }

        public ICommand EditorUnFocusedCommand
        {
            get => (ICommand)GetValue(EditorUnFocusedCommandProperty);
            set => SetValue(EditorUnFocusedCommandProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public IEnumerable<WordComponent> Words
        {
            get => (IEnumerable<WordComponent>)GetValue(WordsProperty);
            set => SetValue(WordsProperty, value);
        }

        public bool IsHighlightEnabled
        {
            get => (bool)GetValue(IsHighlightEnabledProperty);
            set => SetValue(IsHighlightEnabledProperty, value);
        }

        public void SetEditorTextColor()
        {
            if (IsHighlightEnabled)
            {
                Editor.TextColor = Color.Transparent;
            }
            else
            {
                Editor.TextColor = (Color)PrismApplicationBase.Current.Resources["PrimaryTextColor"];
            }
        }

        private static void OnIsHighlightEnabledPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var highlightedEditor = bindable as HighlightedEditor;

            highlightedEditor?.SetEditorTextColor();
        }

        private void OnFocused(object sender, FocusEventArgs e)
        {
            IsHighlightEnabled = false;
        }

        private void OnUnfocused(object sender, FocusEventArgs e)
        {
            EditorUnFocusedCommand?.Execute(this);
        }
    }
}