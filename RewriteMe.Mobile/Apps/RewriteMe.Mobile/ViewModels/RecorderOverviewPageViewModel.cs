﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecorderOverviewPageViewModel : OverviewBaseViewModel
    {
        private readonly IRecordedItemService _recordedItemService;

        private IEnumerable<RecordedItemViewModel> _recordedItems;

        public RecorderOverviewPageViewModel(
            IRecordedItemService recordedItemService,
            ISynchronizationService synchronizationService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(synchronizationService, userSessionService, dialogService, navigationService, loggerFactory)
        {
            _recordedItemService = recordedItemService;
        }

        public IEnumerable<RecordedItemViewModel> RecordedItems
        {
            get => _recordedItems;
            set => SetProperty(ref _recordedItems, value);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                InitializeNavigation(false);

                var userId = await UserSessionService.GetUserIdAsync().ConfigureAwait(false);
                var items = await _recordedItemService.GetAllAsync(userId).ConfigureAwait(false);
                RecordedItems = items
                    .OrderByDescending(x => x.DateCreated)
                    .Select(x => new RecordedItemViewModel(x, NavigationService))
                    .ToList();

                NotAvailableData = !RecordedItems.Any();
            }
        }

        protected override async Task ExecuteNavigateToOverviewAsync()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add(NavigationConstants.NavigationBack, true);
            await NavigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.Overview}", navigationParameters).ConfigureAwait(false);
        }

        protected override async Task ExecuteNavigateToRecorderOverviewAsync()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
