using System.Threading.Tasks;
using System.Windows.Input;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Mobile.Commands;

namespace RewriteMe.Mobile.ViewModels
{
    public class FileItemViewModel
    {
        public FileItemViewModel(FileItem fileItem)
        {
            FileItem = fileItem;

            IsInProgress = fileItem.RecognitionState.ToRecognitionState() == RecognitionState.InProgress;
            IsCompleted = fileItem.RecognitionState.ToRecognitionState() == RecognitionState.Completed;

            NavigateToDetailPageCommand = new AsyncCommand(ExecuteNavigateToDetailPageCommandAsync);
        }

        public FileItem FileItem { get; }

        public bool IsInProgress { get; }

        public bool IsCompleted { get; }

        public ICommand NavigateToDetailPageCommand { get; }

        private async Task ExecuteNavigateToDetailPageCommandAsync()
        { }
    }
}
