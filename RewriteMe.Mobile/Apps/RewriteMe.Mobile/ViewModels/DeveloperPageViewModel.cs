using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.DeviceInfo;
using Prism.Navigation;
using RewriteMe.Common.Utils;
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
        private readonly ILogFileReader _logFileReader;
        private readonly IApplicationSettings _applicationSettings;

        private HtmlWebViewSource _webViewSource;

        public DeveloperPageViewModel(
            ILogFileReader logFileReader,
            IApplicationSettings applicationSettings,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _logFileReader = logFileReader;
            _applicationSettings = applicationSettings;

            CanGoBack = true;

            ClearLogFileCommand = new AsyncCommand(ExecuteClearLogFileCommandAsync);
            SendLogMailCommand = new AsyncCommand(ExecuteSendLogMailCommandAsync);
            ReloadLogCommand = new AsyncCommand(ExecuteReloadLogCommandAsync);
        }

        public HtmlWebViewSource WebViewSource
        {
            get => _webViewSource;
            set => SetProperty(ref _webViewSource, value);
        }

        public ICommand ClearLogFileCommand { get; }

        public ICommand SendLogMailCommand { get; }

        public ICommand ReloadLogCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            await RunInOperationScope(LoadLogFileAsync).ConfigureAwait(false);
        }

        private async Task LoadLogFileAsync()
        {
            var content = await _logFileReader.ReadLogFileAsync().ConfigureAwait(false);
            WebViewSource = new HtmlWebViewSource { Html = content };
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
                var device = CrossDevice.Device;
                var subject = $"{Loc.Text(TranslationKeys.ApplicationTitleLog)}";
                var message = new StringBuilder()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine("_______________________________________")
                    .AppendLine($"Device hardware: {device.Manufacturer} {device.Model}")
                    .AppendLine($"Operating system: {Device.RuntimePlatform} {device.OperatingSystem}")
                    .ToString();

                var emailMessage = new EmailMessage
                {
                    To = new List<string> { _applicationSettings.SupportMailAddress },
                    Subject = subject,
                    Body = message
                };

                var fileInfo = _logFileReader.GetLogFileInfo();
                if (fileInfo.Exists)
                {
                    emailMessage.Attachments.Add(new EmailAttachment(fileInfo.FullName));
                }

                await Email.ComposeAsync(emailMessage).ConfigureAwait(false);
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
