using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Extensions;

namespace RewriteMe.Mobile.ViewModels
{
    public class InformationMessagesPageViewModel : OverviewBaseViewModel
    {
        private readonly IInformationMessageService _informationMessageService;

        private IEnumerable<InformationMessage> _informationMessages;

        public InformationMessagesPageViewModel(
            IInformationMessageService informationMessageService,
            ISynchronizationService synchronizationService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(synchronizationService, userSessionService, dialogService, navigationService, loggerFactory)
        {
            _informationMessageService = informationMessageService;
        }

        public IEnumerable<InformationMessage> InformationMessages
        {
            get => _informationMessages;
            set => SetProperty(ref _informationMessages, value);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                InitializeNavigation(CurrentPage.InformationMessages);

                var informationMessages = await _informationMessageService.GetAllAsync().ConfigureAwait(false);
                InformationMessages = informationMessages.ToList();

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
