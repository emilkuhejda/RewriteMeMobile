using System.Threading.Tasks;
using System.Windows.Input;
using RewriteMe.Domain.Transcription;
using RewriteMe.Mobile.Commands;
using RewriteMe.Resources.Utils;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecordedItemViewModel
    {
        public RecordedItemViewModel(RecordedItem recordedItem)
        {
            RecordedItem = recordedItem;

            NavigateToDetailPageCommand = new AsyncCommand(ExecuteNavigateToDetailPageCommandAsync);
        }

        private RecordedItem RecordedItem { get; }

        public string Title => RecordedItem.DateCreated.ToLocalTime().ToString(Constants.TimeFormat);

        public ICommand NavigateToDetailPageCommand { get; }

        private async Task ExecuteNavigateToDetailPageCommandAsync()
        {
            await Task.CompletedTask;
        }
    }
}
