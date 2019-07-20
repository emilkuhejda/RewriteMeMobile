using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.DataAccess.Transcription;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;
using RewriteMe.Mobile.Transcription;
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
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            FileItemService = fileItemService;
            CanGoBack = true;

            NavigateToLanguageCommand = new AsyncCommand(ExecuteNavigateToLanguageCommandAsync);
            DeleteCommand = new AsyncCommand(ExecuteDeleteCommandAsync);
        }

        protected IFileItemService FileItemService { get; }

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

        public ICommand NavigateToLanguageCommand { get; }

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

        protected void HandleSelectionAsync(DropDownListViewModel dropDownListViewModel)
        {
            if (dropDownListViewModel == null)
                return;

            switch (dropDownListViewModel.Type)
            {
                case nameof(SelectedLanguage):
                    SelectedLanguage = (SupportedLanguage)dropDownListViewModel.Value;
                    break;
                default:
                    throw new NotSupportedException(nameof(SelectedLanguage));
            }
        }

        private async Task ExecuteNavigateToLanguageCommandAsync()
        {
            BeforeExecuteCommand();

            var languages = SupportedLanguages.All.Select(x => new DropDownListViewModel
            {
                Text = x.Title,
                Value = x,
                Type = nameof(SelectedLanguage),
                IsSelected = SelectedLanguage != null && x.Culture == SelectedLanguage.Culture
            });

            var navigationParameters = new NavigationParameters();
            var parameters = new DropDownListNavigationParameters(languages);
            navigationParameters.Add<DropDownListNavigationParameters>(parameters);

            await NavigationService.NavigateWithoutAnimationAsync(Pages.DropDownListPage, navigationParameters).ConfigureAwait(false);
        }

        private async Task ExecuteDeleteCommandAsync()
        {
            await ExecuteDeleteInternalAsync().ConfigureAwait(false);
        }

        private bool CanExecuteTranscribeCommand()
        {
            return CanTranscribe && _selectedLanguage != null;
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
                    await DialogService.AlertAsync(Loc.Text(TranslationKeys.NotEnoughFreeMinutesInSubscriptionErrorMessage)).ConfigureAwait(false);
                }
                catch (OfflineRequestException)
                {
                    await DialogService.AlertAsync(Loc.Text(TranslationKeys.OfflineErrorMessage)).ConfigureAwait(false);
                }

                CanGoBack = true;
            }
        }

        private async Task HandleErrorMessage(int? statusCode)
        {
            string message;
            switch (statusCode)
            {
                case 400:
                    message = Loc.Text(TranslationKeys.UploadedFileNotFoundErrorMessage);
                    break;
                case 403:
                    message = Loc.Text(TranslationKeys.NotEnoughFreeMinutesInSubscriptionErrorMessage);
                    break;
                case 406:
                    message = Loc.Text(TranslationKeys.LanguageNotSupportedErrorMessage);
                    break;
                case 415:
                    message = Loc.Text(TranslationKeys.UploadedFileNotSupportedErrorMessage);
                    break;
                default:
                    message = Loc.Text(TranslationKeys.UnreachableServerErrorMessage);
                    break;
            }

            await DialogService.AlertAsync(message).ConfigureAwait(false);
        }
    }
}
