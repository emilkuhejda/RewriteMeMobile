using RewriteMe.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RewriteMe.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TranscriptionDetailPage
    {
        public TranscriptionDetailPage()
        {
            InitializeComponent();
        }

        private void VisualElement_OnFocused(object sender, FocusEventArgs e)
        {
            var bindingContext = BindingContext as TranscriptionDetailPageViewModel;
            if (bindingContext == null)
                return;

            bindingContext.TapCommand.Execute(this);
        }
    }
}