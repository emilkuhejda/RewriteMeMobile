using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Mobile.ViewModels
{
    public class InfoOverviewPageViewModel : OverviewBaseViewModel
    {
        private readonly ILanguageService _languageService;

        private ObservableCollection<InformationMessageViewModel> _informationMessages;

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

        public ObservableCollection<InformationMessageViewModel> InformationMessages
        {
            get => _informationMessages;
            private set => SetProperty(ref _informationMessages, value);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                await InitializeNavigation(CurrentPage.InformationMessages).ConfigureAwait(false);
                await InitializeInformationMessageAsync().ConfigureAwait(false);

                NotAvailableData = !InformationMessages.Any();
            }
        }

        protected override async Task RefreshList()
        {
            var informationMessages = await InformationMessageService.GetAllForLastWeekAsync().ConfigureAwait(false);

            if (IsCurrent)
            {
                var languageInfo = await _languageService.GetLanguageInfo().ConfigureAwait(false);
                var informationMessagesToAdd = informationMessages.Where(x => !InformationMessages.Select(info => info.Id).Contains(x.Id));
                foreach (var informationMessageToAdd in informationMessagesToAdd)
                {
                    InformationMessages.Insert(0, new InformationMessageViewModel(informationMessageToAdd, languageInfo, NavigationService));
                }
            }
            else
            {
                await InitializeInformationMessageAsync().ConfigureAwait(false);
            }
        }

        private async Task InitializeInformationMessageAsync()
        {
            var languageInfo = await _languageService.GetLanguageInfo().ConfigureAwait(false);
            var informationMessages = await InformationMessageService.GetAllForLastWeekAsync().ConfigureAwait(false);
            var viewModels = informationMessages.OrderByDescending(x => x.DatePublished).Select(x => new InformationMessageViewModel(x, languageInfo, NavigationService));
            InformationMessages = new ObservableCollection<InformationMessageViewModel>(viewModels);
        }
    }
}
