using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Navigation;
using RewriteMe.Domain.WebApi;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Resources.Utils;

namespace RewriteMe.Mobile.ViewModels
{
    public class FileItemViewModel : BindableBase
    {
        private readonly INavigationService _navigationService;

        private bool _isInProgress;
        private bool _isCompleted;
        private bool _isUploading;
        private bool _isErrorIconVisible;
        private int _progress;

        public FileItemViewModel(FileItem fileItem, INavigationService navigationService)
        {
            _navigationService = navigationService;

            Update(fileItem);

            NavigateToDetailPageCommand = new AsyncCommand(ExecuteNavigateToDetailPageCommandAsync);
        }

        public FileItem FileItem { get; private set; }

        public string FileName => FileItem.Name;

        public string DateCreated => FileItem.DateCreated.ToString(Constants.TimeFormat);

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

        public bool IsUploading
        {
            get => _isUploading;
            set => SetProperty(ref _isUploading, value);
        }

        public bool IsErrorIconVisible
        {
            get => _isErrorIconVisible;
            set => SetProperty(ref _isErrorIconVisible, value);
        }

        public int Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public ICommand NavigateToDetailPageCommand { get; }

        public void Update(FileItem fileItem)
        {
            FileItem = fileItem;

            IsInProgress = fileItem.RecognitionState == RecognitionState.Converting || fileItem.RecognitionState == RecognitionState.Prepared || fileItem.RecognitionState == RecognitionState.InProgress;
            IsCompleted = fileItem.RecognitionState == RecognitionState.Completed;
            IsUploading = fileItem.UploadStatus == UploadStatus.InProgress;
            IsErrorIconVisible = !IsUploading && (fileItem.UploadStatus == UploadStatus.Error || fileItem.TranscribeErrorCode.HasValue);
        }

        private async Task ExecuteNavigateToDetailPageCommandAsync()
        {
            var recognitionState = FileItem.RecognitionState;
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add<FileItem>(FileItem);

            if (recognitionState == RecognitionState.None || recognitionState == RecognitionState.Converting || recognitionState == RecognitionState.Prepared)
            {
                switch (FileItem.UploadStatus)
                {
                    case UploadStatus.None:
                    case UploadStatus.Error:
                        await _navigationService.NavigateWithoutAnimationAsync(Pages.Create, navigationParameters).ConfigureAwait(false);
                        break;
                    case UploadStatus.Completed:
                        await _navigationService.NavigateWithoutAnimationAsync(Pages.Transcribe, navigationParameters).ConfigureAwait(false);
                        break;
                }
            }
            else if (recognitionState == RecognitionState.Completed)
            {
                await _navigationService.NavigateWithoutAnimationAsync(Pages.Detail, navigationParameters).ConfigureAwait(false);
            }
        }
    }
}
