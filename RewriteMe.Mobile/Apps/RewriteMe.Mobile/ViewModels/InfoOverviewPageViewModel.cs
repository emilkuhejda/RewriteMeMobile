using System.Collections.Generic;
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
    public class InfoOverviewPageViewModel : OverviewBaseViewModel
    {
        private readonly ILanguageService _languageService;

        private IEnumerable<InformationMessageViewModel> _informationMessages;

        public InfoOverviewPageViewModel(
            ILanguageService languageService,
            IInformationMessageService informationMessageService,
            ISynchronizationService synchronizationService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(informationMessageService, synchronizationService, userSessionService, dialogService, navigationService, loggerFactory)
        {
            _languageService = languageService;
        }

        public IEnumerable<InformationMessageViewModel> InformationMessages
        {
            get => _informationMessages;
            set => SetProperty(ref _informationMessages, value);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                await InitializeNavigation(CurrentPage.InformationMessages).ConfigureAwait(false);

                var languageInfo = await _languageService.GetLanguageInfo().ConfigureAwait(false);
                var informationMessages = await InformationMessageService.GetAllForLastWeekAsync().ConfigureAwait(false);
                InformationMessages = informationMessages.Select(x => new InformationMessageViewModel(x, languageInfo, NavigationService)).ToList();

                NotAvailableData = !InformationMessages.Any();
            }
        }

        protected override async Task ExecuteNavigateToOverviewAsync()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add(NavigationConstants.NavigationBack, true);
            await NavigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.Overview}", navigationParameters).ConfigureAwait(false);

            DisposeInternal();
        }

        protected override async Task ExecuteNavigateToRecorderOverviewAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.RecorderOverview}").ConfigureAwait(false);

            DisposeInternal();
        }
    }
}
