using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.FilePicker;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.DataAccess.Transcription;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;
using RewriteMe.Mobile.Transcription;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class CreatePageViewModel : ViewModelBase
    {
        private readonly IFileItemService _fileItemService;
        private readonly IMediaService _mediaService;

        private string _name;
        private SupportedLanguage _selectedLanguage;
        private FileDataWrapper _selectedFile;
        private IEnumerable<ActionBarTileViewModel> _navigationItems;

        public CreatePageViewModel(
            IFileItemService fileItemService,
            IMediaService mediaService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _fileItemService = fileItemService;
            _mediaService = mediaService;

            CanGoBack = true;

            NavigateToLanguageCommand = new AsyncCommand(ExecuteNavigateToLanguageCommandAsync);
            UploadFileCommand = new AsyncCommand(ExecuteUploadFileCommandAsync);
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

        public FileDataWrapper SelectedFile
        {
            get => _selectedFile;
            set
            {
                if (SetProperty(ref _selectedFile, value))
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

        private ActionBarTileViewModel SaveTileItem { get; set; }

        private ActionBarTileViewModel SaveAndTranscribeTileItem { get; set; }

        public ICommand NavigateToLanguageCommand { get; }

        public ICommand UploadFileCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() == NavigationMode.Back)
                {
                    var dropDownListViewModel = navigationParameters.GetValue<DropDownListViewModel>();
                    HandleSelectionAsync(dropDownListViewModel);
                }

                NavigationItems = CreateNavigation();

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

        private IEnumerable<ActionBarTileViewModel> CreateNavigation()
        {
            SaveTileItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.Save),
                IsEnabled = CanExecuteSaveCommand(),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Save-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Save-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteSaveCommandAsync, CanExecuteSaveCommand)
            };

            SaveAndTranscribeTileItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.SaveAndTranscribe),
                IsEnabled = CanExecuteSaveAndTranscribeCommand(),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.SaveAndTranscribe-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.SaveAndTranscribe-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteSaveAndTranscribeCommandAsync, CanExecuteSaveAndTranscribeCommand)
            };

            return new[] { SaveTileItem, SaveAndTranscribeTileItem };
        }

        private void ReevaluateNavigationItemIconKeys()
        {
            SaveTileItem.IsEnabled = CanExecuteSaveCommand();
            SaveAndTranscribeTileItem.IsEnabled = CanExecuteSaveAndTranscribeCommand();
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

        private async Task ExecuteUploadFileCommandAsync()
        {
            var selectedFile = await CrossFilePicker.Current.PickFile();
            var totalTime = _mediaService.GetTotalTime(selectedFile.FilePath);
            var canTranscribe = await _fileItemService.CanTranscribeAsync(totalTime).ConfigureAwait(false);
            SelectedFile = new FileDataWrapper(selectedFile)
            {
                TotalTime = totalTime,
                CanTranscribe = canTranscribe
            };

            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = SelectedFile.FileName;
            }
        }

        private bool CanExecuteSaveCommand()
        {
            return SelectedFile != null;
        }

        private async Task ExecuteSaveCommandAsync()
        {
            var func = new Func<MediaFile, Task>(async mediaFile =>
            {
                await _fileItemService.UploadAsync(mediaFile).ConfigureAwait(false);
                await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
            });

            await ExecuteSendToServer(func).ConfigureAwait(false);
        }

        private bool CanExecuteSaveAndTranscribeCommand()
        {
            return SelectedFile != null && SelectedFile.CanTranscribe && SelectedLanguage != null;
        }

        private async Task ExecuteSaveAndTranscribeCommandAsync()
        {
            var func = new Func<MediaFile, Task>(async mediaFile =>
            {
                var fileItem = await _fileItemService.UploadAsync(mediaFile).ConfigureAwait(false);
                await _fileItemService.TranscribeAsync(fileItem.Id ?? Guid.Empty, fileItem.Language).ConfigureAwait(false);
                await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
            });

            await ExecuteSendToServer(func).ConfigureAwait(false);
        }

        private async Task ExecuteSendToServer(Func<MediaFile, Task> func)
        {
            using (new OperationMonitor(OperationScope))
            {
                CanGoBack = false;

                try
                {
                    var result = await DialogService.ConfirmAsync(
                        Loc.Text(TranslationKeys.UploadFileItemInfoMessage),
                        okText: Loc.Text(TranslationKeys.Ok),
                        cancelText: Loc.Text(TranslationKeys.Cancel));

                    if (result)
                    {
                        var mediaFile = CreateMediaFile();
                        await func(mediaFile).ConfigureAwait(false);
                    }
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

        private MediaFile CreateMediaFile()
        {
            var name = string.IsNullOrWhiteSpace(Name) ? SelectedFile.FileName : Name;
            return new MediaFile
            {
                Name = name,
                Language = SelectedLanguage?.Culture,
                FileName = SelectedFile.FileName,
                TotalTime = SelectedFile.TotalTime,
                Stream = SelectedFile.FileData.GetStream()
            };
        }
    }
}
