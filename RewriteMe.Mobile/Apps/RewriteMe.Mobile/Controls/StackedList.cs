using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Input;
using RewriteMe.Mobile.Utils;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Controls
{
    public class StackedList : Grid
    {
        private readonly NoBarsScrollViewer _scrollView;
        private readonly StackLayout _itemsStackLayout;
        private ICommand _innerSelectedCommand;
        private INotifyCollectionChanged _sourceCollection;

        public event EventHandler SelectedItemChanged;

        public static readonly BindableProperty SelectedCommandProperty = BindableProperty.Create(
            nameof(SelectedCommand),
            typeof(ICommand),
            typeof(StackedList));

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(StackedList),
            default(IEnumerable<object>),
            BindingMode.TwoWay,
            propertyChanged: ItemsSourceChanged);

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            nameof(SelectedItem),
            typeof(object),
            typeof(StackedList),
            null,
            BindingMode.TwoWay,
            propertyChanged: OnSelectedItemChanged);

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(StackedList),
            default(DataTemplate));

        public virtual ICommand SelectedCommand
        {
            get => (ICommand)GetValue(SelectedCommandProperty);
            set => SetValue(SelectedCommandProperty, value);
        }

        public virtual IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public virtual object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        protected IList<View> ItemViews => _itemsStackLayout.Children;

        public StackOrientation ListOrientation { get; set; }

        public double Spacing { get; set; }

        private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var itemsLayout = (StackedList)bindable;
            itemsLayout.HookUp();
        }

        public StackedList()
        {
            Spacing = 5;
            _scrollView = new NoBarsScrollViewer();
            _itemsStackLayout = new StackLayout
            {
                Padding = Padding,
                Spacing = Spacing,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            _scrollView.Content = _itemsStackLayout;
            Children.Add(_scrollView);
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
            ThreadHelper.InvokeOnUiThread(SetItems);
        }

        protected virtual void SetItems()
        {
            _itemsStackLayout.Children.Clear();
            _itemsStackLayout.Spacing = Spacing;

            _innerSelectedCommand = new Command<View>(
                                            view =>
                                            {
                                                SelectedItem = view.BindingContext;
                                                SelectedItem = null; // Allowing item second time selection
                                            });

            _itemsStackLayout.Orientation = ListOrientation;
            _scrollView.Orientation = ListOrientation == StackOrientation.Horizontal ? ScrollOrientation.Horizontal : ScrollOrientation.Vertical;

            if (ItemsSource == null)
            {
                return;
            }

            foreach (var item in ItemsSource)
            {
                _itemsStackLayout.Children.Add(GetItemView(item));
            }

            SelectedItem = null;
        }

        private DataTemplate GetItemTemplate(object item)
        {
            if (ItemTemplate is DataTemplateSelector dataTemplateSelector)
            {
                return dataTemplateSelector.SelectTemplate(item, this);
            }

            return ItemTemplate;
        }

        protected virtual View GetItemView(object item)
        {
            var dataTemplate = GetItemTemplate(item);
            var content = dataTemplate.CreateContent();
            View view;

            if (content is ViewCell viewCell)
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

            if (!(view is Layout<View> layout))
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
            var itemsView = (StackedList)bindable;
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
