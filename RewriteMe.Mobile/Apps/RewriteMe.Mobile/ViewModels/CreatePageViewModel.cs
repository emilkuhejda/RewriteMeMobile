using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.FilePicker;
using Prism.Commands;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Enums;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Messages;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.Upload;
using RewriteMe.Domain.WebApi;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation.Parameters;
using RewriteMe.Mobile.Transcription;
using RewriteMe.Mobile.Utils;
using RewriteMe.Resources.Localization;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class CreatePageViewModel : ViewModelBase
    {
        private readonly IFileItemService _fileItemService;
        private readonly IUploadedSourceService _uploadedSourceService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private FileItem _fileItem;
        private bool _isEdit;
        private string _name;
        private bool _isPhoneCall;
        private string _uploadErrorMessage;
        private bool _isUploadErrorMessageVisible;
        private SupportedLanguage _selectedLanguage;
        private PickedFile _selectedFile;
        private bool _isUploadButtonVisible;
        private IEnumerable<ActionBarTileViewModel> _navigationItems;

        public CreatePageViewModel(
            IFileItemService fileItemService,
            IUploadedSourceService uploadedSourceService,
            IRewriteMeWebService rewriteMeWebService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _fileItemService = fileItemService;
            _uploadedSourceService = uploadedSourceService;
            _rewriteMeWebService = rewriteMeWebService;
            _cancellationTokenSource = new CancellationTokenSource();

            CanGoBack = true;
            IsUploadButtonVisible = true;

            AvailableLanguages = SupportedLanguages.All.Where(x => !x.OnlyInAzure).ToList();

            UploadFileCommand = new AsyncCommand(ExecuteUploadFileCommandAsync);
            ClearSelectedFileCommand = new DelegateCommand(ExecuteClearSelectedFileCommand);

            ResetLoadingText();
        }

        private FileItem FileItem
        {
            get => _fileItem;
            set
            {
                _fileItem = value;
                IsEdit = _fileItem != null;
            }
        }

        public bool IsEdit
        {
            get => _isEdit;
            set
            {
                if (SetProperty(ref _isEdit, value))
                {
                    RaisePropertyChanged(nameof(IsLanguageLabelVisible));
                }
            }
        }

        public bool IsLanguageLabelVisible => IsEdit && SelectedLanguage != null;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public bool IsPhoneCallModelSupported => SelectedLanguage != null && SupportedLanguages.IsPhoneCallModelSupported(SelectedLanguage);

        public bool IsPhoneCall
        {
            get => _isPhoneCall;
            set => SetProperty(ref _isPhoneCall, value);
        }

        public string UploadErrorMessage
        {
            get => _uploadErrorMessage;
            set => SetProperty(ref _uploadErrorMessage, value);
        }

        public bool IsUploadErrorMessageVisible
        {
            get => _isUploadErrorMessageVisible;
            set => SetProperty(ref _isUploadErrorMessageVisible, value);
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
                    RaisePropertyChanged(nameof(IsPhoneCallModelSupported));
                    RaisePropertyChanged(nameof(IsLanguageLabelVisible));
                }
            }
        }

        public PickedFile SelectedFile
        {
            get => _selectedFile;
            set
            {
                if (SetProperty(ref _selectedFile, value))
                {
                    IsUploadErrorMessageVisible = false;
                    ReevaluateNavigationItemIconKeys();
                }
            }
        }

        public bool IsUploadButtonVisible
        {
            get => _isUploadButtonVisible;
            set => SetProperty(ref _isUploadButtonVisible, value);
        }

        public IEnumerable<ActionBarTileViewModel> NavigationItems
        {
            get => _navigationItems;
            set => SetProperty(ref _navigationItems, value);
        }

        private ActionBarTileViewModel SaveTileItem { get; set; }

        private ActionBarTileViewModel SaveAndTranscribeTileItem { get; set; }

        public ICommand UploadFileCommand { get; }

        public ICommand ClearSelectedFileCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                NavigationItems = CreateNavigation();

                if (navigationParameters.GetNavigationMode() != NavigationMode.New)
                    return;

                var importedFile = navigationParameters.GetValue<ImportedFileNavigationParameters>();
                if (importedFile?.Source != null && importedFile.Source.Any())
                {
                    var canTranscribe = await _fileItemService.CanTranscribeAsync().ConfigureAwait(false);
                    SelectedFile = new PickedFile
                    {
                        FileName = importedFile.FileName,
                        CanTranscribe = canTranscribe,
                        Source = importedFile.Source
                    };

                    Name = SelectedFile.FileName;
                    IsUploadButtonVisible = false;
                }

                FileItem = navigationParameters.GetValue<FileItem>();
                if (IsEdit)
                {
                    if (FileItem.UploadErrorCode == ErrorCode.Unauthorized)
                    {
                        var isSuccess = await _rewriteMeWebService.RefreshTokenIfNeededAsync().ConfigureAwait(false);
                        if (!isSuccess)
                            throw new UnauthorizedCallException();
                    }

                    Name = FileItem.Name;
                    SelectedLanguage = AvailableLanguages.FirstOrDefault(x => x.Culture == FileItem.Language);
                    IsPhoneCall = FileItem.IsPhoneCall;
                    UploadErrorMessage = UploadErrorHelper.GetErrorMessage(FileItem.UploadErrorCode);
                    IsUploadErrorMessageVisible = FileItem.UploadStatus == UploadStatus.Error;
                }

                RaisePropertyChanged(nameof(IsPhoneCallModelSupported));
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

        private async Task ExecuteUploadFileCommandAsync()
        {
            await ThreadHelper.InvokeOnUiThread(async () => await PickFileAsync().ConfigureAwait(false)).ConfigureAwait(false);
        }

        private void ExecuteClearSelectedFileCommand()
        {
            SelectedFile = null;
        }

        private async Task PickFileAsync()
        {
            using (var selectedFile = await CrossFilePicker.Current.PickFile().ConfigureAwait(false))
            {
                if (selectedFile == null)
                    return;

                var canTranscribe = await _fileItemService.CanTranscribeAsync().ConfigureAwait(false);
                SelectedFile = new PickedFile
                {
                    FileName = selectedFile.FileName,
                    CanTranscribe = canTranscribe,
                    Source = selectedFile.DataArray
                };

                if (string.IsNullOrWhiteSpace(Name))
                {
                    Name = SelectedFile.FileName;
                }
            }
        }

        private bool CanExecuteSaveCommand()
        {
            return SelectedFile != null;
        }

        private async Task ExecuteSaveCommandAsync()
        {
            await ExecuteSendToServer(false).ConfigureAwait(false);
        }

        private bool CanExecuteSaveAndTranscribeCommand()
        {
            return SelectedFile != null && SelectedFile.CanTranscribe && SelectedLanguage != null;
        }

        private async Task ExecuteSaveAndTranscribeCommandAsync()
        {
            await ExecuteSendToServer(true).ConfigureAwait(false);
        }

        private async Task ExecuteSendToServer(bool isTranscript)
        {
            IndicatorCaption = Loc.Text(TranslationKeys.UploadFileItemInfoMessage);

            using (new OperationMonitor(OperationScope))
            {
                CanGoBack = false;

                try
                {
                    var mimeType = MimeTypes.GetMimeType(SelectedFile.FileName);
                    if (MediaContentTypes.IsUnsupported(mimeType))
                    {
                        await DialogService.AlertAsync(Loc.Text(TranslationKeys.UploadedFileNotSupportedErrorMessage), okText: Loc.Text(TranslationKeys.Ok)).ConfigureAwait(false);
                        return;
                    }

                    var result = await DialogService.ConfirmAsync(
                        Loc.Text(TranslationKeys.UploadFileItemInfoMessage),
                        okText: Loc.Text(TranslationKeys.Ok),
                        cancelText: Loc.Text(TranslationKeys.Cancel)).ConfigureAwait(false);

                    if (result)
                    {
                        var mediaFile = CreateMediaFile();
                        var fileItem = IsEdit
                            ? FileItem
                            : await _fileItemService.CreateAsync(mediaFile, _cancellationTokenSource.Token).ConfigureAwait(false);
                        var uploadedSource = CreateUploadedSource(fileItem, mediaFile, isTranscript);

                        await _fileItemService.UpdateUploadStatusAsync(fileItem.Id, UploadStatus.InProgress).ConfigureAwait(false);

                        UploadFileItemSource(uploadedSource);

                        await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
                    }
                }
                catch (ErrorRequestException ex)
                {
                    await HandleErrorMessage(ex.ErrorCode).ConfigureAwait(false);
                }
                catch (NoSubscritionFreeTimeException)
                {
                    await DialogService.AlertAsync(Loc.Text(TranslationKeys.NotEnoughFreeMinutesInSubscriptionErrorMessage)).ConfigureAwait(false);
                }
                catch (OfflineRequestException)
                {
                    await DialogService.AlertAsync(Loc.Text(TranslationKeys.OfflineErrorMessage)).ConfigureAwait(false);
                }
                finally
                {
                    CanGoBack = true;
                    ResetLoadingText();
                }
            }
        }

        private void UploadFileItemSource(UploadedSource uploadedSource)
        {
            Task.Run(async () =>
            {
                await _uploadedSourceService.AddAsync(uploadedSource).ConfigureAwait(false);
                MessagingCenter.Send(new StartBackgroundServiceMessage(BackgroundServiceType.UploadFileItem), nameof(BackgroundServiceType.UploadFileItem));
            });
        }

        private UploadedSource CreateUploadedSource(FileItem fileItem, MediaFile mediaFile, bool isTranscript)
        {
            return new UploadedSource
            {
                Id = Guid.NewGuid(),
                FileItemId = fileItem.Id,
                Language = fileItem.Language,
                IsPhoneCall = fileItem.IsPhoneCall,
                Source = mediaFile.Source,
                IsTranscript = isTranscript,
                DateCreated = DateTime.UtcNow
            };
        }

        private async Task HandleErrorMessage(ErrorCode errorCode)
        {
            var message = UploadErrorHelper.GetErrorMessage(errorCode);
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
                IsPhoneCall = IsPhoneCallModelSupported ? IsPhoneCall : false,
                Source = SelectedFile.Source
            };
        }

        private void ResetLoadingText()
        {
            IndicatorCaption = Loc.Text(TranslationKeys.ActivityIndicatorCaptionText);
        }

        protected override void DisposeInternal()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}
