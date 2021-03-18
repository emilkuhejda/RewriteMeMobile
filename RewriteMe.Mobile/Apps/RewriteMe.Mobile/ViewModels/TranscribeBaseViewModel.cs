using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Transcription;
using RewriteMe.Mobile.Utils;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public abstract class TranscribeBaseViewModel : ViewModelBase
    {
        private string _name;
        private SupportedLanguage _selectedLanguage;
        private bool _isPhoneCall;
        private IEnumerable<ActionBarTileViewModel> _navigationItems;
        private bool _canTranscribe;

        protected TranscribeBaseViewModel(
            IFileItemService fileItemService,
            IRewriteMeWebService rewriteMeWebService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            FileItemService = fileItemService;
            RewriteMeWebService = rewriteMeWebService;
            CanGoBack = true;

            AvailableLanguages = SupportedLanguages.All.Where(x => !x.OnlyInAzure).ToList();

            DeleteCommand = new AsyncCommand(ExecuteDeleteCommandAsync);
        }

        protected IFileItemService FileItemService { get; }

        protected IRewriteMeWebService RewriteMeWebService { get; }

        protected bool AudioFileIsInvalid { get; set; }

        protected bool CanTranscribe
        {
            get => _canTranscribe;
            set
            {
                if (SetProperty(ref _canTranscribe, value))
                {
                    ReevaluateNavigationItemIconKeys();
                }
            }
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public IEnumerable<SupportedLanguage> AvailableLanguages { get; set; }

        public SupportedLanguage SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (SetProperty(ref _selectedLanguage, value))
                {
                    IsPhoneCall = false;
                    ReevaluateNavigationItemIconKeys();
                    RaisePropertyChanged(nameof(IsRecordingTypeVisible));
                }
            }
        }

        public bool IsRecordingTypeVisible => SelectedLanguage != null && SupportedLanguages.IsPhoneCallModelSupported(SelectedLanguage);

        public bool IsBasicRecording => !IsPhoneCall;

        public bool IsPhoneCall
        {
            get => _isPhoneCall;
            set
            {
                if (SetProperty(ref _isPhoneCall, value))
                {
                    RaisePropertyChanged(nameof(IsBasicRecording));
                }
            }
        }

        public IEnumerable<ActionBarTileViewModel> NavigationItems
        {
            get => _navigationItems;
            set => SetProperty(ref _navigationItems, value);
        }

        private ActionBarTileViewModel TranscribeTileItem { get; set; }

        public IAsyncCommand DeleteCommand { get; }

        protected abstract Task ExecuteTranscribeInternalAsync();

        protected abstract Task ExecuteDeleteInternalAsync();

        protected virtual void BeforeExecuteCommand()
        { }

        protected void InitializeNavigationItems()
        {
            TranscribeTileItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.Transcribe),
                IsEnabled = CanExecuteTranscribeCommand(),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.SaveAndTranscribe-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.SaveAndTranscribe-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteTranscribeCommandAsync, CanExecuteTranscribeCommand)
            };

            NavigationItems = new[] { TranscribeTileItem };
        }

        private void ReevaluateNavigationItemIconKeys()
        {
            TranscribeTileItem.IsEnabled = CanExecuteTranscribeCommand();
        }

        private async Task ExecuteDeleteCommandAsync()
        {
            await ExecuteDeleteInternalAsync().ConfigureAwait(false);
        }

        private bool CanExecuteTranscribeCommand()
        {
            return !AudioFileIsInvalid && CanTranscribe && _selectedLanguage != null;
        }

        private async Task ExecuteTranscribeCommandAsync()
        {
            BeforeExecuteCommand();

            using (new OperationMonitor(OperationScope))
            {
                CanGoBack = false;

                try
                {
                    await ExecuteTranscribeInternalAsync().ConfigureAwait(false);
                }
                catch (ErrorRequestException ex)
                {
                    await HandleErrorMessage(ex.ErrorCode).ConfigureAwait(false);
                }
                catch (NoSubscritionFreeTimeException)
                {
                    await DialogService
                        .AlertAsync(Loc.Text(TranslationKeys.NotEnoughFreeMinutesInSubscriptionErrorMessage))
                        .ConfigureAwait(false);
                }
                catch (OfflineRequestException)
                {
                    await DialogService.AlertAsync(Loc.Text(TranslationKeys.OfflineErrorMessage)).ConfigureAwait(false);
                }
                catch (UnauthorizedCallException)
                {
                    var isSuccess = await RewriteMeWebService.RefreshTokenIfNeededAsync().ConfigureAwait(false);
                    if (isSuccess)
                    {
                        await DialogService.AlertAsync(Loc.Text(TranslationKeys.UnauthorizedErrorMessage)).ConfigureAwait(false);
                    }
                    else
                    {
                        await SignOutAsync().ConfigureAwait(false);
                    }
                }
                finally
                {
                    CanGoBack = true;
                }
            }
        }

        private async Task HandleErrorMessage(ErrorCode errorCode)
        {
            var message = UploadErrorHelper.GetErrorMessage(errorCode);
            await DialogService.AlertAsync(message).ConfigureAwait(false);
        }
    }
}
