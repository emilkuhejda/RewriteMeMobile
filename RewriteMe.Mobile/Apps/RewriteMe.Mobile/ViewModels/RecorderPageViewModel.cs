using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecorderPageViewModel : ViewModelBase
    {
        public RecorderPageViewModel(
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            CanGoBack = true;

            RecordCommand = new AsyncCommand(ExecuteRecordCommandAsync);
        }

        public ICommand RecordCommand { get; }

        private async Task ExecuteRecordCommandAsync()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
