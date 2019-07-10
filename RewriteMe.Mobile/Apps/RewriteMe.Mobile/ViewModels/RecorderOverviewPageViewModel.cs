using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.LatestVersion.Abstractions;
using Plugin.Messaging;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecorderOverviewPageViewModel : OverviewBaseViewModel
    {
        private readonly IRecordedItemService _recordedItemService;

        private IEnumerable<RecordedItemViewModel> _recordedItems;

        public RecorderOverviewPageViewModel(
            IRecordedItemService recordedItemService,
            IUserSessionService userSessionService,
            IInternalValueService internalValueService,
            ILatestVersion latestVersion,
            IEmailTask emailTask,
            IApplicationSettings applicationSettings,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, internalValueService, latestVersion, emailTask, applicationSettings, dialogService, navigationService, loggerFactory)
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
                var items = await _recordedItemService.GetAllAsync().ConfigureAwait(false);
                RecordedItems = items
                    .OrderByDescending(x => x.DateCreated)
                    .Select(x => new RecordedItemViewModel(x, NavigationService))
                    .ToList();

                IsUserRegistrationSuccess = await InternalValueService.GetValueAsync(InternalValues.IsUserRegistrationSuccess).ConfigureAwait(false);

                InitializeNavigation(false);
            }
        }
    }
}
