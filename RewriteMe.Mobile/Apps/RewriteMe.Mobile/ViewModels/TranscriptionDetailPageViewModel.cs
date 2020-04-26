using Prism.Navigation;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Mobile.ViewModels
{
    public class TranscriptionDetailPageViewModel : ViewModelBase
    {
        public TranscriptionDetailPageViewModel(
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
        }
    }
}
