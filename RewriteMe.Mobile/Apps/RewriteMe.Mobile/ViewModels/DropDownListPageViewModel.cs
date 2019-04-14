﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation.Parameters;

namespace RewriteMe.Mobile.ViewModels
{
    public class DropDownListPageViewModel : ViewModelBase
    {
        private IEnumerable<DropDownListViewModel> _items;
        private DropDownListViewModel _selectedItem;

        public DropDownListPageViewModel(
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            CanGoBack = true;

            SelectedItemChangedCommand = new AsyncCommand(ExecuteSelectedItemChangedCommandAsync, CanExecuteSelectedItemChangedCommand);
        }

        public IEnumerable<DropDownListViewModel> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public DropDownListViewModel SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public ICommand SelectedItemChangedCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                var dropDownListNavigationParameters = navigationParameters.GetValue<DropDownListNavigationParameters>();
                Title = dropDownListNavigationParameters.Title;
                Items = dropDownListNavigationParameters.Items;

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        private bool CanExecuteSelectedItemChangedCommand()
        {
            return SelectedItem != null;
        }

        private async Task ExecuteSelectedItemChangedCommandAsync()
        {
            var navigationParameter = new NavigationParameters();
            navigationParameter.Add<DropDownListViewModel>(SelectedItem);

            await NavigationService.GoBackWithoutAnimationAsync(navigationParameter).ConfigureAwait(false);
        }
    }
}
