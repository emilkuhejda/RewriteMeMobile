using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.DataAccess.Transcription;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;
using RewriteMe.Mobile.Transcription;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class TranscribePageViewModel : ViewModelBase
    {
        private readonly IFileItemService _fileItemService;

        private string _name;
        private SupportedLanguage _selectedLanguage;
        private bool _canTranscribe;

        public TranscribePageViewModel(
            IFileItemService fileItemService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _fileItemService = fileItemService;

            CanGoBack = true;

            NavigateToLanguageCommand = new AsyncCommand(ExecuteNavigateToLanguageCommandAsync);
            TranscribeCommand = new AsyncCommand(ExecuteTranscribeCommandAsync, CanExecuteTranscribeCommand);
        }

        private FileItem FileItem { get; set; }

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
                    TranscribeCommand.ChangeCanExecute();
                }
            }
        }

        public ICommand NavigateToLanguageCommand { get; }

        public IAsyncCommand TranscribeCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() == NavigationMode.New)
                {
                    FileItem = navigationParameters.GetValue<FileItem>();
                    Title = FileItem.Name;
                    Name = FileItem.Name;
                    SelectedLanguage = SupportedLanguages.All.FirstOrDefault(x => x.Culture == FileItem.Language);

                    _canTranscribe = await _fileItemService.CanTranscribeAsync(FileItem.TotalTime).ConfigureAwait(false);
                }
                else if (navigationParameters.GetNavigationMode() == NavigationMode.Back)
                {
                    var dropDownListViewModel = navigationParameters.GetValue<DropDownListViewModel>();
                    HandleSelectionAsync(dropDownListViewModel);
                }

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        private void HandleSelectionAsync(DropDownListViewModel dropDownListViewModel)
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
            var languages = SupportedLanguages.All.Select(x => new DropDownListViewModel
            {
                Text = x.Title,
                Value = x,
                Type = nameof(SelectedLanguage),
                IsSelected = SelectedLanguage != null && x.Culture == SelectedLanguage.Culture
            });

            var navigationParameters = new NavigationParameters();
            var parameters = new DropDownListNavigationParameters(Loc.Text(TranslationKeys.Languages), languages);
            navigationParameters.Add<DropDownListNavigationParameters>(parameters);

            await NavigationService.NavigateWithoutAnimationAsync(Pages.DropDownListPage, navigationParameters).ConfigureAwait(false);
        }

        private bool CanExecuteTranscribeCommand()
        {
            return _canTranscribe && _selectedLanguage != null;
        }

        private async Task ExecuteTranscribeCommandAsync()
        {
            if (!FileItem.Id.HasValue)
                return;

            using (new OperationMonitor(OperationScope))
            {
                CanGoBack = false;

                try
                {
                    await _fileItemService.TranscribeAsync(FileItem.Id.Value, SelectedLanguage.Culture)
                        .ConfigureAwait(false);
                    await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
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
                default:
                    message = Loc.Text(TranslationKeys.UnreachableServerErrorMessage);
                    break;
            }

            await DialogService.AlertAsync(message).ConfigureAwait(false);
        }
    }
}
