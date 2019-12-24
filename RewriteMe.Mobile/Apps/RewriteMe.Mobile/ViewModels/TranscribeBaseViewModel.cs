using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
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
        private IEnumerable<ActionBarTileViewModel> _navigationItems;
        private bool _canTranscribe;

        protected TranscribeBaseViewModel(
            IFileItemService fileItemService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            FileItemService = fileItemService;
            CanGoBack = true;

            AvailableLanguages = SupportedLanguages.All.Where(x => !x.OnlyInAzure).ToList();

            DeleteCommand = new AsyncCommand(ExecuteDeleteCommandAsync);
        }

        protected IFileItemService FileItemService { get; }

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
                    ReevaluateNavigationItemIconKeys();
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
                    await HandleErrorMessage(ex.StatusCode).ConfigureAwait(false);
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
                finally
                {
                    CanGoBack = true;
                }
            }
        }

        private async Task HandleErrorMessage(int? statusCode)
        {
            var message = UploadErrorHelper.GetErrorMessage(statusCode);
            await DialogService.AlertAsync(message).ConfigureAwait(false);
        }
    }
}
