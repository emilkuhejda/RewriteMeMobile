using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Domain.Transcription;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Resources.Utils;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecordedItemViewModel
    {
        public RecordedItemViewModel(RecordedItem recordedItem, INavigationService navigationService)
        {
            RecordedItem = recordedItem;
            NavigationService = navigationService;

            NavigateToDetailPageCommand = new AsyncCommand(ExecuteNavigateToDetailPageCommandAsync);
        }

        private RecordedItem RecordedItem { get; }

        private INavigationService NavigationService { get; }

        public string Title => RecordedItem.DateCreated.ToLocalTime().ToString(Constants.TimeFormat);

        public bool IsRecordingOnly => RecordedItem.IsRecordingOnly;

        public ICommand NavigateToDetailPageCommand { get; }

        private async Task ExecuteNavigateToDetailPageCommandAsync()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add<RecordedItem>(RecordedItem);
            await NavigationService.NavigateWithoutAnimationAsync(Pages.RecordedDetail, navigationParameters).ConfigureAwait(false);
        }
    }
}
