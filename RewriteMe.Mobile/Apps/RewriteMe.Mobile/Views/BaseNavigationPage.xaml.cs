using System.Collections.ObjectModel;
using System.Collections.Specialized;
using RewriteMe.Mobile.Controls;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace RewriteMe.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BaseNavigationPage
    {
        public BaseNavigationPage()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            var rightToolbarItems = new ObservableCollection<NavigationToolbarItem>();
            rightToolbarItems.CollectionChanged += OnToolbarItemsCollectionChanged;
            RightNavigationToolbarItems = rightToolbarItems;

            InitializeComponent();
        }

        public ObservableCollection<NavigationToolbarItem> RightNavigationToolbarItems { get; internal set; }

        private void OnToolbarItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action != NotifyCollectionChangedAction.Add)
                return;

            foreach (var item in args.NewItems)
            {
                var bindableObject = item as BindableObject;
                SetInheritedBindingContext(bindableObject, BindingContext);
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            foreach (var toolbarItem in RightNavigationToolbarItems)
            {
                SetInheritedBindingContext(toolbarItem, BindingContext);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Device.RuntimePlatform == Device.iOS)
            {
                var safeInsets = On<iOS>().SafeAreaInsets();
                Padding = safeInsets;
            }
        }
    }
}