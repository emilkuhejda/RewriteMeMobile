using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Extensions;

namespace RewriteMe.Mobile.ViewModels
{
    public class TranscribePageViewModel : ViewModelBase
    {
        public TranscribePageViewModel(
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            CanGoBack = true;
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                var fileItem = navigationParameters.GetValue<FileItem>();
                Title = fileItem.Name;

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }
    }
}
