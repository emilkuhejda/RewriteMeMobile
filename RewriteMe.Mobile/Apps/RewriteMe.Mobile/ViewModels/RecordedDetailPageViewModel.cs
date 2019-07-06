using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Transcription;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecordedDetailPageViewModel : ViewModelBase
    {
        public RecordedDetailPageViewModel(
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            CanGoBack = true;

            DeleteCommand = new AsyncCommand(ExecuteDeleteCommandAsync);
        }

        public ICommand DeleteCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                var recordedItem = navigationParameters.GetValue<RecordedItem>();

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        private async Task ExecuteDeleteCommandAsync()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
