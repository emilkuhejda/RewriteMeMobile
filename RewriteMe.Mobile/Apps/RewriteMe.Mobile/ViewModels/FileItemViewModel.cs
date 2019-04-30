using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;

namespace RewriteMe.Mobile.ViewModels
{
    public class FileItemViewModel
    {
        private readonly INavigationService _navigationService;

        public FileItemViewModel(FileItem fileItem, INavigationService navigationService)
        {
            _navigationService = navigationService;

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
        {
            var recognitionState = FileItem.RecognitionState.ToRecognitionState();
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add<FileItem>(FileItem);

            if (recognitionState == RecognitionState.None || recognitionState == RecognitionState.Converting || recognitionState == RecognitionState.Prepared)
            {
                await _navigationService.NavigateWithoutAnimationAsync(Pages.Transcribe, navigationParameters).ConfigureAwait(false);
            }
            else if (recognitionState == RecognitionState.Completed)
            {
                await _navigationService.NavigateWithoutAnimationAsync(Pages.Detail, navigationParameters).ConfigureAwait(false);
            }
        }
    }
}
