using Prism.Navigation;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Mobile.ViewModels
{
    public class CreatePageViewModel : ViewModelBase
    {
        public CreatePageViewModel(
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            CanGoBack = true;
        }
    }
}
