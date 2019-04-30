using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Input;
using RewriteMe.Business.Extensions;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Controls
{
    public class StackedGrid : Grid
    {
        public static readonly BindableProperty SelectedCommandProperty = BindableProperty.Create(
            nameof(SelectedCommand),
            typeof(ICommand),
            typeof(StackedGrid));

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(StackedGrid),
            default(IEnumerable<object>),
            BindingMode.TwoWay,
            propertyChanged: ItemsSourceChanged);

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            nameof(SelectedItem),
            typeof(object),
            typeof(StackedGrid),
            null,
            BindingMode.TwoWay,
            propertyChanged: OnSelectedItemChanged);

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(StackedGrid),
            default(DataTemplate));

        private ICommand _innerSelectedCommand;
        private INotifyCollectionChanged _sourceCollection;

        public event EventHandler SelectedItemChanged;

        public ICommand SelectedCommand
        {
            get => (ICommand)GetValue(SelectedCommandProperty);
            set => SetValue(SelectedCommandProperty, value);
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var itemsLayout = (StackedGrid)bindable;
            itemsLayout.HookUp();
        }

        private void HookUp()
        {
            // Remove previous collection changed event
            if (_sourceCollection != null)
            {
                _sourceCollection.CollectionChanged -= HandleSourceCollectionChanged;
            }

            _sourceCollection = ItemsSource as INotifyCollectionChanged;

            // Subscribe to collection changed event
            if (_sourceCollection != null)
            {
                _sourceCollection.CollectionChanged += HandleSourceCollectionChanged;
            }

            HandleSourceCollectionChanged(_sourceCollection, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void HandleSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetItems();
        }

        protected void SetItems()
        {
            Children.Clear();

            _innerSelectedCommand = new Command<View>(
                                            view =>
                                            {
                                                SelectedItem = view.BindingContext;
                                                SelectedItem = null; // Allowing item second time selection
                                            });

            if (ItemsSource == null)
            {
                return;
            }

            ColumnDefinitions.Clear();
            var numberOfColumns = ItemsSource.GetCount();
            for (var i = 0; i < numberOfColumns; i++)
            {
                // Item column
                ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

                // Separator column
                if (i < numberOfColumns - 1)
                {
                    ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1) });
                }
            }

            var x = 0;
            foreach (var item in ItemsSource)
            {
                // Add item
                Children.Add(GetItemView(item), x++, 0);

                // Add separator
                if (x < ColumnDefinitions.Count - 1)
                {
                    var separator = CreateSeparator();
                    Children.Add(separator, x++, 0);
                }
            }

            SelectedItem = null;
        }

        private Grid CreateSeparator()
        {
            var separatorGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Margin = 0,
                Padding = 0,
            };
            separatorGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(.025, GridUnitType.Star) });
            separatorGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(.95, GridUnitType.Star) });
            separatorGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(.025, GridUnitType.Star) });
            separatorGrid.Children.Add(new ContentView { BackgroundColor = BackgroundColor }, 0, 0);
            separatorGrid.Children.Add(new ContentView { BackgroundColor = ((View)Parent).BackgroundColor }, 0, 1);
            separatorGrid.Children.Add(new ContentView { BackgroundColor = BackgroundColor }, 0, 2);
            return separatorGrid;
        }

        protected View GetItemView(object item)
        {
            var content = ItemTemplate.CreateContent();
            View view;
            var viewCell = content as ViewCell;
            if (viewCell != null)
            {
                view = viewCell.View;
            }
            else
            {
                view = content as View;
                if (view == null)
                {
                    throw new InvalidOperationException("ItemTemplate must either be a View or a ViewCell");
                }
            }

            view.BindingContext = item;

            var gesture = new TapGestureRecognizer { Command = _innerSelectedCommand, CommandParameter = view };

            AddGesture(view, gesture);

            return view;
        }

        private void AddGesture(View view, TapGestureRecognizer gesture)
        {
            view.GestureRecognizers.Add(gesture);

            var layout = view as Layout<View>;

            if (layout == null)
            {
                return;
            }

            foreach (var child in layout.Children)
            {
                AddGesture(child, gesture);
            }
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var itemsView = (StackedGrid)bindable;
            if (newValue == oldValue || newValue == null)
            {
                return;
            }

            itemsView.SelectedItemChanged?.Invoke(itemsView, EventArgs.Empty);

            if (itemsView.SelectedCommand?.CanExecute(newValue) ?? false)
            {
                itemsView.SelectedCommand?.Execute(newValue);
            }
        }
    }
}
