using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.DeviceInfo;
using Plugin.Messaging;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Resources.Localization;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class DeveloperPageViewModel : ViewModelBase
    {
        private readonly ILogFileReader _logFileReader;
        private readonly IEmailTask _emailTask;
        private readonly IApplicationSettings _applicationSettings;
        private readonly IHardwareInfo _hardwareInfo;

        private string _logContent;

        public DeveloperPageViewModel(
            ILogFileReader logFileReader,
            IEmailTask emailTask,
            IApplicationSettings applicationSettings,
            IHardwareInfo hardwareInfo,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _logFileReader = logFileReader;
            _emailTask = emailTask;
            _applicationSettings = applicationSettings;
            _hardwareInfo = hardwareInfo;

            CanGoBack = true;

            ClearLogFileCommand = new AsyncCommand(ExecuteClearLogFileCommandAsync);
            SendLogMailCommand = new AsyncCommand(ExecuteSendLogMailCommandAsync);
            ReloadLogCommand = new AsyncCommand(ExecuteReloadLogCommandAsync);
        }

        public string LogContent
        {
            get => _logContent;
            set => SetProperty(ref _logContent, value);
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
            LogContent = string.Empty;
            LogContent = await _logFileReader.ReadLogFileAsync().ConfigureAwait(false);
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

            if (_emailTask.CanSendEmail)
            {
                var subject = $"{Loc.Text(TranslationKeys.ApplicationTitleLog)}";
                var message = new StringBuilder()
                    .AppendLine(LogContent)
                    .AppendLine()
                    .AppendLine()
                    .AppendLine("_______________________________________")
                    .AppendLine($"{Loc.Text(TranslationKeys.DeviceHardwareLabel)} {_hardwareInfo.Manufacturer} {_hardwareInfo.Model}")
                    .AppendLine($"{Loc.Text(TranslationKeys.OperatingSystemLabel)} {Device.RuntimePlatform} {_hardwareInfo.OperatingSystem}")
                    .ToString();

                _emailTask.SendEmail(_applicationSettings.SupportMailAddress, subject, message);
            }
            else
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
