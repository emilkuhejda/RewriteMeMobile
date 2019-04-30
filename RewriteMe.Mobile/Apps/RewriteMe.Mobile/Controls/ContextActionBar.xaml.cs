using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RewriteMe.Mobile.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContextActionBar : Grid
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(ContextActionBar),
            default(IEnumerable<object>),
            BindingMode.TwoWay,
            propertyChanged: ItemsSourceChanged);

        public ContextActionBar()
        {
            InitializeComponent();
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private static void ItemsSourceChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var contextActionBar = (ContextActionBar)bindable;
            contextActionBar.StackedGrid.ItemsSource = (IEnumerable)newvalue;
        }
    }
}