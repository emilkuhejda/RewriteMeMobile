using System.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Transcription;
using RewriteMe.Mobile.Utils;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class TranscribePageViewModel : TranscribeBaseViewModel
    {
        private string _errorMessage;
        private bool _isErrorMessageVisible;

        public TranscribePageViewModel(
            IFileItemService fileItemService,
            IRewriteMeWebService rewriteMeWebService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(fileItemService, rewriteMeWebService, userSessionService, dialogService, navigationService, loggerFactory)
        {
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (SetProperty(ref _errorMessage, value) && !string.IsNullOrWhiteSpace(value))
                {
                    IsErrorMessageVisible = true;
                }
            }
        }

        public bool IsErrorMessageVisible
        {
            get => _isErrorMessageVisible;
            set => SetProperty(ref _isErrorMessageVisible, value);
        }

        private FileItem FileItem { get; set; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            InitializeNavigationItems();

            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() != NavigationMode.New)
                    return;

                FileItem = navigationParameters.GetValue<FileItem>();
                if (FileItem.TranscribeErrorCode.HasValue)
                {
                    var isSuccess = await RewriteMeWebService.RefreshTokenIfNeededAsync().ConfigureAwait(false);
                    if (!isSuccess)
                        throw new UnauthorizedCallException();

                    ErrorMessage = UploadErrorHelper.GetErrorMessage(FileItem.TranscribeErrorCode);
                }

                Name = FileItem.Name;
                SelectedLanguage = SupportedLanguages.All.FirstOrDefault(x => x.Culture == FileItem.Language);
                IsPhoneCall = FileItem.IsPhoneCall;

                CanTranscribe = await FileItemService.CanTranscribeAsync().ConfigureAwait(false);

                RaisePropertyChanged(nameof(IsPhoneCallModelSupported));
            }
        }

        protected override async Task ExecuteTranscribeInternalAsync()
        {
            IsErrorMessageVisible = false;

            await FileItemService.TranscribeAsync(FileItem.Id, SelectedLanguage.Culture, IsPhoneCall).ConfigureAwait(false);
            await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
        }

        protected override async Task ExecuteDeleteInternalAsync()
        {
            var result = await DialogService.ConfirmAsync(
                Loc.Text(TranslationKeys.PromptDeleteFileItemMessage, FileItem.Name),
                okText: Loc.Text(TranslationKeys.Ok),
                cancelText: Loc.Text(TranslationKeys.Cancel)).ConfigureAwait(false);

            if (result)
            {
                using (new OperationMonitor(OperationScope))
                {
                    try
                    {
                        await FileItemService.DeleteAsync(FileItem).ConfigureAwait(false);
                        await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
                    }
                    catch (FileNotUploadedException)
                    {
                        await DialogService.AlertAsync(Loc.Text(TranslationKeys.FileIsNotUploadedErrorMessage)).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
