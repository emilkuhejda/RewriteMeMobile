using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.LatestVersion.Abstractions;
using Plugin.Messaging;
using Prism.Commands;
using Prism.Navigation;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Utils;
using RewriteMe.Resources.Localization;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public abstract class OverviewBaseViewModel : ViewModelBase
    {
        private readonly ILatestVersion _latestVersion;
        private readonly IEmailTask _emailTask;
        private readonly IApplicationSettings _applicationSettings;

        private IEnumerable<ActionBarTileViewModel> _navigationItems;
        private bool _isNotUserRegistrationSuccess;
        private bool _notAvailableData;

        protected OverviewBaseViewModel(
            IUserSessionService userSessionService,
            IInternalValueService internalValueService,
            ILatestVersion latestVersion,
            IEmailTask emailTask,
            IApplicationSettings applicationSettings,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _latestVersion = latestVersion;
            _emailTask = emailTask;
            _applicationSettings = applicationSettings;

            UserSessionService = userSessionService;
            InternalValueService = internalValueService;

            SendEmailCommand = new DelegateCommand(ExecuteSendEmailCommand);
            NavigateToRecorderCommand = new AsyncCommand(ExecuteNavigateToRecorderCommandAsync);
        }

        protected IUserSessionService UserSessionService { get; }

        protected IInternalValueService InternalValueService { get; }

        public ICommand SendEmailCommand { get; }

        public IEnumerable<ActionBarTileViewModel> NavigationItems
        {
            get => _navigationItems;
            set => SetProperty(ref _navigationItems, value);
        }

        public bool IsNotUserRegistrationSuccess
        {
            get => _isNotUserRegistrationSuccess;
            set => SetProperty(ref _isNotUserRegistrationSuccess, value);
        }

        public bool NotAvailableData
        {
            get => _notAvailableData;
            set => SetProperty(ref _notAvailableData, value);
        }

        public ICommand NavigateToRecorderCommand { get; }

        protected void InitializeNavigation(bool isOverview)
        {
            var audioFilesNavigationItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.AudioFiles),
                IsEnabled = !isOverview,
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Overview-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Overview-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteNavigateToOverviewAsync)
            };

            var recordedFilesNavigationItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.RecordedFiles),
                IsEnabled = isOverview,
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.RecorderOverview-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.RecorderOverview-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteNavigateToRecorderOverviewAsync)
            };

            NavigationItems = new[] { audioFilesNavigationItem, recordedFilesNavigationItem };
        }

        private void ExecuteSendEmailCommand()
        {
            ThreadHelper.InvokeOnUiThread(CreateContactUsMailAsync);
        }

        private async Task CreateContactUsMailAsync()
        {
            if (string.IsNullOrWhiteSpace(_applicationSettings.SupportMailAddress))
                return;

            if (_emailTask.CanSendEmail)
            {
                var userId = await UserSessionService.GetUserIdAsync().ConfigureAwait(false);
                var subject = $"{Loc.Text(TranslationKeys.ApplicationTitle)} - {Loc.Text(TranslationKeys.RegistrationErrorTitle)}";
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz", CultureInfo.InvariantCulture);
                var message = new StringBuilder()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine()
                    .AppendLine("_______________________________________")
                    .AppendLine($"User identification: {userId}")
                    .AppendLine($"Application version: {_latestVersion.InstalledVersionNumber} ({Device.RuntimePlatform})")
                    .AppendLine($"Time stamp: {timestamp}")
                    .ToString();

                _emailTask.SendEmail(_applicationSettings.SupportMailAddress, subject, message);
            }
            else
            {
                await DialogService.AlertAsync(Loc.Text(TranslationKeys.EmailIsNotSupported)).ConfigureAwait(false);
            }
        }

        private async Task ExecuteNavigateToRecorderCommandAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.Recorder).ConfigureAwait(false);
        }

        protected abstract Task ExecuteNavigateToOverviewAsync();

        protected abstract Task ExecuteNavigateToRecorderOverviewAsync();
    }
}
