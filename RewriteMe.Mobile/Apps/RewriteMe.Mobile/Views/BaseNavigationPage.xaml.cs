using System.Collections.ObjectModel;
using System.Collections.Specialized;
using RewriteMe.Mobile.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RewriteMe.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BaseNavigationPage
    {
        public static readonly BindableProperty HasNavigationBarProperty =
            BindableProperty.Create(
                nameof(HasNavigationBar),
                typeof(bool),
                typeof(BaseNavigationPage),
                defaultValue: true);

        public BaseNavigationPage()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            var rightToolbarItems = new ObservableCollection<NavigationToolbarItem>();
            rightToolbarItems.CollectionChanged += OnToolbarItemsCollectionChanged;
            RightNavigationToolbarItems = rightToolbarItems;

            InitializeComponent();
        }

        public bool HasNavigationBar
        {
            get => (bool)GetValue(HasNavigationBarProperty);
            set => SetValue(HasNavigationBarProperty, value);
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
    }
}