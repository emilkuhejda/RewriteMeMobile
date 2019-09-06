using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Extensions;

namespace RewriteMe.Mobile.ViewModels
{
    public class InformationMessagesPageViewModel : OverviewBaseViewModel
    {
        private readonly IInformationMessageService _informationMessageService;
        private readonly ILanguageService _languageService;

        private IEnumerable<InformationMessageViewModel> _informationMessages;

        public InformationMessagesPageViewModel(
            IInformationMessageService informationMessageService,
            ILanguageService languageService,
            ISynchronizationService synchronizationService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(synchronizationService, userSessionService, dialogService, navigationService, loggerFactory)
        {
            _informationMessageService = informationMessageService;
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
                InitializeNavigation(CurrentPage.InformationMessages);

                var languageInfo = await _languageService.GetLanguageInfo().ConfigureAwait(false);
                var informationMessages = await _informationMessageService.GetAllAsync().ConfigureAwait(false);
                InformationMessages = informationMessages.Select(x => new InformationMessageViewModel(x, languageInfo, NavigationService)).ToList();

                NotAvailableData = !InformationMessages.Any();
            }
        }

        protected override async Task ExecuteNavigateToOverviewAsync()
        {
            await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
        }

        protected override async Task ExecuteNavigateToRecorderOverviewAsync()
        {
            await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
        }
    }
}
