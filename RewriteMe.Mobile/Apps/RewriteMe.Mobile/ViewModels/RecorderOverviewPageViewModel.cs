using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.LatestVersion.Abstractions;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Extensions;

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
            IInternalValueService internalValueService,
            IEmailService emailService,
            ILatestVersion latestVersion,
            IApplicationSettings applicationSettings,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(synchronizationService, userSessionService, internalValueService, emailService, latestVersion, applicationSettings, dialogService, navigationService, loggerFactory)
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
                var userId = await UserSessionService.GetUserIdAsync().ConfigureAwait(false);
                var items = await _recordedItemService.GetAllAsync(userId).ConfigureAwait(false);
                RecordedItems = items
                    .OrderByDescending(x => x.DateCreated)
                    .Select(x => new RecordedItemViewModel(x, NavigationService))
                    .ToList();

                IsNotUserRegistrationSuccess = !await InternalValueService.GetValueAsync(InternalValues.IsUserRegistrationSuccess).ConfigureAwait(false);
                NotAvailableData = !RecordedItems.Any();

                InitializeNavigation(false);
            }
        }

        protected override async Task ExecuteNavigateToOverviewAsync()
        {
            await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
        }

        protected override async Task ExecuteNavigateToRecorderOverviewAsync()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
