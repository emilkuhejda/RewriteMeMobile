using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;

namespace RewriteMe.Mobile.ViewModels
{
    public class FileItemViewModel : BindableBase
    {
        private readonly INavigationService _navigationService;

        private bool _isInProgress;
        private bool _isCompleted;

        public FileItemViewModel(FileItem fileItem, INavigationService navigationService)
        {
            _navigationService = navigationService;

            Update(fileItem);

            NavigateToDetailPageCommand = new AsyncCommand(ExecuteNavigateToDetailPageCommandAsync);
        }

        public FileItem FileItem { get; private set; }

        public bool IsInProgress
        {
            get => _isInProgress;
            set => SetProperty(ref _isInProgress, value);
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set => SetProperty(ref _isCompleted, value);
        }

        public ICommand NavigateToDetailPageCommand { get; }

        public void Update(FileItem fileItem)
        {
            FileItem = fileItem;

            IsInProgress = fileItem.RecognitionState.ToRecognitionState() == RecognitionState.InProgress;
            IsCompleted = fileItem.RecognitionState.ToRecognitionState() == RecognitionState.Completed;
        }

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
