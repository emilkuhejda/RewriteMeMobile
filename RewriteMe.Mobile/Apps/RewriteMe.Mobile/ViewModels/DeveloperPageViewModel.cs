using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Resources.Localization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class DeveloperPageViewModel : ViewModelBase
    {
        private readonly IInternalValueService _internalValueService;
        private readonly IEmailService _emailService;
        private readonly ILogFileReader _logFileReader;
        private readonly IApplicationSettings _applicationSettings;

        private string _apiUrl;
        private HtmlWebViewSource _webViewSource;

        public DeveloperPageViewModel(
            IInternalValueService internalValueService,
            IEmailService emailService,
            ILogFileReader logFileReader,
            IApplicationSettings applicationSettings,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _internalValueService = internalValueService;
            _emailService = emailService;
            _logFileReader = logFileReader;
            _applicationSettings = applicationSettings;

            CanGoBack = true;

            SaveCommand = new AsyncCommand(ExecuteSaveCommandAsync);
            ClearLogFileCommand = new AsyncCommand(ExecuteClearLogFileCommandAsync);
            SendLogMailCommand = new AsyncCommand(ExecuteSendLogMailCommandAsync);
            ReloadLogCommand = new AsyncCommand(ExecuteReloadLogCommandAsync);
        }

        public string ApiUrl
        {
            get => _apiUrl;
            set => SetProperty(ref _apiUrl, value);
        }

        public HtmlWebViewSource WebViewSource
        {
            get => _webViewSource;
            set => SetProperty(ref _webViewSource, value);
        }

        public ICommand SaveCommand { get; }

        public ICommand ClearLogFileCommand { get; }

        public ICommand SendLogMailCommand { get; }

        public ICommand ReloadLogCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            await RunInOperationScope(LoadLogFileAsync).ConfigureAwait(false);

            using (new OperationMonitor(OperationScope))
            {
                ApiUrl = await _internalValueService.GetValueAsync(InternalValues.ApiUrl).ConfigureAwait(false);
            }
        }

        private async Task LoadLogFileAsync()
        {
            var content = await _logFileReader.ReadLogFileAsync().ConfigureAwait(false);
            WebViewSource = new HtmlWebViewSource { Html = content };
        }

        public async Task ExecuteSaveCommandAsync()
        {
            await RunInOperationScope(async () =>
            {
                await _internalValueService.UpdateValueAsync(InternalValues.ApiUrl, ApiUrl).ConfigureAwait(false);
                await _applicationSettings.InitializeAsync().ConfigureAwait(false);
                await DialogService.ConfirmAsync(Loc.Text(TranslationKeys.ApiUrlSaved), okText: Loc.Text(TranslationKeys.Ok)).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        private async Task ExecuteClearLogFileCommandAsync()
        {
            await RunInOperationScope(async () =>
            {
                await _logFileReader.ClearLogFileAsync().ConfigureAwait(false);
                await LoadLogFileAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        private async Task ExecuteSendLogMailCommandAsync()
        {
            if (string.IsNullOrWhiteSpace(_applicationSettings.SupportMailAddress))
                return;

            try
            {
                var subject = $"{Loc.Text(TranslationKeys.ApplicationTitleLog)}";
                var message = new StringBuilder()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine("_______________________________________")
                    .AppendLine($"Device hardware: {DeviceInfo.Manufacturer} {DeviceInfo.Model}")
                    .AppendLine($"Operating system: {Device.RuntimePlatform} {DeviceInfo.Version}")
                    .ToString();

                var fileInfo = _logFileReader.GetLogFileInfo();
                await _emailService
                    .SendAsync(_applicationSettings.SupportMailAddress, subject, message, fileInfo.FullName)
                    .ConfigureAwait(false);
            }
            catch (Exception)
            {
                await DialogService.AlertAsync(Loc.Text(TranslationKeys.EmailIsNotSupported)).ConfigureAwait(false);
            }
        }

        private async Task ExecuteReloadLogCommandAsync()
        {
            await RunInOperationScope(LoadLogFileAsync).ConfigureAwait(false);
        }

        private Task RunInOperationScope(Func<Task> action)
        {
            using (new OperationMonitor(OperationScope))
            {
                return action.Invoke();
            }
        }
    }
}
