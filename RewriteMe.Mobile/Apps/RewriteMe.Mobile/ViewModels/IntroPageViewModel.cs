using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;

namespace RewriteMe.Mobile.ViewModels
{
    public class IntroPageViewModel : ViewModelBase
    {
        public IntroPageViewModel(
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            OkCommand = new AsyncCommand(ExecuteOkCommand);
        }

        public ICommand OkCommand { get; }

        private async Task ExecuteOkCommand()
        {

        }
    }
}
